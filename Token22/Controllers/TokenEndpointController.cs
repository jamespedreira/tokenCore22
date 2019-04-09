using Geriatria.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Geriatria.Api.Controllers
{
    public class TokenEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public TokenEndpointController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        [Route("api/token"), HttpPost]
        public async Task<IActionResult> Token([FromForm]TokenRequestDTO request)
        {
            try
            {
                if (request.Grant_Type != "password")
                    return BadRequest(new { error = "unsupported_grant_type" });

                var username = request.Username != null ? request.Username.ToUpper() : request.Username;
                var password = request.Password != null ? request.Password.ToUpper() : request.Password;
                
                if( username != "ADMIN" || password != "ADMIN")
                {
                    throw new ApplicationException("Usuário e senha inválido");
                }

                var key = Convert.FromBase64String("NDk4MUFERUQ2MUQyNDM2QkI1RjRFMEE4Q0Q4QzNBRUI =");
                
                var hoursToExpire = 15;

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        //new Claim(ClaimTypes., 1),
                        new Claim(ClaimTypes.Name, username),
                        //new Claim(ClaimTypes.Name, user.USU_DESCRI),
                        //new Claim(RTEClaimTypes.CompanyId, companyId.ToString()),
                        //new Claim(RTEClaimTypes.UserCompanyId, uepIdenti.ToString()),
                        //new Claim(RTEClaimTypes.IsUserMaster, user.USU_MASTER),
                        //new Claim(RTEClaimTypes.AuthType, authTypeEnum.ToString()),
                    }),
                    Issuer ="",
                    Expires = DateTime.UtcNow.AddHours(hoursToExpire),
                    Audience = "",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var now = DateTime.UtcNow;

                return Ok(new Dictionary<string, object>
                {
                    { "access_token", tokenHandler.WriteToken(token) },
                    { "token_type", "bearer" },
                    { "expires_in", hoursToExpire * 60 * 60 },
                    { "userId", 1},
                    { "name", username },
                    { "ConnectionStringCore", _configuration["CORE"] },
                    { "Secrets", _configuration["CORESecrets"] },
                    //{ "companyId", companyId },
                    //{ "userCompanyId", uepIdenti },
                    //{ "isMaster", user.USU_MASTER },
                    //{ ".issued", now.ToString("ddd, dd MMM yyyy HH:mm:ss", new CultureInfo("en-US")) + " GMT" },
                { ".expires", now.AddHours(hoursToExpire).ToString("ddd, dd MMM yyyy HH:mm:ss", new CultureInfo("en-US")) + " GMT" }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "invalid_grant", error_description = ex.ToString() });
            }
        }
    }
}
