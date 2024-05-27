using System;
using System.Linq;
using SharedServices.Environment.V1;
using SharedServices.Locator;
using SharedServices.Locator.V1;
using UnityEngine;
using UnityEngine.Networking;
using Utility.Json;

namespace Services.ObjectStorage
{
    public static class S3Util
    {
        private static IEnvironmentService _environmentService;
        
        public static string GetAuthorizationHeader(string verb, string path, string timeStampIso8601Format, string hashedPayload,
            string range = null)
        {
            const string service = "s3";
            _environmentService ??= Service.Get<IEnvironmentService>();
            var region = GetRegion(path);
            var accessKey = _environmentService.Get("MINIO_ACCESS_KEY");
            var secretKey = _environmentService.Get("MINIO_SECRET_KEY");
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            return GetAuthorizationHeader(path, timeStampIso8601Format, hashedPayload, range, verb, service, region,
                accessKey, secretKey, date);
        }

        public static string GetAuthorizationHeader(string path, string timeStampIso8601Format, string hashedPayload,
            string range, string verb, string service, string region, string accessKey, string secretKey, string date,
            string storageClass = null)
        {
            var canonicalUri = GetCanonicalUri(path);
            var canonicalQueryString = GetCanonicalQueryString(path);
            var canonicalHeaders = GetCanonicalHeaders(path, hashedPayload, timeStampIso8601Format, range, storageClass);
            var signedHeaders = GetSignedHeaders(range, storageClass);
            var credentialScope = $"{date}/{region}/{service}/aws4_request";
            var canonicalRequest = GetCanonicalRequest(verb, canonicalUri, canonicalQueryString, canonicalHeaders,
                signedHeaders, hashedPayload);
            var stringToSign = GetStringToSignVersion4(timeStampIso8601Format, credentialScope, canonicalRequest);
            var signature = GenerateSignatureVersion4(secretKey, date, region, service, stringToSign);
            var authorization = $"AWS4-HMAC-SHA256 ";
            authorization += $"Credential={accessKey}/{date}/{region}/{service}/aws4_request,";
            authorization += $"SignedHeaders={signedHeaders},";
            authorization += $"Signature={signature}";
            Debug.Log($"Canonical Request:\n{canonicalRequest}\n\nString to Sign:\n{stringToSign}\n\nSignature:\n{signature}\n\nAuthorization:\n{authorization}");
            return authorization;
        }

        private static string GetSignedHeaders(string range, string storageClass)
        {
            var output = "host;";
            output += range != null ? "range;" : "";
            output += "x-amz-content-sha256";
            output += ";x-amz-date";
            output += storageClass != null ? ";x-amz-storage-class" : "";
            return output;
        }

        private static string GetCanonicalRequest(string verb, string canonicalUri, string canonicalQueryString,
            string canonicalHeaders, string signedHeaders, string hashedPayload)
        {
            //return "PUT\n/test%24file.text\n\ndate:Fri, 24 May 2013 00:00:00 GMT\nhost:examplebucket.s3.amazonaws.com\nx-amz-content-sha256:44ce7dd67c959e0d3524ffac1771dfbba87d2b6b4b4e99e42034a8b803f8b072\nx-amz-date:20130524T000000Z\nx-amz-storage-class:REDUCED_REDUNDANCY\n\ndate;host;x-amz-content-sha256;x-amz-date;x-amz-storage-class\n44ce7dd67c959e0d3524ffac1771dfbba87d2b6b4b4e99e42034a8b803f8b072";
            return
                $"{verb}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{hashedPayload}";
        }

        private static string GetStringToSignVersion4(string timeStampISO8601Format, string credentialScope,
            string canonicalRequest)
        {
            var canonicalRequestBytes = System.Text.Encoding.UTF8.GetBytes(canonicalRequest);
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedCanonicalRequest = sha256.ComputeHash(canonicalRequestBytes);
            var hashedCanonicalRequestHex = hashedCanonicalRequest.Aggregate("", (s, e) => s + e.ToString("x2"));
            return $"AWS4-HMAC-SHA256\n{timeStampISO8601Format}\n{credentialScope}\n{hashedCanonicalRequestHex}";
        }

