using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class Statistics
{
    // Dictionary to hold city information
    public Dictionary<string, List<CityInfo>> CityData { get; private set; }

    // Constructor to initialize the city data by calling the DataModeler
    public Statistics(string fileName, string fileType)
    {
        DataModeler modeler = new DataModeler();
        CityData = modeler.ParseFile(fileName, fileType); // Parse the data and store in the dictionary
    }

    // Method reporets all the city info in the dictionary for the selected city name
    public void ReportCityInformation(string cityName)
    {
        if (CityData.ContainsKey(cityName))
        {
            Console.WriteLine($"Information for {cityName}:\n");
            foreach (var cityInfo in CityData[cityName])
            {
                // Output the information for each city (multiple entries per name)
                Console.WriteLine(cityInfo.ToString());
            }
        }
        else
        {
            Console.WriteLine($"City {cityName} not found.");
        }
    }

    // Method reports the population density for two cities and compares them indicating the city with the higher population density
    public void ComparePopulationDensity(string cityName1, string cityName2)
    {
        // Ensure both cities exist in the dictionary
        if (CityData.ContainsKey(cityName1) && CityData.ContainsKey(cityName2))
        {
            // Get the first city's information (if city exists in multiple states we grab the first one only)
            var city1 = CityData[cityName1].FirstOrDefault();
            var city2 = CityData[cityName2].FirstOrDefault();

            if (city1 != null && city2 != null)
            {
                // Compare population density and output which city has a higher density
                Console.WriteLine($"Population Density Comparison between {cityName1} and {cityName2}:\n");
                Console.WriteLine($"{cityName1}: {city1.Density} density");
                Console.WriteLine($"{cityName2}: {city2.Density} density\n");

                if (city1.Density > city2.Density)
                {
                    Console.WriteLine($"{cityName1} has a higher population density.");
                }
                else if (city2.Density > city1.Density)
                {
                    Console.WriteLine($"{cityName2} has a higher population density.");
                }
                else
                {
                    Console.WriteLine("Both cities have the same population density.");
                }
            }
            else
            {
                Console.WriteLine("One or both of the cities information were not found.");
            }
        }
        else
        {
            Console.WriteLine("One or both of the cities do not exist in the data.");
        }
    }

    // Method reports the distance between any two cities using latitude and longitude stored in the cities dictionary
    public static double ReportDistanceBetweenCities(double lat1, double lon1, double lat2, double lon2)
    {
        // Radius of Earth in kilometers
        double R = 6371;

        // Convert degrees to radians
        double lat1Rad = ToRadians(lat1);
        double lon1Rad = ToRadians(lon1);
        double lat2Rad = ToRadians(lat2);
        double lon2Rad = ToRadians(lon2);

        // Differences in coordinates
        double latDifference = lat2Rad - lat1Rad;
        double lonDifference = lon2Rad - lon1Rad;

        // Haversine formula
        double a = Math.Pow(Math.Sin(latDifference / 2), 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Pow(Math.Sin(lonDifference / 2), 2);
        double b = 2 * Math.Asin(Math.Sqrt(a));

        // Calculate the distance
        double distance = R * b;
        return distance;
    }

    // Method to convert degrees to radians
    private static double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }

    // Method reports the distaance between a city and the state capital for the same state
    

}