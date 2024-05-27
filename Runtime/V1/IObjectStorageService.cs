using System;
using System.Threading.Tasks;
using SharedServices;
using SharedServices.V1;
using Utility.Async;

namespace Services.ObjectStorage
{
    public interface IObjectStorageService : IService
    {
        void Download<T>(string path, Action<T> callback);
        WaitUntilCallback<string, T> DownloadRoutine<T>(string path);
        Task<T> DownloadAsync<T>(string path);
        
        void Upload<T>(string path, T data, Action<bool> callback);
        WaitUntilCallback<string, T, bool> UploadRoutine<T>(string path, T data);
        Task<bool> UploadAsync<T>(string path, T data);
    }
}