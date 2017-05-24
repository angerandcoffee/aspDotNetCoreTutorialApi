using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Entities;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ICityInfoRepository cityInfoRepository, IMailService mailService, ILogger<PointsOfInterestController> logger)
        {
            _cityInfoRepository = cityInfoRepository;
            _logger = logger;
            _mailService = mailService;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} dosent exist.");
                    return NotFound();
                }

                var pointOfInterestEntities = _cityInfoRepository.GetPointOfInterestsForCity(cityId);
                var result = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestEntities);

                return Ok(result);
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
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, poiId);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<PointOfInterestDto>(pointOfInterest);

            return Ok(result);
        }
        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfIntrest(int cityId,
            [FromBody] PointOfInterestDtoForCreation pointOfIntrest)
        {
            if (pointOfIntrest == null)
            {
                return BadRequest();
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            PointOfInterest poi = Mapper.Map<PointOfInterest>(pointOfIntrest);

           

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
