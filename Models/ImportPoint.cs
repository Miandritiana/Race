using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Race.Models
{
    public class ImportPoint
    {
        public string classement { get; set; }
        public int points { get; set; }
        public ImportPoint (){}

        public ImportPoint (string classement, int points)
        {
            this.classement = classement;
            this.points = points;
        }

        public void insert (Connexion coco, ImportPoint impo)
        {
            try
            {
                new CoureurTemps(impo.classement, impo.points).createPoint(coco);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string import(IFormFile csvFile, Connexion coco)
        {
            try
            {
                var csvContent = new List<string>();
                using (var reader = new StreamReader(csvFile.OpenReadStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            csvContent.Add(line);
                        }
                    }
                }

                for (int i = 1; i < csvContent.Count; i++)
                {
                    List<string> processedValues = new List<string>();
                    StringBuilder currentValue = new StringBuilder();
                    bool insideQuotes = false;

                    foreach (char c in csvContent[i])
                    {
                        if (c == '"' && !insideQuotes)
                        {
                            insideQuotes = true;
                        }
                        else if (c == '"' && insideQuotes)
                        {
                            insideQuotes = false;
                        }
                        else if (c == ',' && !insideQuotes)
                        {
                            processedValues.Add(currentValue.ToString());
                            currentValue.Clear();
                        }
                        else
                        {
                            currentValue.Append(c);
                        }
                    }

                    processedValues.Add(currentValue.ToString());

                    for (int j = 0; j < processedValues.Count; j++)
                    {
                        if (double.TryParse(processedValues[j], out double result))
                        {
                            processedValues[j] = processedValues[j].Replace(',', '.');
                        }
                    }

                        // DateTime date_depart = DateTime.ParseExact(processedValues[4], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        // TimeSpan heure_depart = TimeSpan.ParseExact(processedValues[5], "HH:mm:ss", CultureInfo.InvariantCulture);

                        ImportPoint impo = new ImportPoint(
                            processedValues[0],
                            int.Parse(processedValues[1])
                        );
                        
                        Console.WriteLine(impo.classement);
                        Console.WriteLine(impo.points);
                    
                    impo.insert(coco, impo);

                Console.WriteLine("\n");

                }

                return "CSV file uploaded and data imported into the database. ho any point";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return "Error importing CSV file any point: " + ex.Message;
            }
        }
    }
}