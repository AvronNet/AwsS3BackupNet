using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AwsS3LifeBackup.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly IFilesRepository _filesRepository;

        public FilesController(IFilesRepository filesRepository)
        {
            _filesRepository = filesRepository;
        }

        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<List<AddFileResponse>>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            if (formFiles == null || !formFiles.Any())
            {
                return BadRequest("The request doesn't contain any files to upload.");
            }

            var response = await _filesRepository.UploadFiles(bucketName, formFiles);

            return response == null ? StatusCode((int)HttpStatusCode.InternalServerError)  : Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/list")]
        [Route("{bucketName}/list/{prefix?}")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName, string? prefix = "")
        {
            var response = await _filesRepository.ListFiles(bucketName, prefix);

            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string fileName)
        {
            await _filesRepository.DownloadFile(bucketName, fileName);

            return Ok();
        }

        [HttpDelete]
        [Route("{bucketName}/delete/{fileName}")]
        public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
        {
            var response = await _filesRepository.DeleteFile(bucketName, fileName);

            return Ok(response);
        }

        [HttpPost]
        [Route("{bucketName}/addjsonobject")]
        public async Task<IActionResult> AddJsonObject(string bucketName, AddJsonObjectRequest request)
        {
            await _filesRepository.AddJsonObject(bucketName, request);

            return Ok();
        }

        [HttpGet]
        [Route("{bucketName}/getjsonobject")]
        public async Task<ActionResult<GetJsonObjectResponse>> GetJsonObject(string bucketName, string fileName)
        {
            var response = await _filesRepository.GetJsonObject(bucketName, fileName);

            return Ok(response);
        }
    }
}
