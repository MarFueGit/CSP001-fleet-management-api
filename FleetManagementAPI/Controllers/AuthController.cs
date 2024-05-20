using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FleetManagementAPI.Controllers
{
    [Route("api/auth")]
    [Description("Auth operations")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDbContext _context;
        private readonly IHashPassword _hashPassword;
        private readonly IConfiguration _configuration; // Inject IConfiguration
        private IDbContext object1;
        private IHashPassword object2;

        public AuthController(IDbContext object1, IHashPassword object2)
        {
            this.object1 = object1;
            this.object2 = object2;
        }

        public AuthController(IDbContext context, IHashPassword hashPassword, IConfiguration configuration)
        {
            _context = context;
            _hashPassword = hashPassword;
            _configuration = configuration; // Assign IConfiguration
        }

        [HttpPost("login")]
        [Description("Create authentication token")]
        public async Task<ActionResult<dynamic>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.email);

            if (user == null || !_hashPassword.VerifyPasswordHash(loginDto.password, user.Password))
            {
                return BadRequest("Invalid email or password");
            }

            var token = _hashPassword.GenerateJwtToken(user, _configuration); // Pass IConfiguration to GenerateJwtToken

            return new
            {
                user.Id,
                user.Name,
                Token = token
            };
        }
    }

    public interface IHashPassword
    {
        string EncryptPassword(string password);
        string GenerateJwtToken(User user, IConfiguration configuration);
        bool VerifyPasswordHash(string password, string hashedPassword);
    }
}
