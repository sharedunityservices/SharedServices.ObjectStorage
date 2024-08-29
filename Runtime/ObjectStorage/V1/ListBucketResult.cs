using System;
using System.Xml.Serialization;

namespace SharedServices.ObjectStorage.V1
{
    [XmlRoot("ListBucketResult", Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    public class ListBucketResult
    {
        [XmlElement] public string Name;
        [XmlElement] public string Prefix;
        [XmlElement] public string Marker;
        [XmlElement] public int MaxKeys;
        [XmlElement] public string Delimiter;
        [XmlElement] public bool IsTruncated;
        [XmlElement] public Content[] Contents;
    }
    
    public class Content
    {
        [XmlElement] public string Key;
        [XmlElement] public DateTime LastModified;
        [XmlElement] public string ETag;
        [XmlElement] public long Size;
        [XmlElement] public Owner Owner;
        [XmlElement] public string StorageClass;
    }
    
    public class Owner
    {
        [XmlElement] public string DisplayName;
        [XmlElement] public string ID;
    }
}