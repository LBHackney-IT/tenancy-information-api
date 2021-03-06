using Microsoft.AspNetCore.Mvc;

namespace TenancyInformationApi.Versioning
{
    public static class ApiVersionExtensions
    {
        public static string GetFormattedApiVersion(this ApiVersion apiVersion)
        {
            return apiVersion != null ? $"v{apiVersion.ToString()}" : null;
        }
    }
}
