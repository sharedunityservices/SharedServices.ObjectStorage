using System;
using System.Linq;

namespace SharedServices.ObjectStorage.Archive
{
    public static class ObjectStorageServiceArchive
    {
        // Version 2
        // var date = DateTime.UtcNow.ToString("r");
        // var bucket = ObjectStorageServiceArchive.GetBucketFromPath(path);
        // var httpRequestUri = ObjectStorageServiceArchive.GetHttpRequestUri(path);
        // var signature = ObjectStorageServiceArchive.GenerateSignatureVersion2(accessKey, secretKey, verb, date, bucket, httpRequestUri);
        // request.SetRequestHeader("Authorization", signature);
        // request.SetRequestHeader("x-amz-date", date);
        
        public static string GetBucketFromPath(string path) =>
            path.Contains("amazonaws.com")
                ? new Uri(path).Host.Split('.').First()
                : new Uri(path).Segments[1].TrimEnd('/');
        
        public static string GetHttpRequestUri(string path) =>
            "/" + (path.Contains("amazonaws.com")
                ? string.Join("/", new Uri(path).Segments[1..])
                : string.Join("/", new Uri(path).Segments[2..])).Replace("//", "/");

        private static string GetCanonicalizedAmzHeaders()
        {
            return "";
        }

        private static string GetCanonicalizedResource(string bucket, string httpRequestUri)
        {
            return $"/{bucket}{httpRequestUri}";
        }

        private static string GetStringToSignVersion2(string verb, string date, string bucket, string httpRequestUri,
            string contentMD5 = "", string contentType = "")
        {
            var canonicalizedAmzHeaders = GetCanonicalizedAmzHeaders();
            var canonicalizedResource = GetCanonicalizedResource(bucket, httpRequestUri);
            return $"{verb}\n{contentMD5}\n{contentType}\n{date}\n{canonicalizedAmzHeaders}{canonicalizedResource}";
        }

        public static string GenerateSignatureVersion2(string accessKey, string secretKey, string verb, string date,
            string bucket, string httpRequestUri, string contentMD5 = "", string contentType = "")
        {
            var stringToSign = GetStringToSignVersion2(verb, date, bucket, httpRequestUri, contentMD5, contentType);
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            var stringToSignBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);
            using var hmac = new System.Security.Cryptography.HMACSHA1(secretKeyBytes);
            var signatureBytes = hmac.ComputeHash(stringToSignBytes);
            var signatureBase64 = Convert.ToBase64String(signatureBytes);
            var signature = $"AWS {accessKey}:{signatureBase64}";
            return signature;
        }
    }
}