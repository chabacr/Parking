using GalaSoft.MvvmLight.Command;
using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Parqueo.ViewModels
{
    public class AccionParqueoViewModel:INotifyPropertyChanged
    {

        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        Parking parking;
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Servicios
        NavigationService navigationService;
        DialogService dialogService;
        ApiService apiService;
        #endregion

        #region Sigleton
        static AccionParqueoViewModel instance;

        public static AccionParqueoViewModel GetInstance()
        {
            if(instance == null)
            {
                return new AccionParqueoViewModel();
            }
            return instance;
        }
        #endregion

        #region Propiedades
        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }

            set
            {
                if(_isRunning != value)
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

        public string TitleBtn { get; set; }

        public string Descripcion { get; set; }

        public string Direccion { get; set; }

        public int NumeroZonas { get; set; }
        
        #endregion

        #region Constructores
        public AccionParqueoViewModel()
        {
            instance = this;

            navigationService = new NavigationService();
            dialogService = new DialogService();
            apiService = new ApiService();

            IsEnabled = true;
        }

        public AccionParqueoViewModel(Parking parking, string TitleBtn)
        {
            this.parking = parking;

            instance = this;

            navigationService = new NavigationService();
            dialogService = new DialogService();
            apiService = new ApiService();

            Descripcion = parking.Descripcion;
            Direccion = parking.Direccion;
            NumeroZonas = parking.NumeroZonas;
            this.TitleBtn = TitleBtn;
            IsEnabled = true;
        }
        #endregion

        #region Comandos

        public ICommand ActionCommand
        {
            get
            {
                return new RelayCommand(Go);
            }
        }

        async void Go()
        {
            if (string.IsNullOrEmpty(Descripcion))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar una descripción.");
                return;
            }

            if (string.IsNullOrEmpty(Direccion))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar una dirección.");
                return;
            }


            if (string.IsNullOrEmpty(NumeroZonas.ToString()))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar el número de zonas.");
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

            if (TitleBtn == "Agregar")
            {
                var parking = new Parking
                {
                    Descripcion = Descripcion,
                    Direccion = Direccion,
                    NumeroZonas = NumeroZonas
                };

                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Post(
                    urlAPI,
                    "/api",
                    "/Parqueos",
                    parking);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                parking = (Parking)response.Result;
                var parqueoviewModel = ParqueoViewModel.GetInstance();
                parqueoviewModel.Add(parking);

                await navigationService.BackOnMaster();

                IsRunning = false;
                IsEnabled = true;
            }
            else
            {
                parking.Descripcion = Descripcion;
                parking.Direccion = Direccion;
                parking.NumeroZonas = NumeroZonas;

                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Put(
                     urlAPI,
                    "/api",
                    "/Parqueos",
                    parking);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                var parqueosViewModel = ParqueoViewModel.GetInstance();
                parqueosViewModel.Update(parking);

                await navigationService.BackOnMaster();

                IsRunning = false;
                IsEnabled = true;

            }
        }
       

        #endregion

    }
}
