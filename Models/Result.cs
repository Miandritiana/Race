using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Race.Models
{
    public class Result
    {
        public string idECTemps { get; set; }
        public string idEtapeCoureur { get; set; }
        public string idEtape { get; set; }
        public string etape { get; set; }
        public string idUser { get; set; }
        public string equipe { get; set; }
        public string idCoureur { get; set; }
        public string coureur { get; set; }
        public string numDossard { get; set; }
        public string genre { get; set; }
        public DateTime dtn { get; set; }
        public TimeSpan hDepart { get; set; }
        public TimeSpan hArriver { get; set; }
        public TimeSpan temps { get; set; }
        public int point { get; set; }

        public double lkm { get; set; }
        public int nbCoureur { get; set; }
        public string rangEtape { get; set; }
        public DateTime dhDepart { get; set; }

        public string idCategory { get; set; }
        public string category { get; set; }
        public int totalPoint { get; set; }

        public string classement { get; set; }

        public TimeSpan chronoss { get; set; }
        public TimeSpan penalite { get; set; }
        public TimeSpan tempsFinal { get; set; }
        public string rang { get; set; }


        public Result() { }

        public Result (string coureur, string genre, TimeSpan chronoss, TimeSpan penalite, TimeSpan tempsFinal, string rang)
        {
            this.coureur = coureur;
            this.genre = genre;
            this.chronoss = chronoss;
            this.penalite = penalite;
            this.tempsFinal = tempsFinal;
            this.rang = rang;
        }

        public Result(string idEtape, string idCoureur, string coureur, TimeSpan temps, string classement, int point, string rang)
        {
            this.idEtape = idEtape;
            this.idCoureur = idCoureur;
            this.coureur = coureur;
            this.temps = temps;
            this.classement = classement;
            this.point = point;
            this.rang = rang;
        }

        public Result(string idCategory, string category, string rang, string equipe, int totalPoint)
        {
            this.idCategory = idCategory;
            this.category = category;
            this.rang = rang;
            this.equipe = equipe;
            this.totalPoint = totalPoint;
        }

        public Result(string idCoureur, string numDossard, string coureur, int point)
        {
            this.idCoureur = idCoureur;
            this.numDossard = numDossard;
            this.coureur = coureur;
            this.point = point;
        }

        // public Result(string idEtape, string etape, string coureur, int point)
        // {
        //     this.idEtape = idEtape;
        //     this.etape = etape;
        //     this.coureur = coureur;
        //     this.point = point;
        // }

        public Result(string rang, string equipe, int point)
        {
            this.rang = rang;
            this.equipe = equipe;
            this.point = point;
        }

        public Result(string idEtapeCoureur, string idEtape, string etape, double lkm, int nbCoureur, string rangEtape, DateTime dhDepart, string idUser, string equipe, string coureur, string numDossard, string genre, DateTime dtn, TimeSpan temps, int point, string rang)
        {
            this.idEtapeCoureur = idEtapeCoureur;
            this.idEtape = idEtape;
            this.etape = etape;
            this.lkm = lkm;
            this.nbCoureur = nbCoureur;
            this.rangEtape = rangEtape;
            this.dhDepart = dhDepart;
            this.idUser = idUser;
            this.equipe = equipe;
            this.coureur = coureur;
            this.numDossard = numDossard;
            this.genre = genre;
            this.dtn = dtn;
            this.temps = temps;
            this.point = point;
            this.rang = rang;
        }


        public static List<Result> findAll(Connexion connexion)
        {
            List<Result> results = new List<Result>();
            string query = "SELECT * FROM v_detail_result";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result result = new Result(
                            // reader["idECTemps"].ToString(),
                            // reader["idEtapeCoureur"].ToString(),
                            reader["idEtape"].ToString(),
                            // reader["idUser"].ToString(),
                            // reader["equipe"].ToString(),
                            reader["idCoureur"].ToString(),
                            reader["coureur"].ToString(),
                            reader.GetTimeSpan(reader.GetOrdinal("temps")),
                            reader["classement"].ToString(),
                            Convert.ToInt32(reader["point"]),
                            reader["rang"].ToString()
                        );
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        public static List<Result> CGCoureur(Connexion connexion)
        {
            List<Result> results = new List<Result>();
            // string query = "select v.equipe, c.idCoureur, c.numDossard, c.nom, sum(point) point from v_detail_result v join etape e on v.idEtape = e.idEtape join coureur c on c.idCoureur = v.idCoureur group by c.numDossard, c.nom, v.equipe, c.idCoureur order by point desc";
            string query = "select c.idCoureur, c.numDossard, c.nom, sum(point) point from v_detail_result v join etape e on v.idEtape = e.idEtape join coureur c on c.idCoureur = v.idCoureur group by c.numDossard, c.nom, c.idCoureur order by point desc";
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result result = new Result(
                            reader["idCoureur"].ToString(),
                            reader["numDossard"].ToString(),
                            reader["nom"].ToString(),
                            Convert.ToInt32(reader["point"])
                        );
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public static List<Result> CGPointEtape(Connexion connexion)
        {
            List<Result> results = new List<Result>();
            string query = "select * from v_CGPointEtape ORDER BY idEtape ASC, point DESC";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // string idCategory, string category, string rang, string equipe, int totalPoint
                        Result result = new Result(
                            reader["idEtape"].ToString(),
                            reader["name"].ToString(),
                            reader["idCoureur"].ToString(),
                            reader["nom"].ToString(),
                            Convert.ToInt32(reader["point"])
                        );
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public static List<Result> CG(Connexion connexion)
        {
            List<Result> results = new List<Result>();
            string query = "select * from v_CG order by point desc";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result result = new Result(
                            reader["rang"].ToString(),
                            reader["equipe"].ToString(),
                            Convert.ToInt32(reader["point"])
                        );
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public static List<Result> chronos(Connexion connexion, string idUser)
        {
            List<Result> results = new List<Result>();
            string query = "SELECT * FROM v_chronos where idUser ='"+idUser+"'";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // idEtapeCoureur, idEtape, etape, double lkm, int nbCoureur, rangEtape, DateTime dhDepart, idUser, equipe, coureur, numDossard, genre, DateTime dtn, TimeSpan temps, int point, rang
                        Result result = new Result(
                            reader["etapeID"].ToString(),
                            reader["idEtape"].ToString(),
                            reader["etape"].ToString(),
                            reader.GetDouble(reader.GetOrdinal("lkm")),
                            reader.GetInt32(reader.GetOrdinal("nbCoureur")),
                            reader["rangEtape"].ToString(),
                            Convert.ToDateTime(reader["dhDepart"]),
                            reader["idUser"].ToString(),
                            reader["equipe"].ToString(),
                            reader["coureur"].ToString(),
                            reader["numDossard"].ToString(),
                            reader["genre"].ToString(),
                            Convert.ToDateTime(reader["dtn"]),
                            reader.GetTimeSpan(reader.GetOrdinal("temps")),
                            reader.GetInt32(reader.GetOrdinal("point")),
                            reader["rang"].ToString()
                        );
                        results.Add(result);
                    }
                }
            }

            return results;
        }

        public List<Result> v_result_category(Connexion connexion, string idCategory)
        {
            List<Result> results = new List<Result>();
            string query = "select * from v_result_category where idCategory = '"+idCategory+"' ORDER BY category, pointtotal DESC";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // (string idCategory, string category, string rang, string equipe, int totalPoint)
                        Result result = new Result(
                            reader["idCategory"] != DBNull.Value ? reader["idCategory"].ToString() : "",
                            reader["category"] != DBNull.Value ? reader["category"].ToString() : "",
                            reader["classement"] != DBNull.Value ? reader["classement"].ToString() : "",
                            reader["equipe"] != DBNull.Value ? reader["equipe"].ToString() : "",
                            !reader.IsDBNull(reader.GetOrdinal("pointtotal")) ? reader.GetInt32(reader.GetOrdinal("pointtotal")) : 0
                        );
                        results.Add(result);
                    }
                }
            }

            return results;

        }

        public Result certificate(Connexion connexion, string rang)
        {
            Result result = new Result();
            try
            {
                string query = "select * from v_CG where rang like  '%"+rang+"%'";
                
                using (SqlCommand command = new SqlCommand(query, connexion.connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = new Result(
                                reader["rang"].ToString(),
                                reader["equipe"].ToString(),
                                Convert.ToInt32(reader["point"])
                            );
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Result> aleasJ4(Connexion connexion, string idEtape)
        {
            List<Result> results = new List<Result>();
            try
            {
                string query = "select * from v_aleasJ4 where idEtape = '"+idEtape+"' order by rang";
                Console.WriteLine(query);
                
                using (SqlCommand command = new SqlCommand(query, connexion.connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // string chronos = reader.IsDBNull(reader.GetOrdinal("chronos")) ? "00:00:00" : reader.GetValue(reader.GetOrdinal("chronos")).ToString();
                            // string penalite = reader.IsDBNull(reader.GetOrdinal("penalite")) ? "00:00:00" : reader.GetValue(reader.GetOrdinal("penalite")).ToString();
                            // string tempsFinal = reader.IsDBNull(reader.GetOrdinal("temps_final")) ? "00:00:00" : reader.GetValue(reader.GetOrdinal("temps_final")).ToString();
                            // Result result = new Result(
                            //     reader["coureur"].ToString(),
                            //     reader["genre"].ToString(),
                            //     TimeSpan.Parse(chronos),
                            //     TimeSpan.Parse(penalite),
                            //     TimeSpan.Parse(tempsFinal),
                            //     reader["rang"].ToString()
                            // );

                            Result result = new Result(
                                reader["coureur"].ToString(),
                                reader["genre"].ToString(),
                                reader.GetTimeSpan(reader.GetOrdinal("chronos")),
                                reader.GetTimeSpan(reader.GetOrdinal("penalite")),
                                reader.GetTimeSpan(reader.GetOrdinal("temps_final")),
                                reader["rang"].ToString()
                            );

                            results.Add(result);
                        }
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static List<Result> aleas5(Connexion connexion, string equipe)
        {
            List<Result> results = new List<Result>();
            string query = "SELECT v.idEtape, v.idCoureur, v.coureur, sum(v.point) as point FROM v_detail_result v join coureur c on c.idCoureur = v.idCoureur join uuser u on u.idUser = c.idUser where u.name = '"+equipe+"' group by v.idEtape, v.idCoureur, v.coureur";
            Console.WriteLine(query);
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // (string rang, string equipe, int point)
                        Result result = new Result(
                            reader["idEtape"].ToString(),
                            reader["coureur"].ToString(),
                            Convert.ToInt32(reader["point"])
                        );
                        results.Add(result);
                    }
                }
            }

            return results;
        }
    }
}
