using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// EventArgs subclass to store details of a population change event
public class PopulationChangedEventArgs : EventArgs
{
    public string CityName { get; }
    public string StateAbbrev { get; }
    public int OldPopulation { get; }
    public int NewPopulation { get; }
    public double OldDensity { get; }
    public double AdjustedDensity { get; }

    // Constructor initializes all relevant properties of the event
    public PopulationChangedEventArgs(string cityName, string stateAbbrev, int oldPopulation, int newPopulation, double oldDensity, double adjustedDensity)
    {
        CityName = cityName;
        StateAbbrev = stateAbbrev;
        OldPopulation = oldPopulation;
        NewPopulation = newPopulation;
        OldDensity = oldDensity;
        AdjustedDensity = adjustedDensity;
    }
}

// Class responsible for handling population updates and raising events
public class PopulationChangeEvent
{
    // Delegate and event declaration for notifying population change
    public delegate void PopulationChangedHandler(object sender, PopulationChangedEventArgs e);
    public event PopulationChangedHandler OnPopulationChanged;

    private Dictionary<string, List<CityInfo>> _cityData;

    // Constructor initializes the dictionary containing city data
    public PopulationChangeEvent(Dictionary<string, List<CityInfo>> cityData)
    {
        _cityData = cityData;
    }

    // Method to update population and adjust density accordingly
    public void UpdatePopulation(string cityName, string stateAbbrev, int newPopulation)
    {
        if (_cityData.ContainsKey(cityName)) // Check if city exists
        {
            var city = _cityData[cityName].FirstOrDefault(c => c.StateAbbrev == stateAbbrev);
            if (city != null)
            {
                int oldPopulation = city.Population;
                double oldDensity = city.Density;

                if (oldPopulation > 0) // Avoid division by zero
                {
                    double adjustedDensity = oldDensity * ((double)newPopulation / oldPopulation);

                    // Update city details with new values
                    city.Population = newPopulation;
                    city.Density = adjustedDensity;

                    // Raise event to notify listeners of the change
                    OnPopulationChanged?.Invoke(this, new PopulationChangedEventArgs(cityName, stateAbbrev, oldPopulation, newPopulation, oldDensity, adjustedDensity));

                    Console.WriteLine($"Population updated for {cityName}, {stateAbbrev}: {oldPopulation} -> {newPopulation}");
                    Console.WriteLine($"Density adjusted: {oldDensity} -> {adjustedDensity}");
                }
                else
                {
                    Console.WriteLine("Invalid old population value.");
                }
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
}
