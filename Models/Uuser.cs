using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Race.Models
{
    public class Uuser
    {
        public string idUser { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string uuser { get; set; }
        public string passWord { get; set; }
        public int admin { get; set; }

        public Uuser() { }

        public Uuser(string name, string uuser, string passWord)
        {
            this.name = name;
            this.uuser = uuser;
            this.passWord = passWord;
        }

        
        public Uuser(string idUser, string name, string uuser, string passWord)
        {
            this.idUser = idUser;
            this.name = name;
            this.uuser = uuser;
            this.passWord = passWord;
        }
        
        public string checkLogin(Connexion connexion, string username, string password)
        {
            try
            {
                string query = "SELECT idUser FROM uuser WHERE uuser = '"+username+"' AND passWord = '"+password+"'";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                if (dataReader.Read())
                {
                    string idUser = dataReader.GetString(0);
                    dataReader.Close();
                    return idUser;
                }
                else
                {
                    dataReader.Close();
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return null;
            }
        }

        public bool isAdmin(Connexion connexion, string idUser)
        {
            try
            {
                string query = "SELECT admin FROM uuser WHERE idUser = '"+idUser+"'";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.Read())
                {
                    int adminValue = dataReader.GetInt32(0);
                    dataReader.Close();
                    return adminValue == 1;
                }
                else
                {
                    dataReader.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return false;
            }
        }

        public void createEquipe(Connexion connexion, string key)
        {
            try
            {
                // string query = "INSERT INTO uuser (name, uuser, password, admin) VALUES (Equipe'"+key+"', equipe'"+key+"', '"+key+"', '0')";
                // Console.WriteLine(query);
                // SqlCommand command = new SqlCommand(query, connexion.connection);
                // command.ExecuteNonQuery();
                
                string queryCheckExistence = $"IF NOT EXISTS (SELECT 1 FROM uuser WHERE name = 'Equipe{key}' AND uuser = 'equipe{key}') ";
                string queryInsert = $"INSERT INTO uuser (name, uuser, password, admin) VALUES ('Equipe{key}', 'equipe{key}', '{key}', '0')";

                string finalQuery = queryCheckExistence + queryInsert;

                SqlCommand command = new SqlCommand(finalQuery, connexion.connection);
                Console.WriteLine(finalQuery);

                command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
                Console.WriteLine($"Error: {ex}");
            }
        }

        public string lastId(Connexion connexion)
        {
            string idUser = "";
            try
            {
                string query = "SELECT TOP 1 idUser FROM uuser ORDER BY idUser DESC";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    idUser = dataReader.GetString(0);
                }
                    dataReader.Close();
                    return idUser;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return idUser;
        }

        public List<Uuser> findAll(Connexion connexion)
        {
            List<Uuser> uuserList = new List<Uuser>();
            try
            {
                string query = "SELECT * FROM uuser";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    uuserList.Add(new Uuser(
                        dataReader["idUser"].ToString(),
                        dataReader["name"].ToString(),
                        dataReader["uuser"].ToString(),
                        dataReader["password"].ToString()
                    ));
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return uuserList;
        }

        public List<Uuser> allEquipes(Connexion connexion)
        {
            List<Uuser> uuserList = new List<Uuser>();
            try
            {
                string query = "SELECT * FROM uuser WHERE admin = 0";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    uuserList.Add(new Uuser(
                        dataReader["name"].ToString(),
                        dataReader["uuser"].ToString(),
                        dataReader["password"].ToString()
                    ));
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return uuserList;
        }

        public string getIdUser(Connexion connexion, string password)
        {
            string idUser = "";
            try
            {
                string query = "SELECT idUser FROM uuser WHERE password = '" + password + "'";
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    idUser = dataReader.GetString(0);
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return idUser;
        }

        public List<string> listIdCoureur(Connexion connexion, string idUser)
        {
            List<string> valList = new List<string>();
            try
            {
                string query = "select idCoureur from coureur where idUser = '"+idUser+"'";
                Console.WriteLine(query);
                SqlCommand command = new SqlCommand(query, connexion.connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    valList.Add(dataReader["idCoureur"].ToString());
                }
                dataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
            return valList;

        }

    }
}
