using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;


class Program
{
    static Statistics? statistics;
    static string dataFormat = "NONE";
    static string dataPath = "../../../Data/"; // Set to folder where data files are stored
    static string dataSource = "";

    static void Main(string[] args)
    {
        SelectDataSource();
        MainOptionsMenu();
    }

    static void PrintHeader(string task = "")
    {
        int totalWidth = 80; // Total width of header
        string header = "U.S. Cities Information System v1.0";
        string taskDisplay = string.IsNullOrWhiteSpace(task) ? "" : task;// If there's a current task include it
        string formatLabel = $"Data Format: {dataFormat}";

        // Extend the totalWidth if taskDisplay is longer than 20 chars
        totalWidth += Math.Max(0, taskDisplay.Length - 20);

        Console.WriteLine($"{header,-40}{taskDisplay,-20}{formatLabel,20}");
        Console.WriteLine(new string('-', totalWidth));
    }

    static void SelectDataSource()
    {
        Console.Clear();
        PrintHeader();
        Console.WriteLine("\nData source selection...\n");
        Console.WriteLine("1. CSV - usacities.csv");
        Console.WriteLine("2. JSON - usacities.json");
        Console.WriteLine("3. XML - usacities.xml");
        Console.Write("\nEnter your selection: ");
        string? sourceSelection = Console.ReadLine();

        switch (sourceSelection)
        {
            case "1":
                dataSource = dataPath + "usacities.csv";
                dataFormat = "CSV";
                break;
            case "2":
                dataSource = dataPath + "usacities.json";
                dataFormat = "JSON";
                break;
            case "3":
                dataSource = dataPath + "usacities.xml";
                dataFormat = "XML";
                break;
            default:
                Console.WriteLine("\nInvalid selection. Defaulting to XML.");
                dataSource = dataPath + "usacities.xml";
                dataFormat = "XML";
                Console.Write("Press any key to continue...");
                Console.ReadKey();

                break;
        }

        statistics = new Statistics(dataSource, dataFormat);
    }

