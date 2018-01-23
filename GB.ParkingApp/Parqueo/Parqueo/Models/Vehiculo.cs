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
    public class Vehiculo
    {
        #region Services
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades

        public int IdVehiculo { get; set; }

        public int IdTipoVehiculo { get; set; }

        public int IdUsuario { get; set; }

        public string Placa { get; set; }

        public string Modelo { get; set; }

        public string Marca { get; set; }

        public string Color { get; set; }

        public string Foto { get; set; }

        public TipoVehiculo TipoVehiculo { get; set; }

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

        public string MarcaPlaca
        {
            get
            { 
                return Marca + " " + Placa;
            }
        }
        #endregion

        #region Constructor
        public Vehiculo()
        {
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
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
                string.Format("¿Estás seguro de borrar el registro {0} {1}?",this.Marca,this.Modelo));
            
            if (!response)
            {
                return;
            }

            await VehiculoViewModel.GetInstance().Delete(this);
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
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.AccionCarros = new  AccionVehiculoViewModel(this, "Actualizar");
            
            await navigationService.NavigateOnMaster("AccionVehiculoView");
        }
        #endregion

        #region Metodos
        public override int GetHashCode()
        {
            return IdVehiculo;
        }

        #endregion
    }
}
