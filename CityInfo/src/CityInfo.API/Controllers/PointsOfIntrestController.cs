using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;

        public PointsOfInterestController(IMailService mailService, ILogger<PointsOfInterestController> logger)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} dosent exist.");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"There was an error by getting point of intrest with id {cityId}", ex);
                return StatusCode(500, "A problem has happend while handling your request.");
            }
        }
        [HttpGet("{cityId}/pointsofinterest/{poiId}", Name = "GetPointOfIntrest")]
        public IActionResult GetPointOfIntrest(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfIntrest = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (pointOfIntrest == null)
            {
                return NotFound();
            }
            return Ok(pointOfIntrest);
        }
        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfIntrest(int cityId,
            [FromBody] PointOfInterestDtoForCreation pointOfIntrest)
        {
            if (pointOfIntrest == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //to be improved
            int maxPointOfIntrest = CitiesDataStore.Current.Cities
                .SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

            PointOfInterestDto poi = new PointOfInterestDto
            {
                Id = ++maxPointOfIntrest,
                Name = pointOfIntrest.Name,
                Description = pointOfIntrest.Name
            };

            city.PointsOfInterest.Add(poi);

            return CreatedAtRoute("GetPointOfIntrest", new { cityId = cityId, poiId = poi.Id }, poi);
        }

        [HttpPut("{cityId}/pointsofinterest/{poiId}")]
        public IActionResult UpdatePointOfIntrest(int cityId, int poiId,
             [FromBody] PointOfInterestDtoForUpdate pointOfIntrest)
        {
            if (pointOfIntrest == null)
            {
                return BadRequest();
            }

            if (pointOfIntrest.Description == pointOfIntrest.Name)
            {
                ModelState.AddModelError("Description", "Description shoud be diffrent than name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfIntrestToBeUpdated = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (pointOfIntrestToBeUpdated == null)
            {
                return NotFound();
            }

            pointOfIntrestToBeUpdated.Name = pointOfIntrest.Name;
            pointOfIntrestToBeUpdated.Description = pointOfIntrest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsofinterest/{poiId}")]
        public IActionResult PratiallyUpdatePointOfIntrest(int cityId, int poiId,
            [FromBody] JsonPatchDocument<PointOfInterestDtoForUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfIntrestFromStore = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (pointOfIntrestFromStore == null)
            {
                return NotFound();
            }

            var pointOfIntrestToPatch = new PointOfInterestDtoForUpdate
            {
                Name = pointOfIntrestFromStore.Name,
                Description = pointOfIntrestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfIntrestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfIntrestToPatch.Description == pointOfIntrestToPatch.Name)
            {
                ModelState.AddModelError("Description", "Description shoud be diffrent than name.");
            }

            TryValidateModel(pointOfIntrestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            pointOfIntrestFromStore.Name = pointOfIntrestToPatch.Name;
            pointOfIntrestFromStore.Description = pointOfIntrestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsofinterest/{poiId}")]
        public IActionResult DeletePointOfIntrest(int cityId, int poiId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfIntrestToBeDeleted = city.PointsOfInterest.FirstOrDefault(poi => poi.Id == poiId);
            if (pointOfIntrestToBeDeleted == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfIntrestToBeDeleted);

            _mailService.Send("Point of interest was deleted",
                $"Point of interest {pointOfIntrestToBeDeleted.Name} was deleted for the city {city.Name}.");

            return NoContent();
        }

    }
}
