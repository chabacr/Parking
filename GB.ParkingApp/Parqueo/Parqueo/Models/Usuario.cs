using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parqueo.Models
{
   public class Usuario
    {
        #region Propiedades

        public int IdUsuario { get; set; }

        public int IdTipoUsuario { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string Password { get; set; }

        public string Foto { get; set; }

        public byte[] ImageArray { get; set; }
        
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(Foto))
                {
                    return "noimage";
                }

                return string.Format(
                    "http://api-gb-parking.azurewebsites.net/{0}",
                    Foto.Substring(1));
            }
        }

        public string NombreCompleto
        {
            get
            {
                return Nombre + " " + Apellidos;
            }
        }
        

        public List<Vehiculo> Vehiculos { get; set; }

        public List<Permiso> Permisos { get; set; }

        public List<Reserva> Reservas { get; set; }

        #endregion

        #region Constructor
        public Usuario()
        {
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion

        #region Metodos
        public override int GetHashCode()
        {
            return IdUsuario;
        }

        #endregion

        #region Servicios
        DialogService dialogService;
        NavigationService navigationService;
        #endregion
    }
}
