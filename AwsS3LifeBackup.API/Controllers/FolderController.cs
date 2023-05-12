using AwsS3LifeBackup.Core.Communication.Bucket;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace AwsS3LifeBackup.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : Controller
    {
        private readonly IFolderRepository _folderRepository;

        public FolderController(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        [HttpPost]
        [Route("create/{folderName}/{folderPrefix?}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket(string folderName, string folderPrefix = "")
        {
            var folderNameDecoded = HttpUtility.UrlDecode(folderName.Trim());
            var folderPrefixDecoded = HttpUtility.UrlDecode(folderPrefix.Trim());
            var result = await _folderRepository.CreateFolder(folderNameDecoded, folderPrefixDecoded);

            return result ? Ok() : BadRequest();
        }
    }
}
