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
namespace RestAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/customers")]
    [Authorize(Roles = "Customer")]
    public class CustomerController : ApiController
    {
        [ResponseType(typeof(IList<Ticket>))]
        [Route("find/ticktes", Name = "GetAllTickets")]
        [HttpGet]
        public IHttpActionResult GetAllTickets()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            IList<Ticket> ticktes = new List<Ticket>();
            ticktes = facade.GetAllTickets(token);
            if (ticktes.Count < 1)
                return Content(HttpStatusCode.NoContent, "no ticktes");
            return Content(HttpStatusCode.OK, ticktes);
        }

        [ResponseType(typeof(int))]
        [Route("delete/customers/{id}", Name = "CancelTicket")]
        [HttpDelete]
        public IHttpActionResult CancelTicket([FromBody]Ticket ticket)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            try
            {
                facade.CancelTicket(token, ticket);
                return Ok($"ticket: {ticket.Id} canceled");
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NoContent, e.Message);
            }
        }

        [ResponseType(typeof(Customer))]
        [Route("find/user/{username}", Name = "GetCustomerByUserame")]
        [HttpGet]
        public IHttpActionResult GetCustomerByUserame([FromUri]string username)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            Customer customer = facade.GetCustomerByUserame(username);
            if (customer == null)
                return Content(HttpStatusCode.NoContent, "no customer with this user");
            return Content(HttpStatusCode.OK, customer);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("find/flights/{username}", Name = "GetAllMyFlights")]
        [HttpGet]
        public IHttpActionResult GetAllMyFlights([FromUri]string username)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            IList<Flight> flights = new List<Flight>();
            flights = facade.GetAllMyFlights(token);
            if (flights.Count < 1)
                return Content(HttpStatusCode.NoContent, "no flights for this user");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(Dictionary<Flight, int>))]
        [Route("find/flights/vacancy", Name = "GetFlightsVacancy")]
        [HttpGet]
        public IHttpActionResult GetAllFlightsVacancy()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            Dictionary<Flight, int> flights = new Dictionary<Flight, int>();
            flights = facade.GetAllFlightsVacancy();
            if (flights.Count < 1)
                return Content(HttpStatusCode.NoContent, "not flights");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("find/flights", Name = "GetFlightsByDepatrureDate")]
        [HttpGet]
        public IHttpActionResult GetFlightsByDepatrureDate([FromBody]DateTime date)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            IList<Flight> flights = new List<Flight>();
            flights = facade.GetFlightsByDepatrureDate(date);
            if (flights.Count < 1)
                return Content(HttpStatusCode.NoContent, "not flights for this date");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("find/flights", Name = "GetFlightsByDestinationCountry")]
        [HttpGet]
        public IHttpActionResult GetFlightsByDestinationCountry([FromUri]int countrycode)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            IList<Flight> flights = new List<Flight>();
            flights = facade.GetFlightsByDestinationCountry(countrycode);
            if (flights.Count < 1)
                return Content(HttpStatusCode.NoContent, "not flights for this date");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(Ticket))]
        [Route("buy", Name = "PurchaseTicket")]
        [HttpGet]
        public IHttpActionResult PurchaseTicket([FromBody]Flight flight)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade);
            Ticket ticket = facade.PurchaseTicket(token, flight);
            if(ticket == null)
                return Content(HttpStatusCode.NoContent, "not tickets for this flight");
            return Content(HttpStatusCode.OK, ticket);
        }


        private void GetFacadeToken(ClaimsIdentity claimsIdentity, out LoginToken<Customer> token, out LoggedInCustomerFacade facade)
        {
            IEnumerable<Claim> claims = claimsIdentity.Claims;
            string tt = claims.FirstOrDefault().Value.ToString();
            string getFromClaim = claims.FirstOrDefault(c => c.Type == "Token").Value.ToString();
            LoginToken<Customer> tokenCustomer = JsonConvert.DeserializeObject<LoginToken<Customer>>(getFromClaim);
            if (tt != "Customer")
            {
                throw new Exception("identity breach");
            }
            token = tokenCustomer;
            facade = new LoggedInCustomerFacade();
        }
    }
}
