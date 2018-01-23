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
   public class TipoVehiculoViewModel : INotifyPropertyChanged
    {
        #region Atributos
        List<TipoVehiculo> tipoVehiculos;
        ObservableCollection<TipoVehiculo> _tipoVehiculos;
        bool _isRefreshing;
        #endregion

        #region Comandos

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadTipoVehiculos);
            }
        }
        #endregion

        #region Constructor
        public TipoVehiculoViewModel()
        {
            instance = this;

            apiService = new ApiService();
            dialogService = new DialogService();

            LoadTipoVehiculos();
        }
        
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Metodos
        public void Add(TipoVehiculo tipoVehiculo)
        {
            IsRefreshing = true;
            tipoVehiculos.Add(tipoVehiculo);
            TipoVehiculos = new ObservableCollection<TipoVehiculo>(
                tipoVehiculos.OrderBy(t => t.Descripcion));
            IsRefreshing = false;
        }

        public void Update(TipoVehiculo tipoVehiculo)
        {
            IsRefreshing = true;
            var oldTipoVehiculo = tipoVehiculos
                .Where(t => t.IdTipoVehiculo == tipoVehiculo.IdTipoVehiculo)
                .FirstOrDefault();
            oldTipoVehiculo = tipoVehiculo;
            TipoVehiculos = new ObservableCollection<TipoVehiculo>(
                tipoVehiculos.OrderBy(t => t.Descripcion));
            IsRefreshing = false;
        }

        public async Task Delete(TipoVehiculo tipoVehiculo)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            var response = await apiService.Delete(
                urlAPI,
                "/api",
                "/TipoVehiculoes",
                tipoVehiculo);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            tipoVehiculos.Remove(tipoVehiculo);
            TipoVehiculos = new ObservableCollection<TipoVehiculo>(
                tipoVehiculos.OrderBy(t => t.Descripcion));

            IsRefreshing = false;
        }

        async void LoadTipoVehiculos()
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if(!connection.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Sin Conexión, compruebe que el WIFI " +
                        "o los datos moviles esten activados");
            }
            else
            {
                var urlPI = App.Current.Resources["URLAPI"].ToString();

                var response = await apiService.GetList<TipoVehiculo>(
                    urlPI,
                    "/api",
                    "/TipoVehiculoes");

                if(!response.IsSuccess)
                {
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                tipoVehiculos = (List<TipoVehiculo>) response.Result;

                if(tipoVehiculos.Count<=0)
                {
                    await dialogService.ShowMessage(
                        "Alerta",
                        "No existen datos para mostrar");
                    return;
                }

                TipoVehiculos = new ObservableCollection<TipoVehiculo>(
                    tipoVehiculos.OrderBy(T => T.Descripcion));
            }
            IsRefreshing = false;
        }
        #endregion

        #region Servicios
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Sigleton
        static TipoVehiculoViewModel instance;

        public static TipoVehiculoViewModel GetInstance()
        {
            if(instance == null)
            {
                return new TipoVehiculoViewModel();
            }
            return instance;
        }
        #endregion

        #region propiedades
       public ObservableCollection<TipoVehiculo> TipoVehiculos
        {
            get
            {
                return _tipoVehiculos;
            }
            set
            {
                if(_tipoVehiculos != value)
                {
                    _tipoVehiculos = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(TipoVehiculos)));
                }
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }

            set
            {
                if(_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRefreshing)));
                }
            }
        }

        public string Descripcion { get; set; }
        #endregion
    }
}
