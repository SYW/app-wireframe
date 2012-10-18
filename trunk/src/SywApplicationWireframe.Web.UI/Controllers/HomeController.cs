using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Platform.Client;
using Platform.Client.Common.Context;
using Platform.Client.Configuration;
using SywApplicationWireframe.Domain.Configuration;
using SywApplicationWireframe.Domain.Users;

namespace SywApplicationWireframe.Web.UI.Controllers
{
    public class HomeController : Controller
    {
	    private readonly IApplicationSettings _applicationSettings;
		private readonly IPlatformTokenProvider _platformTokenProvider;
	    private readonly IUsersApi _usersApi;
	    private readonly IPlatformSettings _platformSettings;

	    public HomeController()
	    {
		    _applicationSettings = new ApplicationSettings();
		    _platformTokenProvider = new PlatformTokenProvider(new HttpContextProvider());
		    _platformSettings = new PlatformSettings();
		    _usersApi = new UsersApi(new HttpContextProvider());
	    }

	    public ActionResult Index()
        {
			// Making sure the application is configured correctly and the application is called from a canvas
			// Delete this code once this is done
		    try
		    {
			    var appId = _applicationSettings.AppId;
			    var appSecret = _applicationSettings.AppSecret;
		    }
		    catch (ConfigurationErrorsException)
		    {
			    return Redirect("/landing");
		    }

			if (_platformTokenProvider.Get() == null)
				return Redirect("/landing");

		    var currentUser = _usersApi.Current();
		    var currentUserFollowing = _usersApi.GetFollowing(currentUser.Id);

			return View(ToModel(currentUser, currentUserFollowing));
        }

	    private UserModel ToModel(UserDto userDto, IEnumerable<UserDto> following = null)
	    {
		    return new UserModel
			    {
					Id = userDto.Id,
					Name = userDto.Name,
					ImageUrl = userDto.ImageUrl,
					ProfileUrl = new Uri(_platformSettings.SywWebSiteUrl, userDto.ProfileUrl),

					Following = following == null ? 
						new UserModel[0] : 
						following.Select(x => ToModel(x)).ToArray()
			    };
	    }
    }

	public class UserModel
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public Uri ProfileUrl { get; set; }

		public IList<UserModel> Following { get; set; }
	}
}
