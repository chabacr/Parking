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
    public class AccionTipoVehiculoViewModel:INotifyPropertyChanged
    {

        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        TipoVehiculo tipoVehiculo;
        #endregion

        #region Constructor

        public AccionTipoVehiculoViewModel()
        {
            instance = this;

            navigationService = new NavigationService();
            apiService = new ApiService();
            dialogService = new DialogService();

            IsEnabled = true;
        }

        public AccionTipoVehiculoViewModel(TipoVehiculo tipoVehiculo, string titleBtn)
        {
            this.tipoVehiculo = tipoVehiculo;
            instance = this;

            navigationService = new NavigationService();
            apiService = new ApiService();
            dialogService = new DialogService();

            Descripcion = tipoVehiculo.Descripcion;
            TitleBtn = titleBtn;

            IsEnabled = true;
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        public string TitleBtn { get; set; }

        public string Descripcion { get; set; }

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

        #region Servicios
        NavigationService navigationService;
        ApiService apiService;
        DialogService dialogService;
        #endregion
        
        #region Sigleton
        static AccionTipoVehiculoViewModel instance;
        
        public static AccionTipoVehiculoViewModel GetInstance()
        {
            if (instance == null)
            {
                return new AccionTipoVehiculoViewModel();
            }
            return instance;
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

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();
            var tipoVehiculoViewModel = TipoVehiculoViewModel.GetInstance();

            if (TitleBtn == "Agregar")
            {
              
                var tipoVehiculo = new TipoVehiculo
                {
                    Descripcion = Descripcion,
                };
                
                var response = await apiService.Post(
                    urlAPI,
                    "/api",
                    "/TipoVehiculoes",
                    tipoVehiculo);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                tipoVehiculo = (TipoVehiculo) response.Result;
                
                tipoVehiculoViewModel.Add(tipoVehiculo);

                await navigationService.BackOnMaster();
                
            }
            
            if(TitleBtn == "Actualizar")
            {
                

                tipoVehiculo.Descripcion = Descripcion;
                
                var response = await apiService.Put(
                     urlAPI,
                    "/api",
                    "/TipoVehiculoes",
                    tipoVehiculo);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }
                
                tipoVehiculoViewModel.Update(tipoVehiculo);

                await navigationService.BackOnMaster();
                
            }

            IsRunning = false;
            IsEnabled = true;
        }
        #endregion
    }
}
