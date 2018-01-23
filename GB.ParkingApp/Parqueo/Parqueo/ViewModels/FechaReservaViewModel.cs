using GalaSoft.MvvmLight.Command;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Parqueo.ViewModels
{
    public class FechaReservaViewModel : INotifyPropertyChanged
    {
        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        public DateTime Fecha { get; set;}

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

        #region Constructor
        public FechaReservaViewModel()
        {
            instance = this;

            Fecha = DateTime.Today;

            navigationService = new NavigationService();
            apiService = new ApiService();
            dialogService = new DialogService();

            IsEnabled = true;
        }
        #endregion

        #region Servicios
        NavigationService navigationService;
        ApiService apiService;
        DialogService dialogService;
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
            if(Fecha < DateTime.Today)
            {
                await dialogService.ShowMessage(
                "Alerta",
                string.Format("Debes seleccionar una fecha igual o mayor a hoy '{0}' ",DateTime.Today.ToString("yyyy/MM/dd")));
            }
            else
            {
                var mainViewModel = MainViewModel.GetInstance();
                mainViewModel.ParqueosReserva = new ParqueoReservaViewModel(Fecha.ToString("yyyy/MM/dd"));
                await navigationService.NavigateOnMaster("ParqueoReservaView");
            }
           
        }
        #endregion

        #region Sigleton
        static FechaReservaViewModel instance;


        public static FechaReservaViewModel GetInstance()
        {
            if (instance == null)
            {
                return new FechaReservaViewModel();
            }
            return instance;
        }
        #endregion
    }
}
