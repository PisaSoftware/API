﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HlidacStatu.Api.V2.Dataset.Typed;

namespace Playground
{
    class Program
    {


        public class kvalifikovany_dodavatel
        {
            public string ico { get; set; }
            public string obchodni_firma_nazev_dodavatele { get; set; }
            public string mesto { get; set; }
            public string psc { get; set; }
            public string okres { get; set; }
            public string nuts { get; set; }
            public string zeme { get; set; }
            public string evidencni_cislo { get; set; }
            public string dt_nabyti_pm { get; set; }
            public string potvrzeny_clen_skd { get; set; }
            public string pravni_skutecnost { get; set; }
            public string legalid { get; set; }
            public string konvertovan { get; set; }
            public string vypis { get; set; }
            public string id { get; set; }
        }

        static void Main(string[] args)
        {
            string apikey = System.Configuration.ConfigurationManager.AppSettings["apikey"];

            var ds = Dataset<kvalifikovany_dodavatel>.OpenDataset(apikey,"kvalifikovanidodavatele");
            var s = ds.Search("*", 1);
        }
    }
}
