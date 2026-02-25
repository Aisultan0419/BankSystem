using Application.CustomExceptions;
using Application.Interfaces.Services.AppUsers;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Services.AppUserServices
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Guid GetUserId()
        {
            var user = _contextAccessor.HttpContext?.User;
            var userId = user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                    ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId ?? throw new InvalidTokenException());
        }
    }
}
