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
    public class VehiculoViewModel :INotifyPropertyChanged
    {
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        List<Vehiculo> carros;
        ObservableCollection<Vehiculo> _carros;
        bool _isRefreshing;
        #endregion

        #region Propiedades
        public ObservableCollection<Vehiculo> Carros
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
                        new PropertyChangedEventArgs(nameof(Carros)));
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
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRefreshing)));
                }
            }
        }

        public Usuario usuario { get; set; }
        #endregion

        #region Constructor
        public VehiculoViewModel(List<Vehiculo> carros)
        {
            instance = this;

            this.carros = carros;
            apiService = new ApiService();
            dialogService = new DialogService();
            if (carros.Count < 1)
            {
                App.Current.MainPage.DisplayAlert("Alerta", "No hay registros para mostrar", "OK");
            }
            else
            {
                Carros = new ObservableCollection<Vehiculo>(
                carros.OrderBy(c => c.Marca));
            }
        }
       
        #endregion

        #region Sigleton
        static VehiculoViewModel instance;

        public static VehiculoViewModel GetInstance()
        {
          
            return instance;
        }
        #endregion

        #region Servicios
        ApiService apiService;
        DialogService dialogService;
        #endregion
        #region Metodos
        public void Add(Vehiculo vehiculo)
        {
            IsRefreshing = true;
            carros.Add(vehiculo);
            Carros = new ObservableCollection<Vehiculo>(
                carros.OrderBy(c => c.Marca));
            IsRefreshing = false;
        }

        public void Update(Vehiculo vehiculo)
        {
            IsRefreshing = true;
            var oldCarros = carros
                .Where(c => c.IdVehiculo == vehiculo.IdVehiculo)
                .FirstOrDefault();
            carros.Remove(oldCarros);
            Add(vehiculo);
        }

        public async Task Delete(Vehiculo vehiculo)
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
                "/Vehiculos",
                vehiculo);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            carros.Remove(vehiculo);
            Carros = new ObservableCollection<Vehiculo>(
                carros.OrderBy(c => c.Marca));

            IsRefreshing = false;
        }
        //private void LoadCars()
        //{
        //    IsRefreshing = true;
            
          
        //    IsRefreshing = false;
        //}
        #endregion

        #region Comandos 
        //public ICommand RefreshCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(LoadCars);
        //    }
        //}
        
        #endregion
    }
}
