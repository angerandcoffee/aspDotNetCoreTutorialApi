using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {

        [HttpGet]
        public JsonResult GetCities()
        {
            return new JsonResult(new List<object>
                {
                    new {id=1, Name="New York"},
                    new {id=2, Name="Antwerp"}
                }
                );
        }
    }
}
