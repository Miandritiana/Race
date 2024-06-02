using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Race.Models
{
    public class Coureur
    {
        public string idCoureur { get; set; }
        public int id { get; set; }
        public string nom { get; set; }
        public string numDossard { get; set; }
        public string genre { get; set; }
        public DateTime dtn { get; set; }
        public string idUser { get; set; }
        public string equipe { get; set; }
        public string idEtape { get; set; }
        public string idEquipe { get; set; }
        public string idEtapeCoureur { get; set; }

        public Coureur() { }

        public Coureur(string idCoureur, string nom, string numDossard, string genre, DateTime dtn, string equipe, string idUser)
        {
            this.idCoureur = idCoureur;
            this.nom = nom;
            this.numDossard = numDossard;
            this.genre = genre;
            this.dtn = dtn;
            this.equipe = equipe;
            this.idUser = idUser;
        }

        public Coureur (string idEtape, string idEquipe, string idCoureur)
        {
            this.idEtape = idEtape;
            this.idEquipe = idEquipe;
            this.idCoureur = idCoureur;
        }

        public Coureur(string idEtapeCoureur, string idEtape, string idUser, string equipe, string idCoureur, string nom, string numDossard, string genre, DateTime dtn)
        {
            this.idEtapeCoureur = idEtapeCoureur;
            this.idEtape = idEtape;
            this.idUser = idUser;
            this.equipe = equipe;
            this.idCoureur = idCoureur;
            this.nom = nom;
            this.numDossard = numDossard;
            this.genre = genre;
            this.dtn = dtn;
        }

        public List<Coureur> findAll(Connexion connexion)
        {
            List<Coureur> coureurList = new List<Coureur>();
            try
            {
                string query = "SELECT * FROM v_coureur";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    coureurList.Add(new Coureur(
                        dataReader["idCoureur"].ToString(),
                        dataReader["nom"].ToString(),
                        dataReader["numDossard"].ToString(),
                        dataReader["genre"].ToString(),
                        (DateTime)dataReader["dtn"],
                        dataReader["name"].ToString(),
                        dataReader["idUser"] != DBNull.Value ? dataReader["idUser"].ToString() : null
                    ));
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
            return coureurList;
        }

        public List<Coureur> findByIdEquipe(Connexion connexion, string idEquipe)
        {
            List<Coureur> coureurList = new List<Coureur>();
            try
            {
                string query = "SELECT * FROM v_coureur where idUser ='"+idEquipe+"'";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    coureurList.Add(new Coureur(
                        dataReader["idCoureur"].ToString(),
                        dataReader["nom"].ToString(),
                        dataReader["numDossard"].ToString(),
                        dataReader["genre"].ToString(),
                        (DateTime)dataReader["dtn"],
                        dataReader["name"].ToString(),
                        dataReader["idUser"] != DBNull.Value ? dataReader["idUser"].ToString() : null
                    ));
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
            return coureurList;
        }

        public void create(Connexion connexion)
        {
            try
            {
                string query = "INSERT INTO coureur (nom, numDossard, genre, dtn, idUser) VALUES (@nom, @numDossard, @genre, @dtn, @idUser)";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.Parameters.AddWithValue("@nom", this.nom);
                command.Parameters.AddWithValue("@numDossard", this.numDossard);
                command.Parameters.AddWithValue("@genre", this.genre);
                command.Parameters.AddWithValue("@dtn", this.dtn);
                command.Parameters.AddWithValue("@idUser", (object)this.idUser ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
        }

        public void update(Connexion connexion)
        {
            try
            {
                string query = "UPDATE coureur SET nom = @nom, numDossard = @numDossard, genre = @genre, dtn = @dtn, idUser = @idUser WHERE idCoureur = @idCoureur";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.Parameters.AddWithValue("@idCoureur", this.idCoureur);
                command.Parameters.AddWithValue("@nom", this.nom);
                command.Parameters.AddWithValue("@numDossard", this.numDossard);
                command.Parameters.AddWithValue("@genre", this.genre);
                command.Parameters.AddWithValue("@dtn", this.dtn);
                command.Parameters.AddWithValue("@idUser", (object)this.idUser ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
        }

        public void createEtapeCoureur(Connexion connexion)
        {
            try
            {
                string query = "INSERT INTO etapecoureur (idEtape, idUser, idCoureur) VALUES (@idEtape, @idUser, @idCoureur)";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.Parameters.AddWithValue("@idEtape", this.idEtape);
                command.Parameters.AddWithValue("@idUser", this.idEquipe);
                command.Parameters.AddWithValue("@idCoureur", this.idCoureur);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }

        }

        public List<Coureur> etapeCoureur(Connexion connexion, string idEtape)
        {
            List<Coureur> coureurList = new List<Coureur>();
            try
            {
                string query = "SELECT * FROM v_infoEtapeCoureur where idEtape ='"+idEtape+"' order by equipe";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                // string idEtapeCoureur, string idEtape, string idUser, string equipe, string idCoureur, string nom, string numDossard, string genre, DateTime dtn
                while (dataReader.Read())
                {
                    coureurList.Add(new Coureur(
                        dataReader["idEtapeCoureur"].ToString(),
                        dataReader["idEtape"].ToString(),
                        dataReader["idUser"].ToString(),
                        dataReader["equipe"].ToString(),
                        dataReader["idCoureur"].ToString(),
                        dataReader["coureur"].ToString(),
                        dataReader["numDossard"].ToString(),
                        dataReader["genre"].ToString(),
                        (DateTime)dataReader["dtn"]
                    ));
                }

                dataReader.Close();
            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
            return coureurList;
        }
    }
}
