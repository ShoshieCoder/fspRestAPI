using FlightSystemProject;
using Newtonsoft.Json;
using RestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace RestAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class AccountController : ApiController
    {
        [ResponseType(typeof(string))]
        [Route("api/register", Name = "Register")]
        [HttpPost]
        public IHttpActionResult Register([FromBody]Customer newCustomer)
        {
            LoggedInAdministratorFacade facade = new LoggedInAdministratorFacade();
            LoginToken<Admin> token = new LoginToken<Admin>();
            try
            {
                facade.CreateNewCustomer(token, newCustomer);
                return Content(HttpStatusCode.Created, "user created please login");
            }
            catch(Exception e)
            {
                return Content(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize(Roles = "Administrator,Customer,ArilineCompany")]
        [ResponseType(typeof(string))]
        [Route("api/update", Name = "Update")]
        [HttpPut]
        public IHttpActionResult UpdateUser([FromBody]IUser user)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out ILoginToken token, out FacadeBase facade);

            if (token as LoginToken<Admin> != null)
            {
                var adminUser = user as Admin;
                var adminFacade = facade as LoggedInAdministratorFacade;
                adminFacade.Update(adminUser);
                return Ok("Admin updated");
            }
            else if (token as LoginToken<Customer> != null)
            {
                var customer = user as Customer;
                var AdminFacade = facade as LoggedInAdministratorFacade;
                var adminToken = new LoginToken<Admin>();
                AdminFacade.UpdateCustomerDetails(adminToken, customer);
                return Ok("Customer updated");
            }
            else if (token as LoginToken<AirlineCompany> != null)
            {
                var company = user as AirlineCompany;
                var AdminFacade = facade as LoggedInAdministratorFacade;
                var adminToken = new LoginToken<Admin>();
                AdminFacade.UpdateAirlineDetails(adminToken, company);
                return Ok("Airline company updated");
            }
            return Content(HttpStatusCode.NotAcceptable, "identity breach");
        }

        private void GetFacadeToken(ClaimsIdentity claimsIdentity, out ILoginToken token, out FacadeBase facade)
        {
            IEnumerable<Claim> claims = claimsIdentity.Claims;
            string tt = claims.FirstOrDefault().Value.ToString();
            string getFromClaim = claims.FirstOrDefault(c => c.Type == "Token").Value.ToString();

            if(tt == "Administrator")
            {
                LoginToken<Admin> tokenFromClaim = JsonConvert.DeserializeObject<LoginToken<Admin>>(getFromClaim);
                token = tokenFromClaim;
                facade = new LoggedInAdministratorFacade();
            }
            else if(tt == "Customer")
            {
                LoginToken<Customer> tokenFromClaim = JsonConvert.DeserializeObject<LoginToken<Customer>>(getFromClaim);
                token = tokenFromClaim;
                facade = new LoggedInAdministratorFacade();
            }
            else if(tt == "AirlineCompany")
            {
                LoginToken<AirlineCompany> tokenFromClaim = JsonConvert.DeserializeObject<LoginToken<AirlineCompany>>(getFromClaim);
                token = tokenFromClaim;
                facade = new LoggedInAdministratorFacade();
            }
  
            throw new Exception("invalid request");
        }
    }
}
