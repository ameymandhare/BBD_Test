using Microsoft.AspNetCore.Http;
using YIPLCrimping.BAL.Service;
using YIPLCrimping.DAL.Models;

namespace YIPLCrimping.BAL
{
    public class ActivityLoggerHelper
    {
        private readonly SecurityService _securityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivityLoggerHelper(SecurityService securityService, IHttpContextAccessor httpContextAccessor)
        {
            _securityService = securityService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActivity(int? userId, int? plantId, string? request, string? response)
        {
            if (userId == null || userId <= 0)
                return;

            var httpContext = _httpContextAccessor.HttpContext;
            var endpoint = httpContext?.Request.Path.ToString();
            var method = httpContext?.Request.Method;

            var log = new ActivityLog
            {
                UserId = userId,
                PlantId = plantId,
                Request = request,
                Response = response,
                CreatedDate = DateTime.Now,
                ApiEndpoint = endpoint,
                HttpMethod = method
            };

            await _securityService.AddActivityLog(log);
        }
    }
}