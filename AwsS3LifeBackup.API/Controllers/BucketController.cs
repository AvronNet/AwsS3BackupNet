using Microsoft.AspNetCore.Mvc;

namespace AwsS3LifeBackup.API.Controllers
{
    public class BucketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
