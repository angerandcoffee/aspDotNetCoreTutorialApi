using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        public CitiesDataStore()
        {
            Cities = new List<CityDto>
            {
                new CityDto
                {
                    Id =1,
                    Name = "New York City",
                    Description = "The Big Apple",
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "Big park"
                        },
                        new PointOfInterestDto
                        {
                            Id = 2,
                            Name = "Empire state building",
                            Description = "Big builing"
                        }
                    }
                },
                new CityDto
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "Diamonds and so on",
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            Id = 3,
                            Name = "Cathedral",
                            Description = "Never ending story."
                        }
                    }
                },
                new CityDto
                {
                    Id = 3,
                    Name = "Wroclaw",
                    Description = "Germans call it Breslau",
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto
                        {
                            Id = 4,
                            Name = "Ostrow Tumski",
                            Description = "An island with tons of churches"
                        },
                        new PointOfInterestDto
                        {
                            Id = 5,
                            Name = "Hydropolis",
                            Description = "Knowledge center about water"
                        }
                    }
                }
            };
        }
    }
}
