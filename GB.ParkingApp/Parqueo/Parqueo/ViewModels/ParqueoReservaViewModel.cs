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
    public class ParqueoReservaViewModel:INotifyPropertyChanged
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
        public string Fecha { get; set; }

        public ObservableCollection<Parking> ParqueosReserva
        {
            get
            {
                return _parqueos;
            }
            set
            {
                if (_parqueos != value)
                {
                    _parqueos = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(ParqueosReserva)));
                }
            }
        }

        #endregion

        #region Sigleton
        static ParqueoReservaViewModel instance;

        public static ParqueoReservaViewModel GetInstance()
        {
           
            return instance;
        }
        #endregion

        #region Constructor
        public ParqueoReservaViewModel(string fecha)
        {
            instance = this;
            dialogService = new DialogService();
            apiService = new ApiService();
            Fecha = fecha;
            LoadParkingReserva();
        }
        #endregion

        #region Metodos
        

        async void LoadParkingReserva()
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

                
                var response = await apiService.GetListParqueosZonasDisponibles<Parking>(
                    urlAPI,
                    "/api",
                    "/GetParqueosZonasDisponibles",
                    Fecha);

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

                foreach (var p in parqueos)
                {
                    var zonas = p.Zonas;
                    foreach (var z in zonas)
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

                ParqueosReserva = new ObservableCollection<Parking>(
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
                return new RelayCommand(LoadParkingReserva);
            }
        }
        #endregion

    }
}
