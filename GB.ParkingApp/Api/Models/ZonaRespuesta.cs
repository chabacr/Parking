using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ZonaRespuesta
    {

        public int IdZona { get; set; }

        public int IdParking { get; set; }
        
        public string NZona { get; set; }

        public int Estado { get; set; }
    }
}