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

namespace Parqueo.ViewModels
{
   public class ReservaViewModel:INotifyPropertyChanged
    {
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        List<Reserva> reservas;
        ObservableCollection<Reserva> _reservas;
        bool _isRefreshing;
        #endregion

        #region Propiedades
        public ObservableCollection<Reserva> Reservas
        {
            get
            {
                return _reservas;
            }
            set
            {
                if (_reservas != value)
                {
                    _reservas = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Reservas)));
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
        public ReservaViewModel(List<Reserva> reservas)
        {
            instance = this;

            this.reservas = reservas;
            apiService = new ApiService();
            dialogService = new DialogService();
            if (reservas.Count < 1)
            {
                App.Current.MainPage.DisplayAlert("Alerta", "No hay registros para mostrar", "OK");
            }
            else
            {
                Reservas = new ObservableCollection<Reserva>(
                reservas.OrderBy(c => c.FechaRegistro));
            }
        }

        #endregion



        #region Sigleton
        static ReservaViewModel instance;

        public static ReservaViewModel GetInstance()
        {

            return instance;
        }
        #endregion

        #region Servicios
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Metodos
        public void Add(Reserva reserva)
        {
            IsRefreshing = true;
            reservas.Add(reserva);
            Reservas = new ObservableCollection<Reserva>(
                reservas.OrderBy(c => c.FechaRegistro));
            IsRefreshing = false;
        }

        public void Update(Reserva reserva)
        {
            IsRefreshing = true;
            var oldReservas = reservas
                .Where(c => c.IdReserva == reserva.IdReserva)
                .FirstOrDefault();
            reservas.Remove(oldReservas);
            Add(reserva);
        }

       
        private void LoadReservas()
        {
            IsRefreshing = true;


            IsRefreshing = false;
        }
        #endregion

        #region Comandos 
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadReservas);
            }
        }

        #endregion
    }
}
