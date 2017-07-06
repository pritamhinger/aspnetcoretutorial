using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;

namespace HelloWorldAspNetCore.Global
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var cultureQuery = context.Request.Query["culture"];
            if (!string.IsNullOrWhiteSpace(cultureQuery)) {
                var culture = new CultureInfo(cultureQuery);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }

            return this._next(context);
        }
    }
}
