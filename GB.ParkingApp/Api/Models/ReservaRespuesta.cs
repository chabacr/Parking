using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ReservaRespuesta
    {
        public int IdReserva { get; set; }
        
        public DateTime FechaRegistro { get; set; }

        public DateTime FechaReserva { get; set; }

        public DateTime? FechaFinReserva { get; set; }
        
        
        public  Vehiculo Vehiculo { get; set; }
        
        public Zona Zona { get; set; }
        
    }
}