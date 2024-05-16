using FleetManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FleetManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDbContext _context;
        private readonly IHashPassword _hashPassword;

        public AuthController(IDbContext context, IHashPassword hashPassword)
        {
            _context = context;
            _hashPassword = hashPassword;
        }

        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.email);

            if (user == null || !_hashPassword.VerifyPasswordHash(loginDto.password, user.Password))
            {
                return BadRequest("Invalid email or password");
            }

            var token = _hashPassword.GenerateJwtToken(user);

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
        string GenerateJwtToken(User user);
        bool VerifyPasswordHash(string password, string hashedPassword);
    }
}
