using Microsoft.VisualBasic;
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
        SelectDataSource(); // Choose the data source for the application
        MainOptionsMenu(); // Display the main options menu for user interaction
    }

    //Prints the header with task description and data format
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

    //Purpose: Allows the user to select the data source for the cities' information
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

        // Switch between options and set the appropriate data source and format
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
        // Initialize the statistics object with selected data
        statistics = new Statistics(dataSource, dataFormat);
    }

    //Purpose: Displays the main options menu for interacting with the program
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
                    QueryByCity();// Proceed to city query section
                    break;
                case "2":
                    QueryByState();// Proceed to state query section
                    break;
                case "3":
                    SelectDataSource();// Change data source if needed
                    break;
                case "4":
                    return; // Exit the program
                default:
                    Console.WriteLine("\nInvalid option. Please try again.");
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
   // Purpose: Provides city-specific queries and displays options for the user
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
            // Provide the user with a list of city-related queries
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

            // Switch between different city query options
            switch (cityOption)
            {
                case "1":
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("CITY INFORMATION");
                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();// Reads the city name

                        if (string.IsNullOrEmpty(city)) // Checks if the city name is empty
                        {
                            Console.WriteLine("\nCity name cannot be empty. Please enter a valid city name: ");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();// Prompts the user to try again
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city))// Checks if the city exists in the data
                            {
                                statistics.ReportCityInformation(city);// Reports the city information if found
                                break;// Breaks the loop if city info is successfully displayed
                            }
                            else
                            {
                                Console.Write($"\n'{city}' not found. Enter another city name: ");
                                Console.Write("\nPress any key to continue...");
                                Console.ReadKey();// Prompts the user to try again with a different city
                            }
                        }
                    }
                    break;
                case "2": // Case for comparing population density between two cities
                    while (true)// Loops until valid city names are entered
                    {
                        Console.Clear();
                        PrintHeader("COMPARE POPULATION DENSITY");// Prints the header for population density comparison
                        Console.Write("\nEnter first city name: ");
                        city = Console.ReadLine();
                        Console.Write("Enter second city name: ");
                        city2 = Console.ReadLine();

                        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(city2))  // Checks if either city name is empty
                        {
                            Console.WriteLine("\nCity names cannot be empty. Please enter valid city names.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city) && statistics.CityData.ContainsKey(city2))// Checks if both cities exist in the data
                            {
                                Console.WriteLine();
                                statistics.ComparePopulationDensity(city, city2);// Compares the population density between the two cities
                                break;// Breaks the loop once comparison is done
                            }
                            else
                            {
                                Console.WriteLine($"\n'{city}' or '{city2}' not found.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();// Prompts the user to try again if cities are not found
                            }
                                
                        }
                    }
                    break;
                case "3":// Case for calculating the distance between two cities.
                    while (true)
                    {
                        Console.Clear();
                        PrintHeader("DISTANCE BETWEEN CITIES");// Prints the header for distance query.
                        Console.Write("\nEnter first city name: ");
                        city = Console.ReadLine();
                        Console.Write("Enter second city name: ");
                        city2 = Console.ReadLine();

                        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(city2))// Checks if either city name is empty
                        {
                            Console.WriteLine("\nCity names cannot be empty. Please enter valid city names.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city) && statistics.CityData.ContainsKey(city2))// Reports the distance between the two cities
                            {
                                Console.WriteLine();
                                statistics.ReportDistanceBetweenCities(city, city2);
                                break;// Breaks the loop once the distance is reported
                            }
                            else
                            {
                                Console.WriteLine($"\n'{city}' or '{city2}' not found.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();// Prompts the user to try again if cities are not found.
                            }
                        }
                    }
                    break;
                case "4":// Case for calculating the distance from the capital city
                    while (true)
                    {
                        Console.Clear(); // Clears the console for capital distance input
                        PrintHeader("DISTANCE FROM CAPITAL");// Prints the header for capital distance query
                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine(); // Reads the city name.

                        if (string.IsNullOrEmpty(city))
                        {
                            Console.WriteLine("\nCity name cannot be empty. Please enter a valid city name.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey(); // Prompts the user to try again
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city))// Checks if the city exists in the data
                            {
                                Console.WriteLine();
                                statistics.ReportDistanceFromCapital(city);
                                break;
                            }
                            else
                            {
                                Console.Write($"\n'{city}' not found. Enter another city name: ");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();// Prompts the user to try again if city is not found
                            }
                        }
                    }
                    break;
                case "5":// Case for displaying the city on a map.
                    while (true)// Loops until valid city name and state abbreviation are entered
                    {
                        Console.Clear();
                        PrintHeader("SHOW CITY ON MAP"); // Prints the header for map display

                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();
                        Console.Write("Enter state abbreviation: ");
                        stateAbbr = Console.ReadLine();// Reads the state abbreviation.

                        if (string.IsNullOrEmpty(city) || string.IsNullOrEmpty(stateAbbr))// Checks if either field is empty.
                        {
                            Console.WriteLine("\nCity names and stateAbbr cannot be empty. Please enter valid city names and stateAbbr.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            statistics.ShowCityOnMap(city, stateAbbr);// Displays the city on the map
                            break;// Breaks the loop once the city is displayed.
                        }
                    }
                    break;
                case "6":// Case for adjusting the city's population.
                    while (true)// Loops until valid city name and population are entered
                    {
                        Console.Clear();
                        PrintHeader("ADJUST CITY POPULATION");// Prints the header for population adjustment
                        Console.Write("\nEnter city name: ");
                        city = Console.ReadLine();

                        if (string.IsNullOrEmpty(city))// Checks if the city name is empty
                        {
                            Console.WriteLine("\nCity name cannot be empty. Please enter a valid city name.");
                            Console.Write("Press any key to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            if (statistics.CityData.ContainsKey(city))// Checks if the city exists in the data.
                            {
                                Console.Write("Enter new population: ");
                                if (int.TryParse(Console.ReadLine(), out population) && population > 0)// Ensures valid population
                                {
                                    Console.WriteLine();
                                    statistics.UpdateCityPopulation(city, population);// Updates the population of the city.
                                    break;// Breaks the loop once the population is updated
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid population input.");
                                    Console.Write("Press any key to continue...");
                                    Console.ReadKey(); // Prompts the user to try again if the population input is invalid.
                                }
                            }
                            else
                            {
                                Console.Write($"\n'{city}' not found. Enter another city name: ");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();// Prompts the user to try again if city is not found.
                            }
                        }
                    }
                    break;
                case "7":// Case for returning to the main options.
                    return;// Exits the current method, returning to the main menu.
                default:
                    Console.Write("\nInvalid option. Please try again.");
                    break; // Handles invalid options.
            }
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    static void QueryByState()
    {  // Ensure the statistics object is initialized before proceeding.
        if (statistics == null)
        {
            throw new InvalidOperationException("Statistics object is not initialized.");
        }

        while (true)
        {
            string? stateAbbr = string.Empty;

            Console.Clear();
            PrintHeader();
            // Display the available state-related options to the user.
            Console.WriteLine("\nState Options...\n");
            Console.WriteLine("1. All Cities");
            Console.WriteLine("2. Largest City");
            Console.WriteLine("3. Smallest City");
            Console.WriteLine("4. Capital City");
            Console.WriteLine("5. State Population");
            Console.WriteLine("6. Back to Main Options");
            Console.Write("\nEnter your selection: ");
            string? stateOption = Console.ReadLine();

            // If the user did not provide a valid input, prompt them again.
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
                    case "1": // Option 1: Query for all cities in a state.
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("ALL CITIES");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();
                            // Check for valid state abbreviation input.
                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                // Report all cities in the given state.
                                Console.WriteLine();
                                statistics.ReportAllCities(stateAbbr);
                                break; // Break the loop once the query is completed.
                            }
                        }
                        break;
                    case "2":// Option 2: Query for the largest city in a state.
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("LARGEST CITY");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();
                            // Check for valid state abbreviation input.
                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                // Report the largest city in the given state.
                                Console.WriteLine();
                                statistics.ReportLargestCity(stateAbbr);
                                break;// Break the loop once the query is completed.
                            }
                        }
                        break;
                    case "3":// Option 3: Query for the smallest city in a state.
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("SMALLEST CITY");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            // Check for valid state abbreviation input.
                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                // Report the smallest city in the given state.
                                Console.WriteLine();
                                statistics.ReportSmallestCity(stateAbbr);
                                break;// Break the loop once the query is completed.
                            }
                        }
                        break;
                    case "4":// Option 4: Query for the capital city of a state.
                        while (true)
                        {
                            Console.Clear();
                            PrintHeader("CAPITAL CITY");
                            Console.Write("\nEnter 2-letter state abbreviation: ");
                            stateAbbr = Console.ReadLine();

                            // Check for valid state abbreviation input.
                            if (string.IsNullOrEmpty(stateAbbr))
                            {
                                Console.WriteLine("\nState abbreviation cannot be empty. Please enter a valid state abbreviation.");
                                Console.Write("Press any key to continue...");
                                Console.ReadKey();
                            }
                            else
                            {
                                // Get and display the capital city details.
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
                                break;// Break the loop once the query is completed.
                            }
                        }
                        break;
                    case "5":// Option 5: Query for the population of a state.
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
                                // Report the population of the given state.
                                Console.WriteLine();
                                statistics.ReportStatePopulation(stateAbbr);
                                break;// Break the loop once the query is completed.
                            }
                        }
                        break;
                    case "6":  // Option 6: Exit back to the main menu.
                        Console.Clear();
                        return;// Exit the method and return to the main menu.
                    default:
                        // Handle invalid option input.
                        Console.Clear();
                        PrintHeader();
                        Console.WriteLine("\nInvalid option. Please try again.");
                        break;
                }
                Console.Write("Press any key to continue..."); // Prompt user to continue after each action.
                Console.ReadKey();
            }
        }
    }
}