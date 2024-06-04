using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Race.Models
{
    public class Penalite
    {
        public string idpenalite { get; set; }
        public int id { get; set; }
        public string idEtape { get; set; }
        public string etape { get; set; }
        public string iduser { get; set; }
        public string equipe { get; set; }
        public TimeSpan tempsplenalite { get; set; }    
        // public string idEtapeCoureur { get; set; }

        public Penalite(){}

        public Penalite(string idpenalite, int id, string idEtape, string etape, string iduser, string equipe, TimeSpan tempsplenalite)
        {
            this.idpenalite = idpenalite;
            this.id = id;
            this.idEtape = idEtape;
            this.etape = etape;
            this.iduser = iduser;
            this.equipe = equipe;
            this.tempsplenalite = tempsplenalite;
            // this.idEtapeCoureur = idEtapeCoureur;
        }

        public Penalite(string idEtape, string iduser, TimeSpan tempsplenalite)
        {
            this.idEtape = idEtape;
            this.iduser = iduser;
            this.tempsplenalite = tempsplenalite;
            // this.idEtapeCoureur = idEtapeCoureur;
        }


        public static List<Penalite> findAll(Connexion coco)
        {
            List<Penalite> penalites = new List<Penalite>();

            try
            {
                string query = "SELECT * FROM penalite";
                SqlCommand command = new SqlCommand(query, coco.connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Penalite penalite = new Penalite
                    {
                        idpenalite = reader["idpenalite"].ToString(),
                        id = Convert.ToInt32(reader["id"]),
                        idEtape = reader["idEtape"].ToString(),
                        etape = reader["etape"].ToString(),
                        iduser = reader["iduser"].ToString(),
                        equipe = reader["equipe"].ToString(),
                        tempsplenalite = (TimeSpan)reader["tempsplenalite"]
                        // idEtapeCoureur = reader["idEtapeCoureur"].ToString()
                    };

                    penalites.Add(penalite);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }

            return penalites;
        }

        public void create(Connexion connexion)
        {
            try
            {
                string query = "insert into penalite (idEtape, etape, idUser, equipe, tempsPlenalite) values ('"+this.idEtape+"', (select name from etape where idEtape = '"+this.idEtape+"'), '"+this.iduser+"', (select name from uuser where idUser = '"+this.iduser+"'), '"+this.tempsplenalite+"')";

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

        public void createPenalite(Connexion connexion, string idEc, TimeSpan temps)
        {
            try
            {
                string query = "INSERT INTO etapeCoureurTemps (idEtapeCoureur, hDepart, hArriver, temps, dhDepart, dhArriver) VALUES (@idEc, '00:00:00', '00:00:00', @temps, null, null)";

                using (SqlCommand command = new SqlCommand(query, connexion.connection))
                {
                    command.Parameters.AddWithValue("@idEc", idEc);
                    command.Parameters.AddWithValue("@temps", temps);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
        }

        public void penalise(Connexion connexion, string idEtape, string idEquipe, TimeSpan temps)
        {
            try
            {
                List<string> listIdEc = new();
                List<string> listIdCoureur = new Uuser().listIdCoureur(connexion, idEquipe);
                foreach (var id in listIdCoureur)
                {
                    listIdEc.Add(new CoureurTemps().getIdECByetapecoureur(connexion, idEtape, id));
                }

                new Penalite(idEtape, idEquipe, temps).create(connexion);

                foreach (var insert in listIdEc)
                {
                    if (insert != "")
                    {
                        Console.WriteLine(insert);
                        this.createPenalite(connexion, insert, temps);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
        }

        public void delete(Connexion connexion, string idpenalite, string idEquipe, string idEtape)
        {
            try
            {
                string query = "delete from penalite where idPenalite = '"+idpenalite+"' and idUser = '"+idEquipe+"' and idEtape = '"+idEtape+"'";

                using (SqlCommand command = new SqlCommand(query, connexion.connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw ex;
            }
        }
    }
}
