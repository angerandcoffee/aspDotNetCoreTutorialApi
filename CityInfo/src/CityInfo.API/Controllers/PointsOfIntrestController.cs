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
                    _logger.LogInformation($"City with id {cityId} doesn't exist.");
                    return NotFound();
                }

                var pointOfInterestEntities = _cityInfoRepository.GetPointOfInterestsForCity(cityId);
                var result = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestEntities);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"There was an error by getting point of interest with id {cityId}", ex);
                return StatusCode(500, "A problem has happened while handling your request.");
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

            var poi = Mapper.Map<PointOfInterest>(pointOfIntrest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, poi);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdPointOfInterestToReturn = Mapper.Map<PointOfInterestDto>(poi);

            return CreatedAtRoute("GetPointOfIntrest", new { cityId = cityId, poiId = createdPointOfInterestToReturn.Id }, 
                createdPointOfInterestToReturn);
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
                ModelState.AddModelError("Description", "Description should be different than name.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, poiId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(pointOfIntrest, pointOfInterestEntity);
            
            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

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

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterestForCity(cityId, poiId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }
            ;
            var pointOfInterestToPatch = Mapper.Map<PointOfInterestDtoForUpdate>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "Description should be different than name.");
            }

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

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
