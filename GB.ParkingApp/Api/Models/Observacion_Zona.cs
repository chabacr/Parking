namespace Api.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Observacion_Zona
    {
        [Key]
        public int IdObservacion_Zona { get; set; }

        public int IdUsuario { get; set; }

        public int IdObservacion { get; set; }

        public int IdZona { get; set; }

        public DateTime Fecha { get; set; }

        public virtual Observacion Observacion { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual Zona Zona { get; set; }
    }
}
