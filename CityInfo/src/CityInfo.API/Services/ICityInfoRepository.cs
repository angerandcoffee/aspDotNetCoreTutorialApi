using System.Collections.Generic;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();
        City GetCity(int cityId, bool includePointOfInterest);
        IEnumerable<PointOfInterest> GetPointOfInterestsForCity(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);
    }
}
