
using FlightSystemProject;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AuthController : ApiController
    {

        [HttpGet]
        [Authorize]
        [Route("ok")]
        public IHttpActionResult Authenticated() => Ok("Authenticated");

        [HttpGet]
        [Route("notok")]
        public IHttpActionResult NotAuthenticated() => Unauthorized();

        [HttpPost]
        [Route("api/auth")]
        public IHttpActionResult Authenticate([FromBody]User user)
        {
            var instance = FlyingCenterSystem.GetInstance();
    
            instance.GetFacade(user.UserName, user.Password, out ILoginToken iToken, out FacadeBase facade);
            lock (this)
            {
                if (iToken != null)
                {
                    string instanceToken = CreateInstanceToken(user, iToken);
                    return Ok(instanceToken);
                }
            }
            return Unauthorized();
        }

        private string CreateInstanceToken(User user, ILoginToken iToken)
        {

            var claims = new List<Claim>();
            if (iToken as LoginToken<Admin> != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                claims.Add(new Claim("Token", JsonConvert.SerializeObject(iToken)));
            }
            else if (iToken as LoginToken<Customer> != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Customer"));
                claims.Add(new Claim("Token", JsonConvert.SerializeObject(iToken)));
            }
            else if (iToken as LoginToken<AirlineCompany> != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, "AirlineCompany"));
                claims.Add(new Claim("Token", JsonConvert.SerializeObject(iToken)));
            }


            //security key
            string securityKey = "this_is_our_supper_long_security_key_for_token_validation_project_2018_09_07$smesk.in";
            //symmetric security key
            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey));

            //signing credentials
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //create token
            var token = new JwtSecurityToken(
                    issuer: "smesk.in",
                    audience: "readers",
                    expires: DateTime.Now.AddHours(9),
                    signingCredentials: signingCredentials,
                    claims: claims
                );

            //return token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
