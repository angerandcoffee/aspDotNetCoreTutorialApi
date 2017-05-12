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
                    Description = "The Big Apple"
                },
                new CityDto
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "Diamonds and so on"
                },
                new CityDto
                {
                    Id = 3,
                    Name = "Wroclaw",
                    Description = "Germans call it Breslau"
                }
            };
        }
    }
}
