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
    public class AccionZonaViewModel:INotifyPropertyChanged
    {
        #region Atributos
        Zona zona;
        List<Estados> estados;

        bool _isRunning;
        bool _isEnabled;

        #endregion

        #region Constructor
        public AccionZonaViewModel()
        {
            instance = this;
           

            Estados = LoadCmbEstados();

            dialogService = new DialogService();
            apiService = new ApiService();
            navigationService = new NavigationService();

            IsEnabled = true;

        }

        public AccionZonaViewModel(Zona zona)
        {
            this.zona = zona;

            instance = this;
            
            Estados = LoadCmbEstados();
            
            dialogService = new DialogService();
            apiService = new ApiService();
            navigationService = new NavigationService();

            NZona = zona.NZona;

            if(zona.Estado == 1)
            {
                SelectEstado = Estados[0];
            }
            else
            {
                SelectEstado = Estados[1];
            }
           
            TitleBtn = "Actualizar";
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
           
            int estado = 0 ;

            if (string.IsNullOrEmpty(NZona))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar un número de zona.");
                return;
            }

            if (SelectEstado == null)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe seleccionar un estado para el número de zona.");
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

            if (SelectEstado.Nombre == "Habilitado")
            {
                estado = 1;
            }

            if (SelectEstado.Nombre == "Deshabilitado")
            {
                estado = 0;
            }


            var mainViewModel = MainViewModel.GetInstance();

            if (TitleBtn == "Agregar")
            {
                var zonaViewModel = ZonaViewModel.GetInstance();
                if( zonaViewModel.Zonas.Count >= mainViewModel.Parking.NumeroZonas)
                {
                    await dialogService.ShowMessage(
                    "Error",
                    string.Format("El parqueo {0} ya cuenta con el número máximo de zonas '{1}'",
                    mainViewModel.Parking.Descripcion,
                    mainViewModel.Parking.NumeroZonas));

                    IsRunning = false;
                    IsEnabled = true;

                    return;
                }
                var zona = new Zona
                {
                    IdParqueo = mainViewModel.Parking.IdParqueo,                    
                    NZona = NZona,
                    Estado = estado,
                };

                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Post(
                    urlAPI,
                    "/api",
                    "/Zonas",
                    zona);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                zona = (Zona)response.Result;
                zona.Condicion = SelectEstado.Nombre;
                
                zonaViewModel.Add(zona);

                await navigationService.BackOnMaster();

                IsRunning = false;
                IsEnabled = true;
            }
            
            if(TitleBtn=="Actualizar")
            {
                zona.NZona = NZona;
                zona.Estado = estado;
                zona.IdParqueo = mainViewModel.Parking.IdParqueo;
                

                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Put(
                     urlAPI,
                    "/api",
                    "/Zonas",
                    zona);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                var zonaViewModel = ZonaViewModel.GetInstance();
                zona.Condicion = SelectEstado.Nombre;
                zonaViewModel.Update(zona);

                await navigationService.BackOnMaster();

                IsRunning = false;
                IsEnabled = true;
            }
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Propiedades
        public List<Estados> Estados
        {
            get
            {
                return estados;
            }
            set
            {
                if(estados != value)
                {
                    estados = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Estados)));
                }
            }
        }

        public string TitleBtn { get; set; }

        public string NZona { get; set; }

        public int IdParqueo { get; set; }

        public Estados SelectEstado { get; set; }
        
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
        DialogService dialogService;
        ApiService apiService;
        #endregion

        #region Metodos
        private List<Estados> LoadCmbEstados()
        {
           var estados = new List<Estados>();
            estados.Add(new Estados
            {
                Nombre = "Habilitado",
            });
            estados.Add(new Estados
            {
                Nombre = "Deshabilitado",
            });

            return estados;
            
        }
        #endregion

        #region Sigleton
        static AccionZonaViewModel instance;

        public static AccionZonaViewModel GetInstance()
        {
            return instance;
        }
        #endregion
    }

    public class Estados
    {
        public string Nombre { get; set; }
    }
}
