using System;
using System.Collections.Generic;

namespace Race.Models
{
    public class Data
    {
        public List<Etape> etapeList = new List<Etape>();
        public Etape etape = new Etape();
        public List<Coureur> coureurList = new List<Coureur>();
        public List<Result> resultList = new List<Result>();
        public List<Result> CGPointEtape = new List<Result>();
        public List<Result> CG = new List<Result>();
        public List<Uuser> userList = new List<Uuser>();
        public List<Uuser> equipeList = new List<Uuser>();

        public Data()
        {
            this.etapeList = new List<Etape>();
            this.etape = new Etape();
            this.coureurList = new List<Coureur>();
            this.resultList = new List<Result>();
            this.CGPointEtape = new List<Result>();
            this.CG = new List<Result>();
            this.userList = new List<Uuser>();
            this.equipeList = new List<Uuser>();
        }
    }
}