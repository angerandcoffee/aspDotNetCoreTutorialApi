using System.Collections.Generic;

namespace CityInfo.API.Models
{
    public class CityWithPointOfInterestDto : CityWithoutPointOfInterestDto
    {
        public ICollection<PointOfInterestDto> PointsOfInterest { get; set; }
        = new List<PointOfInterestDto>();
        public int NumberOfPointsOfInterest => PointsOfInterest.Count;
    }
}
