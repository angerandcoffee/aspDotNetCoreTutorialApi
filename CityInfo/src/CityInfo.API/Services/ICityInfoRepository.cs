using System.Collections.Generic;
using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();
        City GetCity(int cityId, bool includePointOfInterest);
        IEnumerable<PointOfInterest> GetPointOfInterestsForCity(int cityId);
        PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId);
        bool CityExists(int cityId);
        void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest);

    }
}
