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
    public class ParqueoViewModel : INotifyPropertyChanged
    {
        #region atributos
        List<Parking> parqueos;
        ObservableCollection<Parking> _parqueos;
        bool _isRefreshing;
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Servicios
        DialogService dialogService;
        ApiService apiService;
        #endregion

        #region Propiedades
        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRefreshing)));
                }
            }
        }

        public ObservableCollection<Parking> Parqueos
        {
            get
            {
                return _parqueos;
            }
            set
            {
                if(_parqueos != value)
                {
                    _parqueos = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Parqueos)));
                }
            }
        }
        
        #endregion

        #region Sigleton
        static ParqueoViewModel instance;

        public static ParqueoViewModel GetInstance()
        {
            if(instance == null)
            {
                return new ParqueoViewModel();
            }
            return instance;
        }
        #endregion

        #region Constructor
        public ParqueoViewModel()
        {
            instance = this;
            dialogService = new DialogService();
            apiService = new ApiService();

            LoadParking();
        }
        #endregion

        #region Metodos

        public void Add(Parking parking)
        {
            IsRefreshing = true;
            parqueos.Add(parking);
            Parqueos = new ObservableCollection<Parking>(
                parqueos.OrderBy(c => c.Descripcion));
            IsRefreshing = false;
        }

        public void Update(Parking parking)
        {
            IsRefreshing = true;
            var oldParqueo = parqueos
                .Where(c => c.IdParqueo == parking.IdParqueo)
                .FirstOrDefault();
            oldParqueo = parking;
            Parqueos = new ObservableCollection<Parking>(
                parqueos.OrderBy(c => c.Descripcion));
            IsRefreshing = false;
        }

        public async Task Delete(Parking parking)
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
                "/Parqueos",
                parking);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            parqueos.Remove(parking);
            Parqueos = new ObservableCollection<Parking>(
                parqueos.OrderBy(c => c.Descripcion));

            IsRefreshing = false;
        }

        async void LoadParking()
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage(
                       "Error",
                        "Sin Conexión, compruebe que el WIFI " +
                        "o los datos moviles esten activados");
            }
            else
            {


                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.GetList<Parking>(
                    urlAPI,
                    "/api",
                    "/Parqueos");

                if (!response.IsSuccess)
                {
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                parqueos = (List<Parking>)response.Result;


                if (parqueos.Count <= 0)
                {
                    await dialogService.ShowMessage(
                      "Alerta",
                       "No existen datos para mostrar");
                }

                foreach(var p in parqueos)
                {
                    var zonas = p.Zonas;
                    foreach( var z in zonas)
                    {
                        if (z.Estado == 1)
                        {
                            z.Condicion = "Habilitado";
                        }
                        if (z.Estado == 0)
                        {
                            z.Condicion = "Deshabilitado";
                        }
                    }
                }

                Parqueos = new ObservableCollection<Parking>(
                    parqueos.OrderBy(C => C.Descripcion));
               
            }

           
            IsRefreshing = false;
        }
        #endregion

        #region Comamdos
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadParking);
            }
        }
        #endregion

    }
}
