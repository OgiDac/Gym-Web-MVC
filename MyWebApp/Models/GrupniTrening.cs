using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyWebApp.Models
{
    public class GrupniTrening
    {
        private bool obrisan = false;

        private int id;
        private string naziv;
        private string tipTreninga;
        private FitnesCentar centarZaOdrzavanje;
        private double trajanje;
        private DateTime datumIVreme;
        private int maksPosetilaca;
        private List<string> listaPosetilaca = new List<string>();

        public string Naziv { get => naziv; set => naziv = value; }
        public string TipTreninga { get => tipTreninga; set => tipTreninga = value; }
        public FitnesCentar CentarZaOdrzavanje { get => centarZaOdrzavanje; set => centarZaOdrzavanje = value; }
        public double Trajanje { get => trajanje; set => trajanje = value; }
        public DateTime DatumIVreme { get => datumIVreme; set => datumIVreme = value; }
        public int MaksPosetilaca { get => maksPosetilaca; set => maksPosetilaca = value; }
        public List<string> ListaPosetilaca { get => listaPosetilaca; set => listaPosetilaca = value; }
        public int Id { get => id; set => id = value; }
        public bool Obrisan { get => obrisan; set => obrisan = value; }

        public GrupniTrening()
        {
            Id = GenerateId();
        }

        private static int GenerateId()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode());
        }
    }
}