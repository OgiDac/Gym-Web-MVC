using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class PrijavljeniController : Controller
    {
        private static string fileNameCentri = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/fitnesCentri.txt");
        private static string fileNameKorisnici = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/korisnici.txt");
        private static string fileNameTreninzi = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/treninzi.txt");
        private static string fileNameKomentariZaPotvrdu = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/komentariZaPotvrdu.txt");

        // GET: Prijavljeni
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Pretraga(string pretragaNaziv, string pretragaAdresa, int minGodina = -1, int maxGodina = -1)
        {

            List<FitnesCentar> lista = new List<FitnesCentar>();


            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar c = JsonSerializer.Deserialize<FitnesCentar>(s);
                    lista.Add(c);
                }
            }
            List<FitnesCentar> retValue = new List<FitnesCentar>();

            foreach (FitnesCentar element in lista)
            {
                if (element.Naziv == pretragaNaziv || pretragaNaziv.Trim() == "")
                    if (element.Adresa == pretragaAdresa || pretragaAdresa.Trim() == "")
                        if (element.GodinaOtvaranja >= minGodina || minGodina == -1)
                            if (element.GodinaOtvaranja <= maxGodina || maxGodina == -1)
                                retValue.Add(element);
            }
            return View("Posetilac", retValue);

        }

        public ActionResult PretragaTrening(string pretragaNaziv, string pretragaTip, string fitnesCentar)
        {

            List<GrupniTrening> lista = new List<GrupniTrening>();


            using (StreamReader stream = new StreamReader(fileNameTreninzi))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    GrupniTrening c = JsonSerializer.Deserialize<GrupniTrening>(s);
                    lista.Add(c);
                }
            }
            Korisnik k = (Korisnik)(Session["Korisnik"]);
            List<GrupniTrening> retValue = new List<GrupniTrening>();

            foreach (GrupniTrening element in k.ListaPrijavljenih)
            {
                if (element.Naziv == pretragaNaziv || pretragaNaziv.Trim() == "")
                    if (element.TipTreninga == pretragaTip || pretragaTip.Trim() == "")
                        if (element.CentarZaOdrzavanje.Naziv == fitnesCentar || fitnesCentar.Trim() == "")
                            retValue.Add(element);
            }

            Session["TreninziPrijavljeni"] = retValue;
            return RedirectToAction("Posetilac");

        }

        public ActionResult Posetilac()
        {
            List<FitnesCentar> lista = new List<FitnesCentar>();

            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar c = JsonSerializer.Deserialize<FitnesCentar>(s);
                    lista.Add(c);
                }
            }
            return View(lista);
        }

        public ActionResult PosetilacOpis(int Id)
        {
            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar c = JsonSerializer.Deserialize<FitnesCentar>(s);
                    if (c.Id == Id)
                    {
                        using (StreamReader streamt = new StreamReader(fileNameTreninzi))
                        {
                            string st;
                            while ((st = streamt.ReadLine()) != null)
                            {
                                GrupniTrening temp = JsonSerializer.Deserialize<GrupniTrening>(st);
                                if (temp.CentarZaOdrzavanje.Id == Id && DateTime.Compare(temp.DatumIVreme, DateTime.Now) > 0) c.Treninzi.Add(temp);

                            }
                        }

                        return View(c);
                    }
                }
            }
            return View();

        }

        public ActionResult PosetilacKomentar(int IdCentra)
        {
            /*
            List<GrupniTrening> listaTreninga = (List<GrupniTrening>)(Session["TreninziPrijavljeni"]);
            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while((s = stream.ReadLine())!= null)
                {
                    FitnesCentar temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    if(temp.Id == IdCentra)
                    {
                        foreach(GrupniTrening trening in listaTreninga)
                        {
                            if(trening.CentarZaOdrzavanje.Id == IdCentra)
                            {
                                return View("PosetilacKomentar", IdCentra);
                            }
                        }
                    }
                }
            }*/
            Korisnik k = (Korisnik)Session["Korisnik"];
            using (StreamReader stream = new StreamReader(fileNameTreninzi))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    GrupniTrening temp = JsonSerializer.Deserialize<GrupniTrening>(s);
                    if (temp.CentarZaOdrzavanje.Id == IdCentra && temp.ListaPosetilaca.Contains(k.KorisnickoIme))
                    {
                        return View("PosetilacKomentar", IdCentra);

                    }
                }
            }

            TempData["Greska"] = "Nikada niste bili u tom fitnes centru!!";
            return View("Greska");
        }

        public ActionResult OstaviKomentar(string Tekst, int Ocena, int IdCentra)
        {

            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    if (temp.Id == IdCentra)
                    {
                        Komentar komentar = new Komentar();
                        komentar.Tekst = Tekst;
                        komentar.Ocena = Ocena;
                        komentar.Osoba = (Korisnik)(Session["Korisnik"]);
                        komentar.Centar = temp;
                        using (StreamWriter streamw = new StreamWriter(fileNameKomentariZaPotvrdu, true))
                        {
                            streamw.WriteLine(JsonSerializer.Serialize<Komentar>(komentar));
                        }
                    }
                }
            }


            return RedirectToAction("Posetilac");
        }

        public ActionResult PrijavaPosetilac(int Id)
        {
            List<Korisnik> listaKorisnika = new List<Korisnik>();
            using (StreamReader streamt = new StreamReader(fileNameKorisnici))
            {
                string st;
                while ((st = streamt.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(st);
                    listaKorisnika.Add(temp);
                }
            }

            Korisnik k = (Korisnik)(Session["Korisnik"]);
            foreach (GrupniTrening gtrening in k.ListaPrijavljenih)
            {
                if (gtrening.Id == Id)
                {
                    TempData["Greska"] = "Vec si prijavljen za taj trening";
                    return View("Greska");
                }
            }

            List<GrupniTrening> lista = new List<GrupniTrening>();
            GrupniTrening trening = new GrupniTrening();
            using (StreamReader streamt = new StreamReader(fileNameTreninzi))
            {
                string st;
                while ((st = streamt.ReadLine()) != null)
                {
                    GrupniTrening temp = JsonSerializer.Deserialize<GrupniTrening>(st);
                    lista.Add(temp);
                }
            }
            foreach (GrupniTrening g in lista)
            {
                if (g.Id == Id)
                {
                    if (g.ListaPosetilaca.Count >= g.MaksPosetilaca)
                    {
                        TempData["Greska"] = "Maksimalan broj ljudi je prijavljen na taj trening";
                        return View("Greska");
                    }
                    g.ListaPosetilaca.Add(k.KorisnickoIme);
                    k.ListaPrijavljenih.Add(g);
                    break;
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik kor in listaKorisnika)
                {
                    if (kor.KorisnickoIme == k.KorisnickoIme)
                        stream.WriteLine(JsonSerializer.Serialize<Korisnik>(k));
                    else
                        stream.WriteLine(JsonSerializer.Serialize<Korisnik>(kor));
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameTreninzi))
            {
                foreach (GrupniTrening gr in lista)
                {
                    stream.WriteLine(JsonSerializer.Serialize<GrupniTrening>(gr));
                }
            }
            return RedirectToAction("Posetilac");
        }

        public ActionResult Sortiranje(string dugme, string vrsta)
        {
            if (dugme == "PO NAZIVU")
            {

                if (vrsta == "RASTUCE")
                    ((List<GrupniTrening>)(Session["TreninziPrijavljeni"])).Sort((x, y) => (x.Naziv.CompareTo(y.Naziv)));
                else
                    ((List<GrupniTrening>)(Session["TreninziPrijavljeni"])).Sort((x, y) => (y.Naziv.CompareTo(x.Naziv)));
                return RedirectToAction("Posetilac");
            }
            else if (dugme == "PO TIPU TRENINGA")
            {

                if (vrsta == "RASTUCE")
                    ((List<GrupniTrening>)(Session["TreninziPrijavljeni"])).Sort((x, y) => (x.TipTreninga.CompareTo(y.TipTreninga)));
                else
                    ((List<GrupniTrening>)(Session["TreninziPrijavljeni"])).Sort((x, y) => (y.TipTreninga.CompareTo(x.TipTreninga)));
                return RedirectToAction("Posetilac");
            }
            else if (dugme == "PO DATUMU ODRZAVANJA")
            {

                if (vrsta == "RASTUCE")
                    ((List<GrupniTrening>)(Session["TreninziPrijavljeni"])).Sort((x, y) => (x.DatumIVreme.ToString().CompareTo(y.DatumIVreme.ToString())));
                else
                    ((List<GrupniTrening>)(Session["TreninziPrijavljeni"])).Sort((x, y) => (y.DatumIVreme.ToString().CompareTo(x.DatumIVreme.ToString())));
                return RedirectToAction("Posetilac");

            }
            return RedirectToAction("Posetilac");
        }

        public ActionResult OdjaviSe()
        {
            Session["Korisnik"] = null;

            return RedirectToAction("Index", "Centri");
        }

        public ActionResult IzmenaProfila()
        {
            return View((Korisnik)(Session["Korisnik"]));
        }

        public ActionResult IzmeniProfil(Korisnik korisnik)
        {
            List<Korisnik> lista = new List<Korisnik>();
            Korisnik temp = new Korisnik();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<Korisnik>(s);
                    lista.Add(temp);
                }
            }

            foreach (Korisnik f in lista)
            {
                if (f.KorisnickoIme == korisnik.KorisnickoIme)
                {
                    f.Ime = korisnik.Ime;
                    f.Prezime = korisnik.Prezime;
                    f.Lozinka = korisnik.Lozinka;
                    f.Pol = korisnik.Pol;
                    f.Email = korisnik.Email;
                    f.DatumRodjenja = korisnik.DatumRodjenja;
                    Session["Korisnik"] = f;
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik f in lista)
                {
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(f));
                }
            }

            return RedirectToAction("Posetilac");
        }
    }
}