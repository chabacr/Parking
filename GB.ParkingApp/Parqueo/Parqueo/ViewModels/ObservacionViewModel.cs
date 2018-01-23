using GalaSoft.MvvmLight.Command;
using Parqueo.Models;
using Parqueo.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Parqueo.ViewModels
{
    public class ObservacionViewModel:INotifyPropertyChanged
    {
        #region Atributos
        List<Observacion> observations;
        ObservableCollection<Observacion> _observations;
        bool _isRefreshing;
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Servicios
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Sigleton
        static ObservacionViewModel instance;
        public static ObservacionViewModel GetInstance()
        {
            if(instance == null)
            {
                return new ObservacionViewModel();
            }
            return instance;
        }
        #endregion

        #region Constructor
        public ObservacionViewModel()
        {
            instance = this;

            apiService = new ApiService();
            dialogService = new DialogService();

            LoadObservations();
        }
        #endregion

        #region Propiedades
        public ObservableCollection<Observacion> Observations
        {
            get
            {
                return _observations;
            }
            set
            {
                if(_observations!= value)
                {
                    _observations = value;
                    PropertyChanged?.Invoke(
                        this, 
                        new PropertyChangedEventArgs(nameof(Observations)));
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
        #endregion

        #region Metodos
        public void Add(Observacion observation)
        {
            IsRefreshing = true;
            observations.Add(observation);
            Observations = new ObservableCollection<Observacion>(
                observations.OrderBy(c => c.Descripcion));
            IsRefreshing = false;
        }

        public void Update(Observacion observation)
        {
            IsRefreshing = true;
            var oldObservacion = observations
                .Where(c => c.IdObservacion == observation.IdObservacion)
                .FirstOrDefault();
            oldObservacion = observation;
            Observations = new ObservableCollection<Observacion>(
                observations.OrderBy(c => c.Descripcion));
            IsRefreshing = false;
        }

        public async Task Delete(Observacion observation)
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
                "/Observacions",
                observation);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            observations.Remove(observation);
            Observations = new ObservableCollection<Observacion>(
                observations.OrderBy(c => c.Descripcion));

            IsRefreshing = false;
        }

        async void LoadObservations()
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

                var response = await apiService.GetList<Observacion>(
                    urlAPI,
                    "/api",
                    "/Observacions");

                if (!response.IsSuccess)
                {
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                observations = (List<Observacion>)response.Result;
                

                if (observations.Count<=0)
                {
                    await dialogService.ShowMessage(
                      "Alerta",
                       "No existen datos para mostrar");
                    return;
                }

                Observations = new ObservableCollection<Observacion>(
                    observations.OrderBy(C => C.Descripcion));
               
            }
            
            IsRefreshing = false;
        }

        
        #endregion


        #region Comandos
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadObservations);
            }
        }
        #endregion
    }
}
