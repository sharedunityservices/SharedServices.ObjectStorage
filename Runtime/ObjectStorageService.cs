using System;
using System.Threading.Tasks;
using Services.Environment;
using SharedServices.Locator;
using SharedServices.Locator.V1;
using UnityEngine;
using UnityEngine.Networking;
using Utility.Async;
using Utility.Json;
using Utility.WebRequest;

namespace Services.ObjectStorage
{
    public class ObjectStorageService : IObjectStorageService
    {
        private IEnvironmentService _environmentService;

        public void Initialize()
        {
            _environmentService = Service.Get<IEnvironmentService>();
        }

        public void Download<T>(string path, Action<T> callback)
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
                        callback?.Invoke(JsonUtil.FromJson<T>(request.downloadHandler.text));
                });
        }
        
        public WaitUntilCallback<string, T> DownloadRoutine<T>(string path) => 
            AsyncUtil.CallbackToIEnumerator<string, T>(path, Download);
        
        public Task<T> DownloadAsync<T>(string path) => 
            AsyncUtil.CallbackToTask<string, T>(path, Download);
        
        public void Upload<T>(string path, T data, Action<bool> callback)
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
        
        public WaitUntilCallback<string, T, bool> UploadRoutine<T>(string path, T data) => 
            AsyncUtil.CallbackToIEnumerator<string, T, bool>(path, data, Upload);
        
        public Task<bool> UploadAsync<T>(string path, T data) =>
            AsyncUtil.CallbackToTask<string, T, bool>(path, data, Upload);
    }
}