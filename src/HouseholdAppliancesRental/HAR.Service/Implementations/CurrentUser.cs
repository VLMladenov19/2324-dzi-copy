using HAR.Service.Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HAR.Service.Implementations
{
    /// <summary>
    /// Stores the current user's id
    /// </summary>
    public class CurrentUser : ICurrentUser
    {
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            this.UserId = httpContextAccessor
                .HttpContext
                .User
                .Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                .Value;

            //this.UserId = httpContextAccessor
            //    .HttpContext
            //    .User
            //    .FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string? UserId { get; }
    }
}
