﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static PopulationChangeEvent;

public class Statistics
{
    // Dictionary to hold city information
    public Dictionary<string, List<CityInfo>> CityData { get; private set; }
    private PopulationChangeEvent _populationChangeEvent;

    // Constructor to initialize the city data by calling the DataModeler
    public Statistics(string fileName, string fileType)
    {
        DataModeler modeler = new DataModeler();
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);//new change
        CityData = modeler.ParseFile(fileName, fileType); // Parse the data and store in the dictionary
        _populationChangeEvent = new PopulationChangeEvent(CityData);
        _populationChangeEvent.OnPopulationChanged += PopulationChangeHandler;
    }

    // Method reports all the city info in the dictionary for the selected city name
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

    // Method reports the distance between any two cities in km using latitude and longitude stored in the cities dictionary
    public double ReportDistanceBetweenCities(double lat1, double lon1, double lat2, double lon2)
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
    private double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }

    // Method reports the distance between a city and the state capital for the same state
    public void ReportDistanceFromCapital(string cityName)
    {
        try
        {
            if (CityData.ContainsKey(cityName))
            {
                var city = CityData[cityName].FirstOrDefault();

                if (city != null)
                {
                    // Get the capital of the state using its abbreviation
                    var capitalCity = ReportCapital(city.StateAbbrev);

                    if (capitalCity.HasValue)
                    {
                        // Extract capital details
                        string capitalName = capitalCity.Value.CapitalName;
                        double capitalLat = capitalCity.Value.Latitude;
                        double capitalLon = capitalCity.Value.Longitude;

                        // Calculate the distance
                        double distance = ReportDistanceBetweenCities(city.Latitude, city.Longitude, capitalLat, capitalLon);

                        Console.WriteLine($"The distance between {cityName} and {capitalName} (capital of {city.State}) is: {distance} km.");
                    }
                    else
                    {
                        Console.WriteLine($"No capital found for state {city.State}.");
                    }
                }
                else
                {
                    Console.WriteLine($"City {cityName} does not have valid data.");
                }
            }
            else
            {
                Console.WriteLine($"City {cityName} not found in the data.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while calculating distance: {ex.Message}");
        }
    }



    // Method to report the capital city and the longitude and latitude of a given state, returns a tuple
    public (string CapitalName, double Latitude, double Longitude)? ReportCapital(string stateabbrev)
    {
        var capital = CityData.Values
            .SelectMany(cityList => cityList)
            .FirstOrDefault(city => city.StateAbbrev == stateabbrev && !string.IsNullOrEmpty(city.Capital));

        if (capital != null)
        {
            return (capital.Name, capital.Latitude, capital.Longitude);
        }
        return null;
    }


    // Method to display map of the specified city
    public void ShowCityOnMap(string cityName, string stateAbbrev)
    {
        if (CityData.ContainsKey(cityName))
        {
            
            var city = CityData[cityName].FirstOrDefault(c => c.StateAbbrev == stateAbbrev);
            if (city != null)
            {
               
                string url = $"https://www.google.com/maps/search/?api=1&query={city.Latitude},{city.Longitude}";
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            else
            {
                Console.WriteLine("City not found in the specified state.");
            }
        }
        else
        {
            Console.WriteLine("City not found.");
        }
    }

    // Method to reports all cities in the specified state
    public void ReportAllCities(string stateAbbrev)
    {
        // Retrieves all cities matching the state abbreviation
        var cities = CityData.Values.SelectMany(c => c).Where(c => c.StateAbbrev == stateAbbrev).ToList();
        if (cities.Any())
        {
            Console.WriteLine($"Cities in {stateAbbrev}:");
            foreach (var city in cities)
            {
                Console.WriteLine(city.Name);
            }
        }
        else
        {
            Console.WriteLine("No cities found for the given state abbreviation.");
        }
    }

    //Method to reports the largest city in the specified state based on population
    public void ReportLargestCity(string stateAbbrev)
    {
        var largestCity = CityData.Values.SelectMany(c => c)
            .Where(c => c.StateAbbrev == stateAbbrev)
            .OrderByDescending(c => c.Population) // Sorts in descending order by population
            .FirstOrDefault(); 

        if (largestCity != null)
        {
            Console.WriteLine($"Largest city in {stateAbbrev}: {largestCity.Name} with population {largestCity.Population}");
        }
        else
        {
            Console.WriteLine("No cities found for the given state abbreviation.");
        }
    }

    // Method to reports the smallest city in the specified state based on population
    public void ReportSmallestCity(string stateAbbrev)
    {
        var smallestCity = CityData.Values.SelectMany(c => c)
            .Where(c => c.StateAbbrev == stateAbbrev)
            .OrderBy(c => c.Population) 
            .FirstOrDefault(); // Retrieves the first entry, which has the lowest population

        if (smallestCity != null)
        {
            Console.WriteLine($"Smallest city in {stateAbbrev}: {smallestCity.Name} with population {smallestCity.Population}");
        }
        else
        {
            Console.WriteLine("No cities found for the given state abbreviation.");
        }
    }

    // Method to reports the total population of a specified state by summing all city populations
    public void ReportStatePopulation(string stateAbbrev)
    {
        int totalPopulation = CityData.Values.SelectMany(c => c)
            .Where(c => c.StateAbbrev == stateAbbrev)
            .Sum(c => c.Population); // Computes the sum of populations

        Console.WriteLine($"Total population of {stateAbbrev}: {totalPopulation}");
    }

    // Method to reports the average population density of a specified state
    private void PopulationChangeHandler(object sender, PopulationChangedEventArgs e)
    {
        Console.WriteLine($"[EVENT] Population updated for {e.CityName}, {e.StateAbbrev}:");
        Console.WriteLine($"Old Population: {e.OldPopulation} → New Population: {e.NewPopulation}");
        Console.WriteLine($"Old Density: {e.OldDensity} → Adjusted Density: {e.AdjustedDensity}");
    }
    public void ChangeCityPopulation(string cityName, string stateAbbrev, int newPopulation)
    {
        _populationChangeEvent.UpdatePopulation(cityName, stateAbbrev, newPopulation);
    }


}