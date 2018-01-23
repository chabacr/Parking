using GalaSoft.MvvmLight.Command;
using Parqueo.Services;
using Parqueo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Parqueo.Models
{
   public class TipoVehiculo
    {

        #region Atributos

        #endregion

        #region Comandos
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }

        async void Delete()
        {
            var response = await dialogService.ShowConfirm(
                "Confirmar",
                "¿Estás seguro de borrar el registro " + this.Descripcion + "?");
            if (!response)
            {
                return;
            }

            await TipoVehiculoViewModel.GetInstance().Delete(this);
        }


        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }

        async void Edit()
        {
            MainViewModel.GetInstance().AccionTipoVehiculo = new AccionTipoVehiculoViewModel(this, "Actualizar");

            await navigationService.NavigateOnMaster("AccionTipoVehiculoView");
        }

        #endregion

        #region Constructor
        public TipoVehiculo()
        {
            dialogService = new DialogService();
            navigationService = new NavigationService();
        }
        #endregion

        #region Eventos

        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return IdTipoVehiculo;
        }
        #endregion

        #region Servicios
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region propiedades
        public int IdTipoVehiculo { get; set; }

        public string Descripcion { get; set; }
        #endregion
    }
}
