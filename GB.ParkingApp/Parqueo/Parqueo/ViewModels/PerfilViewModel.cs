using GalaSoft.MvvmLight.Command;
using Parqueo.Helpers;
using Parqueo.Models;
using Parqueo.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
    public class PerfilViewModel : INotifyPropertyChanged
    {
        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        bool _isRunningPassword;
        bool _isEnabledPassword;
        ImageSource _imageSource;
        MediaFile file;
        Usuario usuario;
        #endregion

        #region Propiedades

        public int IdUsuario { get; set; }

        public int IdTipoUsuario { get; set; }

        public TipoUsuario TipoUsuario { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public string NombreCompleto
        {
            get
            {
                return Nombre + " " + Apellidos;
            }
        }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string Password { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string Confirm { get; set; }

        public string Foto { get; set; }

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


        public bool IsRunningPassword
        {
            get
            {
                return _isRunningPassword;
            }
            set
            {
                if (_isRunningPassword != value)
                {
                    _isRunningPassword = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunningPassword)));
                }
            }
        }

        public bool IsEnabledPassword
        {
            get
            {
                return _isEnabledPassword;
            }
            set
            {
                if (_isEnabledPassword != value)
                {
                    _isEnabledPassword = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabledPassword)));
                }
            }
        }

        public ImageSource ImageSource
        {
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(ImageSource)));
                }
            }
            get
            {
                return _imageSource;
            }
        }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(Foto))
                {
                    return "noimage";
                }

                return string.Format(
                    "http://api-gb-parking.azurewebsites.net/{0}",
                    Foto.Substring(1));
            }
        }
        #endregion

        #region Constructor
        public PerfilViewModel(Usuario usuario)
        {
            this.usuario = usuario;

            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            Nombre = usuario.Nombre;
            Apellidos = usuario.Apellidos;
            Telefono = usuario.Telefono;
            ImageSource = usuario.ImageFullPath;

            IsEnabled = true;
            IsEnabledPassword = true;

            
        }
        #endregion

        #region Commands
        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

        async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable &&
                CrossMedia.Current.IsTakePhotoSupported)
            {
                var source = await dialogService.ShowImageOptions();

                if (source == "Cancelar")
                {
                    file = null;
                    return;
                }

                if (source == "De la Cámara")
                {
                    file = await CrossMedia.Current.TakePhotoAsync(
                        new StoreCameraMediaOptions
                        {
                            Directory = "Sample",
                            Name = "test.jpg",
                            PhotoSize = PhotoSize.Small,
                        }
                    );
                }
                else
                {
                    file = await CrossMedia.Current.PickPhotoAsync();
                }
            }
            else
            {
                file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }

        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        async void Save()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debes ingresar un nombre");
                return;
            }

            if (string.IsNullOrEmpty(Apellidos))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar un Apellido");
                return;
            }



            IsRunning = true;
            IsEnabled = false;

            byte[] imageArray = null;
            if (file != null)
            {
                imageArray = FilesHelper.ReadFully(file.GetStream());
            }


            usuario.Nombre = Nombre;
            usuario.Apellidos = Apellidos;
            usuario.Telefono = Telefono;
            usuario.ImageArray = imageArray;


            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage(
                       "Error",
                        "Sin Conexión, compruebe que el WIFI " +
                        "o los datos moviles esten activados");
                return;
            }
            else
            {
                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.Put(
                    urlAPI,
                    "/api",
                    "/Usuarios",
                    usuario);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }

                await dialogService.ShowMessage(
                      "Alerta",
                       "Perfil Modificado!!");

                if (file != null)
                {
                    file.Dispose();
                }

                var mainViewModel = MainViewModel.GetInstance();
                mainViewModel.Usuario = (Usuario)response.Result;
                mainViewModel.Perfil = new PerfilViewModel(mainViewModel.Usuario);

                IsRunning = false;
                IsEnabled = true;

                navigationService.SetMainPage("MasterView");
            }

            IsRunning = false;
            IsEnabled = true;
        }

        public ICommand SaveCommandPassword
        {
            get
            {
                return new RelayCommand(SavePassword);
            }
        }

        async void SavePassword()
        {
            if (string.IsNullOrEmpty(OldPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar una nueva Contraseña.");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar una nueva Contraseña.");
                return;
            }

            if (NewPassword.Length < 6)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "La contraseña nueva debe tener al menos 6 caracteres de longitud.");
                return;
            }

            if (string.IsNullOrEmpty(Confirm))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar una contraseña de confirmación.");
                return;
            }

            if (!NewPassword.Equals(Confirm))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "La contraseña nueva y confirmar, no coincide.");
                return;
            }



            IsRunningPassword = true;
            IsEnabledPassword = false;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRunningPassword = false;
                IsEnabledPassword = true;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            else
            {
                
                var urlAPI = Application.Current.Resources["URLAPI"].ToString();

                var response = await apiService.GetBy<Usuario>(
                    urlAPI,
                    "/api",
                    "/Usuarios",
                    usuario.Correo);


                var user = (Usuario) response.Result;

                if (OldPassword != user.Password)
                {
                    IsRunningPassword = false;
                    IsEnabledPassword = true;
                    await dialogService.ShowMessage(
                        "Error",
                        "Contraseña actual incorrecta");
                   // OldPassword = null;
                   // NewPassword = null;
                   // Confirm = null;
                    return;
                }

                var otherConnection = await apiService.CheckConnection();
                if (!otherConnection.IsSuccess)
                {
                    IsRunningPassword = false;
                    IsEnabledPassword = true;
                    await dialogService.ShowMessage("Error", connection.Message);
                    return;
                }
                else
                {

                    usuario.Password = NewPassword;
                    var Newresponse = await apiService.Put(
                            urlAPI,
                            "/api",
                            "/Usuarios",
                            usuario);

                    if (!Newresponse.IsSuccess)
                    {
                        IsRunningPassword = false;
                        IsEnabledPassword = true;
                        await dialogService.ShowMessage(
                            "Error",
                            Newresponse.Message);
                        return;
                    }

                    var mainViewModel = MainViewModel.GetInstance();
                    mainViewModel.Usuario = (Usuario)response.Result;
                    mainViewModel.Perfil = new PerfilViewModel(mainViewModel.Usuario);

                    IsRunningPassword = false;
                    IsEnabledPassword = true;

                    await dialogService.ShowMessage(
                   "Alerta",
                   "La contraseña ha sido cambiada.");

                    navigationService.SetMainPage("LoginView"); 
                }
            }
        }
        #endregion

        #region Servicios
        DialogService dialogService;
        ApiService apiService;
        NavigationService navigationService;
        #endregion


        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
