using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointOfInterest)
        {
            if (includePointOfInterest)
            {
                return _context.Cities.Include(c=>c.PointsOfInterest).FirstOrDefault(c => c.Id == cityId);
            }

            return _context.Cities.FirstOrDefault(c => c.Id == cityId);
        }

        public IEnumerable<PointOfInterest> GetPointOfInterestsForCity(int cityId)
        {
            return _context.Cities.FirstOrDefault(c => c.Id == cityId).PointsOfInterest.ToList();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context.Cities.FirstOrDefault(c => c.Id == cityId)
                .PointsOfInterest.FirstOrDefault(p => p.Id == pointOfInterestId);
        }
    }
}
