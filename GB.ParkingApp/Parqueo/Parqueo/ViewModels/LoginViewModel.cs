using GalaSoft.MvvmLight.Command;
using Parqueo.Helpers;
using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Parqueo.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #region Eventos

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Singleton

        static LoginViewModel instance;

        public static LoginViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new LoginViewModel();
            }

            return instance;
        }

        #endregion

        #region Atributos

        string _email;

        string _password;

        bool _isToggled;

        bool _isRunning;
       
        bool _isEnabled;

        bool _isEnabledRegister;
        
        #endregion

        #region Propiedades
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                if( _email != value)
                {
                    _email = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
                }
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
                }
            }
        }

        public bool IsToggled
        {
            get
            {
                return _isToggled;
            }
            set
            {
                if (_isToggled != value)
                {
                    _isToggled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsToggled)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public bool IsEnabledRegister
        {
            get
            {
                return _isEnabledRegister;
            }
            set
            {
                if (_isEnabledRegister != value)
                {
                    _isEnabledRegister = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledRegister)));
                }
            }
        }
        #endregion

        #region Servicios

        DialogService dialogService;
        NavigationService navigationService;
        ApiService apiService;
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            dialogService = new DialogService();
            navigationService = new NavigationService();
            apiService = new ApiService();
            instance = this;

            Email = "cesar.badilla@grupobabel.com";
            Password = "Ing.C2s1r";

            IsEnabledRegister = true;
            IsEnabled = true;
            IsToggled = true;
        }
        #endregion

        #region comandos
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
        }

        async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar un correo electrónico");
                return;
            }

            if (!RegexUtilities.IsValidEmail(Email))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar un Correo válido.");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debes ingresar una contraseña");
                return;
            }

            IsRunning = true;
            IsEnabled = false;
            IsEnabledRegister = false;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                IsEnabledRegister = true;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            var response = await apiService.GetBy<Usuario>(
                urlAPI,
                "/api",
                "/Usuarios",
                Email);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                IsEnabledRegister = true;
                await dialogService.ShowMessage(
                    "Error",
                    "Usuario o contraseña incorrecta");
                Password = null;
                return;
            }

            var usuario = (Usuario) response.Result;

            if (Password != usuario.Password)
            {
                IsRunning = false;
                IsEnabled = true;
                IsEnabledRegister = true;
                await dialogService.ShowMessage(
                    "Error",
                    "Usuario o contraseña incorrecta");
                Password = null;
                return;
            }

            Email = null;
            Password = null;

            IsRunning = false;
            IsEnabled = true;
            IsEnabledRegister = true;

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Usuario = usuario;
            mainViewModel.Vehiculos = mainViewModel.Usuario.Vehiculos;
            mainViewModel.MisReservas = mainViewModel.Usuario.Reservas;
            //mainViewModel.Permisos =  (List<Permiso>) otherResponse.Result;
            mainViewModel.LoadMenu(usuario.Permisos);
            
            mainViewModel.Perfil = new PerfilViewModel(mainViewModel.Usuario);
            //mainViewModel.Carros = new VehiculoViewModel(mainViewModel.Vehiculos);
            //mainViewModel.Reservas = new ReservaViewModel(mainViewModel.MisReservas);
            navigationService.SetMainPage("MasterView");

            
        }


        public ICommand RegisterNewUserCommand
        {
            get
            {
                return new RelayCommand(RegisterNewUser);
            }
        }

        async void RegisterNewUser()
        {
            MainViewModel.GetInstance().NewRegister = new RegistroViewModel();
            await navigationService.NavigateOnLogin("RegistroView");
        }
        #endregion
    }
}
