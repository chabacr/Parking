using GalaSoft.MvvmLight.Command;
using Parqueo.Services;
using System.ComponentModel;
using System.Windows.Input;
using System;
using Parqueo.Models;
using Xamarin.Forms;

namespace Parqueo.ViewModels
{
    public class AccionObservacionViewModel : INotifyPropertyChanged
    {
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        Observacion observacion;
        #endregion

        #region Propiedades
        public string TitleBtn { get; set; }

        public string Observation { get; set; }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if(_isRunning!=value)
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

        #region Constructor
        
        public AccionObservacionViewModel()
        {
            instance = this;

            navigationService = new NavigationService();
            apiService = new ApiService();
            dialogService = new DialogService();

            IsEnabled = true;
        }

        public AccionObservacionViewModel(Observacion observacion, string titleBtn)
        {
            this.observacion = observacion;
            instance = this;

            navigationService = new NavigationService();
            apiService = new ApiService();
            dialogService = new DialogService();

            Observation = observacion.Descripcion;
            TitleBtn = titleBtn;

            IsEnabled = true;
        }
        #endregion

        #region Sigleton
        static AccionObservacionViewModel instance;
       

        public static AccionObservacionViewModel GetInstance()
        {
            if(instance == null)
            {
                return new AccionObservacionViewModel();
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
            if (TitleBtn == "Agregar")
            {
                if (string.IsNullOrEmpty(Observation))
                {
                    await dialogService.ShowMessage(
                        "Error",
                        "Debe ingresar una observación.");
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

                var observacion = new Observacion
                {
                    Descripcion = Observation,
                };

                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Post(
                    urlAPI,
                    "/api",
                    "/Observacions",
                    observacion);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                observacion = (Observacion)response.Result;
                var observacionViewModel = ObservacionViewModel.GetInstance();
                observacionViewModel.Add(observacion);

                await navigationService.BackOnMaster();

                IsRunning = false;
                IsEnabled = true;
            }
            else
            {
                if (string.IsNullOrEmpty(Observation))
                {
                    await dialogService.ShowMessage(
                        "Error",
                        "Debe ingresar una observación.");
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

                observacion.Descripcion = Observation;

                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Put(
                     urlAPI,
                    "/api",
                    "/Observacions",
                    observacion);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                var observacionViewModel = ObservacionViewModel.GetInstance();
                observacionViewModel.Update(observacion);

                await navigationService.BackOnMaster();

                IsRunning = false;
                IsEnabled = true;
            }
        }
        #endregion
    }
}
