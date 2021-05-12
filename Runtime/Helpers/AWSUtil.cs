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

// force to include all scripts in assembly and don't accidentally strip something as they might be added at runtime
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
        public static string S3Root_Live = "https://travrsal-live.sfo2.digitaloceanspaces.com/";
        public static string S3CDNRoot_Alpha = "https://travrsal-alpha.sfo2.cdn.digitaloceanspaces.com/";
        public static string S3CDNRoot_Beta = "https://travrsal-beta.sfo2.cdn.digitaloceanspaces.com/";
        public static string S3CDNRoot_Live = "https://travrsal-live.sfo2.cdn.digitaloceanspaces.com/";
        public static string S3CDNRoot_Runs = "https://travrsal-runs.sfo2.cdn.digitaloceanspaces.com/";

        // FIXME: only leave in for beta, new mechanism as soon as backend is up and running
        public static string AccessKey = "TJLL2B73DJSGQPBKJRP7";
        public static string AccessKeySecret = "H8mxC/akX8W9jqIFXvY+UFOj5zrW0bTsEtz1Bh5HDvg";

        // Amazon-only: private static string S3BucketName = "eu.west1.travrsal.repo";
        private static string S3BucketName = "travrsal-upload";

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

        public bool LastActionSuccessful;
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
                if (_s3Client == null) _s3Client = new AmazonS3Client(Credentials, new AmazonS3Config {ServiceURL = S3LoginRoot});
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

        public async Task UploadFile(string fileName, string remoteName, string bucket)
        {
            LastActionSuccessful = true;
            await CarryOutAWSTask(async () =>
            {
                FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                TransferUtility fileTransferUtility = new TransferUtility(Client);
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = stream,
                    Key = remoteName,
                    BucketName = bucket,
                    CannedACL = S3CannedACL.PublicRead
                };

                await fileTransferUtility.UploadAsync(uploadRequest);
            }, "storing file");
        }

        public async Task UploadDirectory(string path, Action<float> progressCallback, string pattern = "*")
        {
            LastActionSuccessful = true;
            await CarryOutAWSTask(async () =>
            {
                TransferUtility fileTransferUtility = new TransferUtility(Client);
                TransferUtilityUploadDirectoryRequest uploadRequest = new TransferUtilityUploadDirectoryRequest
                {
                    BucketName = S3BucketName,
                    Directory = path,
                    // Amazon-only: StorageClass = S3StorageClass.StandardInfrequentAccess,
                    CannedACL = S3CannedACL.PublicRead,
                    SearchPattern = pattern,
                    SearchOption = SearchOption.AllDirectories,
                };

                uploadRequest.UploadDirectoryProgressEvent += (sender, e) => { progressCallback((float) e.TransferredBytes / e.TotalBytes); };
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
                LastActionSuccessful = false;
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