using Microsoft.AspNetCore.Mvc;
using APICORE.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace APICORE.Controllers
{
    public class AutenticacionController : Controller
    {
        private readonly string SecretKey;

        public AutenticacionController(IConfiguration config)
        {
            SecretKey = config.GetSection("Settings").GetSection("Secretkey").ToString();
        }

        [HttpPost]
        [Route("api/Validar")]
        public IActionResult Validar([FromBody] Usuario request)
        {
            if (request.usuario == "correo@gmail.com" && request.clave == "123")
            {

                var keyBytes = Encoding.ASCII.GetBytes(SecretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.usuario));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokencreado = tokenHandler.WriteToken(tokenConfig);


                return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });

            }
            else
            {

                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
            }
        }
    }
}
