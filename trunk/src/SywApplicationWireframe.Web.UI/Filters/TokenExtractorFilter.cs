using System.Web.Mvc;
using Platform.Client;
using Platform.Client.Common.Context;

namespace SywApplicationWireframe.Web.UI.Filters
{
	public class TokenExtractingFilter : ActionFilterAttribute
	{
		private readonly IPlatformTokenProvider _platformToken;

		public TokenExtractingFilter()
		{
			_platformToken = new PlatformTokenProvider(new HttpContextProvider());
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);

			var token = filterContext.HttpContext.Request.QueryString["token"];

			if (string.IsNullOrEmpty(token))
				return;

			_platformToken.Set(token);
		}
	}
}
