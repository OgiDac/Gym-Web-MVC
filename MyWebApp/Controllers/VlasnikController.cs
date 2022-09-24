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
    public class VlasnikController : Controller
    {
        private static string fileNameCentri = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/fitnesCentri.txt");
        private static string fileNameKorisnici = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/korisnici.txt");
        private static string fileNameTreninzi = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/treninzi.txt");
        private static string fileNameKomentariZaPotvrdu = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/komentariZaPotvrdu.txt");
        private static string fileNameKomentari = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/komentari.txt");

        public ActionResult VlasnikOpis(int Id)
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

        // GET: Vlasnik
        public ActionResult Index()
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
        public ActionResult PocetnaStrana()
        {
            List<FitnesCentar> lista = new List<FitnesCentar>();
            Korisnik k = (Korisnik)(Session["Korisnik"]);

            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar c = JsonSerializer.Deserialize<FitnesCentar>(s);
                    if (c.Vlasnik != null)
                        if (c.Vlasnik.KorisnickoIme == k.KorisnickoIme)
                            lista.Add(c);
                }
            }
            return View(lista);
        }

        public ActionResult Izmena(int IdCentra)
        {
            FitnesCentar centar = new FitnesCentar();
            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    if (temp.Id == IdCentra) centar = temp;
                }
            }
            return View(centar);
        }

        public ActionResult IzmeniCentar(FitnesCentar centar)
        {
            List<FitnesCentar> lista = new List<FitnesCentar>();
            FitnesCentar temp = new FitnesCentar();
            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    lista.Add(temp);
                }
            }

            foreach (FitnesCentar f in lista)
            {
                if (f.Id == centar.Id)
                {
                    f.Naziv = centar.Naziv;
                    f.Adresa = centar.Adresa;
                    f.GodinaOtvaranja = centar.GodinaOtvaranja;
                    f.MesecnaCena = centar.MesecnaCena;
                    f.GodisnjaCena = centar.GodisnjaCena;
                    f.CenaJednogTreninga = centar.CenaJednogTreninga;
                    f.CenaGrupnogTreninga = centar.CenaGrupnogTreninga;
                    f.CenaSaPersonalnim = centar.CenaSaPersonalnim;

                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameCentri))
            {
                foreach (FitnesCentar f in lista)
                {
                    stream.WriteLine(JsonSerializer.Serialize<FitnesCentar>(f));
                }
            }

            return RedirectToAction("Index");

        }

        public ActionResult Brisanje(int Id)
        {
            List<FitnesCentar> lista = new List<FitnesCentar>();
            FitnesCentar temp = new FitnesCentar();
            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    lista.Add(temp);
                }
            }

            foreach (FitnesCentar f in lista)
            {
                if (f.Id == Id)
                {
                    f.Obrisan = true;
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameCentri))
            {
                foreach (FitnesCentar f in lista)
                {
                    stream.WriteLine(JsonSerializer.Serialize<FitnesCentar>(f));
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Komentari(int IdCentra)
        {
            List<Komentar> lista = new List<Komentar>();
            using (StreamReader stream = new StreamReader(fileNameKomentariZaPotvrdu))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Komentar temp = JsonSerializer.Deserialize<Komentar>(s);
                    if (temp.Centar.Id == IdCentra) lista.Add(temp);
                }

            }

            return View("KomentariCentra", lista);
        }

        public ActionResult Odobri(int Id)
        {
            FitnesCentar centar = new FitnesCentar();


            List<Komentar> lista = new List<Komentar>();
            Komentar temp = new Komentar();
            using (StreamReader stream = new StreamReader(fileNameKomentariZaPotvrdu))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<Komentar>(s);
                    lista.Add(temp);
                }
            }

            foreach (Komentar k in lista)
            {
                if (k.Id == Id)
                {
                    lista.Remove(k);
                    k.Odobren = true;
                    using (StreamWriter sw = new StreamWriter(fileNameKomentari, true))
                    {
                        sw.WriteLine(JsonSerializer.Serialize<Komentar>(k));
                    }

                    List<FitnesCentar> centri = new List<FitnesCentar>();
                    using (StreamReader stream = new StreamReader(fileNameCentri))
                    {
                        string s;
                        while ((s = stream.ReadLine()) != null)
                        {
                            FitnesCentar tempC = JsonSerializer.Deserialize<FitnesCentar>(s);
                            centri.Add(tempC);
                            if (tempC.Id == k.Centar.Id)
                            {
                                tempC.Komentari.Add(k);
                            }
                        }
                    }
                    using (StreamWriter stream = new StreamWriter(fileNameCentri))
                    {
                        foreach (FitnesCentar f in centri) stream.WriteLine(JsonSerializer.Serialize<FitnesCentar>(f));
                    }
                    break;
                }
            }
            using (StreamWriter sw = new StreamWriter(fileNameKomentariZaPotvrdu))
            {
                foreach (Komentar k in lista)
                {

                    sw.WriteLine(JsonSerializer.Serialize<Komentar>(k));
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Odbij(int Id)
        {
            List<Komentar> lista = new List<Komentar>();
            Komentar temp = new Komentar();
            using (StreamReader stream = new StreamReader(fileNameKomentariZaPotvrdu))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<Komentar>(s);
                    if (temp.Id != Id)
                        lista.Add(temp);
                }
            }

            using (StreamWriter sw = new StreamWriter(fileNameKomentariZaPotvrdu))
            {
                foreach (Komentar k in lista)
                {

                    sw.WriteLine(JsonSerializer.Serialize<Komentar>(k));

                }
            }

            return RedirectToAction("Index");

        }


        public ActionResult Dodaj()
        {
            return View("NoviCentar");
        }

        public ActionResult DodajCentar(FitnesCentar centar)
        {
            centar.Vlasnik = (Korisnik)(Session["Korisnik"]);
            using (StreamWriter stream = new StreamWriter(fileNameCentri, true))
            {
                stream.WriteLine(JsonSerializer.Serialize<FitnesCentar>(centar));
            }
            List<Korisnik> lista = new List<Korisnik>();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    lista.Add(JsonSerializer.Deserialize<Korisnik>(s));
                }
            }
            foreach (Korisnik k in lista) if (k.KorisnickoIme == ((Korisnik)(Session["Korisnik"])).KorisnickoIme) k.CentarVlasnik.Add(centar);
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik k in lista)
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(k));
            }

            return RedirectToAction("Index");
        }

        public ActionResult DodajTrenera(int IdCentra)
        {
            //using (StreamReader stream = new StreamReader(fileNameKorisnici))
            //{
            //    string s;
            //    while ((s = stream.ReadLine()) != null)
            //    {
            //        Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
            //        if (temp.CentarAngazovani != null)
            //        {
            //            if (temp.CentarAngazovani.Id == IdCentra)
            //            {
            //                TempData["Greska"] = "Centar vec ima trenera";
            //                return View("Greska");
            //            }
            //        }
            //    }
            //}

            Session["IdCentra"] = IdCentra;

            List<Korisnik> lista = new List<Korisnik>();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.CentarAngazovani.Naziv == null && temp.Uloga == Uloga.Trener)
                    {
                        lista.Add(temp);
                    }
                }
            }

            return View("Treneri", lista);
        }
        public ActionResult Blok(int IdCentra)
        {


            Session["IdCentra"] = IdCentra;

            List<Korisnik> lista = new List<Korisnik>();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.CentarAngazovani.Id == IdCentra && temp.Uloga == Uloga.Trener)
                    {
                        lista.Add(temp);
                    }
                }
            }

            return View(lista);
        }

        public ActionResult AngazujTrenera(string Id)
        {

            List<FitnesCentar> lista = new List<FitnesCentar>();
            FitnesCentar temp = new FitnesCentar();
            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    lista.Add(temp);
                }
            }
            FitnesCentar centar = new FitnesCentar();
            foreach (FitnesCentar f in lista)
            {
                if (f.Id == (int)Session["IdCentra"])
                {
                    centar = f;
                }
            }

            List<Korisnik> listaKorisnika = new List<Korisnik>();
            Korisnik tempKorisnik = new Korisnik();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    tempKorisnik = JsonSerializer.Deserialize<Korisnik>(s);
                    listaKorisnika.Add(tempKorisnik);
                }
            }

            foreach (Korisnik k in listaKorisnika)
            {
                if (k.KorisnickoIme == Id)
                {
                    k.CentarAngazovani = centar;
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik k in listaKorisnika)
                {
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(k));
                }
            }

            return RedirectToAction("Index");
        }
        public ActionResult BlokirajTrenera(string Id)
        {


            List<Korisnik> listaKorisnika = new List<Korisnik>();
            Korisnik tempKorisnik = new Korisnik();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    tempKorisnik = JsonSerializer.Deserialize<Korisnik>(s);
                    listaKorisnika.Add(tempKorisnik);
                }
            }

            foreach (Korisnik k in listaKorisnika)
            {
                if (k.KorisnickoIme == Id)
                {
                    k.Banned = true;
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik k in listaKorisnika)
                {
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(k));
                }
            }

            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
        }
    }
}

