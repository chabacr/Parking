using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class UsuarioRespuestas
    {
        public int IdUsuario { get; set; }

        public int IdTipoUsuario { get; set; }
        
        public string Nombre { get; set; }
        
        public string Apellidos { get; set; }
        
        public string Correo { get; set; }
        
        public string Telefono { get; set; }
        
        public string Password { get; set; }

        public string Foto { get; set; }

       
        public  List<Observacion_Zona> Observacion_Zona { get; set; }
     
        public List<ReservaRespuesta> Reservas { get; set; }
        
        public  TipoUsuario TipoUsuario { get; set; }

        public List<Vehiculo> Vehiculos { get; set; }

        public List<Permiso> Permisos { get; set; }
    }
}