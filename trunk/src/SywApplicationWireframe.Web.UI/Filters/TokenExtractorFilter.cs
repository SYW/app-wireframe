using System;
using System.Web.Mvc;
using Platform.Client;
using Platform.Client.Common.Context;
using Platform.Client.Configuration;
using SywApplicationWireframe.Domain.Configuration;

namespace SywApplicationWireframe.Web.UI.Filters
{
	public class TokenExtractingFilter : ActionFilterAttribute
	{
		private readonly IPlatformTokenProvider _platformToken;
		private readonly IApplicationSettings _applicationSettings;

		public TokenExtractingFilter()
		{
			_applicationSettings = new ApplicationSettings();
			_platformToken = new PlatformTokenProvider(new HttpContextProvider());
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			var token = GetToken(filterContext);

			if (string.IsNullOrEmpty(token))
				return;

			_platformToken.Set(token);
		}

		private string GetToken(ActionExecutingContext filterContext)
		{
			var token = filterContext.HttpContext.Request.QueryString["token"];
			if (!String.IsNullOrEmpty(token))
				return token;

			token = filterContext.HttpContext.Request.Form["token"];
			if (!String.IsNullOrEmpty(token))
				return token;

			var cookie = filterContext.HttpContext.Request.Cookies[_applicationSettings.CookieName];
			if (cookie != null)
				token = cookie["token"];

			return token;
		}
	}
}
