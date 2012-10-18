using System.Web.Mvc;

namespace SywApplicationWireframe.Web.UI.Controllers
{
    public class LoginController : Controller
    {
		[RequireHttps]
        public ActionResult Index()
        {
            return View();
        }

    }
}
