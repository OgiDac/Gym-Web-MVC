using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class FitnesCentar
    {
        private int id;
        private bool obrisan = false;

        private string naziv;
        private string adresa;
        private int godinaOtvaranja;
        private Korisnik vlasnik;
        private double mesecnaCena;
        private double godisnjaCena;
        private double cenaJednogTreninga;
        private double cenaGrupnogTreninga;
        private double cenaSaPersonalnim;

        private List<GrupniTrening> treninzi = new List<GrupniTrening>();
        private List<Komentar> komentari = new List<Komentar>();

        public FitnesCentar(string naziv, string adresa, int godinaOtvaranja)
        {
            this.naziv = naziv;
            this.adresa = adresa;
            this.godinaOtvaranja = godinaOtvaranja;
        }

        public FitnesCentar()
        {
            Id = GenerateId();
        }
        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }

        public string Naziv { get => naziv; set => naziv = value; }
        public string Adresa { get => adresa; set => adresa = value; }
        public int GodinaOtvaranja { get => godinaOtvaranja; set => godinaOtvaranja = value; }
        public Korisnik Vlasnik { get => vlasnik; set => vlasnik = value; }
        public double MesecnaCena { get => mesecnaCena; set => mesecnaCena = value; }
        public double GodisnjaCena { get => godisnjaCena; set => godisnjaCena = value; }
        public double CenaJednogTreninga { get => cenaJednogTreninga; set => cenaJednogTreninga = value; }
        public double CenaGrupnogTreninga { get => cenaGrupnogTreninga; set => cenaGrupnogTreninga = value; }
        public double CenaSaPersonalnim { get => cenaSaPersonalnim; set => cenaSaPersonalnim = value; }
        public int Id { get => id; set => id = value; }
        public List<GrupniTrening> Treninzi { get => treninzi; set => treninzi = value; }
        public List<Komentar> Komentari { get => komentari; set => komentari = value; }
        public bool Obrisan { get => obrisan; set => obrisan = value; }
    }
}