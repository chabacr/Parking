using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ParqueoRespuesta
    {
        public int IdParqueo { get; set; }

        public string Descripcion { get; set; }

        public String Direccion { get; set; }

        public int NumeroZonas { get; set; }

        public List<ZonaRespuesta> Zonas { get; set; }
    }
}