        private static string GenerateSignatureVersion4(string secretKey, string date, string region, string service,
            string stringToSign)
        {
            var secretAccessKey = $"AWS4{secretKey}";
            var secretAccessKeyBytes = System.Text.Encoding.UTF8.GetBytes(secretAccessKey);
            var dateBytes = System.Text.Encoding.UTF8.GetBytes(date);
            using var hmacDate = new System.Security.Cryptography.HMACSHA256(secretAccessKeyBytes);
            var dateKeyBytes = hmacDate.ComputeHash(dateBytes);
            var regionBytes = System.Text.Encoding.UTF8.GetBytes(region);
            using var hmacRegion = new System.Security.Cryptography.HMACSHA256(dateKeyBytes);
            var dataRegionKeyBytes = hmacRegion.ComputeHash(regionBytes);
            var serviceBytes = System.Text.Encoding.UTF8.GetBytes(service);
            using var hmacService = new System.Security.Cryptography.HMACSHA256(dataRegionKeyBytes);
            var dataServiceKeyBytes = hmacService.ComputeHash(serviceBytes);
            var signingBytes = System.Text.Encoding.UTF8.GetBytes("aws4_request");
            using var hmacSigning = new System.Security.Cryptography.HMACSHA256(dataServiceKeyBytes);
            var signingKeyBytes = hmacSigning.ComputeHash(signingBytes);
            var stringToSignBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);
            using var hmacSignature = new System.Security.Cryptography.HMACSHA256(signingKeyBytes);
            var signatureBytes = hmacSignature.ComputeHash(stringToSignBytes);
            var signatureHex = signatureBytes.Aggregate("", (s, e) => s + e.ToString("x2"));
            return signatureHex;
        }

        private static string GetCanonicalUri(string path)
        {
            return UnityWebRequest.EscapeURL(new Uri(path).AbsolutePath).Replace("%2f", "/");
        }

        private static string GetCanonicalQueryString(string path)
        {
            return new Uri(path).Query;
        }

        private static string GetCanonicalHeaders(string path, string hashedPayload, string xAmzDate, string range,
            string storageClass)
        {
            var host = new Uri(path).Authority;
            var output = $"host:{host}\n";
            output += range != null ? $"range:{range}\n" : "";
            output += $"x-amz-content-sha256:{hashedPayload}\n";
            output += $"x-amz-date:{xAmzDate}\n";
            output += storageClass != null ? $"x-amz-storage-class:{storageClass}\n" : "";
            return output;
        }

        public static byte[] GetBytes<T>(T data)
        {
            if (typeof(T) == typeof(string))
            {
                var dataString = data as string;
                return System.Text.Encoding.UTF8.GetBytes(dataString);
            }
            if (typeof(T) == typeof(byte[]))
            {
                return data as byte[];
            }
            else
            {
                var dataString = JsonUtil.ToJson(data);
                return System.Text.Encoding.UTF8.GetBytes(dataString);
            }
        }

        public static string GetHashedPayload(byte[] payloadBytes)
        {
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedPayload = sha256.ComputeHash(payloadBytes);
            var hashedPayloadHex = hashedPayload.Aggregate("", (s, e) => s + e.ToString("x2"));
            return hashedPayloadHex;
        }

        private static string GetRegion(string path)
        {
            var regions = new[]
            {
                "us-east-2",
                "us-east-1",
                "us-west-1",
                "us-west-2",
                "af-south-1",
                "ap-east-1",
                "ap-south-2",
                "ap-southeast-3",
                "ap-southeast-4",
                "ap-south-1",
                "ap-northeast-3",
                "ap-northeast-2",
                "ap-southeast-1",
                "ap-southeast-2",
                "ap-northeast-1",
                "ca-central-1",
                "ca-west-1",
                "eu-central-1",
                "eu-west-1",
                "eu-west-2",
                "eu-south-1",
                "eu-west-3",
                "eu-south-2",
                "eu-north-1",
                "eu-central-2",
                "il-central-1",
                "me-south-1",
                "me-central-1",
                "sa-east-1",
                "us-gov-east-1",
                "us-gov-west-1"
            };

            var uri = new Uri(path);
            foreach (var region in regions)
                if (uri.Authority.Contains(region))
                    return region;

            return "us-east-1";
        }

        public static string GetContentType<T>()
        {
            if (typeof(T) == typeof(string))
                return "text/plain";
            if (typeof(T) == typeof(byte[]))
                return "application/octet-stream";
            return "application/json";
        }
    }
}