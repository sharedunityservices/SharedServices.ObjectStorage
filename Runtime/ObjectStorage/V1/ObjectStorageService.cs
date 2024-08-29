using System;
using System.Threading.Tasks;
using SharedServices.Json.V1;
using SharedServices.Xml.V1;
using UnityEngine;
using UnityEngine.Networking;

namespace SharedServices.ObjectStorage.V1
{
    [UnityEngine.Scripting.Preserve]
    public class ObjectStorageService : IObjectStorageService
    {
        public void GetObject<T>(string path, Action<T> callback)
        {
            var timeStampIso8601Format = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var hashedPayload = S3Util.GetHashedPayload(S3Util.GetBytes(""));
            var authorization = S3Util.GetAuthorizationHeader("GET", path, timeStampIso8601Format, hashedPayload);
            WebRequestUtil
                .HttpGetRequest(path)
                .SetRequestHeader("Authorization", authorization)
                .SetRequestHeader("x-amz-content-sha256", hashedPayload)
                .SetRequestHeader("x-amz-date", timeStampIso8601Format)
                .SetDownloadHandler(new DownloadHandlerBuffer())
                .SendWebRequest()
                .OnCompleted(request =>
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        var error = request.error;
                        if (request.downloadHandler != null)
                            error += $"\n{request.downloadHandler.text}";
                        Debug.LogError(error);
                        callback?.Invoke(default);
                        return;
                    }

                    if (typeof(T) == typeof(string))
                        callback?.Invoke((T)(object)request.downloadHandler.text);
                    else if (typeof(T) == typeof(byte[]))
                        callback?.Invoke((T)(object)request.downloadHandler.data);
                    else
                        callback?.Invoke(IJsonService.FromJson<T>(request.downloadHandler.text));
                });
        }
        
        public WaitUntilCallback<string, T> GetObjectRoutine<T>(string path) => 
            AsyncUtil.CallbackToIEnumerator<string, T>(path, GetObject);
        
        public Task<T> GetObjectAsync<T>(string path) => 
            AsyncUtil.CallbackToTask<string, T>(path, GetObject);
        
        public void PutObject<T>(string path, T data, Action<bool> callback)
        {
            var timeStampIso8601Format = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var dataBytes = S3Util.GetBytes(data);
            var hashedPayload = S3Util.GetHashedPayload(dataBytes);
            var authorization = S3Util.GetAuthorizationHeader("PUT", path, timeStampIso8601Format, hashedPayload);
            WebRequestUtil
                .HttpPutRequest(path)
                .SetRequestHeader("Authorization", authorization)
                .SetRequestHeader("x-amz-content-sha256", hashedPayload)
                .SetRequestHeader("x-amz-date", timeStampIso8601Format)
                .SetRequestHeader("Content-Type", S3Util.GetContentType<T>())
                .SetUploadHandler(new UploadHandlerRaw(dataBytes))
                .SendWebRequest()
                .OnCompleted(request =>
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        var error = request.error;
                        if (request.downloadHandler != null)
                            error += $"\n{request.downloadHandler.text}";
                        Debug.LogError(error);
                        callback?.Invoke(false);
                        return;
                    }

                    callback?.Invoke(true);
                });
        }
        
        public WaitUntilCallback<string, T, bool> PutObjectRoutine<T>(string path, T data) => 
            AsyncUtil.CallbackToIEnumerator<string, T, bool>(path, data, PutObject);
        
        public Task<bool> PutObjectAsync<T>(string path, T data) =>
            AsyncUtil.CallbackToTask<string, T, bool>(path, data, PutObject);
        
        public void DeleteObject(string path, Action<bool> callback)
        {
            var timeStampIso8601Format = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var hashedPayload = S3Util.GetHashedPayload(S3Util.GetBytes(""));
            var authorization = S3Util.GetAuthorizationHeader("DELETE", path, timeStampIso8601Format, hashedPayload);
            WebRequestUtil
                .HttpDeleteRequest(path)
                .SetRequestHeader("Authorization", authorization)
                .SetRequestHeader("x-amz-content-sha256", hashedPayload)
                .SetRequestHeader("x-amz-date", timeStampIso8601Format)
                .SendWebRequest()
                .OnCompleted(request =>
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        var error = request.error;
                        if (request.downloadHandler != null)
                            error += $"\n{request.downloadHandler.text}";
                        Debug.LogError(error);
                        callback?.Invoke(false);
                        return;
                    }

                    callback?.Invoke(true);
                });
        }
        
        public WaitUntilCallback<string, bool> DeleteObjectRoutine(string path) => 
            AsyncUtil.CallbackToIEnumerator<string, bool>(path, DeleteObject);
        
        public Task<bool> DeleteObjectAsync(string path) =>
            AsyncUtil.CallbackToTask<string, bool>(path, DeleteObject);
        
        public void ListObjects(string path, string prefix = null, string marker = null, int maxKeys = 1000, 
            string delimiter = null, Action<ListBucketResult> callback = null)
        {
            var timeStampIso8601Format = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var hashedPayload = S3Util.GetHashedPayload(S3Util.GetBytes(""));
            var hasPrefix = !string.IsNullOrEmpty(prefix);
            var hasMarker = !string.IsNullOrEmpty(marker);
            var hasMaxKeys = maxKeys != 1000;
            var hasDelimiter = !string.IsNullOrEmpty(delimiter);

            if (hasPrefix || hasMarker || hasMaxKeys || hasDelimiter)
            {
                path += "?";
                if (hasPrefix) path += $"prefix={prefix}&";
                if (hasMarker) path += $"marker={marker}&";
                if (hasMaxKeys) path += $"max-keys={maxKeys}&";
                if (hasDelimiter) path += $"delimiter={delimiter}&";
                path = path.TrimEnd('&');
            }

            var authorization = S3Util.GetAuthorizationHeader("GET", path, timeStampIso8601Format, hashedPayload);
            
            WebRequestUtil
                .HttpGetRequest(path)
                .SetRequestHeader("Authorization", authorization)
                .SetRequestHeader("x-amz-content-sha256", hashedPayload)
                .SetRequestHeader("x-amz-date", timeStampIso8601Format)
                .SetDownloadHandler(new DownloadHandlerBuffer())
                .SendWebRequest()
                .OnCompleted(request =>
                {
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        var error = request.error;
                        if (request.downloadHandler != null)
                            error += $"\n{request.downloadHandler.text}";
                        Debug.LogError(error);
                        callback?.Invoke(null);
                        return;
                    }

                    var listBucketResultXml = request.downloadHandler.text;
                    var listBucketResult = IXmlService.FromXml<ListBucketResult>(listBucketResultXml);
                    callback?.Invoke(listBucketResult);
                });
        }
        
        public WaitUntilCallback<string, string, string, int, string, ListBucketResult> ListObjectsRoutine(string path, 
            string prefix = null, string marker = null, int maxKeys = 1000, string delimiter = null) => 
            AsyncUtil.CallbackToIEnumerator<string, string, string, int, string, ListBucketResult>(path, prefix, marker, 
                maxKeys, delimiter, ListObjects);

        public Task<ListBucketResult> ListObjectsAsync(string path, string prefix = null, string marker = null, 
            int maxKeys = 1000, string delimiter = null) => 
            AsyncUtil.CallbackToTask<string, string, string, int, string, ListBucketResult>(path, prefix, marker, maxKeys, 
                delimiter, ListObjects);
        
        public void Exists(string path, Action<bool> callback)
        {
            var timeStampIso8601Format = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            var hashedPayload = S3Util.GetHashedPayload(S3Util.GetBytes(""));
            var authorization = S3Util.GetAuthorizationHeader("HEAD", path, timeStampIso8601Format, hashedPayload);
            WebRequestUtil
                .HttpHeadRequest(path)
                .SetRequestHeader("Authorization", authorization)
                .SetRequestHeader("x-amz-content-sha256", hashedPayload)
                .SetRequestHeader("x-amz-date", timeStampIso8601Format)
                .SendWebRequest()
                .OnCompleted(request =>
                {
                    if (request.result != UnityWebRequest.Result.Success && request.result != UnityWebRequest.Result.ProtocolError)
                    {
                        var error = request.error;
                        if (request.downloadHandler != null)
                            error += $"\n{request.downloadHandler.text}";
                        Debug.LogError(error);
                        callback?.Invoke(false);
                        return;
                    }

                    callback?.Invoke(request.result == UnityWebRequest.Result.Success);
                });
        }
        
        public WaitUntilCallback<string, bool> ExistsRoutine(string path) => 
            AsyncUtil.CallbackToIEnumerator<string, bool>(path, Exists);
        
        public Task<bool> ExistsAsync(string path) =>
            AsyncUtil.CallbackToTask<string, bool>(path, Exists);
    }
}