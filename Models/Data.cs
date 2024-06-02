using System;
using System.Collections.Generic;

namespace Race.Models
{
    public class Data
    {
        public List<Etape> etapeList = new List<Etape>();
        public Etape etape = new Etape();
        public List<Coureur> coureurList = new List<Coureur>();

        public Data()
        {
            this.etapeList = new List<Etape>();
            this.etape = new Etape();
            this.coureurList = new List<Coureur>();
        }
    }
}