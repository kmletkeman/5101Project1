using System;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            // Test the Statistics class with a sample CSV file (or JSON/XML)
            Statistics stats = new Statistics("../../../Data/usacities.csv", "csv");

            // Output the parsed data
            foreach (var city in stats.CityData)
            {
                Console.WriteLine($"City: {city.Key}");
                foreach (var cityInfo in city.Value)
                {
                    Console.WriteLine($" - {cityInfo.Name}, {cityInfo.State}, Population: {cityInfo.Population}, Latitude: {cityInfo.Latitude}");
                }
                Console.WriteLine("\n");
            }

            // Report city information
            string cityName = "Atlanta";
            stats.ReportCityInformation(cityName);

            // Compare population density between two cities
            Console.WriteLine("\nEnter two cities to compare population density:");
            string city1 = "Atlanta";
            string city2 = "Denver";
            stats.ComparePopulationDensity(city1, city2);

            // Report the distance between two cities
            // Get lat/lon from the CityData dictionary
            var cityAInfo = stats.CityData[city1].FirstOrDefault();
            var cityBInfo = stats.CityData[city2].FirstOrDefault();

            if (cityAInfo != null && cityBInfo != null)
            {
                double distance = stats.ReportDistanceBetweenCities(
                    cityAInfo.Latitude, cityAInfo.Longitude,
                    cityBInfo.Latitude, cityBInfo.Longitude);

                Console.WriteLine($"The distance between {city1} and {city2} is {distance} km.");
            }
            else
            {
                Console.WriteLine("One or both of the cities were not found in the data.");
            }

            // Test ReportDistanceFromCapital method
            string testCity = "Miami";
            Console.WriteLine($"\nCalculating distance from {testCity} to its state capital...");
            stats.ReportDistanceFromCapital(testCity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
