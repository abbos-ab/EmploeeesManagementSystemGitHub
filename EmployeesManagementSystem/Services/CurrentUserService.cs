using System.Security.Claims;

namespace EmployeesManagementSystem.Services
{
    public class CurrentUserService(IHttpContextAccessor _contextAccessor)
    {
        public Guid GetUserIdFromToken()
        {
            var userId = _contextAccessor.HttpContext?.User;
            var userIdClaim = userId?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim not found in the token.");

            return Guid.Parse(userIdClaim.Value);
        }
    }
}
