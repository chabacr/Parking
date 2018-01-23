using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parqueo.Models
{
    public class Reserva
    {
        #region Propiedades
        public int IdReserva { get; set; }

        public int IdUsuario { get; set; }

        public int IdZona { get; set; }

        public int IdVehiculo { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string FechaReservaToString
        {
            get
            {
                return FechaReserva.ToString("yyyy/MM/dd");
            }
        }

        public string FechaRegistroToString
        {
            get
            {
                return FechaRegistro.ToString("yyyy/MM/dd");
            }
        }

        public DateTime FechaReserva { get; set; }

        public DateTime FechaFinReserva { get; set; }

        public Zona Zona { get; set; }

        public Vehiculo Vehiculo { get; set; }

        #endregion

        #region Servicios
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Constructor
        public Reserva()
        {
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion
    }
}
