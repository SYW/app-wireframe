using System;
using System.Web;
using System.Web.Mvc;
using Platform.Client.Configuration;
using SywApplicationWireframe.Domain.Configuration;

namespace SywApplicationWireframe.Web.UI.Filters
{
	public class TokenPersistenceFilter : IActionFilter
	{
		private readonly IApplicationSettings _applicationSettings;

		public TokenPersistenceFilter()
		{
			_applicationSettings = new ApplicationSettings();
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			CreateOrUpdateCookie(filterContext);
		}

		private void CreateOrUpdateCookie(ActionExecutingContext filterContext)
		{

			var token = filterContext.HttpContext.Request["token"];

			if (String.IsNullOrEmpty(token)) return;

			var cookie = filterContext.HttpContext.Response.Cookies[_applicationSettings.CookieName];

			if (cookie == null)
			{
				cookie = new HttpCookie(_applicationSettings.CookieName);
				filterContext.HttpContext.Response.Cookies.Add(cookie);
			}
			cookie["token"] = token;
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{

		}
	}
}