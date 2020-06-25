// derived from https://github.com/sachabarber/AWS/blob/master/Storage/S3BucketsAndKeys/S3BucketsAndKeys/Program.cs

using UnityEngine;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using System.Threading;
using UnityEngine.Scripting;

[assembly: Preserve]

namespace traVRsal.SDK
{
    public class AWSUtil
    {
        // Amazon settings
        // Amazon-only: public static string IdentityPoolId = "eu-west-1:87055e81-6bbc-4556-aa65-7b6df7d1ebe7";
        // Amazon-only: public static string S3Root = "https://s3-eu-west-1.amazonaws.com/eu.west1.travrsal.repo/";

        // Digital Ocean settings
        public static string S3LoginRoot = "https://sfo2.digitaloceanspaces.com/";
        public static string S3Root = "https://travrsal-live.sfo2.digitaloceanspaces.com/";

        // FIXME: only leave in for beta, new mechanism as soon as backend is up and running
        public static string AccessKey = "56OHHRYAJWSCDODSF6X3";
        public static string AccessKeySecret = "mNfWLF/J40vn65nQoyCrnU+/g89TM1lIAohiikSxNm8";
        // Amazon-only: public static string S3BucketName = "eu.west1.travrsal.repo";
        public static string S3BucketName = "travrsal-live";

        public string CognitoIdentityRegion = RegionEndpoint.EUWest1.SystemName;
        private RegionEndpoint _CognitoIdentityRegion
        {
            get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
        }
        public string S3Region = RegionEndpoint.EUWest1.SystemName;
        private RegionEndpoint _S3Region
        {
            get { return RegionEndpoint.GetBySystemName(S3Region); }
        }

        private IAmazonS3 _s3Client;
        private AWSCredentials _credentials;

        private AWSCredentials Credentials
        {
            get
            {
                // Amazon-only: if (_credentials == null) _credentials = new CognitoAWSCredentials(IdentityPoolId, _CognitoIdentityRegion);
                if (_credentials == null) _credentials = new BasicAWSCredentials(AccessKey, AccessKeySecret);
                return _credentials;
            }
        }

        private IAmazonS3 Client
        {
            get
            {
                // Amazon-only: if (_s3Client == null) _s3Client = new AmazonS3Client(Credentials, _S3Region);
                if (_s3Client == null) _s3Client = new AmazonS3Client(Credentials, new AmazonS3Config { ServiceURL = S3LoginRoot });
                return _s3Client;
            }
        }

        public async void GetBucketList()
        {
            await CarryOutAWSTask(async () =>
            {
                ListBucketsResponse response = await Client.ListBucketsAsync();
                foreach (S3Bucket bucket in response.Buckets)
                {
                    Debug.Log($"You own bucket: {bucket.BucketName}");
                }
            }, "listing buckets");
        }

        public async void GetObjects()
        {
            await CarryOutAWSTask(async () =>
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = S3BucketName;
                ListObjectsResponse response = await Client.ListObjectsAsync(request);
                foreach (S3Object entry in response.S3Objects)
                {
                    Debug.Log($"key = {entry.Key} size = {entry.Size}");
                }
            }, "listing objects");
        }

        public async void UploadFile(string fileName)
        {
            await CarryOutAWSTask(async () =>
            {
                var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                var fileTransferUtility = new TransferUtility(Client);
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = stream,
                    Key = fileName,
                    BucketName = S3BucketName,
                    CannedACL = S3CannedACL.PublicRead
                };

                await fileTransferUtility.UploadAsync(uploadRequest);
                Debug.Log($"File upload of {fileName} completed");
            }, "storing file");
        }

        public async Task UploadDirectory(string path, Action<float> progressCallback)
        {
            await CarryOutAWSTask(async () =>
            {
                var fileTransferUtility = new TransferUtility(Client);
                var uploadRequest = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = S3BucketName,
                    Directory = path,
                    // Amazon-only: StorageClass = S3StorageClass.StandardInfrequentAccess,
                    CannedACL = S3CannedACL.PublicRead,
                    SearchOption = SearchOption.AllDirectories,

                };

                uploadRequest.UploadDirectoryProgressEvent += new EventHandler<UploadDirectoryProgressArgs>((sender, e) =>
                {
                    progressCallback((float)e.TransferredBytes / e.TotalBytes);
                });
                await fileTransferUtility.UploadDirectoryAsync(uploadRequest);
                progressCallback(1);
            }, "storing directory");
        }

        public async void DeleteObject(string fileName)
        {
            await CarryOutAWSTask(async () =>
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = S3BucketName,
                    Key = fileName
                };

                await Client.DeleteObjectAsync(request);

            }, "delete object");
        }

        public async void GetObject(string fileName)
        {
            await CarryOutAWSTask(async () =>
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = S3BucketName,
                    Key = fileName
                };

                using (GetObjectResponse response = await Client.GetObjectAsync(request))
                {
                    string title = response.Metadata["x-amz-meta-title"];
                    Debug.Log($"The object's title is {title}");
                    string dest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
                    if (!File.Exists(dest))
                    {
                        await response.WriteResponseStreamToFileAsync(dest, true, CancellationToken.None);
                    }
                }
            }, "read object");
        }

        private async Task CarryOutAWSTask(Func<Task> taskToPerform, string op)
        {
            try
            {
                await taskToPerform();
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null && (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Debug.LogError("Please check the provided AWS credentials.");
                }
                else
                {
                    Debug.LogError($"Error {amazonS3Exception.ErrorCode} connecting to AWS, for {op}: {amazonS3Exception.Message}");
                }
            }
        }
    }
}
