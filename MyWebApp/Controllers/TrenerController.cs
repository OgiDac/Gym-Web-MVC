using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWebApp.Models;
using System.Text.Json;
using System.IO;

namespace MyWebApp.Controllers
{
    public class TrenerController : Controller
    {
        private static string fileNameCentri = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/fitnesCentri.txt");
        private static string fileNameKorisnici = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/korisnici.txt");
        private static string fileNameTreninzi = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/treninzi.txt");
        private static string fileNameKomentariZaPotvrdu = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/komentariZaPotvrdu.txt");
        private static string fileNameKomentari = System.Web.Hosting.HostingEnvironment.MapPath(@"~/baze/komentari.txt");

        // GET: Trener
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
            DateTime danas = DateTime.Now;
            List<GrupniTrening> lista = new List<GrupniTrening>();
            Korisnik k = (Korisnik)Session["Korisnik"];
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.KorisnickoIme == k.KorisnickoIme)
                    {
                        foreach (GrupniTrening trening in temp.ListaAngazovanih)
                        {

                            lista.Add(trening);

                        }
                    }
                }
            }
            return View(lista);
        }

        public ActionResult TrenerOpis(int Id)
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

        public ActionResult Izmena(int Id)
        {
            GrupniTrening g = new GrupniTrening();
            using (StreamReader stream = new StreamReader(fileNameTreninzi))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    GrupniTrening temp = JsonSerializer.Deserialize<GrupniTrening>(s);
                    if (temp.Id == Id) g = temp;
                }
            }
            if (DateTime.Compare(g.DatumIVreme, DateTime.Now) < 0)
            {
                TempData["Greska"] = "Trening se vec odrzao";
                return View("Greska");
            }

            return View(g);
        }

        public ActionResult IzmeniTrening(GrupniTrening trening)
        {

            List<GrupniTrening> lista = new List<GrupniTrening>();
            GrupniTrening temp = new GrupniTrening();
            List<Korisnik> listaK = new List<Korisnik>();
            Korisnik tempK = new Korisnik();
            Korisnik k = (Korisnik)Session["Korisnik"];

            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    tempK = JsonSerializer.Deserialize<Korisnik>(s);
                    listaK.Add(tempK);
                }
            }
            foreach (Korisnik kor in listaK)
            {
                if (kor.KorisnickoIme == k.KorisnickoIme)
                {

                    foreach (GrupniTrening t in kor.ListaAngazovanih)
                    {
                        if (t.Id == trening.Id)
                        {
                            t.Naziv = trening.Naziv;
                            t.TipTreninga = trening.TipTreninga;
                            t.Trajanje = trening.Trajanje;
                            t.DatumIVreme = trening.DatumIVreme;
                            t.MaksPosetilaca = trening.MaksPosetilaca;
                        }
                    }
                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik kor in listaK)
                {
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(kor));
                }
            }

            using (StreamReader stream = new StreamReader(fileNameTreninzi))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<GrupniTrening>(s);
                    lista.Add(temp);
                }
            }

            foreach (GrupniTrening f in lista)
            {
                if (f.Id == trening.Id)
                {
                    f.Naziv = trening.Naziv;
                    f.TipTreninga = trening.TipTreninga;
                    f.Trajanje = trening.Trajanje;
                    f.DatumIVreme = trening.DatumIVreme;
                    f.MaksPosetilaca = trening.MaksPosetilaca;

                }
            }
            using (StreamWriter stream = new StreamWriter(fileNameTreninzi))
            {
                foreach (GrupniTrening f in lista)
                {
                    stream.WriteLine(JsonSerializer.Serialize<GrupniTrening>(f));
                }
            }

            return RedirectToAction("Index");

        }

        public ActionResult Brisanje(int Id)
        {
            List<GrupniTrening> lista = new List<GrupniTrening>();
            GrupniTrening temp = new GrupniTrening();
            using (StreamReader stream = new StreamReader(fileNameTreninzi))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    temp = JsonSerializer.Deserialize<GrupniTrening>(s);
                    lista.Add(temp);
                }
            }

            foreach (GrupniTrening f in lista)
            {
                if (f.Id == Id)
                {
                    if (DateTime.Compare(f.DatumIVreme, DateTime.Now) < 0)
                    {
                        TempData["Greska"] = "Trening se vec odrzao";
                        return View("Greska");
                    }

                    if (f.ListaPosetilaca.Count == 0)
                        f.Obrisan = true;
                    else
                    {
                        TempData["Greska"] = "Postoje prijavljeni korisnici za taj trening";
                        return View("Greska");
                    }
                }

            }
            using (StreamWriter stream = new StreamWriter(fileNameTreninzi))
            {
                foreach (GrupniTrening f in lista)
                {
                    stream.WriteLine(JsonSerializer.Serialize<GrupniTrening>(f));
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Dodaj()
        {
            Korisnik k = (Korisnik)(Session["Korisnik"]);
            if (k.CentarAngazovani.Naziv == null)
            {
                TempData["Greska"] = "Niste zaposleni nigde!!";
                return View("Greska");
            }
            return View("NoviTrening");
        }

        public ActionResult DodajTrening(GrupniTrening trening)
        {
            var za3dana = DateTime.Now;
            za3dana = za3dana.AddDays(3);
            if (DateTime.Compare(trening.DatumIVreme, za3dana) < 0)
            {
                TempData["Greska"] = "Mora bar 3 dana unaprijed!!";
                return View("Greska");
            }

            Korisnik k = (Korisnik)(Session["Korisnik"]);


            using (StreamReader stream = new StreamReader(fileNameCentri))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    FitnesCentar temp = JsonSerializer.Deserialize<FitnesCentar>(s);
                    if (temp.Id == k.CentarAngazovani.Id)
                    {
                        trening.CentarZaOdrzavanje = temp;
                        break;
                    }
                }
            }
            k.ListaAngazovanih.Add(trening);
            using (StreamWriter stream = new StreamWriter(fileNameTreninzi, true))
            {
                stream.WriteLine(JsonSerializer.Serialize<GrupniTrening>(trening));
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
            foreach (Korisnik ko in lista) if (ko.KorisnickoIme == ((Korisnik)(Session["Korisnik"])).KorisnickoIme) ko.ListaAngazovanih.Add(trening);
            using (StreamWriter stream = new StreamWriter(fileNameKorisnici))
            {
                foreach (Korisnik korisnik in lista)
                    stream.WriteLine(JsonSerializer.Serialize<Korisnik>(korisnik));
            }

            return RedirectToAction("Index");
        }

        public ActionResult Spisak(int Id)
        {
            /*List<Korisnik> listaKorisnika = new List<Korisnik>();
            Korisnik k = (Korisnik)Session["Korisnik"];
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.KorisnickoIme == k.KorisnickoIme)
                    {
                        foreach (GrupniTrening trening in temp.ListaAngazovanih)
                        {
                            if (trening.Id == Id)
                            {
                                foreach (string ko in trening.ListaPosetilaca) listaKorisnika.Add(NadjiKorisnika(ko));
                            }
                            return View(listaKorisnika);
                        }
                    }
                }
            }*/
            List<Korisnik> listaKorisnika = new List<Korisnik>();
            Korisnik k = (Korisnik)Session["Korisnik"];
            using (StreamReader stream = new StreamReader(fileNameTreninzi))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    GrupniTrening temp = JsonSerializer.Deserialize<GrupniTrening>(s);
                    if (temp.Id == Id)
                    {
                        foreach (string ko in temp.ListaPosetilaca)
                        {
                            listaKorisnika.Add(NadjiKorisnika(ko));

                        }
                        return View(listaKorisnika);
                    }
                }
            }


            return View("Greska");
        }

        public Korisnik NadjiKorisnika(string kIme)
        {
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.KorisnickoIme == kIme)
                    {
                        return temp;
                    }
                }
            }
            return null;
        }

        public ActionResult IzProslosti()
        {
            DateTime danas = DateTime.Now;
            List<GrupniTrening> lista = new List<GrupniTrening>();
            Korisnik k = (Korisnik)Session["Korisnik"];
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.KorisnickoIme == k.KorisnickoIme)
                    {
                        foreach (GrupniTrening trening in temp.ListaAngazovanih)
                        {
                            if (DateTime.Compare(trening.DatumIVreme, danas) < 0)
                            {
                                lista.Add(trening);
                            }
                        }
                    }
                }
            }
            return View(lista);
        }

        public ActionResult Pretraga(string pretragaNaziv, string pretragaTip, DateTime? minGodina, DateTime? maksGodina)
        {

            DateTime danas = DateTime.Now;
            List<GrupniTrening> lista = new List<GrupniTrening>();
            Korisnik k = (Korisnik)Session["Korisnik"];
            using (StreamReader stream = new StreamReader(fileNameKorisnici))
            {
                string s;
                while ((s = stream.ReadLine()) != null)
                {
                    Korisnik temp = JsonSerializer.Deserialize<Korisnik>(s);
                    if (temp.KorisnickoIme == k.KorisnickoIme)
                    {
                        foreach (GrupniTrening trening in temp.ListaAngazovanih)
                        {

                            lista.Add(trening);

                        }
                    }
                }
            }
            List<GrupniTrening> retValue = new List<GrupniTrening>();

            foreach (GrupniTrening element in lista)
            {
                if (element.Naziv == pretragaNaziv || pretragaNaziv.Trim() == "")
                    if (element.TipTreninga == pretragaTip || pretragaTip.Trim() == "")
                    {
                        if (minGodina != null)
                        {
                            if (maksGodina != null)
                            {
                                if (DateTime.Compare(element.DatumIVreme, (DateTime)(minGodina)) > 0 && DateTime.Compare(element.DatumIVreme, (DateTime)(maksGodina)) < 0)
                                    retValue.Add(element);
                            }
                            else if (DateTime.Compare(element.DatumIVreme, (DateTime)(minGodina)) > 0)
                            {
                                retValue.Add(element);
                            }

                        }
                        else if (maksGodina != null)
                        {
                            if (DateTime.Compare(element.DatumIVreme, (DateTime)(maksGodina)) < 0)
                                retValue.Add(element);
                        }
                        else
                            retValue.Add(element);
                    }
            }
            return View("PocetnaStrana", retValue);

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
