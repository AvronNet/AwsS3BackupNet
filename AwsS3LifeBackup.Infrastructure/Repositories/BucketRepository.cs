using Amazon.S3;
using Amazon.S3.Model;
using AwsS3LifeBackup.Core.Communication.Bucket;
using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Infrastructure.Repositories
{
    public class BucketRepository : IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;

        public BucketRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<bool> DoesS3BucketExist(string bucketName)
        {
            return await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        }

        public async Task<CreateBucketResponse> CreateBucket(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest { BucketName = bucketName, UseClientRegion = true };
            var putBucketResponse =  await _s3Client.PutBucketAsync(putBucketRequest);

            return new CreateBucketResponse(putBucketResponse.ResponseMetadata.RequestId, bucketName);
        }

        public async Task AddFolder(string bucketName, string folderName, AddJsonObjectRequest request)
        {
            var s3Key = folderName.Last() == '/' ? folderName : $"{folderName}/";

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = s3Key
            };

            await _s3Client.PutObjectAsync(putObjectRequest);
        }

        public async Task<IEnumerable<ListS3BucketsResponse>> ListBuckets()
        {
            var response = await _s3Client.ListBucketsAsync();

            return response.Buckets.Select(x => new ListS3BucketsResponse()
            {
                BucketName = x.BucketName,
                CreationDate = x.CreationDate,
            });
        }

        public async Task DeleteBucket(string bucketName)
        {
            await _s3Client.DeleteBucketAsync(bucketName);
        }
    }
}
