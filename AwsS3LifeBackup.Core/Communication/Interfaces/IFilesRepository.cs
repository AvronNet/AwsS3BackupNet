using AwsS3LifeBackup.Core.Communication.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsS3LifeBackup.Core.Communication.Interfaces
{
    public interface IFilesRepository
    {
        Task<bool> FileExists(string bucketName, string filePrefix);
        Task<List<AddFileResponse>> UploadFiles(string bucketName, IList<IFormFile> files, string prefix = "");
        string GetPresignedUrlForFile(string bucketName, string filePrefix);
        Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName, string prefix = "");
        Task DownloadFile(string bucketName, string fileName);
        Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName);
        Task AddJsonObject(string bucketName, AddJsonObjectRequest request);

        Task<GetJsonObjectResponse?> GetJsonObject(string bucketName, string fileName);

        Task AddBase64File(string bucketName, AddBase64FileRequest request);
    }
}
