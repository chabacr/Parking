using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parqueo.ViewModels
{
   public class ZonaReservaViewModel : INotifyPropertyChanged
    {
        #region Atributos
        bool _isRefreshing;
        List<Zona> zonas;
        ObservableCollection<Zona> _zonas;
        #endregion

        #region Constructor
        public ZonaReservaViewModel(List<Zona> zonas)
        {
            instance = this;

            this.zonas = zonas;

            dialogService = new DialogService();
            apiService = new ApiService();

            ZonasReserva = new ObservableCollection<Zona>(
                zonas.OrderBy(z => z.NZona));
        }
        #endregion

        #region Comandos

        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Metodos
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

        public ObservableCollection<Zona> ZonasReserva
        {
            get
            {
                return _zonas;
            }
            set
            {
                if (_zonas != value)
                {
                    _zonas = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(ZonasReserva)));
                }
            }
        }
        #endregion

        #region Servicios
        DialogService dialogService;
        ApiService apiService;
        #endregion

        #region Sigleton
        static ZonaReservaViewModel instance;

        public static ZonaReservaViewModel GetInstance()
        {
            return instance;
        }
        #endregion
    }
}
