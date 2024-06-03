using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Race.Models
{
    public class CoureurTemps
    {
        public string idECTemps { get; set; }
        public int id { get; set; }
        public string idEtapeCoureur { get; set; }
        public TimeSpan hDepart { get; set; }
        public TimeSpan hArriver { get; set; }
        public TimeSpan temps { get; set; }
        public DateTime dhDepart { get; set; }
        public DateTime dhArriver { get; set; }

        public string classement { get; set; }
        public int points { get; set; }


        public CoureurTemps() { }

        public CoureurTemps(string idECTemps, int id, string idEtapeCoureur, TimeSpan temps)
        {
            this.idECTemps = idECTemps;
            this.id = id;
            this.idEtapeCoureur = idEtapeCoureur;
            this.temps = temps;
        }

        public CoureurTemps(string idEtapeCoureur, TimeSpan hDepart, TimeSpan hArriver, DateTime dhDepart, DateTime dhArriver)
        {
            this.idEtapeCoureur = idEtapeCoureur;
            this.hDepart = hDepart;
            this.hArriver = hArriver;
            this.dhDepart = dhDepart;
            this.dhArriver = dhArriver;
        }

        public CoureurTemps(string classement, int points)
        {
            this.classement = classement;
            this.points = points;
        }

        public static List<CoureurTemps> findAll(Connexion connexion)
        {
            List<CoureurTemps> CoureurTempsList = new List<CoureurTemps>();
            try
            {
                string query = "SELECT * FROM CoureurTemps";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    CoureurTempsList.Add(new CoureurTemps(
                        dataReader["idECTemps"].ToString(),
                        (int)dataReader["id"],
                        dataReader["idEtapeCoureur"].ToString(),
                        (TimeSpan)dataReader["temps"]
                    ));
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return CoureurTempsList;
        }

        public TimeSpan duration(DateTime dhDepart, DateTime dhArriver)
        {
            return dhArriver - dhDepart;
        }

        public void create(Connexion connexion)
        {
            try
            {
                    TimeSpan temps = this.duration(this.dhDepart, this.dhArriver);
                    // string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps, dhDepart, dhArriver) VALUES ('"
                    //         + this.idEtapeCoureur + "', '"
                    //         + this.hDepart + "', '"
                    //         + this.hArriver + "', '"
                    //         + temps + "', CONVERT(datetime, '"
                    //         + this.dhDepart.ToString("yyyy-MM-dd hh:mm:ss tt") + "', 120), CONVERT(datetime, '"
                    //         + this.dhArriver.ToString("yyyy-MM-dd hh:mm:ss tt") + "', 120))";

                    string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps, dhDepart, dhArriver) VALUES ('"
                            + this.idEtapeCoureur + "', '"
                            + this.hDepart + "', '"
                            + this.hArriver + "', '"
                            + temps + "', CONVERT(datetime, '"
                            + this.dhDepart.ToString("yyyy-MM-dd HH:mm:ss tt") + "', 121), CONVERT(datetime, '"
                            + this.dhArriver.ToString("yyyy-MM-dd HH:mm:ss tt") + "', 121))";


                // string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps, dhDepart, dhArriver) VALUES ('"+this.idEtapeCoureur+"', '"+this.hDepart+"', '"+this.hArriver+"', '"+temps+"', CONVERT(datetime, '"+this.dhDepart.ToString("yyyy-MM-dd hh:mm:ss")+"', 120), CONVERT(datetime, '"+this.dhArriver.ToString("yyyy-MM-dd hh:mm:ss")+"', 120))";
                // string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps, dhDepart, dhArriver) VALUES ('"+this.idEtapeCoureur+"', '"+this.hDepart+"', '"+this.hArriver+"', '"+temps+"', '"+this.dhDepart+"', '"+this.dhArriver+"')";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
        }

        // public void create(Connexion connexion)
        // {
        //     try
        //     {
        //         TimeSpan temps = this.duration(this.dhDepart, this.dhArriver);
        //         string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps, dhDepart, dhArriver) " +
        //                        "VALUES (@idEtapeCoureur, @hDepart, @hArriver, @temps, @dhDepart, @dhArriver)";

        //         using (SqlCommand command = new SqlCommand(query, connexion.connection))
        //         {
        //             command.Parameters.AddWithValue("@idEtapeCoureur", this.idEtapeCoureur);
        //             command.Parameters.AddWithValue("@hDepart", this.hDepart);
        //             command.Parameters.AddWithValue("@hArriver", this.hArriver);
        //             command.Parameters.AddWithValue("@temps", temps);
        //             command.Parameters.AddWithValue("@dhDepart", this.dhDepart);
        //             command.Parameters.AddWithValue("@dhArriver", this.dhArriver);

        //             command.ExecuteNonQuery();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error: {ex}");
        //         throw;
        //     }
        // }

        public void createPoint(Connexion connexion)
        {
            try
            {
                string query = "INSERT INTO point (classement, points) VALUES ('"+this.classement+"', '"+this.points+"')";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }

        }
    }
}
