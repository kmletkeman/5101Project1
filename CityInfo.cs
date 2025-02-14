using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CityInfo
{
    // Properties with original JSON format to ensure consistency
    [JsonProperty("id")]
    public int ID { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("state_abbrev")]
    public string StateAbbrev { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("capital")]
    public string Capital { get; set; }

    [JsonProperty("lat")]
    public double Lattitude { get; set; }

    [JsonProperty("lng")]
    public double Longitude { get; set; }

    [JsonProperty("population")]
    public int Population { get; set; }

    [JsonProperty("density")]
    public double Density { get; set; }

    [JsonProperty("timezone")]
    public string TimeZone { get; set; }

    [JsonProperty("zips")]
    public string Zips { get; set; }


    // Constructor
    public CityInfo(int id, string name, string stateAbbrev, string state, string capital,
                    double lattitude, double longitude, int population, double density,
                    string timeZone, string zips)
    {
        ID = id;
        Name = name;
        StateAbbrev = stateAbbrev;
        State = state;
        Capital = capital;
        Lattitude = lattitude;
        Longitude = longitude;
        Population = population;
        Density = density;
        TimeZone = timeZone;
        Zips = zips;
    }

    // Method to update the city's population and trigger the population change event
    public void UpdatePopulation(int newPopulation, PopulationChangeEvent populationEvent)
    {
        populationEvent.UpdatePopulation(this, newPopulation);
    }

    // Method to display city details
    public override string ToString()
    {
        return $"{Name}, {StateAbbrev}\n\n" +
               $"State:            {State}\n" +
               $"Population:       {Population.ToString("N0")}\n" +
               $"Pop. Density:     {Density.ToString("N2")}\n" +
               $"Longitude:        {Longitude.ToString("N2")}\n" +
               $"Lattitude:        {Lattitude.ToString("N2")}\n" +
               $"Time Zone:        {TimeZone}\n" +
               $"Capital:          {(string.IsNullOrWhiteSpace(Capital) ? "No" : "Yes")}";
    }
}