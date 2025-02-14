using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

public class Statistics
{
    // Dictionary to hold city information
    public Dictionary<string, List<CityInfo>> CityData { get; private set; }
    private PopulationChangeEvent _populationChangeEvent;
    private string _fileName;
    private string _fileType;

    // Constructor to initialize the city data by calling the DataModeler
    public Statistics(string fileName, string fileType)
    {
        _fileName = fileName;
        _fileType = fileType;
        DataModeler modeler = new DataModeler();
        CityData = modeler.ParseFile(fileName, fileType); // Parse the data and store in the dictionary

        _populationChangeEvent = new PopulationChangeEvent();
        _populationChangeEvent.PopulationChanged += OnPopulationChanged;
    }

    // Method reports all the city info in the dictionary for the selected city name
    public void ReportCityInformation(string cityName)
    {
        int count = 0;
        if (CityData.ContainsKey(cityName))
        {
            foreach (var city in CityData[cityName])
                count++;
            Console.WriteLine($"\nNumber of matches: {count}");
            count = 0;
            foreach (var city in CityData[cityName])
            {
                // Output the information for each city (multiple entries per name)
                Console.WriteLine();
                Console.WriteLine(++count + ". " + city.ToString());
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

                Console.WriteLine($"{cityName1}, {city1.StateAbbrev} has a population density of {city1.Density.ToString("N0")} people per sq. km.");
                Console.WriteLine($"{cityName2}, {city2.StateAbbrev} has a population density of {city2.Density.ToString("N0")} people per sq. km.\n");

                if (city1.Density > city2.Density)
                {
                    Console.WriteLine($"{cityName1}, {city1.StateAbbrev} has the higher population density.");
                }
                else if (city2.Density > city1.Density)
                {
                    Console.WriteLine($"{cityName2}, {city2.StateAbbrev} has the higher population density.");
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

    // Method reports the distance between any two cities in km using lattitude and longitude stored in the cities dictionary
    public void ReportDistanceBetweenCities(string cityName1, string cityName2) 
    {
        // Ensure both cities exist in the dictionary
        if (CityData.ContainsKey(cityName1) && CityData.ContainsKey(cityName2))
        {
            var city1 = CityData[cityName1].FirstOrDefault();
            var city2 = CityData[cityName2].FirstOrDefault();

            // Ensure both cities are not null
            if (city1 == null || city2 == null)
            {
                Console.WriteLine("One or both cities were found but do not contain valid data.");
                return;
            }

            double lat1 = city1.Lattitude;
            double lon1 = city1.Longitude;
            double lat2 = city2.Lattitude;
            double lon2 = city2.Longitude;

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

            Console.WriteLine($"The distance between {cityName1}, {city1.StateAbbrev} and {cityName2}, {city2.StateAbbrev} is {distance.ToString("N0")} km.");
        }
        else
        {
            Console.WriteLine("One or both of the cities do not exist in the data.");
        }
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
                        // Report the distance
                        ReportDistanceBetweenCities(cityName, capitalCity.Value.CapitalName);
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

    // Method to display map of the specified city
    public void ShowCityOnMap(string cityName, string stateAbbrev)
    {
        if (CityData.ContainsKey(cityName))
        {
            var city = CityData[cityName].FirstOrDefault(c => c.StateAbbrev == stateAbbrev);
            if (city != null)
            {
                string url = $"https://www.google.com/maps/search/?api=1&query={city.Lattitude},{city.Longitude}";
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            else
            {
                Console.Write("\nCity not found in the specified state.");
            }
        }
        else
        {
            Console.Write("\nCity not found.");
        }
    }

    // Method reports all cities in the specified state
    public void ReportAllCities(string stateAbbrev)
    {
        // Retrieves all cities matching the state abbreviation
        var cities = CityData.Values.SelectMany(c => c).Where(c => c.StateAbbrev == stateAbbrev).ToList();
        if (cities.Any())
        {
            Console.WriteLine($"The following cities are in {cities[0].State}...\n");
            int count = 0;
            foreach (var city in cities)
            {
                count++;
                Console.WriteLine(city.Name);
            }
            Console.WriteLine($"\n{count} cities found.\n");
        }
        else
        {
            Console.WriteLine("No cities found for the given state abbreviation.\n");
        }
    }

    //Method reports the largest city in the specified state based on population
    public void ReportLargestCity(string stateAbbrev)
    {
        var largestCity = CityData.Values.SelectMany(c => c)
            .Where(c => c.StateAbbrev == stateAbbrev)
            .OrderByDescending(c => c.Population) // Sorts in descending order by population
            .FirstOrDefault(); 

        if (largestCity != null)
        {
            Console.WriteLine($"The largest city in {largestCity.State} is {largestCity.Name} with a population of {largestCity.Population.ToString("N0")}.\n");
        }
        else
        {
            Console.WriteLine("No cities found for the given state abbreviation.");
        }
    }

    // Method reports the smallest city in the specified state based on population
    public void ReportSmallestCity(string stateAbbrev)
    {
        var smallestCity = CityData.Values.SelectMany(c => c)
            .Where(c => c.StateAbbrev == stateAbbrev)
            .OrderBy(c => c.Population) 
            .FirstOrDefault(); // Retrieves the first entry, which has the lowest population

        if (smallestCity != null)
        {
            Console.WriteLine($"The smallest city in {smallestCity.State} is {smallestCity.Name} with a population of {smallestCity.Population.ToString("N0")}.\n");
        }
        else
        {
            Console.WriteLine("No cities found for the given state abbreviation.");
        }
    }

    // Method to report the capital city and the longitude and lattitude of a given state, returns a tuple
    public (string CapitalName, double Lattitude, double Longitude)? ReportCapital(string stateabbrev)
    {
        var capital = CityData.Values
            .SelectMany(cityList => cityList)
            .FirstOrDefault(city => city.StateAbbrev == stateabbrev && !string.IsNullOrEmpty(city.Capital));

        if (capital != null)
        {
            return (capital.Name, capital.Lattitude, capital.Longitude);
        }
        return null;
    }

    // Method reports the total population of a specified state by summing all city populations
    public void ReportStatePopulation(string stateAbbrev)
    {
        var cities = CityData.Values.SelectMany(c => c)
            .Where(c => c.StateAbbrev == stateAbbrev)
            .ToList(); // Computes the sum of populations

        int totalPopulation = cities.Sum(c => c.Population);

        if (cities.Count > 0)
        {
            Console.WriteLine($"Total population of the {cities.Count} cities in {cities[0].State} is {totalPopulation.ToString("N0")}.\n");
        }
        else
        {
            Console.WriteLine($"No cities found for the state abbreviation: {stateAbbrev}.\n");
        }
    }

    // Method sends a notification message to user when Population has been updated
    private void OnPopulationChanged(object? sender, PopulationChangedEventArgs e)
    {
        Console.WriteLine($"Changing the population figure for {e.CityName}, {e.StateAbbrev}...");
        Console.WriteLine($"Current Population: {e.OldPopulation.ToString("N0")}");
        Console.WriteLine($"Revised Population: {e.NewPopulation.ToString("N0")}");
    }

    // Method to update the city population and save the changes to the file
    public void UpdateCityPopulation(string cityName, int newPopulation)
    {
        if (CityData.ContainsKey(cityName))
        {
            foreach (var city in CityData[cityName])
            {
                double oldPopulation = city.Population;
                _populationChangeEvent.PopulationChanged -= OnPopulationChanged; // Unsubscribe first
                _populationChangeEvent.PopulationChanged += OnPopulationChanged;  // Subscribe event
                _populationChangeEvent.UpdatePopulation(city, newPopulation);  // Use PopulationChangeEvent to update population

                DataModeler modeler = new DataModeler();
                modeler.SaveFile(CityData, _fileName, _fileType);// Save updates to file

                Console.WriteLine($"\nPopulation of {city.Name}, {city.StateAbbrev} in {_fileType} file " +
                    $"successfully changed from {oldPopulation.ToString("N0")} to {city.Population.ToString("N0")}.");
            }
        }
        else
        {
            Console.WriteLine("City not found.");
        }
    }
}