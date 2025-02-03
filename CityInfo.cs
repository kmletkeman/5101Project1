using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CityInfo
{
    // Properties
    public int ID { get; set; }
    public string Name { get; set; }
    public string StateAbbrev { get; set; }
    public string State { get; set; }
    public string Capital { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Population { get; set; }
    public double Density { get; set; }
    public string TimeZone { get; set; }
    public string Zips { get; set; }

    // Constructor
    public CityInfo(int id, string name, string stateAbbrev, string state, string capital,
                    double latitude, double longitude, int population, double density,
                    string timeZone, string zips)
    {
        ID = id;
        Name = name;
        StateAbbrev = stateAbbrev;
        State = state;
        Capital = capital;
        Latitude = latitude;
        Longitude = longitude;
        Population = population;
        Density = density;
        TimeZone = timeZone;
        Zips = zips;
    }

    // Method to update population and adjust density
    public void UpdatePopulation(int newPopulation)
    {
        if (newPopulation > 0 && newPopulation != Population)
        {
            double oldPopulation = Population;
            Population = newPopulation;
            Density = Density * (newPopulation / oldPopulation);
        }
    }

    // Method to display city details
    public override string ToString()
    {
        return $"{Name}, {StateAbbrev} ({State})\n" +
               $"Capital: {Capital}\n" +
               $"Latitude: {Latitude}, Longitude: {Longitude}\n" +
               $"Population: {Population}, Density: {Density}\n" +
               $"Time Zone: {TimeZone}\n" +
               $"ZIP Codes: {Zips}\n";
    }
}
