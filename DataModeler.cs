using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;
using System.Text;

public class DataModeler
{
    public delegate void ParseDelegate(string fileName);// Delegate for parsing methods
    public delegate void SaveDelegate(Dictionary<string, List<CityInfo>> cityData, string fileName);// Delegate for saving methods
    private Dictionary<string, List<CityInfo>> _cityData = new Dictionary<string, List<CityInfo>>();// Dictionary to hold city data

    // Method to parse file based on type using delegate
    public Dictionary<string, List<CityInfo>> ParseFile(string fileName, string fileType)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"Error: File '{fileName}' not found.");
            return new Dictionary<string, List<CityInfo>>();  // Return empty dictionary instead of crashing
        }

        var parsers = new Dictionary<string, ParseDelegate>(StringComparer.OrdinalIgnoreCase)
        {
            { "json", ParseJSON },
            { "xml", ParseXML },
            { "csv", ParseCSV }
        };

        if (!parsers.TryGetValue(fileType, out var parser))
            throw new ArgumentException($"Unsupported file type: {fileType}");

        parser(fileName);
        return _cityData;
    }

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
                    var lattitude = double.Parse(cityNode["lat"]?.InnerText ?? "0");
                    var longitude = double.Parse(cityNode["lng"]?.InnerText ?? "0");
                    var population = int.Parse(cityNode["population"]?.InnerText ?? "0");
                    var density = double.Parse(cityNode["density"]?.InnerText ?? "0");
                    var timeZone = cityNode["timezone"]?.InnerText ?? "N/A";
                    var zips = cityNode["zips"]?.InnerText ?? "N/A";

                    // Create CityInfo object
                    var city = new CityInfo(id, name, stateAbbrev, state, capital, lattitude, longitude, population, density, timeZone, zips);

                    // Add the city to the list
                    cities.Add(city);
                }

                // If the list is not empty, forward it to AddCitiesToDictionary
                if (cities.Count > 0)
                {
                    AddCitiesToDictionary(cities);
                }
                else
                {
                    Console.WriteLine("No valid cities were found in the XML file.");
                }
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

            // Deserialize the JSON data into a list of CityInfo objects
            var cities = JsonConvert.DeserializeObject<List<CityInfo>>(jsonData);

            // Ensure the list is valid
            if (cities == null)
            {
                Console.WriteLine("No valid cities were found in the JSON file.");
                return;
            }

            // If the list is not empty, forward it to AddCitiesToDictionary
            if (cities.Count > 0)
            {
                AddCitiesToDictionary(cities);
            }
            else
            {
                Console.WriteLine("No valid cities were found in the JSON file.");
            }
        }
        catch (JsonSerializationException ex)
        {
            Console.WriteLine($"Error deserializing JSON data: {ex.Message}");
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
                if (!int.TryParse(columns[0], out int id))
                    throw new FormatException($"Invalid ID format: {columns[0]}");

                string name = columns[1];
                string stateAbbrev = columns[2];
                string state = columns[3];
                string capital = columns[4];

                if (!double.TryParse(columns[5], out double lattitude))
                    throw new FormatException($"Invalid lattitude format: {columns[5]}");

                if (!double.TryParse(columns[6], out double longitude))
                    throw new FormatException($"Invalid longitude format: {columns[6]}");

                if (!int.TryParse(columns[7], out int population))
                    throw new FormatException($"Invalid population format: {columns[7]}");

                if (!double.TryParse(columns[8], out double density))
                    throw new FormatException($"Invalid density format: {columns[8]}");

                string timeZone = columns[9];
                string zips = columns[10]; // Zips as a single string

                // Create CityInfo object
                var city = new CityInfo(id, name, stateAbbrev, state, capital, lattitude, longitude, population, density, timeZone, zips);

                // Add the city to the list
                cities.Add(city);
            }

            // If the list is not empty, forward it to AddCitiesToDictionary
            if (cities.Count > 0)
            {
                AddCitiesToDictionary(cities);
            }
            else
            {
                Console.WriteLine("No valid cities were found in the CSV file.");
            }
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
            if (!_cityData.ContainsKey(city.Name))
            {
                _cityData[city.Name] = new List<CityInfo>();
            }
            _cityData[city.Name].Add(city);
        }
    }

    // Method to save file based on type using delegate
    public void SaveFile(Dictionary<string, List<CityInfo>> cityData, string fileName, string fileType)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"Error: File '{fileName}' not found. Cannot save changes.");
            return;
        }

        var savers = new Dictionary<string, SaveDelegate>(StringComparer.OrdinalIgnoreCase)
        {
            { "json", SaveJson },
            { "xml", SaveXml },
            { "csv", SaveCsv }
        };

        if (!savers.TryGetValue(fileType, out var saver))
            throw new ArgumentException($"Unsupported file type: {fileType}");

        saver(cityData, fileName);
    }

    public void SaveXml(Dictionary<string, List<CityInfo>> cityData, string fileName)
    {
        // Create an XmlDocument to work with XML nodes
        XmlDocument xmlDoc = new XmlDocument();

        // Create XML declaration
        XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
        xmlDoc.AppendChild(xmlDeclaration);

        // Create root element
        XmlElement root = xmlDoc.CreateElement("usa_cities");
        xmlDoc.AppendChild(root);

        // Iterate through the dictionary to create city elements
        foreach (var cityList in cityData.Values)
        {
            foreach (var city in cityList)
            {
                XmlElement cityElement = xmlDoc.CreateElement("city");

                // Add each city data element
                cityElement.AppendChild(CreateElement(xmlDoc, "id", city.ID.ToString()));
                cityElement.AppendChild(CreateElement(xmlDoc, "name", city.Name));
                cityElement.AppendChild(CreateElement(xmlDoc, "state_abbrev", city.StateAbbrev));
                cityElement.AppendChild(CreateElement(xmlDoc, "state", city.State));
                cityElement.AppendChild(CreateElement(xmlDoc, "capital", city.Capital));
                cityElement.AppendChild(CreateElement(xmlDoc, "lat", city.Lattitude.ToString()));
                cityElement.AppendChild(CreateElement(xmlDoc, "lng", city.Longitude.ToString()));
                cityElement.AppendChild(CreateElement(xmlDoc, "population", city.Population.ToString()));
                cityElement.AppendChild(CreateElement(xmlDoc, "density", city.Density.ToString()));
                cityElement.AppendChild(CreateElement(xmlDoc, "timezone", city.TimeZone));
                cityElement.AppendChild(CreateElement(xmlDoc, "zips", city.Zips));

                root.AppendChild(cityElement);
            }
        }

        // Create a writer with formatting enabled to preserve indentation
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,  // Enables indentation
            IndentChars = "\t",  // Uses a tab character for indentation
            NewLineOnAttributes = false, // Prevents attributes from breaking into new lines
        };

        using (XmlWriter writer = XmlWriter.Create(fileName, settings))
        {
            xmlDoc.Save(writer);// Save the XML document to file
        }
    }

    // Helper method to create XML elements
    private XmlElement CreateElement(XmlDocument xmlDoc, string name, string value)
    {
        XmlElement element = xmlDoc.CreateElement(name);
        element.InnerText = value;
        return element;
    }

    // Method to update json file with new cityData
    public void SaveJson(Dictionary<string, List<CityInfo>> cityData, string fileName)
    {
        // Flatten the dictionary into a single list of CityInfo objects
        var cityList = cityData.SelectMany(kvp => kvp.Value).ToList();
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(cityList, Newtonsoft.Json.Formatting.Indented);

        File.WriteAllText(fileName, json);
    }

    // Method to update csv file with new cityData
    public void SaveCsv(Dictionary<string, List<CityInfo>> cityData, string fileName)
    {
        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine("id,city,state_abbrev,state,capital,lat,lng,population,density,timezone,zips");

            foreach (var cityList in cityData.Values)
            {
                foreach (var city in cityList)
                {
                    writer.WriteLine($"{city.ID},{city.Name},{city.StateAbbrev},{city.State},{city.Capital}," +
                                     $"{city.Lattitude},{city.Longitude},{city.Population},{city.Density},{city.TimeZone},{city.Zips}");
                }
            }
        }
    }
}