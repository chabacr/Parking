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
    public class Zona
    {
        #region Propiedades
        public int IdZona { get; set; }

        public int IdParqueo { get; set; }

        public Parking Parqueo { get; set; }

        public string NZona { get; set; }

        public int Estado { get; set; }

        public string Condicion { get; set; }

        
        #endregion

        #region Servicios
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region Constructor
        public Zona()
        {
            
            dialogService = new DialogService();
            navigationService = new NavigationService();
        }
        #endregion

        #region Metodos
        public override int GetHashCode()
        {
            return IdZona;
        }
        #endregion

        #region Comandos
     
        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }

        async void Edit()
        {
            //MainViewModel.GetInstance().Parqueo.Descripcion = Parqueo.Descripcion;
            MainViewModel.GetInstance().ActionZona = new AccionZonaViewModel(this);

            await navigationService.NavigateOnMaster("AccionZonaView");
        }


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
                "¿Estás seguro de borrar el registro # de zona: " + this.NZona + "?");
            if (!response)
            {
                return;
            }

            await ZonaViewModel.GetInstance().Delete(this);
        }

        public ICommand SelectZonaReservaCommand
        {
            get
            {
                return new RelayCommand(SelectZonaReserva);
            }
        }

        async void SelectZonaReserva()
        {
            var mainViewModel = MainViewModel.GetInstance();

            mainViewModel.AccionReserva = new AccionReservaViewModel(mainViewModel.Vehiculos);
            mainViewModel.Zona = this;
            await navigationService.NavigateOnMaster("AccionReservaView");
        }

        #endregion
    }
}
