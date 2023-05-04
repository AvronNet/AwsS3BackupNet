using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        /// <summary>
        /// Determines whether a file exists within the specified bucket
        /// </summary>
        /// <param name="bucketName">The name of the bucket to search</param>
        /// <param name="filePrefix">Match files that begin with this prefix</param>
        /// <returns>True if the file exists</returns>
        public async Task<bool> FileExists(string bucketName, string filePrefix)
        {
            var request = new ListObjectsRequest
            {
                BucketName = bucketName,
                Prefix = filePrefix,
                MaxKeys = 1
            };

            var response = await _amazonS3Client.ListObjectsAsync(request);

            return response.S3Objects.Any();
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

                    var url = _amazonS3Client.GetPreSignedURL(expiryUrlRequest);
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

        public async Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName, string prefix = "")
        {
            var responses = await _amazonS3Client.ListObjectsAsync(bucketName, prefix);

            return responses.S3Objects.Select(b => new ListFilesResponse
            {
                BucketName = b.BucketName,
                Key = b.Key,
                Owner = b.Owner.DisplayName,
                Size = b.Size
            });
        }

        public async Task DownloadFile(string bucketName, string fileName)
        {
            var pathAndFileName = $"C:\\S3Temp\\{fileName}";

            var downloadRequest = new TransferUtilityDownloadRequest
            {
                BucketName = bucketName,
                Key = fileName,
                FilePath = pathAndFileName
            };

            using (var transferUtility = new TransferUtility(_amazonS3Client))
            {
                await transferUtility.DownloadAsync(downloadRequest);
            }
        }

        public async Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName)
        {
            var multiObjectDeleteRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName
            };

            multiObjectDeleteRequest.AddKey(fileName);

            var response = await _amazonS3Client.DeleteObjectsAsync(multiObjectDeleteRequest);

            return new DeleteFileResponse
            {
                NumberOfDeletedObjects = response.DeletedObjects.Count
            };
        }

        public async Task AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            var createdOnUtc = DateTime.UtcNow;

            var s3Key = $"{createdOnUtc:yyyy}/{createdOnUtc:MM}/{createdOnUtc:dd}/{request.Id}";

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = s3Key,
                ContentBody = JsonSerializer.Serialize(request)
            }; 

            await _amazonS3Client.PutObjectAsync(putObjectRequest);
        }

        public async Task<GetJsonObjectResponse?> GetJsonObject(string bucketName, string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = fileName
            };

            var response = await _amazonS3Client.GetObjectAsync(request);

            using (var reader = new StreamReader(response.ResponseStream))
            {
                var contents = reader.ReadToEnd();
                return string.IsNullOrWhiteSpace(contents) ? null : JsonSerializer.Deserialize<GetJsonObjectResponse>(contents);
            }
        }

        public async Task AddBase64File(string bucketName, AddBase64FileRequest request)
        {
            var fileContentHeaders = request.Base64Content.Substring(0, request.Base64Content.IndexOf("base64,") + 7);
            var fileContent = request.Base64Content.Replace(fileContentHeaders, string.Empty);

            Regex regex = new Regex("(?<= data :)(.*)(?=; base64)");
            Match match = regex.Match(request.Base64Content);
            var contentType = match.Value;

            byte[] bytes = Convert.FromBase64String(fileContent);

            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = request.FileName,                
                ContentType = contentType
            };

            using var ms = new MemoryStream(bytes);
            putObjectRequest.InputStream = ms;
            await _amazonS3Client.PutObjectAsync(putObjectRequest);

        }
    }
}
