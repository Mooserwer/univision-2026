using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure
{
    public class StorageUpload
    {
        static CloudStorageAccount _storageAccount;
        static CloudBlobClient _blobClient;
        static CloudBlobContainer _container;
        static CloudBlockBlob _blockBlob;
        
        static StorageUpload()
        {

            _storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _container = _blobClient.GetContainerReference("wooyoung");
        }

        public static void FileUp(HttpPostedFileBase[] files)
        {
            try
            {
                foreach (var data in files)
                {

                    ////업로드 차단할 확장자 입력.
                    //if (!fi.FullName.Contains(".exe")) 
                    //    continue;
                    _blockBlob = _container.GetBlockBlobReference(data.FileName);

                    var file = File.OpenRead(data.FileName);
                    _blockBlob.UploadFromStream(file); //Blob에 파일 업로드

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    static class UploadInfo
    {
        public static string storageConnectionString { get; set; }
        public static string blobContainerName { get; set; }
    }
}