using System;
using System.Configuration;
using System.Web.Mvc;
using Platform.Client.Configuration;
using SywApplicationWireframe.Domain.Configuration;

namespace SywApplicationWireframe.Web.UI.Controllers
{
    public class LandingController : Controller
    {
	    private readonly IApplicationSettings _applicationSettings;
	    private readonly IPlatformSettings _platformSettings;

	    public LandingController()
	    {
		    _applicationSettings = new ApplicationSettings();
		    _platformSettings = new PlatformSettings();
	    }

	    public ActionResult Index()
	    {
		    var model = new LandingModel();

		    try
		    {
			    model.AppId = _applicationSettings.AppId;
			    model.ShopYourWayUrl = _platformSettings.SywWebSiteUrl;
		    }
		    catch (ConfigurationErrorsException)
		    {
			    model.DisplayProperConfiguraionMessage = true;
		    }

			return View(model);
        }

    }

	public class LandingModel
	{
		public bool DisplayProperConfiguraionMessage { get; set; }
		public long AppId { get; set; }
		public Uri ShopYourWayUrl { get; set; }

		public Uri LoginPageUrl { get { return new Uri(ShopYourWayUrl, "app/" + AppId + "/login"); } }
		public Uri LandingPageUrl { get { return new Uri(ShopYourWayUrl, "app/" + AppId + "/l"); } }
	}
}
