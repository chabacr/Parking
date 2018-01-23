using GalaSoft.MvvmLight.Command;
using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Parqueo.ViewModels
{
    public class AccionReservaViewModel :INotifyPropertyChanged
    {
        #region Attributes
        List<Vehiculo> _carros;
        bool _isRunning;
        bool _isEnabled;
        #endregion

        #region Constructor
        public AccionReservaViewModel(List<Vehiculo> _carros)
        {
            instance = this;

            this.Vehiculos = _carros;

            navigationService = new NavigationService();
            apiService = new ApiService();
            dialogService = new DialogService();

            SelectVehiculos = null;
            IsEnabled = true;
        }
        #endregion

        #region Propiedades
        public List<Vehiculo> Vehiculos
        {
            get
            {
                return _carros;
            }
            set
            {
                if (_carros != value)
                {
                    _carros = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Vehiculos)));
                }
            }
        }

        public Vehiculo SelectVehiculos { get; set; }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if (_isRunning != value)
                {
                    _isRunning = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Sigleton
        static AccionReservaViewModel instance;


        public static AccionReservaViewModel GetInstance()
        {
            return instance;
        }
        #endregion

        #region Servicios
        NavigationService navigationService;
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Comandos
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        async void Save()
        {
            var fechaReservaViewModel = FechaReservaViewModel.GetInstance();
            var mainViewModel = MainViewModel.GetInstance();
            
            

            if(SelectVehiculos == null)
            {
                await dialogService.ShowMessage(
                  "Error",
                  "Debe Seleccionar un vehiculo");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var reserva = new Reserva
            {
                IdVehiculo = SelectVehiculos.IdVehiculo,
                IdUsuario = mainViewModel.Usuario.IdUsuario,
                IdZona = mainViewModel.Zona.IdZona,
                FechaRegistro = DateTime.Today,
                FechaReserva = fechaReservaViewModel.Fecha,
                FechaFinReserva = fechaReservaViewModel.Fecha,
            };

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            var response = await apiService.Post(
                urlAPI,
                "/api",
                "/Reservas",
                reserva);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            reserva.Zona = mainViewModel.Zona;
            reserva.Zona.Parqueo = mainViewModel.Parking;
            reserva.Vehiculo = SelectVehiculos;

            var reservaViewModel = ReservaViewModel.GetInstance();
            reservaViewModel.Add(reserva);

            IsRunning = false;
            IsEnabled = true;

             navigationService.SetMainPage("MasterView");
            await navigationService.NavigateOnMaster("FechaReservaView");
            

           
        }
        #endregion
    }
}
