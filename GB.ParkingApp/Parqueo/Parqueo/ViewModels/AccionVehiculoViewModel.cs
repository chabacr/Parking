using GalaSoft.MvvmLight.Command;
using Parqueo.Helpers;
using Parqueo.Models;
using Parqueo.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
  public class AccionVehiculoViewModel:INotifyPropertyChanged
    {
        #region Atributos
        List<TipoVehiculo> tipoVehiculos;
        bool _isRunning;
        bool _isEnabled;
        bool _isEnabledSelect;
        string _titleSelect;
        ImageSource _imageSource;
        MediaFile file;
        Vehiculo vehiculo;
        TipoVehiculo _tipoVehiculo;
        #endregion

        #region Constructor
        public AccionVehiculoViewModel(string btnTitle)
        {
            instance = this;

            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            
            LoadTipoVehiculos(vehiculo);
            IsEnabled = true;
            this.btnTitle = btnTitle;
            ImageSource = "noimage";
        }

        public  AccionVehiculoViewModel(Vehiculo vehiculo,string btnTitle)
        {
            
            instance = this;
            this.vehiculo = vehiculo;
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            LoadTipoVehiculos(vehiculo);
            IsEnabled = true;
            this.btnTitle = btnTitle;
            ImageSource = vehiculo.ImageFullPath;
            Marca = vehiculo.Marca;
            Modelo = vehiculo.Modelo;
            Placa = vehiculo.Placa;
            Color = vehiculo.Color;
            
            
            //IdTipoVehiculo = vehiculo.TipoVehiculo.IdTipoVehiculo;
            //SelectTipoVehiculos = vehiculo.TipoVehiculo;
            
        }
        #endregion

        #region Comandos
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

        public ICommand AccionCommand
        {
            get
            {
                return new RelayCommand(Accion);
            }
        }

        async void Accion()
        {
            
            if (SelectTipoVehiculos==null)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debes seleccionar un tipo de vehiculo");
                return;
            }

            if (string.IsNullOrEmpty(Marca))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debes ingresar una marca");
                return;
            }

            if (string.IsNullOrEmpty(Modelo))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar el modelo");
                return;
            }


            if (string.IsNullOrEmpty(Placa))
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Debe ingresar la placa.");
                return;
            }

            
            IsRunning = true;
            IsEnabled = false;

            byte[] imageArray = null;
            if (file != null)
            {
                imageArray = FilesHelper.ReadFully(file.GetStream());
                
            }

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage(
                       "Error",
                        "Sin Conexión, compruebe que el WIFI " +
                        "o los datos moviles esten activados");
                IsRunning = false;
                IsEnabled = true;
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();
            var urlAPI = Application.Current.Resources["URLAPI"].ToString();

            if (btnTitle == "Agregar")
            {
                var vehiculo = new Vehiculo
                {
                    Color = Color,
                    IdUsuario = mainViewModel.Usuario.IdUsuario,
                    IdTipoVehiculo = SelectTipoVehiculos.IdTipoVehiculo,
                    Marca = Marca,
                    Placa = Placa,
                    Modelo = Modelo,
                    ImageArray = imageArray,
                };

                
                    var response = await apiService.Post(
                        urlAPI,
                        "/api",
                        "/Vehiculos",
                        vehiculo);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                }
                else
                {
                    vehiculo = (Vehiculo)response.Result;
                    vehiculo.TipoVehiculo = SelectTipoVehiculos;
                    var vehiculoViewModel = VehiculoViewModel.GetInstance();
                    vehiculoViewModel.Add(vehiculo);

                    IsRunning = false;
                    IsEnabled = true;

                    if (file != null)
                    {
                        file.Dispose();
                    }
                   
                    await navigationService.BackOnMaster();
                }
                
            }

            if(btnTitle == "Actualizar")
            {
                var oldVehiculo = new Vehiculo
                {
                    Marca = vehiculo.Marca,
                    Modelo = vehiculo.Modelo,
                    Placa = vehiculo .Placa,
                    Color = vehiculo.Color,
                    IdTipoVehiculo = vehiculo.IdTipoVehiculo
                };

                
                vehiculo.Color = Color;
                vehiculo.Marca = Marca;
                vehiculo.Modelo = Modelo;
                vehiculo.Placa = Placa;
                vehiculo.ImageArray = imageArray;
                vehiculo.IdUsuario = mainViewModel.Usuario.IdUsuario;
                vehiculo.IdTipoVehiculo = SelectTipoVehiculos.IdTipoVehiculo;
                vehiculo.TipoVehiculo = SelectTipoVehiculos;

                var response = await apiService.Put(
                    urlAPI,
                    "/api",
                    "/Vehiculos",
                    vehiculo);

                if (!response.IsSuccess)
                {
                    IsRunning = false;
                    IsEnabled = true;
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    vehiculo.Color = oldVehiculo.Color;
                    vehiculo.Marca = oldVehiculo.Marca;
                    vehiculo.Modelo = oldVehiculo.Modelo;
                    vehiculo.Placa = oldVehiculo.Placa;
                    vehiculo.IdTipoVehiculo = oldVehiculo.IdTipoVehiculo;
                    return;
                }
                else
                {
                    mainViewModel.Carros.Update((Vehiculo)response.Result);


                    IsRunning = false;
                    IsEnabled = true;

                    if (file != null)
                    {
                        file.Dispose();
                    }

                    await navigationService.BackOnMaster();
                }                
            }

            IsRunning = false;
            IsEnabled = true;
        }
        #endregion

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Metodos
        async void LoadTipoVehiculos(Vehiculo vehiculo)
        {
            IsEnabledSelect = false;
            TitleSelect = "Cargando...";

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error",
                    "Sin Conexión, compruebe que el WIFI " +
                        "o los datos moviles esten activados");
                await navigationService.BackOnMaster();
            }
            else
            {
                var urlPI = App.Current.Resources["URLAPI"].ToString();

                var response = await apiService.GetList<TipoVehiculo>(
                    urlPI,
                    "/api",
                    "/TipoVehiculoes");

                if (!response.IsSuccess)
                {
                    await dialogService.ShowMessage(
                        "Error",
                        response.Message);
                    return;
                }
                else
                {
                    
                   var tipoVehiculos = (List<TipoVehiculo>)response.Result;

                    if (tipoVehiculos.Count <= 0)
                    {
                        await dialogService.ShowMessage(
                            "Alerta",
                            "No existen datos para mostrar");
                        return;
                    }
                    else
                    {
                        TipoVehiculos = tipoVehiculos;
                        IsEnabledSelect = true;
                        TitleSelect = "Seleccione un tipo de Vehículo";
                        if(vehiculo != null)
                        {
                            this.SelectTipoVehiculos = tipoVehiculos[tipoVehiculos.FindIndex(x => x.IdTipoVehiculo == vehiculo.IdTipoVehiculo)];
                        }
                    }   
                }
            }
            
        }
        #endregion

        #region Propiedades
        public List<TipoVehiculo> TipoVehiculos
        {
            get
            {
                return tipoVehiculos;
            }
            set
            {
                
                if (tipoVehiculos != value)
                {
                    
                    tipoVehiculos = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(TipoVehiculos)));
                    

                }
            }
        }
        public string TitleSelect
        {
            get
            {
                return _titleSelect;
            }
            set
            {
                if(_titleSelect != value)
                {
                    _titleSelect = value;
                    PropertyChanged?.Invoke(
                        this,
                         new PropertyChangedEventArgs(nameof(TitleSelect)));
                }
            }
        }

        public string btnTitle { get; set; }

        public int IdVehiculo { get; set; }

        public int IdTipoVehiculo { get; set; }

        public int IdUsuario { get; set; }

        public string Placa { get; set; }

        public string Modelo { get; set; }

        public string Marca { get; set; }

        public string Color { get; set; }

        public string Foto { get; set; }

        public TipoVehiculo SelectTipoVehiculos
        {
            get
            {
                return _tipoVehiculo;
            }
            set
            {
                if(_tipoVehiculo != value)
                {
                    _tipoVehiculo = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(SelectTipoVehiculos)));
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

        public bool IsEnabledSelect
        {
            get
            {
                return _isEnabledSelect;
            }
            set
            {
                if (_isEnabledSelect != value)
                {
                    _isEnabledSelect = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsEnabledSelect)));
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

        

        #endregion

        #region Servicios
        ApiService apiService;
        DialogService dialogService;
        NavigationService navigationService;
        #endregion

        #region Sigleton
        static AccionVehiculoViewModel instance;

        public static AccionVehiculoViewModel GetInstance()
        {
            return instance;
        }
        #endregion


    }
}
