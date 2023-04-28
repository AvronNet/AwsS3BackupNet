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
    }
}