    static void MainOptionsMenu()
    {
        while (true)
        {
            Console.Clear();
            PrintHeader();
            Console.WriteLine("\nMain Options...\n");
            Console.WriteLine("1. Query by City");
            Console.WriteLine("2. Query by State");
            Console.WriteLine("3. Change Data Source");
            Console.WriteLine("4. Exit the Program");
            Console.Write("\nEnter your selection: ");
            string? mainOption = Console.ReadLine();

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
                    Console.WriteLine("\nInvalid option. Please try again.");
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void QueryByCity()
    {
        if (statistics == null)
        {
            throw new InvalidOperationException("Statistics object is not initialized.");
        }

        while (true)
        {
            string? city = string.Empty;
            string? city2 = string.Empty;
            string? stateAbbr = string.Empty;
            int population = 0;

            Console.Clear();
            PrintHeader();
            Console.WriteLine("\nCity Options...\n");
            Console.WriteLine("1. City Information");
            Console.WriteLine("2. Compare Population Density");
            Console.WriteLine("3. Distance Between Cities");
            Console.WriteLine("4. Distance From Capital");
            Console.WriteLine("5. Show City on Map");
            Console.WriteLine("6. Adjust Population");
            Console.WriteLine("7. Back to Main Options");
            Console.Write("\nEnter your selection: ");
            string? cityOption = Console.ReadLine();

            switch (cityOption)
            {
                case "1":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("CITY INFORMATION");
                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();

                        if (string.IsNullOrEmpty(city))
                        {
                            Console.WriteLine("\nCity name cannot be empty. Please enter a valid city name: ");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city))
                            {
                                statistics.ReportCityInformation(city);
                                break;
                            }
                            else
                            {
                                Console.Write($"\n'{city}' not found. Enter another city name: ");
                                Console.Write("\nPress any key to continue...");
                                Console.ReadKey();
                            }
                        }
                    }
                    break;
                case "2":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("COMPARE POPULATION DENSITY");
                        Console.Write("\nEnter first city name: ");
                        city = Console.ReadLine();
                        Console.Write("Enter second city name: ");
                        city2 = Console.ReadLine();

                        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(city2))
                        {
                            Console.WriteLine("\nCity names cannot be empty. Please enter valid city names.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city) && statistics.CityData.ContainsKey(city2))
                            {
                                Console.WriteLine();
                                statistics.ComparePopulationDensity(city, city2);
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"\n'{city}' or '{city2}' not found.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                                
                        }
                    }
                    break;
                case "3":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("DISTANCE BETWEEN CITIES");
                        Console.Write("\nEnter first city name: ");
                        city = Console.ReadLine();
                        Console.Write("Enter second city name: ");
                        city2 = Console.ReadLine();

                        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(city2))
                        {
                            Console.WriteLine("\nCity names cannot be empty. Please enter valid city names.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city) && statistics.CityData.ContainsKey(city2))
                            {
                                Console.WriteLine();
                                statistics.ReportDistanceBetweenCities(city, city2);
                                break;
                            }
                            else
                            {
                                Console.WriteLine($"\n'{city}' or '{city2}' not found.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                    }
                    break;
                case "4":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("DISTANCE FROM CAPITAL");
                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();
                        if (string.IsNullOrEmpty(city))
                        {
                            Console.WriteLine("\nCity name cannot be empty. Please enter a valid city name.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city))
                            {
                                Console.WriteLine();
                                statistics.ReportDistanceFromCapital(city);
                                break;
                            }
                            else
                            {
                                Console.Write($"\n'{city}' not found. Enter another city name: ");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                    }
                    break;
                case "5":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("SHOW CITY ON MAP");

                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();
                        Console.Write("Enter state abbreviation: ");
                        stateAbbr = Console.ReadLine();

                        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(stateAbbr))
                        {
                            Console.WriteLine("\nCity names and stateAbbr cannot be empty. Please enter valid city names and stateAbbr.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            statistics.ShowCityOnMap(city, stateAbbr);
                            break;
                        }
                    }
                    break;
                case "6":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("ADJUST CITY POPULATION");
                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();
                        if (string.IsNullOrEmpty(city))
                        {
                            Console.WriteLine("\nCity name cannot be empty. Please enter a valid city name.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city))
                            {
                                Console.Write("Enter new population: ");
                                if (int.TryParse(Console.ReadLine(), out population) && population > 0)// Ensures valid population
                                {
                                    Console.WriteLine();
                                    statistics.UpdateCityPopulation(city, population);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid population input.");
                                    Console.Write("Press any key to continue...");
                                    Console.ReadKey();
                                }
                            }
                            else
                            {
                                Console.Write($"\n'{city}' not found. Enter another city name: ");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                        }
                    }
                    break;
                case "7":
                    return;
                default:
                    Console.Write("\nInvalid option. Please try again.");
                    break;
            }
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    static void QueryByState()
    {
        if (statistics == null)
        {
            throw new InvalidOperationException("Statistics object is not initialized.");
        }

        while (true)
        {
            string? stateAbbr = string.Empty;

            Console.Clear();
            PrintHeader();
            Console.WriteLine("\nState Options...\n");
            Console.WriteLine("1. All Cities");
            Console.WriteLine("2. Largest City");
            Console.WriteLine("3. Smallest City");
            Console.WriteLine("4. Capital City");
            Console.WriteLine("5. State Population");
            Console.WriteLine("6. Back to Main Options");
            Console.Write("\nEnter your selection: ");
            string? stateOption = Console.ReadLine();

            if (string.IsNullOrEmpty(stateOption))
            {
                Console.WriteLine("\nState option cannot be empty. Please enter a valid option.");
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
            else
            {
                switch (stateOption)
                {
                    case "1":
                        while(true)
                        {
                            Console.Clear();
                            PrintHeader("ALL CITIES");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine();
                                statistics.ReportAllCities(stateAbbr);
                                break;
                            }
                        }
                        break;
                    case "2":
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("LARGEST CITY");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine();
                                statistics.ReportLargestCity(stateAbbr);
                                break;
                            }
                        }
                        break;
                    case "3":
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("SMALLEST CITY");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine();
                                statistics.ReportSmallestCity(stateAbbr);
                                break;
                            }
                        }
                        break;
                    case "4":
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("CAPITAL CITY");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                var capital = statistics.ReportCapital(stateAbbr);
                                if (capital.HasValue)
                                {
                                    Console.WriteLine($"\nThe capital city of {statistics.CityData[capital.Value.CapitalName][0].State} is {capital.Value.CapitalName}.");
                                    Console.WriteLine($"It's coordinates are {capital.Value.Lattitude.ToString("N4")} degrees lattitude, {capital.Value.Longitude.ToString("N4")} degrees longitude.\n");
                                }
                                else
                                {
                                    Console.WriteLine("\nCapital not found.");
                                }
                                break;
                            }
                        }
                        break;
                    case "5":
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("STATE POPULATION");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.WriteLine();
                                statistics.ReportStatePopulation(stateAbbr);
                                break;
                            }
                        }
                        break;
                    case "6":
                        Console.Clear();
                        return;
                    default:
                        Console.Clear();
                        PrintHeader();
                        Console.WriteLine("\nInvalid option. Please try again.");
                        break;
                }
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}