using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace WebApi.Extensions.Localization
{
    public class RouteDataRequestCultureProviderExtension : RequestCultureProvider
    {
        public int IndexOfCulture;
        public int IndexofUICulture;

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            string uiCulture;
            string culture = uiCulture = httpContext.Request.Path.Value.Split('/')[IndexOfCulture]?.ToString();

            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}
