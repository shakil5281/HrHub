using HrHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HrHubAPI.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PermissionMiddleware> _logger;

        public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            // Check if the endpoint requires permission-based authorization
            var endpoint = context.GetEndpoint();
            var permissionAttribute = endpoint?.Metadata.GetMetadata<RequirePermissionAttribute>();

            if (permissionAttribute != null)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: User not authenticated");
                    return;
                }

                var hasPermission = await permissionService.HasPermissionAsync(
                    userId, 
                    permissionAttribute.PermissionCode, 
                    permissionAttribute.Resource);

                if (!hasPermission)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync($"Forbidden: User does not have permission '{permissionAttribute.PermissionCode}'");
                    return;
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Attribute to mark endpoints that require specific permissions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequirePermissionAttribute : Attribute
    {
        public string PermissionCode { get; }
        public string? Resource { get; }

        public RequirePermissionAttribute(string permissionCode, string? resource = null)
        {
            PermissionCode = permissionCode;
            Resource = resource;
        }
    }

    /// <summary>
    /// Extension method to register permission middleware
    /// </summary>
    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }
}
