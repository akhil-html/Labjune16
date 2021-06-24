using System;
using System.Linq;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;
            var total_2020_precipitation = measurements
                .Where(weatherObservation => weatherObservation.year == 2020)
                .Select(weatherObservation => weatherObservation.precipitation)
                .Sum();
            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");

            //
            // Heating Degree days have a mean temp of < 18C
            //
            var heatingDegreeDays = measurements
                .Where(weatherObservation => weatherObservation.meantemp < 18)
                .GroupBy(
                    weatherObservation => weatherObservation.year,
                    weatherObservation => weatherObservation,
                    (key, yearlyWeatherObservation) => new{
                        year = key,
                        count = yearlyWeatherObservation.Count()
                    }
                );

            //
            // Cooling degree days have a mean temp of >=18C
            //
            var coolingDegreeDays = measurements
                .Where(weatherObservation => weatherObservation.meantemp >= 18)
                .GroupBy(
                    weatherObservation => weatherObservation.year,
                    weatherObservation => weatherObservation,
                    (key, yearlyWeatherObservation) => new{
                        year = key,
                        count = yearlyWeatherObservation.Count()
                    }
                );

            //
            // Most Variable days are the days with the biggest temperature
            // range. That is, the largest difference between the maximum and
            // minimum temperature
            //
            // Oh: and number formatting to zero pad.
            // 
            // For example, if you want:
            //      var x = 2;
            // To display as "0002" then:
            //      $"{x:d4}"
            //

            Console.WriteLine("Year\tHDD\tCDD");
            var records = heatingDegreeDays
                .Join(
                    coolingDegreeDays,
                    heatingDegreeDay => heatingDegreeDay.year,
                    coolingDegreeDay => coolingDegreeDay.year,
                    (heatingDegreeDay, coolingDegreeDay) => new
                    {
                        year = heatingDegreeDay.year,
                        heatingDegreeDay = heatingDegreeDay.count,
                        coolingDegreeDay = coolingDegreeDay.count
                    }
                ).OrderBy(record=>record.year);
            foreach (var record in records)
            {
                Console.WriteLine($"{record.year:d4}\t{record.heatingDegreeDay:d4}\t{record.coolingDegreeDay:d4}");
            }

            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");
            var mostVariableDayRecords = measurements
                .Select(weatherObservation => new
                {
                    year = weatherObservation.year,
                    month = weatherObservation.month,
                    day = weatherObservation.day,
                    temperatureDifference = weatherObservation.maxtemp - weatherObservation.mintemp,
                })
                .OrderByDescending(record=>record.temperatureDifference)
                .Take(5);
             foreach(var record in mostVariableDayRecords)
            {
                Console.WriteLine($"{record.year:d4}-{record.month:d2}-{record.day:d2}\t{record.temperatureDifference:0.00}");
            }                      
        }
    }
}
