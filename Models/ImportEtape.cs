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
    public class ImportEtape
    {
        public string etape { get; set; }
        public double longueur { get; set; }
        public int nb_coureur { get; set; }
        public string rang { get; set; }
        public DateTime date_depart { get; set; }
        public TimeSpan heure_depart { get; set; }

        public ImportEtape (){}

        public ImportEtape (string etape, double longueur, int nb_coureur, string rang, DateTime date_depart, TimeSpan heure_depart)
        {
            this.etape = etape;
            this.longueur = longueur;
            this.nb_coureur = nb_coureur;
            this.rang = rang;
            this.date_depart = date_depart;
            this.heure_depart = heure_depart;
        }

        public DateTime combineDateTimeAndTimeSpan(string dateString, string timeString)
        {
            DateTime date = DateTime.ParseExact(dateString, "dd/MM/yyyy hh:mm:ss", null);
            TimeSpan time = TimeSpan.Parse(timeString);
            return date.Add(time);
        }

        public void insert (Connexion coco, ImportEtape impo)
        {
            try
            {
                Console.WriteLine(this.combineDateTimeAndTimeSpan(impo.date_depart.ToString(), impo.heure_depart.ToString()));
                new Etape(impo.etape, impo.longueur, impo.nb_coureur, impo.rang, this.combineDateTimeAndTimeSpan(impo.date_depart.ToString(), impo.heure_depart.ToString())).create(coco);
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

                        ImportEtape impo = new ImportEtape(
                            processedValues[0],
                            double.Parse(processedValues[1]),
                            int.Parse(processedValues[2]),
                            processedValues[3],
                            DateTime.Parse(processedValues[4]),
                            TimeSpan.Parse(processedValues[5])
                        );
                        
                        Console.WriteLine(impo.etape);
                        Console.WriteLine(impo.longueur);
                        Console.WriteLine(impo.nb_coureur);
                        Console.WriteLine(impo.rang);
                        Console.WriteLine(impo.date_depart);
                        Console.WriteLine(impo.heure_depart);
                    
                    impo.insert(coco, impo);

                Console.WriteLine("\n");

                }

                return "CSV file uploaded and data imported into the database.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return "Error importing CSV file: " + ex.Message;
            }
        }
    }
}