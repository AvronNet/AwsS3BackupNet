using Amazon.S3;
using Amazon.S3.Transfer;
using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AwsS3LifeBackup.Infrastructure.Repositories
{
    public class FilesRepository : IFilesRepository
    {
        private readonly IAmazonS3 _amazonS3Client;
        private readonly IConfiguration _configuration;

        public FilesRepository(IAmazonS3 amazonS3Client, IConfiguration configuration)
        {
            _amazonS3Client = amazonS3Client;
            _configuration = configuration;
        }

        public async Task<List<AddFileResponse>> UploadFiles(string bucketName, IList<IFormFile> files)
        {
            var response = new List<AddFileResponse>();
            var cloudfrondDomain = _configuration["CloudFront:DistributionUrl"];

            foreach (var file in files)
            {
                var uploadRequest = new TransferUtilityUploadRequest { BucketName = bucketName, InputStream = file.OpenReadStream(), Key = file.FileName, CannedACL = S3CannedACL.NoACL };

                using (var fileTransferUtility = new TransferUtility(_amazonS3Client))
                {
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                /* if needed you can get a download link with an expiration time, code below */
                #region Get PreSigned URL
                /*
                    var expiryUrlRequest = new GetPreSignedUrlRequest
                    {
                        BucketName = bucketName,
                        Key = file.FileName,
                        Expires = DateTime.Now.AddDays(1)
                    };

                    var url = _s3Client.GetPreSignedURL(expiryUrlRequest);
                */
                #endregion

                response.Add(new AddFileResponse()
                {
                    FileName = file.FileName,
                    Url = $"{cloudfrondDomain}/{file.FileName}"
                });
            }

            return response;
        }
    }
}
