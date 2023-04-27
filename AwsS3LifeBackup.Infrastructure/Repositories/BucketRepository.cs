using Amazon.S3;
using Amazon.S3.Model;
using AwsS3LifeBackup.Core.Communication.Bucket;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
