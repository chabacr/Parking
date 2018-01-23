using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parqueo.Models
{
    public class TipoUsuario
    {
        #region Propiedades
        public int IdTipoUsuario { get; set; }

        public string Descripcion { get; set; }

        #endregion

        #region Constructor
        public TipoUsuario()
        {
            dialogService = new DialogService();
            navigationService = new NavigationService();
        }
        #endregion

        #region Servicios
        DialogService dialogService;
        NavigationService navigationService;
        #endregion
    }
}
