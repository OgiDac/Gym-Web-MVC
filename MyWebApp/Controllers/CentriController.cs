using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.Json;
using System.IO;
using MyWebApp.Models;

namespace MyWebApp.Controllers
{
    public class CentriController : Controller
    {
        private static string fileNameCentri = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/fitnesCentri.txt");
        private static string fileNameKorisnici = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/korisnici.txt");
        private static string fileNameTreninzi = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/treninzi.txt");


        // GET: Centri
        public ActionResult Index()
        {
            //Korisnik vlasnik = new Korisnik() { KorisnickoIme = "Ognjen", Lozinka = "Ogi", Uloga = Uloga.Vlasnik, };
            //Korisnik trener = new Korisnik() { KorisnickoIme = "Trener", Lozinka = "trener", Uloga = Uloga.Trener };
            //Korisnik trener2 = new Korisnik() { KorisnickoIme = "Trener2", Lozinka = "trener", Uloga = Uloga.Trener };
            //Korisnik trener3 = new Korisnik() { KorisnickoIme = "Trener3", Lozinka = "trener", Uloga = Uloga.Trener };
            //Korisnik posetilac = new Korisnik() { KorisnickoIme = "Posetilac1", Lozinka = "posetilac", Uloga = Uloga.Posetilac };
            //Korisnik posetilac2 = new Korisnik() { KorisnickoIme = "Posetilac2", Lozinka = "posetilac", Uloga = Uloga.Posetilac };
            //using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            //{
            //    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(vlasnik));
            //    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(trener));
            //    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(trener2));
            //    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(trener3));
            //    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(posetilac));
            //    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(posetilac2));
            //}

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

        public ActionResult Opis(int Id)
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
            return View("Index", retValue);

        }

        public ActionResult Prijava()
        {

            return View();
        }
        public ActionResult Registracija()
        {
            return View();
        }

        public ActionResult RegistrujSe(Korisnik k)
        {
            bool flag = false;

            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik c = JsonSerializer.Deserialize<Korisnik>(s);
                    if (c.KorisnickoIme == k.KorisnickoIme) flag = true;
                }
            }

            if (flag)
            {
                TempData["Greska"] = "Vec postoji korisnik sa unetim korisnickim imenom";
                TempData["GreskaNazad"] = "http://localhost:61930/Centri/Registracija";
                return View("Greska");
            }
            else
            {
                k.Uloga = Uloga.Posetilac;
                Session["Korisnik"] = k;
                Session["TreninziPrijavljeni"] = k.ListaPrijavljenih;
                Session["TreninziAngazovani"] = k.ListaPrijavljenih;
                using (StreamWriter stream = new StreamWriter(fileNameKorisnici, true))
                {
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(k));
                }

                return RedirectToAction("Posetilac", "Prijavljeni");
            }
        }

        public ActionResult PrijaviSe(Korisnik k)
        {



            bool flag = false;
            string lozinka = "";
            Korisnik korisnik = new Korisnik();
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik c = JsonSerializer.Deserialize<Korisnik>(s);
                    if (c.KorisnickoIme == k.KorisnickoIme)
                    {
                        lozinka = k.Lozinka;
                        flag = true;
                        korisnik = c;
                        //break;
                    }
                }
            }

            if (!flag)
            {
                TempData["Greska"] = "Ne postoji korisnik sa unetim korisnickim imenom";
                return View("Greska");
            }
            else
            {
                if (korisnik.Lozinka != lozinka)
                {
                    TempData["Greska"] = "Netacna lozinka, pokusajte ponovo!!";
                    return View("Greska");
                }
                Session["Korisnik"] = korisnik;
                Session["TreninziPrijavljeni"] = korisnik.ListaPrijavljenih;
                Session["TreninziAngazovani"] = korisnik.ListaPrijavljenih;

                if (korisnik.Banned == true)
                {
                    TempData["Greska"] = "Vlasnik vas je blokirao";
                    return View("Greska");
                }

                if (korisnik.Uloga == Uloga.Posetilac)
                    return RedirectToAction("Posetilac", "Prijavljeni");
                if (korisnik.Uloga == Uloga.Vlasnik)
                    return RedirectToAction("Index", "Vlasnik");
                if (korisnik.Uloga == Uloga.Trener)
                {
                    using (StreamReader stream = new StreamReader(fileNameCentri))
                    {
                        string s;
                        while ((s = stream.ReadLine()) != null)
                        {
                            FitnesCentar temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                            if (temp.Id == korisnik.CentarAngazovani.Id && temp.Obrisan == true)
                            {
                                TempData["Greska"] = "Centar u kojem radite je obrisan";
                                return View("Greska");
                            }
                        }
                    }

                    if (korisnik.Banned == true)
                    {
                        TempData["Greska"] = "Vlasnik vas je blokirao";
                        return View("Greska");
                    }
                    return RedirectToAction("Index", "Trener");
                }
                TempData["Greska"] = korisnik.Uloga.ToString();
                return View("Greska");
            }
        }

        public ActionResult Sortiranje(string dugme, string vrsta)
        {
            if (dugme == "PO NAZIVU")
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
                if (vrsta == "RASTUCE")
                    lista.Sort((x, y) => (x.Naziv.CompareTo(y.Naziv)));
                else
                    lista.Sort((x, y) => (y.Naziv.CompareTo(x.Naziv)));
                return View("Index", lista);
            }
            else if (dugme == "PO ADRESI")
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

                if (vrsta == "RASTUCE")
                    lista.Sort((x, y) => (x.Adresa.CompareTo(y.Adresa)));
                else
                    lista.Sort((x, y) => (y.Adresa.CompareTo(x.Adresa)));
                return View("Index", lista);
            }
            else if (dugme == "PO GODINI OTVARANJA")
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

                if (vrsta == "RASTUCE")
                    lista.Sort((x, y) => (x.GodinaOtvaranja.CompareTo(y.GodinaOtvaranja)));
                else
                    lista.Sort((x, y) => (y.GodinaOtvaranja.CompareTo(x.GodinaOtvaranja)));
                return View("Index", lista);

            }
            return View("Index");
        }

    }
}