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
    public class ImportResult
    {
        public string etape_rang { get; set; }
        public string numero_dossard { get; set; }
        public string nom { get; set; }
        public string genre { get; set; }
        public DateTime date_naissance { get; set; }
        public string equipe { get; set; }
        public DateTime arrivee { get; set; }

        public ImportResult (){}

        public ImportResult (string etape_rang, string numero_dossard, string nom, string genre, DateTime date_naissance, string equipe, DateTime arrivee)
        {
            this.etape_rang = etape_rang;
            this.numero_dossard = numero_dossard;
            this.nom = nom;
            this.genre = genre;
            this.date_naissance = date_naissance;
            this.equipe = equipe;
            this.arrivee = arrivee;
        }
        public void insert (Connexion coco, ImportResult impo)
        {
            try
            {
                Uuser u = new Uuser();
                u.createEquipe(coco, impo.equipe);
                string lastIdEquipe = u.lastId(coco);

                Coureur c = new();
                string genre = "";
                if (impo.genre == "M" || impo.genre == "m")
                {
                    genre = "Masculin";
                }else if (impo.genre == "F" || impo.genre == "f")
                {
                    genre = "Feminin";
                }
                new Coureur(impo.nom, impo.numero_dossard, genre, impo.date_naissance, lastIdEquipe).create(coco);
                string lastIdCoureur = c.lastIdCoureur(coco);

                string idEtape = new Etape().getIdEtapeRg(coco, impo.etape_rang);

                new Coureur(idEtape, lastIdCoureur).createEtapeCoureur2(coco);

                string lastIdetapeCoureur = c.lastIdetapeCoureur(coco);

                DateTime dhDepart = new Etape().getDateparIdEtape(coco, idEtape);
                TimeSpan hDepart = new TimeSpan();
                TimeSpan hArriver = new TimeSpan();
                if (DateTime.TryParse(dhDepart.ToString(), out DateTime dateTime))
                {
                    hDepart = dateTime.TimeOfDay;
                }
                if (DateTime.TryParse(impo.arrivee.ToString(), out DateTime dateTimeArriver))
                {
                    hArriver = dateTimeArriver.TimeOfDay;
                }
                new CoureurTemps(lastIdetapeCoureur, hDepart, hArriver, dhDepart, impo.arrivee).create(coco);
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

                        ImportResult impo = new ImportResult(
                            processedValues[0],
                            processedValues[1],
                            processedValues[2],
                            processedValues[3],
                            DateTime.Parse(processedValues[4]),
                            processedValues[6],
                            DateTime.Parse(processedValues[7])
                        );
                        
                        Console.WriteLine(impo.etape_rang);
                        Console.WriteLine(impo.numero_dossard);
                        Console.WriteLine(impo.nom);
                        Console.WriteLine(impo.genre);
                        Console.WriteLine(impo.date_naissance);
                        Console.WriteLine(impo.equipe);
                        Console.WriteLine(impo.arrivee);
                    
                    // impo.insert(coco, impo);

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