using FlightSystemProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Security.Claims;
using RestAPI.Models;
using System.Web;
using System.Security.Principal;
using System.Web.Http.Description;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/admins")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : ApiController
    {

        [ResponseType(typeof(Admin))]
        [Route("new/admin", Name = "CreateNewAdmin")]
        [HttpPost]
        public IHttpActionResult CreateNewAdmin([FromBody]Admin admin)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            facade.CreateAdmin(token, admin);
            return CreatedAtRoute("GetAdminById", new {token.user.Id}, admin);
        }

        [Route("new/airline", Name = "CreateNewAirline")]
        [HttpPost]
        public IHttpActionResult CreateNewAirlne([FromBody]AirlineCompany company)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            facade.CreateNewAirline(token, company);
            return Ok("created");
        }

        [ResponseType(typeof(Customer))]
        [Route("new/customer", Name = "CreateNewCustomer")]
        [HttpPost]
        public IHttpActionResult CreateNewCustomer([FromBody]Customer customer)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            facade.CreateNewCustomer(token, customer);
            return CreatedAtRoute("GetCustomerByUser", new { username = customer.UserName}, customer);
        }

        [ResponseType(typeof(IList<AirlineCompany>))]
        [Route("find/companies", Name = "GetAllAirlineCompanies")]
        [HttpGet]
        public IHttpActionResult GetAllAirlineCompanies()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            IList<AirlineCompany> companies = new List<AirlineCompany>();
            companies = facade.GetAllAirlineCompanies();
            if (companies.Count < 1)
                return Content(HttpStatusCode.NoContent, "no companies");
            return Content(HttpStatusCode.OK, companies);
        }

        [ResponseType(typeof(IList<Customer>))]
        [Route("find/customers", Name = "GetAllCustomers")]
        [HttpGet]
        public IHttpActionResult GetAllCustomers()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            IList<Customer> customers = new List<Customer>();
            customers = facade.GetAllCustomers();
            if (customers.Count < 1)
                return Content(HttpStatusCode.NoContent, "no customers");
            return Content(HttpStatusCode.OK, customers);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("find/flights", Name = "GetAllFlights")]
        [HttpGet]
        public IHttpActionResult GetAllFlights()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            IList<Flight> flights = new List<Flight>();
            flights = facade.GetAllFlights();
            if (flights.Count < 1)
                return Content(HttpStatusCode.NoContent, "no flights");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(Dictionary<Flight, int>))]
        [Route("find/vacancy", Name = "GetAllFlightsVacancy")]
        [HttpGet]
        public IHttpActionResult GetAllFlightsVacancy()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Dictionary<Flight, int> vacancy = facade.GetAllFlightsVacancy();
            if (vacancy.Count < 1)
                return Content(HttpStatusCode.NoContent, "flights are full");
            return Content(HttpStatusCode.OK, vacancy);
        }

        [ResponseType(typeof(Flight))]
        [Route("find/flights/{id}", Name = "GetFlightById")]
        [HttpGet]
        public IHttpActionResult GetFlightById([FromUri] int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Flight flight = facade.GetFlightById(id);
            if (flight == null)
                return Content(HttpStatusCode.NoContent, "no flight with this id");
            return Content(HttpStatusCode.OK, flight);
        }

        [ResponseType(typeof(AirlineCompany))]
        [Route("find/companies/{id}", Name = "GetCompanyById")]
        [HttpGet]
        public IHttpActionResult GetCompanyById([FromUri] int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            AirlineCompany company = facade.GetAirlineById(id);
            if (company == null)
                return Content(HttpStatusCode.NoContent, "no company with this id");
            return Content(HttpStatusCode.OK, company);
        }

        [ResponseType(typeof(Customer))]
        [Route("find/customers/{id}", Name = "GetCustomerById")]
        [HttpGet]
        public IHttpActionResult GetCustomerById([FromUri]int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Customer customer = facade.GetCustomerById(id);
            if (customer == null)
                return Content(HttpStatusCode.NoContent, "no customer with this id");
            return Content(HttpStatusCode.OK, customer);
        }

        [ResponseType(typeof(Customer))]
        [Route("find/customers/user/{username}", Name = "GetCustomerByUser")]
        [HttpPost]
        public IHttpActionResult GetCustomerByUser([FromBody]string username)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Customer customer = facade.GetCustomerByUsername(username);
            if (customer == null)
                return Content(HttpStatusCode.NoContent, "no customer with this id");
            return Content(HttpStatusCode.OK, customer);
        }


        [ResponseType(typeof(Admin))]
        [Route("find/admins/user", Name = "GetAdminByUser")]
        [HttpGet]
        public IHttpActionResult GetAdminByUser([FromUri]string username)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Admin admin = facade.GetAdminByUsername(username);
            if (admin == null)
                return Content(HttpStatusCode.NoContent, "no admin with this user");
            return Content(HttpStatusCode.OK, admin);
        }

        [ResponseType(typeof(Admin))]
        [Route("find/admins/{id}", Name = "GetAdminById")]
        [HttpGet]
        public IHttpActionResult GetAdminById([FromUri] int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Admin admin = facade.GetAdminById(id);
            if (admin == null)
                return Content(HttpStatusCode.NoContent, "no flight with this id");
            return Content(HttpStatusCode.OK, admin);
        }

        [ResponseType(typeof(string))]
        [Route("delete/companies/{id}", Name = "RemoveAirline")]
        [HttpDelete]
        public IHttpActionResult RemoveAirline([FromUri] int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            AirlineCompany company = facade.GetAirlineById(id);
            if (company == null)
                return Content(HttpStatusCode.NoContent, "no company with this id");
            facade.RemoveAirline(token, company);
            return Ok($"company: {id} deleted");
        }

        [ResponseType(typeof(string))]
        [Route("delete/customers/{id}", Name = "RemoveCustomer")]
        [HttpDelete]
        public IHttpActionResult RemoveCustomer([FromUri] int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);
            Customer customer = facade.GetCustomerById(id);
            if (customer == null)
                return Content(HttpStatusCode.NoContent, "no customer with this id");
            facade.RemoveCustomer(token, customer);
            return Ok($"customer: {id} deleted");
        }

        [ResponseType(typeof(string))]
        [Route("update/companies/{id}", Name = "UpdateCompany")]
        [HttpPut]
        public IHttpActionResult UpdateCompany([FromUri] int id, [FromBody]AirlineCompany company)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);

            try
            {
                facade.UpdateAirlineDetails(token, company);
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NoContent, e.Message);
            }
            
            return Ok($"company: {id} updated");
        }

        [ResponseType(typeof(string))]
        [Route("update/customers/{id}", Name = "UpdateCustomer")]
        [HttpPut]
        public IHttpActionResult UpdateCustomer([FromUri] int id, [FromBody]Customer customer)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade);

            try
            {
                facade.UpdateCustomerDetails(token, customer);
            }
            catch (Exception)
            {
                return Content(HttpStatusCode.NoContent, "no customer with this id");
            }

            return Ok($"customer: {id} updated");
        }



        private void GetFacadeToken(ClaimsIdentity claimsIdentity, out LoginToken<Admin> token, out LoggedInAdministratorFacade facade)
        {
            IEnumerable<Claim> claims = claimsIdentity.Claims;
            string tt = claims.FirstOrDefault().Value.ToString();
            string getFromClaim = claims.FirstOrDefault(c => c.Type == "Token").Value.ToString();
            LoginToken<Admin> tokenAdmin = JsonConvert.DeserializeObject<LoginToken<Admin>>(getFromClaim);
            if (tt != "Administrator")
            {
                throw new Exception("identity breach");
            }
            token = tokenAdmin;
            facade = new LoggedInAdministratorFacade();
        }
    }
}
