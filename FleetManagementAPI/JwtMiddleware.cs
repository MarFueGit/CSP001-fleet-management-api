using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetManagementAPI
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;

        public JwtMiddleware(RequestDelegate next, string secretKey)
        {
            _next = next;
            _secretKey = secretKey;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("JwtMiddleware invoked"); // Log a message indicating that the middleware is invoked

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey); // Use the provided secret key
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

                // If the token is valid, you can retrieve user information from the validated token
                // For example:
                // var userId = int.Parse(validatedToken.Claims.First(x => x.Type == "userId").Value);
                // context.Items["UserId"] = userId;
            }
            catch (SecurityTokenException ex)
            {
                // Log the SecurityTokenException details
                Console.WriteLine($"Token validation failed: {ex.Message}");

                // Token validation failed due to SecurityTokenException
                // Respond with 401 Unauthorized and a JSON message
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new
                {
                    code = StatusCodes.Status401Unauthorized,
                    message = "Unauthorized"
                };
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
                context.Response.WriteAsync(jsonResponse);
            }
            catch (Exception)
            {
                // Other unexpected exceptions
                // Respond with 500 Internal Server Error and a JSON message
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var response = new
                {
                    code = StatusCodes.Status500InternalServerError,
                    message = "Internal Server Error"
                };
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
                context.Response.WriteAsync(jsonResponse);
            }
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder, string secretKey)
        {
            return builder.UseMiddleware<JwtMiddleware>(secretKey);
        }
    }
}