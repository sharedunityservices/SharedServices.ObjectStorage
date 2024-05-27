using System;
using UnityEngine.Networking;

namespace Utility.WebRequest
{
    public static class WebRequestUtil
    {
        public static WebRequest HttpGetRequest(string url) => new("GET", url);
        public static WebRequest HttpPostRequest(string url) => new("POST", url);
        public static WebRequest HttpPutRequest(string url) => new("PUT", url);
        public static WebRequest HttpDeleteRequest(string url) => new("DELETE", url); 
    }

    public class WebRequest
    {
        private readonly UnityWebRequest _unityWebRequest;
        private UnityWebRequestAsyncOperation _unityWebRequestOperation;

        internal WebRequest(string method, string url)
        {
            _unityWebRequest = new UnityWebRequest(url, method);
        }
        
        public WebRequest SetRequestHeader(string key, string value)
        {
            _unityWebRequest.SetRequestHeader(key, value);
            return this;
        }
        
        public WebRequest SetDownloadHandler(DownloadHandler downloadHandler)
        {
            _unityWebRequest.downloadHandler = downloadHandler;
            return this;
        }
        
        public WebRequest SetUploadHandler(UploadHandler uploadHandler)
        {
            _unityWebRequest.uploadHandler = uploadHandler;
            return this;
        }
        
        public WebRequest SendWebRequest()
        {
            _unityWebRequestOperation = _unityWebRequest.SendWebRequest();
            return this;
        }
        
        public WebRequest OnCompleted(Action<UnityWebRequest> onComplete)
        {
            _unityWebRequestOperation.completed += _ => onComplete(_unityWebRequest);
            return this;
        }
    }
}