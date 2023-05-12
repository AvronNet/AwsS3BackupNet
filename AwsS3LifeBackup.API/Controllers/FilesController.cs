using AwsS3LifeBackup.Core.Communication.Files;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

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
        [Route("{bucketName}/add/{prefix?}")]
        public async Task<ActionResult<List<AddFileResponse>>> AddFiles(string bucketName, IList<IFormFile> formFiles, string prefix = "")
        {
            if (formFiles == null || !formFiles.Any())
            {
                return BadRequest("The request doesn't contain any files to upload.");
            }
            var decodedPrefix = HttpUtility.UrlDecode(prefix);
            var response = await _filesRepository.UploadFiles(bucketName, formFiles, decodedPrefix);

            return response == null ? StatusCode((int)HttpStatusCode.InternalServerError)  : Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/list")]
        [Route("{bucketName}/list/{prefix?}")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName, string prefix = "")
        {
            var decodedPrefix = HttpUtility.UrlDecode(prefix);
            var response = await _filesRepository.ListFiles(bucketName, decodedPrefix);

            return Ok(response);
        }

        [HttpGet]
        [Route("{bucketName}/download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string fileName)
        {
            var fileNameDecoded = HttpUtility.UrlDecode(fileName);
            await _filesRepository.DownloadFile(bucketName, fileNameDecoded);

            return Ok();
        }

        [HttpDelete]
        [Route("{bucketName}/delete/{fileName}")]
        public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
        {
            var fileNameDecoded = HttpUtility.UrlDecode(fileName);
            var response = await _filesRepository.DeleteFile(bucketName, fileNameDecoded);

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

        [HttpPost]
        [Route("{bucketName}/addbase64file")]
        public async Task<IActionResult> AddJsonObject(string bucketName, AddBase64FileRequest request)
        {
            if(!request.Base64Content.Contains("base64"))
            {
                return BadRequest("Base64Content property must contain the full base64 file representation!");
            }
            await _filesRepository.AddBase64File(bucketName, request);

            return Ok();
        }

        [HttpGet]
        [Route("{bucketName}/get-presigned-url/{fileName}")]
        public ActionResult<string> GetPresignedUrl(string bucketName, string fileName)
        {
            var fileNameDecoded = HttpUtility.UrlDecode(fileName);
            var response = _filesRepository.GetPresignedUrlForFile(bucketName, fileNameDecoded);

            return Ok(response);
        }
    }
}
