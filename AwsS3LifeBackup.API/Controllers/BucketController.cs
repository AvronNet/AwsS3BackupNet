using AwsS3LifeBackup.Core.Communication.Bucket;
using AwsS3LifeBackup.Core.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AwsS3LifeBackup.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketController : Controller
    {
        private readonly IBucketRepository _bucketRepository;

        public BucketController(IBucketRepository bucketRepository)
        {
            _bucketRepository = bucketRepository;
        }

        [HttpPost]
        [Route("create/{bucketName}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateS3Bucket(string bucketName)
        {
            var bucketExists = await _bucketRepository.DoesS3BucketExist(bucketName);

            if (bucketExists)
            {
                return BadRequest("S3 bucket already exists");
            }

            var result = await _bucketRepository.CreateBucket(bucketName);

            return result == null ? BadRequest() : Ok(result);
        }
    }
}
