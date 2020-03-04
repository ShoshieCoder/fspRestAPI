using FlightSystemProject;
using Newtonsoft.Json;
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
    [RoutePrefix("api/company")]
    [Authorize(Roles = "AirlineCompany")]
    public class CompanyController : ApiController
    {
        [ResponseType(typeof(string))]
        [Route("delete/flight", Name = "CancelFlight")]
        [HttpDelete]
        public IHttpActionResult CancelFlight([FromBody]Flight flight)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<AirlineCompany> token, out LoggedinAirlineFacade facade);
            try
            {
                facade.CancelFlight(token, flight);
                return Ok($"flight: {flight.Id} deleted");
            }
            catch(Exception e)
            {
                return Content(HttpStatusCode.NoContent, e.Message);
            }   
        }

        [ResponseType(typeof(Flight))]
        [Route("create/flight", Name = "CreateFlight")]
        [HttpPost]
        public IHttpActionResult CreateFlight([FromBody]Flight flight)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<AirlineCompany> token, out LoggedinAirlineFacade facade);
            try
            {
                facade.CreateFlight(token, flight);
                return CreatedAtRoute("GetFlightById", new { flight.Id }, flight);
            }
            catch(Exception e)
            {
                return Content(HttpStatusCode.NoContent, e.Message);
            }
        }

        [ResponseType(typeof(Flight))]
        [Route("flights/{id}", Name = "GetFlightById")]
        [HttpGet]
        public IHttpActionResult GetFlightById([FromUri]int id)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<AirlineCompany> token, out LoggedinAirlineFacade facade);
            Flight flight = facade.GetFlightById(id);
            if (flight == null)
                return Content(HttpStatusCode.NoContent, "no flights with this id");
            return Content(HttpStatusCode.OK, flight);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("all/flights", Name = "GetAllCompanyFlights")]
        [HttpGet]
        public IHttpActionResult GetAllCompanyFlights()
        {
            var clainsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(clainsIdentity, out LoginToken<AirlineCompany> token, out LoggedinAirlineFacade facade);
            IList<Flight> flights = new List<Flight>();
            flights = facade.GetAllFlights(token);
            if (flights == null)
                return Content(HttpStatusCode.NoContent, "no flights found");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(string))]
        [Route("update/flight", Name = "UpdateFlight")]
        public IHttpActionResult UpdateFlight([FromUri]Flight flight)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            GetFacadeToken(claimsIdentity, out LoginToken<AirlineCompany> token, out LoggedinAirlineFacade facade);
            try
            {
                facade.UpdateFlight(token, flight);
                return Content(HttpStatusCode.OK, $"flight {flight.Id} updated");
            }
            catch(Exception e)
            {
                return Content(HttpStatusCode.NoContent, e.Message);
            }
        }

        private void GetFacadeToken(ClaimsIdentity claimsIdentity, out LoginToken<AirlineCompany> token, out LoggedinAirlineFacade facade)
        {
            IEnumerable<Claim> claims = claimsIdentity.Claims;
            string tt = claims.FirstOrDefault().Value.ToString();
            string getFromClaim = claims.FirstOrDefault(c => c.Type == "Token").Value.ToString();
            LoginToken<AirlineCompany> tokenCompany = JsonConvert.DeserializeObject<LoginToken<AirlineCompany>>(getFromClaim);
            if (tt != "AirlineCompany")
            {
                throw new Exception("identity breach");
            }
            token = tokenCompany;
            facade = new LoggedinAirlineFacade();
        }
    }
}
