namespace Parqueo.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Models;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Services;
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using Xamarin.Forms;
    public class RegistroViewModel:INotifyPropertyChanged
    {
        #region Eventos

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Singleton

        static RegistroViewModel instance;

        public static RegistroViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new RegistroViewModel();
            }

            return instance;
        }

        #endregion

        #region Services
        ApiService apiService;
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region Atributos
        bool _isRunning;
        bool _isEnabled;
        ImageSource _imageSource;
        MediaFile file;
        #endregion

        #region Propiedades

        public string Nombre
        {
            get;
            set;
        }

        public string Apellidos
        {
            get;
            set;
        }

        public string Correo
        {
            get;
            set;
        }

        public string Telefono
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string Confirm
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if(_isRunning != value)
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

        public string Foto
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public RegistroViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();

            ImageSource = "noimage";
           
            IsEnabled = true;
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
            

            if (string.IsNullOrEmpty(Correo))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar un Correo.");
                return;
            }

            if (!RegexUtilities.IsValidEmail(Correo))
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
                    "Debe ingresar una Contraseña.");
                return;
            }

            if (Password.Length < 6)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "La contraseña debe tener al menos 6 caracteres de longitud.");
                return;
            }

            if (string.IsNullOrEmpty(Confirm))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar una contraseña de confirmación.");
                return;
            }

            if (!Password.Equals(Confirm))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "La contraseña y confirmar, no coincide.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            byte[] imageArray = null;
            if (file != null)
            {
                imageArray = FilesHelper.ReadFully(file.GetStream());
               
            }

            var mainViewModel = MainViewModel.GetInstance();

            var usuario = new Usuario
            {
                IdTipoUsuario = 1,
                Nombre = Nombre,
                ImageArray = imageArray,
                Apellidos = Apellidos,
                Telefono = Telefono,
                Password = Password,
                Correo = Correo,
            };

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

                var response = await apiService.Post(
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

                IsRunning = false;
                IsEnabled = true;

                if (file != null)
                {
                    file.Dispose();
                }
                await navigationService.BackOnLogin();
            }
            
            

            IsRunning = false;
            IsEnabled = true;
        }
        #endregion
    }
}
