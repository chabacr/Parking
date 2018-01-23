namespace Api.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Permiso_TipoUsuario
    {
        [Key]
        public int IdPermisoTipoUsuario { get; set; }

        public int IdPermiso { get; set; }

        public int IdTipoUsuario { get; set; }
        [JsonIgnore]
        public virtual Permiso Permiso { get; set; }
        [JsonIgnore]
        public virtual TipoUsuario TipoUsuario { get; set; }
    }
}
