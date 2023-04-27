using Microsoft.AspNetCore.Mvc;

namespace AwsS3LifeBackup.API.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
