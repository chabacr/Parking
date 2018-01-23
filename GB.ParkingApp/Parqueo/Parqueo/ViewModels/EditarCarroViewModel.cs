using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parqueo.Models;
using System.ComponentModel;
using Parqueo.Services;

namespace Parqueo.ViewModels
{
    public class EditarCarroViewModel : INotifyPropertyChanged
    {

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Servicios
        NavigationService navigationService;
        #endregion
        

        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        Vehiculo carro;
        #endregion

        #region Popiedades
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

        public string Brand
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public EditarCarroViewModel(Vehiculo carro)
        {
            this.carro = carro;

            navigationService = new NavigationService();

            

            IsEnabled = true;
        } 
        #endregion
    }
}
