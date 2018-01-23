namespace Api.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reserva")]
    public partial class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        public int IdZona { get; set; }

        public int IdUsuario { get; set; }

        public int IdVehiculo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public DateTime FechaReserva { get; set; }

        public DateTime? FechaFinReserva { get; set; }

        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }
        [JsonIgnore]
        public virtual Vehiculo Vehiculo { get; set; }
        [JsonIgnore]
        public virtual Zona Zona { get; set; }
    }
}
