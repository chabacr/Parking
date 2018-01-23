namespace Api.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vehiculo")]
    public partial class Vehiculo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vehiculo()
        {
            Reservas = new HashSet<Reserva>();
        }

        [Key]
        public int IdVehiculo { get; set; }

        public int IdTipoVehiculo { get; set; }

        
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(10)]
        public string Placa { get; set; }

        [Required]
        [StringLength(10)]
        public string Modelo { get; set; }

        [Required]
        [StringLength(50)]
        public string Marca { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        public string Foto { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Reserva> Reservas { get; set; }
        
        public virtual TipoVehiculo TipoVehiculo { get; set; }

        [JsonIgnore]
        public virtual Usuario Usuario { get; set; }
    }
}
