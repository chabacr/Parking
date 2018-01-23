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
   public class Parking
    {
        #region Servicios
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades
        public int IdParqueo { get; set; }

        public string Descripcion { get; set; }

        public string Direccion { get; set; }

        public int NumeroZonas { get; set; }

        public List<Zona> Zonas { get; set; }
        #endregion

        #region Constructor
        public Parking()
        {
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion

        #region Metodos
        public override int GetHashCode()
        {
            return IdParqueo;
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
                "¿Estás seguro de borrar el registro "+this.Descripcion+"?");
            if (!response)
            {
                return;
            }

            await ParqueoViewModel.GetInstance().Delete(this);
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
            MainViewModel.GetInstance().ActionParking = new AccionParqueoViewModel(this, "Actualizar");

            await navigationService.NavigateOnMaster("AccionParqueoView");
        }

        public ICommand DetailCommand
        {
            get
            {
                return new RelayCommand(Detail);
            }
        }

        async void Detail()
        {
            await dialogService.ShowMessage(
                "Detalles",
                "Descripción: "+this.Descripcion +
                " Dirección: "+this.Direccion+ 
                " Número de zonas: "+this.NumeroZonas);
        }

        public ICommand SelectParkingCommand
        {
            get
            {
                return new RelayCommand(SelectParking);
            }
        }

        async void SelectParking()
        {
            var mainViewModel = MainViewModel.GetInstance();
            
            mainViewModel.Zonas = new ZonaViewModel(Zonas);
            mainViewModel.Parking = this;
            await navigationService.NavigateOnMaster("ZonaView");
        }

        public ICommand SelectParkingReservaCommand
        {
            get
            {
                return new RelayCommand(SelectParkingReserva);
            }
        }

        async void SelectParkingReserva()
        {
            var mainViewModel = MainViewModel.GetInstance();

            mainViewModel.ZonasReserva = new ZonaReservaViewModel(Zonas);
            mainViewModel.Parking = this;
            await navigationService.NavigateOnMaster("ZonaReservaView");
        }
        #endregion
    }
}
