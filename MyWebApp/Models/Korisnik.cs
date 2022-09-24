using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class Korisnik
    {
        private bool banned = false;

        private string korisnickoIme;
        private string lozinka;
        private string ime;
        private string prezime;
        private string pol;
        private string email;
        private string datumRodjenja;
        private Uloga uloga;
        private List<GrupniTrening> listaPrijavljenih = new List<GrupniTrening>();
        private List<GrupniTrening> listaAngazovanih = new List<GrupniTrening>();
        private FitnesCentar centarAngazovani = new FitnesCentar();
        private List<FitnesCentar> centarVlasnik = new List<FitnesCentar>();

        public string KorisnickoIme { get => korisnickoIme; set => korisnickoIme = value; }
        public string Lozinka { get => lozinka; set => lozinka = value; }
        public string Ime { get => ime; set => ime = value; }
        public string Prezime { get => prezime; set => prezime = value; }
        public string Pol { get => pol; set => pol = value; }
        public string Email { get => email; set => email = value; }
        public string DatumRodjenja { get => datumRodjenja; set => datumRodjenja = value; }
        public Uloga Uloga { get => uloga; set => uloga = value; }
        public List<GrupniTrening> ListaPrijavljenih { get => listaPrijavljenih; set => listaPrijavljenih = value; }
        public List<GrupniTrening> ListaAngazovanih { get => listaAngazovanih; set => listaAngazovanih = value; }
        public FitnesCentar CentarAngazovani { get => centarAngazovani; set => centarAngazovani = value; }

        public bool Banned { get => banned; set => banned = value; }
        public List<FitnesCentar> CentarVlasnik { get => centarVlasnik; set => centarVlasnik = value; }
    }
}