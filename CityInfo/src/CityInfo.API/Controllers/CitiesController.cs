using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {

        [HttpGet]
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
