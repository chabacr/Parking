using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Parqueo.ViewModels
{
   public class ZonaViewModel:INotifyPropertyChanged
    {
        #region Atributos
        bool _isRefreshing;
        List<Zona> zonas;
        ObservableCollection<Zona> _zonas;
        #endregion

        #region Constructor
        public ZonaViewModel(List<Zona> zonas)
        {
            instance = this;

            this.zonas = zonas;

            dialogService = new DialogService();
            apiService = new ApiService();
            
                Zonas = new ObservableCollection<Zona>(
                zonas.OrderBy(z => z.NZona));
            
        }
        #endregion

        #region Comandos

        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Metodos
        public void Add(Zona zona)
        {
            IsRefreshing = true;
            zonas.Add(zona);
            Zonas = new ObservableCollection<Zona>(
                zonas.OrderBy(z => z.NZona));
            IsRefreshing = false;
        }

        public void Update(Zona zona)
        {
            IsRefreshing = true;
            var oldZona = zonas
                .Where(z => z.IdZona == zona.IdZona)
                .FirstOrDefault();
            oldZona = zona;
            Zonas = new ObservableCollection<Zona>(
                zonas.OrderBy(z => z.NZona));
            IsRefreshing = false;
        }
        public async Task Delete(Zona zona)
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
                "/Zonas",
                zona);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            zonas.Remove(zona);
            Zonas = new ObservableCollection<Zona>(
                zonas.OrderBy(z => z.NZona));

            IsRefreshing = false;
        }
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
                if(_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRefreshing)));
                }
            }
        }

        public ObservableCollection<Zona> Zonas
        {
            get
            {
                return _zonas;
            }
            set
            {
                if( _zonas != value)
                {
                    _zonas = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Zonas)));
                }
            }
        }
        #endregion

        #region Servicios
        DialogService dialogService;
        ApiService apiService;
        #endregion

        #region Sigleton
        static ZonaViewModel instance;

        public static ZonaViewModel GetInstance()
        {
            return instance;
        }
        #endregion
    }
}
