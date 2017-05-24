using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            var result = new List<CityWithoutPointOfInterestDto>();

            foreach (var city in cityEntities)
            {
                result.Add(new CityWithoutPointOfInterestDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointOfInterest = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePointOfInterest);

            if (city == null)
            {
                return NotFound();
            }

            if (includePointOfInterest)
            {
                var cityResult = new CityWithPointOfInterestDto
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                };

                foreach (var poi in city.PointsOfInterest)
                {
                    cityResult.PointsOfInterest.Add(new PointOfInterestDto
                    {
                        Id = poi.Id,
                        Name = poi.Name,
                        Description = poi.Description
                    });
                }
                return Ok(cityResult);
            }

            var cityResultWithoutPointsOfInterest = new CityWithoutPointOfInterestDto
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description
            };

            return Ok(cityResultWithoutPointsOfInterest);
        }
    }
}
