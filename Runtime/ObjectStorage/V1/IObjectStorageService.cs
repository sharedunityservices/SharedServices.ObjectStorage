using System;
using System.Threading.Tasks;
using SharedServices.V1;

namespace SharedServices.ObjectStorage.V1
{
    public interface IObjectStorageService : IService
    {
        void GetObject<T>(string path, Action<T> callback);
        WaitUntilCallback<string, T> GetObjectRoutine<T>(string path);
        Task<T> GetObjectAsync<T>(string path);
        
        void PutObject<T>(string path, T data, Action<bool> callback);
        WaitUntilCallback<string, T, bool> PutObjectRoutine<T>(string path, T data);
        Task<bool> PutObjectAsync<T>(string path, T data);
        
        void DeleteObject(string path, Action<bool> callback);
        WaitUntilCallback<string, bool> DeleteObjectRoutine(string path);
        Task<bool> DeleteObjectAsync(string path);

        void ListObjects(string path, string prefix = null, string marker = null, int maxKeys = 1000, string delimiter = null, Action<ListBucketResult> callback = null);
        WaitUntilCallback<string, string, string, int, string, ListBucketResult> ListObjectsRoutine(string path, string prefix = null, string marker = null, int maxKeys = 1000, string delimiter = null);
        Task<ListBucketResult> ListObjectsAsync(string path, string prefix = null, string marker = null, int maxKeys = 1000, string delimiter = null);
    }
}