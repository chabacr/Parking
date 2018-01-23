namespace Api.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Observacion")]
    public partial class Observacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Observacion()
        {
            Observacion_Zona = new HashSet<Observacion_Zona>();
        }

        [Key]
        public int IdObservacion { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Observacion_Zona> Observacion_Zona { get; set; }
    }
}
