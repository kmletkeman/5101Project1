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
    public double AdjustedDensity { get; }

    // Constructor initializes all relevant properties of the event
    public PopulationChangedEventArgs(string cityName, string stateAbbrev, int oldPopulation, int newPopulation, double adjustedDensity)
    {
        CityName = cityName;
        OldPopulation = oldPopulation;
        NewPopulation = newPopulation;
        AdjustedDensity = adjustedDensity;
        StateAbbrev = stateAbbrev;
    }
}

public class PopulationChangeEvent
{
    // Define an event
    public event EventHandler<PopulationChangedEventArgs> PopulationChanged = default!;

    // Method to trigger the event
    protected virtual void OnPopulationChanged(PopulationChangedEventArgs e)
    {
        PopulationChanged.Invoke(this, e);
    }

    // Method to update population
    public void UpdatePopulation(CityInfo city, int newPopulation)
    {
        if (newPopulation <= 0 || city == null) return;

        int oldPopulation = city.Population;
        double adjustedDensity = city.Density * ((double)newPopulation / oldPopulation);

        city.Population = newPopulation;
        city.Density = adjustedDensity;

        // Raise the event
        OnPopulationChanged(new PopulationChangedEventArgs(city.Name, city.StateAbbrev, oldPopulation, newPopulation, adjustedDensity));
    }
}