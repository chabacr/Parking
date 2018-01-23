using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Api.Models
{
    [NotMapped]
    public class VehiculoSolicitud:Vehiculo
    {
        public byte[] ImageArray { get; set; }
    }
}