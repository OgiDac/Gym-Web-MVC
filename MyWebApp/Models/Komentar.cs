using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class Komentar
    {
        private int id;
        private bool odobren = false;

        private Korisnik osoba;
        private FitnesCentar centar;
        private string tekst;
        private int ocena;

        public int Id { get => id; set => id = value; }
        public Korisnik Osoba { get => osoba; set => osoba = value; }
        public FitnesCentar Centar { get => centar; set => centar = value; }
        public string Tekst { get => tekst; set => tekst = value; }
        public int Ocena { get => ocena; set => ocena = value; }
        public bool Odobren { get => odobren; set => odobren = value; }

        public Komentar()
        {
            Id = GenerateId();
        }

        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }
    }
}