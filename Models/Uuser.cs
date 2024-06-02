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
        public string num { get; set; }

        public Uuser() { }

        public Uuser(string idUser, int id, string name, string passWord, int admin, string num)
        {
            this.idUser = idUser;
            this.id = id;
            this.name = name;
            this.passWord = passWord;
            this.admin = admin;
            this.num = num;
        }

        public Uuser(string num)
        {
            this.num = num;
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

        // public void createClient(Connexion connexion, Uuser uuser)
        // {
        //     try
        //     {
        //         string query = "INSERT INTO uuser (name, uuser, password, admin) VALUES ('RAsoa', '123', '0')";
        //         SqlCommand command = new SqlCommand(query, connexion.connection);
        //         command.Parameters.AddWithValue("@num", uuser.num);
        //         command.ExecuteNonQuery();
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error: {ex}");
        //     }
        // }

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

    }
}
