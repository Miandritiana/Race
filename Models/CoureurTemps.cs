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

        public CoureurTemps() { }

        public CoureurTemps(string idECTemps, int id, string idEtapeCoureur, TimeSpan temps)
        {
            this.idECTemps = idECTemps;
            this.id = id;
            this.idEtapeCoureur = idEtapeCoureur;
            this.temps = temps;
        }

        public CoureurTemps(string idEtapeCoureur, TimeSpan hDepart, TimeSpan hArriver)
        {
            this.idEtapeCoureur = idEtapeCoureur;
            this.hDepart = hDepart;
            this.hArriver = hArriver;
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

        public TimeSpan duration(TimeSpan hDepart, TimeSpan hArriver)
        {
            return hArriver - hDepart;
        }

        public void create(Connexion connexion)
        {
            try
            {
                TimeSpan temps = this.duration(this.hDepart, this.hArriver);
                string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps) VALUES ('"+this.idEtapeCoureur+"', '"+this.hDepart+"', '"+this.hArriver+"', '"+temps+"')";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
        }

    }
}
