using Amazon.S3;
using Amazon.S3.Model;
using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using AwsS3LifeBackup.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Infrastructure.Repositories
{
    public class FolderRepository : IFolderRepository
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3BucketConfiguration _bucketConfiguration;


        public FolderRepository(IAmazonS3 amazonS3Client, IOptions<S3BucketConfiguration> bucketConfiguration)
        {
            _s3Client = amazonS3Client;
            _bucketConfiguration = bucketConfiguration.Value;
        }

        public async Task<bool> CreateFolder(string folderName, string pathToFolder = "")
        {
            var s3Key = EnsureFolderTrailingSlash(folderName);
            if(!string.IsNullOrEmpty(pathToFolder))
            {
                s3Key = $"{EnsureFolderTrailingSlash(pathToFolder)}{s3Key}";
            }

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = _bucketConfiguration.Name,
                Key = s3Key
            };

            var result = await _s3Client.PutObjectAsync(putObjectRequest);

            return result.HttpStatusCode.Equals(System.Net.HttpStatusCode.OK);
        }

        private string EnsureFolderTrailingSlash(string folderName) => folderName.Last() == '/' ? folderName : $"{folderName}/";

    }
}
