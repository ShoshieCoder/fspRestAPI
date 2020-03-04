using FlightSystemProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace WebApi.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/find")]
    public class AnonymousController : ApiController
    {
        private IAnonymousUserFacade facade = new AnonymousUserFacade();
        // GET: api/Anonymous

        [ResponseType(typeof(IList<AirlineCompany>))]
        [Route("companies", Name = "GetAllCompanies")]
        [HttpGet]
        public IHttpActionResult GetAllCompanies()
        {
            IList<AirlineCompany> companies = facade.GetAllAirlineCompanies();
            if (companies == null)
                return Content(HttpStatusCode.NoContent, "No companies");
            return Content(HttpStatusCode.OK, companies);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("flights", Name = "GetAllFlights")]
        [HttpGet]
        public IHttpActionResult GetAllFlights()
        {
            IList<Flight> flights = facade.GetAllFlights();
            if (flights == null)
                return Content(HttpStatusCode.NoContent, "No flights");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(Dictionary<Flight, int>))]
        [Route("flights/vacancy", Name = "GetAllVacancy")]
        [HttpGet]
        public IHttpActionResult GetAllVacancy()
        {
            Dictionary<Flight, int> vacancy = facade.GetAllFlightsVacancy();
            if (vacancy == null)
                return Content(HttpStatusCode.NoContent, "No flights");
            return Content(HttpStatusCode.OK, vacancy);
        }

        [ResponseType(typeof(Flight))]
        [Route("flights/{id}", Name = "GetFlightById")]
        [HttpGet]
        public IHttpActionResult GetFlightById([FromUri]int id)
        {
            Flight flight = facade.GetFlightById(id);
            if (flight == null)
                return Content(HttpStatusCode.NoContent, "No flight with with this id");
            return Content(HttpStatusCode.OK, flight);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("flights/country/{id}", Name = "GetFlightsByDestinationCountry")]
        [HttpGet]
        public IHttpActionResult GetFlightsByDestinationCountry([FromUri]int id)
        {
            IList<Flight> flights = facade.GetFlightsByDestinationCountry(id);
            if (flights == null)
                return Content(HttpStatusCode.NoContent, "No flights with this country code");
            return Content(HttpStatusCode.OK, flights);
        }

        [ResponseType(typeof(IList<Flight>))]
        [Route("flights/country/departure/{date}", Name = "GetFlightsByDepatrureDate")]
        [HttpGet]
        public IHttpActionResult GetFlightsByDepatrureDate([FromUri]DateTime date)
        {
            IList<Flight> flights = facade.GetFlightsByDepatrureDate(date);
            if (flights == null)
                return Content(HttpStatusCode.NoContent, "No flights with this departure date");
            return Content(HttpStatusCode.OK, flights);
        }

    }
}

