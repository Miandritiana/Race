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

        public string idCategory { get; set; }
        public string category { get; set; }

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

        public Coureur (string idCoureur, string idCategory, int id)
        {
            this.idCoureur = idCoureur;
            this.idCategory = idCategory;
            this.id = id;
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

        public Coureur(string nom, string numDossard, string genre, DateTime dtn, string idUser)
        {
            this.nom = nom;
            this.numDossard = numDossard;
            this.genre = genre;
            this.dtn = dtn;
            this.idUser = idUser;
        }

        public Coureur (string idEtape, string idCoureur)
        {
            this.idEtape = idEtape;
            this.idCoureur = idCoureur;
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

        public string getIdCategory(Connexion connexion, string category)
        {
            string coureurList = "";
            try
            {
                string query = "SELECT idCategory FROM category where name='"+category+"'";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    coureurList = dataReader["idCategory"].ToString();
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
                string query = @"
                    IF NOT EXISTS (SELECT 1 FROM coureur WHERE nom = @nom AND numDossard = @numDossard)
                    BEGIN
                        INSERT INTO coureur (nom, numDossard, genre, dtn, idUser) 
                        VALUES (@nom, @numDossard, @genre, @dtn, @idUser)
                    END";
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
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }

        }

        public void createEtapeCoureur2(Connexion connexion)
        {
            try
            {
                string query = "INSERT INTO etapecoureur (idEtape, idUser, idCoureur) VALUES ('"+this.idEtape+"', (select idUser from coureur where idCoureur='"+this.idCoureur+"'), '"+this.idCoureur+"')";
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

        // public void createEtapeCoureur2(Connexion connexion, string idEtape, string idCoureur)
        // {
        //     try
        //     {
        //         string query = "INSERT INTO etapecoureur (idEtape, idUser, idCoureur) " +
        //                "SELECT '" + idEtape + "', (SELECT idUser FROM coureur WHERE idCoureur = '" + idCoureur + "'), '" + idCoureur + "' " +
        //                "WHERE NOT EXISTS (SELECT 1 FROM etapecoureur WHERE idEtape = '" + idEtape + "' AND idCoureur = '" + idCoureur + "')";

        //         // string query = "INSERT INTO etapecoureur (idEtape, idUser, idCoureur) VALUES ('"+idEtape+"', (select idUser from coureur where idCoureur='"+idCoureur+"'), '"+idCoureur+"')";
        //         Console.WriteLine(query);
        //         SqlCommand command = new SqlCommand(query, connexion.connection);
        //         command.ExecuteNonQuery();
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error: {ex}");
        //         throw ex;
        //     }

        // }

        public void createEtapeCoureur2(Connexion connexion, string idEtape, string idCoureur)
        {
            try
            {
                // Check if the combination of idEtape and idCoureur already exists
                string checkQuery = "SELECT COUNT(*) FROM etapecoureur WHERE idEtape = @idEtape AND idCoureur = @idCoureur";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connexion.connection);
                checkCommand.Parameters.AddWithValue("@idEtape", idEtape);
                checkCommand.Parameters.AddWithValue("@idCoureur", idCoureur);
                int count = (int)checkCommand.ExecuteScalar();

                // Insert only if the combination does not exist
                if (count == 0)
                {
                    string insertQuery = "INSERT INTO etapecoureur (idEtape, idUser, idCoureur) " +
                                        "SELECT @idEtape, idUser, @idCoureur FROM coureur WHERE idCoureur = @idCoureur";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connexion.connection);
                    insertCommand.Parameters.AddWithValue("@idEtape", idEtape);
                    insertCommand.Parameters.AddWithValue("@idCoureur", idCoureur);
                    insertCommand.ExecuteNonQuery();
                    Console.WriteLine("Record inserted successfully.");
                }
                else
                {
                    Console.WriteLine("The idCoureur already exists for the given idEtape.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }


        public string lastIdetapeCoureur(Connexion connexion)
        {
            string idEtape = "";
            try
            {
                string query = "SELECT TOP 1 idEtapeCoureur FROM etapecoureur ORDER BY id DESC";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    idEtape = dataReader.GetString(0);
                }
                    dataReader.Close();
                    return idEtape;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
            return idEtape;
        }

        public List<Coureur> etapeCoureur(Connexion connexion, string idEtape)
        {
            List<Coureur> coureurList = new List<Coureur>();
            try
            {
                // string query = "SELECT * FROM v_infoEtapeCoureur where idEtape ='"+idEtape+"' order by equipe";
                string query = "SELECT * FROM v_infoEtapeCoureur where idEtape ='"+idEtape+"' and idCoureur not in (select c.idCoureur from etapeCoureurTemps e join etapeCoureur ec on e.idEtapeCoureur = e.idEtapeCoureur join coureur c on c.idCoureur = ec.idCoureur) order by equipe";
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

        public string lastIdCoureur(Connexion connexion)
        {
            try
            {
                string query = "SELECT TOP 1 idCoureur FROM coureur ORDER BY id DESC";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                string id = command.ExecuteScalar().ToString();
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
        }

        public string getIdCoureurByNom(Connexion connexion, string nom)
        {
            try
            {
                string query = "SELECT idCoureur FROM coureur where nom = '"+nom+"'";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                string id = command.ExecuteScalar().ToString();
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
        }

        public void createCategoryCoureur(Connexion connexion)
        {
            try
            {
                string query = "INSERT INTO categoryCoureur (idCoureur, idCategory) VALUES ('"+this.idCoureur+"', '"+this.idCategory+"')";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }

        }

        public int getAge(DateTime dtn)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dtn.Year;
            if (dtn.Date > today.AddYears(-age)) age--;
            return age;
        }

        public void generateCategory(Connexion coco)
        {
            try
            {
                string category = "";
                string genre = "";

                List<Coureur> coureurList = this.findAll(coco);
                foreach (var coureur in coureurList)
                {
                    if (this.getAge(coureur.dtn) < 18)
                    {
                        category = "junior";

                    }else
                    {
                        category = "senior";
                    }

                    if (coureur.genre == "Masculin")
                    {
                        genre = "homme";

                    }else if(coureur.genre == "Feminin")
                    {
                        genre = "femme";
                    }

                    Console.WriteLine(coureur.idCoureur + " , " +this.getIdCategory(coco, category));
                    Console.WriteLine(coureur.idCoureur + " , " +this.getIdCategory(coco, genre));

                    new Coureur(coureur.idCoureur, this.getIdCategory(coco, category), 1).createCategoryCoureur(coco);
                    new Coureur(coureur.idCoureur, this.getIdCategory(coco, genre), 1).createCategoryCoureur(coco);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public List<Coureur> listCAtegory(Connexion connexion)
        {
            List<Coureur> coureurList = new List<Coureur>();
            try
            {
                string query = "SELECT * FROM category";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    // (string idCoureur, string idCategory, int id)
                    coureurList.Add(new Coureur(
                        dataReader["idCategory"].ToString(),
                        dataReader["name"].ToString(),
                        (int)dataReader["id"]
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
