using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController : Controller
    {
        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
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
            [FromBody] PointOfIntrestDtoForCreation pointOfIntrest)
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

            PointOfIntrestDto poi = new PointOfIntrestDto
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
             [FromBody] PointOfIntrestDtoForUpdate pointOfIntrest)
        {
            if (pointOfIntrest == null)
            {
                return BadRequest();
            }

            if(pointOfIntrest.Description == pointOfIntrest.Name)
            {
                ModelState.AddModelError("Description", "Description shoud be diffrent than name.");
            }

            if(!ModelState.IsValid)
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
            [FromBody] JsonPatchDocument<PointOfIntrestDtoForUpdate> patchDoc)
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

            var pointOfIntrestToPatch = new PointOfIntrestDtoForUpdate
            {
                Name = pointOfIntrestFromStore.Name,
                Description = pointOfIntrestFromStore.Description
            };

            patchDoc.ApplyTo(pointOfIntrestToPatch, ModelState);

            if(!ModelState.IsValid)
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


    }
}
