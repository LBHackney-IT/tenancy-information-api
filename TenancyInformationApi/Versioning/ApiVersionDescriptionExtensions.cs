using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace TenancyInformationApi.Versioning
{
    public static class ApiVersionDescriptionExtensions
    {
        public static string GetFormattedApiVersion(this ApiVersionDescription apiVersionDescription)
        {
            return apiVersionDescription != null ? $"v{apiVersionDescription.ApiVersion.ToString()}" : null;
        }
    }
}

