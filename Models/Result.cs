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
        public string rang { get; set; }

        public double lkm { get; set; }
        public int nbCoureur { get; set; }
        public string rangEtape { get; set; }
        public DateTime dhDepart { get; set; }

        public string idCategory { get; set; }
        public string category { get; set; }
        public int totalPoint { get; set; }


        public Result() { }

        public Result(string idECTemps, string idEtapeCoureur, string idEtape, string idUser, string equipe, string idCoureur, string coureur, string numDossard, string genre, DateTime dtn, TimeSpan hDepart, TimeSpan hArriver, TimeSpan temps, int point, string rang)
        {
            this.idECTemps = idECTemps;
            this.idEtapeCoureur = idEtapeCoureur;
            this.idEtape = idEtape;
            this.idUser = idUser;
            this.equipe = equipe;
            this.idCoureur = idCoureur;
            this.coureur = coureur;
            this.numDossard = numDossard;
            this.genre = genre;
            this.dtn = dtn;
            this.hDepart = hDepart;
            this.hArriver = hArriver;
            this.temps = temps;
            this.point = point;
            this.rang = rang;
        }

        // public Result(string idCategory, string category, string rang, string equipe, int totalPoint)
        // {
        //     this.idCategory = idCategory;
        //     this.category = category;
        //     this.rang = rang;
        //     this.equipe = equipe;
        //     this.totalPoint = totalPoint;
        // }

        public Result(string equipe, string idCoureur, string numDossard, string coureur, int point)
        {
            this.equipe = equipe;
            this.idCoureur = idCoureur;
            this.numDossard = numDossard;
            this.coureur = coureur;
            this.point = point;
        }

        public Result(string idEtape, string etape, string coureur, int point)
        {
            this.idEtape = idEtape;
            this.etape = etape;
            this.coureur = coureur;
            this.point = point;
        }

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
                            reader["idECTemps"].ToString(),
                            reader["idEtapeCoureur"].ToString(),
                            reader["idEtape"].ToString(),
                            reader["idUser"].ToString(),
                            reader["equipe"].ToString(),
                            reader["idCoureur"].ToString(),
                            reader["coureur"].ToString(),
                            reader["numDossard"].ToString(),
                            reader["genre"].ToString(),
                            Convert.ToDateTime(reader["dtn"]),
                            TimeSpan.Parse(reader["hDepart"].ToString()),
                            TimeSpan.Parse(reader["hArriver"].ToString()),
                            TimeSpan.Parse(reader["temps"].ToString()),
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
            string query = "select v.equipe, c.idCoureur, c.numDossard, c.nom, sum(point) point from v_detail_result v join etape e on v.idEtape = e.idEtape join coureur c on c.idCoureur = v.idCoureur group by c.numDossard, c.nom, v.equipe, c.idCoureur order by point desc";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result result = new Result(
                            reader["equipe"].ToString(),
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
            string query = "select v.idEtape, e.name, v.idUser, v.equipe, sum(point) point from v_detail_result v join etape e on v.idEtape = e.idEtape join coureur c on c.idCoureur = v.idCoureur group by v.idEtape, e.name, v.idUser, v.equipe order by idEtape desc, point desc";
            // string query = "select v.idEtape, e.name, c.idCoureur, c.nom, sum(point) point from v_detail_result v join etape e on v.idEtape = e.idEtape join coureur c on c.idCoureur = v.idCoureur group by v.idEtape, e.name, c.idCoureur, c.nom order by point desc";
            
            using (SqlCommand command = new SqlCommand(query, connexion.connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result result = new Result(
                            reader["idEtape"].ToString(),
                            reader["name"].ToString(),
                            reader["equipe"].ToString(),
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
                            reader["idEtapeCoureur"].ToString(),
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
                        // (string equipe, string idCoureur, string numDossard, string coureur, int point)
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
    }
}
