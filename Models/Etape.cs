using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Race.Models
{
    public class Etape
    {
        public string idEtape { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public double lkm { get; set; }
        public int nbCoureur { get; set; }
        public string rangEtape { get; set; }
        public DateTime dhDepart { get; set; }

        public Etape() { }

        public Etape(string idEtape, int id, string name, double lkm, int nbCoureur, string rangEtape, DateTime dhDepart)
        {
            this.idEtape = idEtape;
            this.id = id;
            this.name = name;
            this.lkm = lkm;
            this.nbCoureur = nbCoureur;
            this.rangEtape = rangEtape;
            this.dhDepart = dhDepart;
        }

        public Etape(string name, double lkm, int nbCoureur, string rangEtape, DateTime dhDepart)
        {
            this.name = name;
            this.lkm = lkm;
            this.nbCoureur = nbCoureur;
            this.rangEtape = rangEtape;
            this.dhDepart = dhDepart;
        }

        public Etape(string name, double lkm, int nbCoureur, string rangEtape)
        {
            this.name = name;
            this.lkm = lkm;
            this.nbCoureur = nbCoureur;
            this.rangEtape = rangEtape;
        }

        public List<Etape> findAll(Connexion connexion)
        {
            List<Etape> etapeList = new List<Etape>();
            try
            {
                string query = "SELECT * FROM etape order by rangEtape asc";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    etapeList.Add(new Etape(
                        dataReader["idEtape"].ToString(),
                        (int)dataReader["id"],
                        dataReader["name"].ToString(),
                        dataReader.GetDouble(dataReader.GetOrdinal("lkm")),
                        (int)dataReader["nbCoureur"],
                        dataReader["rangEtape"].ToString(),
                        DateTime.Parse(dataReader["dhDepart"].ToString())
                    ));
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return etapeList;
        }

        public Etape findById(Connexion connexion, string idEtape)
        {
            Etape etape = null;
            try
            {
                string query = "SELECT * FROM etape WHERE idEtape = @idEtape";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.Parameters.AddWithValue("@idEtape", idEtape);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    etape = new Etape(
                        dataReader["idEtape"].ToString(),
                        (int)dataReader["id"],
                        dataReader["name"].ToString(),
                        dataReader.GetDouble(dataReader.GetOrdinal("lkm")),
                        (int)dataReader["nbCoureur"],
                        dataReader["rangEtape"].ToString(),
                        DateTime.Parse(dataReader["dhDepart"].ToString())
                    );
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return etape;
        }

        public void create(Connexion connexion)
        {
            try
            {
                string query = "INSERT INTO etape (name, lkm, nbCoureur, rangEtape, dhDepart) VALUES ('"+this.name+"', "+this.lkm+", "+this.nbCoureur+", 'rang"+this.rangEtape+"', CONVERT(datetime, '"+this.dhDepart.ToString("yyyy-MM-dd hh:mm:ss")+"', 120))";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        public void update(Connexion connexion)
        {
            try
            {
                string query = "UPDATE etape SET name = @name, lkm = @lkm, nbCoureur = @nbCoureur, rangEtape = @rangEtape WHERE idEtape = @idEtape";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.Parameters.AddWithValue("@idEtape", this.idEtape);
                command.Parameters.AddWithValue("@name", this.name);
                command.Parameters.AddWithValue("@lkm", this.lkm);
                command.Parameters.AddWithValue("@nbCoureur", this.nbCoureur);
                command.Parameters.AddWithValue("@rangEtape", this.rangEtape);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        public string getIdEtapeRg(Connexion connexion, string rangEtape)
        {
            string idEtape = null;
            try
            {
                string query = "SELECT idEtape FROM etape WHERE rangEtape = 'rang"+rangEtape+"'";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    idEtape = dataReader["idEtape"].ToString();
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return idEtape;
        }

        public DateTime getDateparIdEtape(Connexion connexion, string idEtape)
        {
            DateTime dhDepart = new DateTime();
            try
            {
                string query = "SELECT dhDepart FROM etape WHERE idEtape = @idEtape";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.Parameters.AddWithValue("@idEtape", idEtape);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    dhDepart = DateTime.Parse(dataReader["dhDepart"].ToString());
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return dhDepart;
        }

        public string getIdEtape(Connexion connexion, string rangEtape)
        {
            
            string idEtape = null;
            try
            {
                string query = "select idEtape from etape where rangEtape like '%"+rangEtape+"%'";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    idEtape = dataReader["idEtape"].ToString();
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return idEtape;
        }
    }
}
