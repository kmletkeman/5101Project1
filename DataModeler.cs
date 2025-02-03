using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Globalization;

public class DataModeler
{
    // Delegate for parsing methods
    public delegate void ParseDelegate(string fileName);

    // Dictionary to hold city data
    private Dictionary<string, List<CityInfo>> cityData = new();

    // Method to parse XML file
    public void ParseXML(string fileName)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException($"File not found: {fileName}");

        try
        {
            // Load the XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(fileName);

            // Get all city nodes (adjust XPath based on XML structure)
            var cityNodes = xmlDoc.SelectNodes("//usa_cities/city");

            List<CityInfo> cities = new List<CityInfo>();

            if (cityNodes != null)
            {
                foreach (XmlNode cityNode in cityNodes)
                {
                    var id = int.Parse(cityNode["id"]?.InnerText ?? "0");
                    var name = cityNode["name"]?.InnerText ?? "N/A";
                    var stateAbbrev = cityNode["state_abbrev"]?.InnerText ?? "N/A";
                    var state = cityNode["state"]?.InnerText ?? "N/A";
                    var capital = cityNode["capital"]?.InnerText ?? "N/A";
                    var latitude = double.Parse(cityNode["lat"]?.InnerText ?? "0");
                    var longitude = double.Parse(cityNode["lng"]?.InnerText ?? "0");
                    var population = int.Parse(cityNode["population"]?.InnerText ?? "0");
                    var density = double.Parse(cityNode["density"]?.InnerText ?? "0");
                    var timeZone = cityNode["timezone"]?.InnerText ?? "N/A";
                    var zips = cityNode["zips"]?.InnerText ?? "N/A";

                    // Create CityInfo object
                    var city = new CityInfo(id, name, stateAbbrev, state, capital, latitude, longitude, population, density, timeZone, zips);

                    // Add the city to the list
                    cities.Add(city);
                }

                // Forward the list of cities to AddCitiesToDictionary
                AddCitiesToDictionary(cities);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing XML file: {ex.Message}");
        }
    }

    // Method to parse JSON file
    public void ParseJSON(string fileName)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException($"File not found: {fileName}");

        try
        {
            string jsonData = File.ReadAllText(fileName);
            var cities = JsonConvert.DeserializeObject<List<CityInfo>>(jsonData) ?? new List<CityInfo>();
            AddCitiesToDictionary(cities);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing JSON file: {ex.Message}");
        }
    }

    // Method to parse CSV file
    public void ParseCSV(string fileName)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException($"File not found: {fileName}");

        try
        {
            // Read all lines from the CSV file
            var lines = File.ReadAllLines(fileName);

            List<CityInfo> cities = new List<CityInfo>();

            // Skip the header line (first line)
            for (int i = 1; i < lines.Length; i++)
            {
                var columns = lines[i].Split(',');

                // Ensure there are enough columns in the current line
                if (columns.Length < 11)
                    continue;

                // Parse the city data from the columns
                var id = int.Parse(columns[0]);
                var name = columns[1];
                var stateAbbrev = columns[2];
                var state = columns[3];
                var capital = columns[4];
                var latitude = double.Parse(columns[5], CultureInfo.InvariantCulture);
                var longitude = double.Parse(columns[6], CultureInfo.InvariantCulture);
                var population = int.Parse(columns[7]);
                var density = double.Parse(columns[8], CultureInfo.InvariantCulture);
                var timeZone = columns[9];
                var zips = columns[10]; // Zips as a single string

                // Create CityInfo object
                var city = new CityInfo(id, name, stateAbbrev, state, capital, latitude, longitude, population, density, timeZone, zips);

                // Add the city to the list
                cities.Add(city);
            }

            // Forward the list of cities to AddCitiesToDictionary
            AddCitiesToDictionary(cities);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing CSV file: {ex.Message}");
        }
    }

    // Helper method to add parsed cities to dictionary
    private void AddCitiesToDictionary(List<CityInfo> cities)
    {
        foreach (var city in cities)
        {
            if (!cityData.ContainsKey(city.Name))
            {
                cityData[city.Name] = new List<CityInfo>();
            }
            cityData[city.Name].Add(city);
        }
    }

    // Method to parse file based on type using delegate
    public Dictionary<string, List<CityInfo>> ParseFile(string fileName, string fileType)
    {
        var parsers = new Dictionary<string, ParseDelegate>(StringComparer.OrdinalIgnoreCase)
        {
            { "json", ParseJSON },
            { "xml", ParseXML },
            { "csv", ParseCSV }
        };

        if (!parsers.TryGetValue(fileType, out var parser))
            throw new ArgumentException($"Unsupported file type: {fileType}");

        parser(fileName);
        return cityData;
    }
}