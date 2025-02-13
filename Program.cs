using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;


class Program
{
    static Statistics statistics;
    static string dataFormat = "NONE";
    static string dataSource = string.Empty;

    static void Main(string[] args)
    {
        SelectDataSource();
        MainOptionsMenu();
    }

    static void SelectDataSource()
    {
        Console.WriteLine("Selecting the Data Source Format.");
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine("Data source selection...");
        Console.WriteLine("1. CSV - usacities.csv");
        Console.WriteLine("2. JSON - usacities.json");
        Console.WriteLine("3. XML - usacities.xml");
        Console.Write("Enter your selection: ");
        string sourceSelection = Console.ReadLine();

        switch (sourceSelection)
        {
            case "1":
                dataSource = "usacities.csv";
                dataFormat = "CSV";
                break;
            case "2":
                dataSource = "usacities.json";
                dataFormat = "JSON";
                break;
            case "3":
                dataSource = "usacities.xml";
                dataFormat = "XML";
                break;
            default:
                Console.WriteLine("Invalid selection. Defaulting to XML.");
                dataSource = "usacities.xml";
                dataFormat = "XML";
                break;
        }

        Console.WriteLine($"\nU.S. Cities Information System v1.0 Data Format: {dataFormat}");
        Console.WriteLine("--------------------------------------------------------------------------------");

        //if (!File.Exists(dataSource))
      //  {
        //    Console.WriteLine($"Error: File '{dataSource}' not found. Please ensure it is in the correct location.");
       //     return;  // Prevent further execution if the file is missing
      //  }

        statistics = new Statistics(dataSource, dataFormat);

    }

    static void MainOptionsMenu()
    {
        while (true)
        {
            Console.WriteLine($"\nU.S. Cities Information System v1.0 Data Format: {dataFormat}");
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine("Main Options...");
            Console.WriteLine();
            Console.WriteLine("1. Query by City");
            Console.WriteLine("2. Query by State");
            Console.WriteLine("3. Change Data Source");
            Console.WriteLine("4. Exit the Program");
            Console.Write("Enter your selection: ");
            string mainOption = Console.ReadLine();

            switch (mainOption)
            {
                case "1":
                    QueryByCity();
                    break;
                case "2":
                    QueryByState();
                    break;
                case "3":
                    SelectDataSource();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void QueryByCity()
    {
         Console.WriteLine($"\nU.S. Cities Information System v1.0 Data Format: {dataFormat}");
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("City Options...");
        Console.WriteLine();
        Console.WriteLine("1. City Information");
        Console.WriteLine("2. Compare Population Density");
        Console.WriteLine("3. Distance Between Cities");
        Console.WriteLine("4. Distance From Capital");
        Console.WriteLine("5. Show City on Map");
        Console.WriteLine("6. Adjust Population");
        Console.WriteLine("7. Back to Main Options");
        Console.WriteLine();
        Console.Write("Enter your selection: ");
        string cityOption = Console.ReadLine();

        switch (cityOption)
        {
            case "1":
                Console.Write("Enter city name: ");
                statistics.ReportCityInformation(Console.ReadLine());
                break;
            case "2":
                Console.Write("Enter first city name: ");
                string city1 = Console.ReadLine();
                Console.Write("Enter second city name: ");
                string city2 = Console.ReadLine();
                statistics.ComparePopulationDensity(city1, city2);
                break;
            case "3":
                // Distance logic placeholder
                break;
            case "4":
                Console.Write("Enter city name: ");
                statistics.ReportDistanceFromCapital(Console.ReadLine());
                break;
            case "5":
                Console.Write("Enter city name: ");
                string cityName = Console.ReadLine();
                Console.Write("Enter state abbreviation: ");
                string stateAbbr = Console.ReadLine();
                statistics.ShowCityOnMap(cityName, stateAbbr);
                break;
            case "6":
                Console.Write("Enter city name: ");
                string cityToUpdate = Console.ReadLine();
                Console.Write("Enter new population: ");
                if (int.TryParse(Console.ReadLine(), out int newPop))
                {
                    statistics.ChangeCityPopulation(cityToUpdate, "", newPop);
                }
                else
                {
                    Console.WriteLine("Invalid population input.");
                }
                break;
            case "7":
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
 

    static void QueryByState()
    {
        Console.WriteLine("State Options...");
        Console.WriteLine("1. All Cities");
        Console.WriteLine("2. Largest City");
        Console.WriteLine("3. Smallest City");
        Console.WriteLine("4. Capital City");
        Console.WriteLine("5. State Population");
        Console.WriteLine("6. Back to Main Options");
        Console.Write("Enter your selection: ");
        string stateOption = Console.ReadLine();

        Console.Write("Enter 2-letter state abbreviation: ");
        string stateAbbr = Console.ReadLine();

        switch (stateOption)
        {
            case "1":
                statistics.ReportAllCities(stateAbbr);
                break;
            case "2":
                statistics.ReportLargestCity(stateAbbr);
                break;
            case "3":
                statistics.ReportSmallestCity(stateAbbr);
                break;
            case "4":
                var capital = statistics.ReportCapital(stateAbbr);
                if (capital.HasValue)
                {
                    Console.WriteLine($"The capital city of {stateAbbr} is {capital.Value.CapitalName}.");
                }
                else
                {
                    Console.WriteLine("Capital not found.");
                }
                break;
            case "5":
                statistics.ReportStatePopulation(stateAbbr);
                break;
            case "6":
                return;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }
}
























































/*

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
*/