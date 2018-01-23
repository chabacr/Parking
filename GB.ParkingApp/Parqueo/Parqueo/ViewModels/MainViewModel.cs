using GalaSoft.MvvmLight.Command;
using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Parqueo.ViewModels
{
   public class MainViewModel
    {
        #region Singleton

        static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }

            return instance;
        }

        #endregion
        
        #region Services
        NavigationService navigationService;
        #endregion

        #region Propiedades

        public ObservableCollection<MenuItemViewModel> Menu { get; set; }

        public LoginViewModel Login { get; set; }

        
        public RegistroViewModel NewRegister { get; set; }

        public ObservacionViewModel Observations { get; set; }

        public AccionObservacionViewModel ActionObservation { get; set; }

        public ParqueoViewModel Parqueos { get; set; }
  
        public AccionParqueoViewModel   ActionParking { get; set; }

        public Parking Parking { get; set; }


        public ParqueoReservaViewModel ParqueosReserva { get; set; }

        public AccionZonaViewModel ActionZona { get; set; }

        public ZonaViewModel Zonas { get; set; }

        public Zona Zona { get; set; }

        public ZonaReservaViewModel ZonasReserva { get; set; }

        public TipoVehiculoViewModel TipoVehiculos { get; set; }

        public AccionTipoVehiculoViewModel AccionTipoVehiculo { get; set; }

        public Usuario Usuario { get; set; }

        public PerfilViewModel Perfil { get; set; }

        public List<Permiso> Permisos { get; set; }

        public List<Vehiculo> Vehiculos { get; set; }

        public List<Reserva> MisReservas { get; set; }

        public VehiculoViewModel Carros { get; set; }

        public AccionVehiculoViewModel AccionCarros { get; set; }
        
        public FechaReservaViewModel FechaReserva { get; set; }

        public ReservaViewModel Reservas { get; set; }

        public AccionReservaViewModel AccionReserva { get; set; }
        #endregion

        #region Contructores

        public MainViewModel()
        {
            instance = this;
         

            navigationService = new NavigationService();

            Login = new LoginViewModel();
            
            
        }

        #endregion

        #region Metodos
        public void LoadMenu(List<Permiso> permisos)
        {
            Permisos = permisos;
            Menu = new ObservableCollection<MenuItemViewModel>();
            foreach (var permiso in Permisos)
            {
                Menu.Add(new MenuItemViewModel
                {
                    Icon = permiso.Icono,
                    Title = permiso.Descripcion,
                    PageName = permiso.Pagina,
                });
            }
            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "ic_Perfil.png",
            //    Title = "Mi Perfil",
            //    PageName = "PerfilView",
            //});

            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "ic_Carro.png",
            //    Title = "Mi Vehiculo",
            //    PageName = "CarroView",
            //});

            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "ic_Reservar.png",
            //    Title = "Reservar",
            //    PageName = "ReservaView",
            //});

            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "ic_Sincronizar.png",
            //    Title = "Sincronizar",
            //    PageName = "SincronizarView",
            //});

            Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_Salir.png",
                Title = "Salir",
                PageName = "LogoutView",
            });
            Menu.OrderBy(c => c.Title);
            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "",
            //    Title = "Observaciones",
            //    PageName = "ObservacionView",
            //});

            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "",
            //    Title = "Parqueos",
            //    PageName = "ParqueoView",
            //});

            //Menu.Add(new MenuItemViewModel
            //{
            //    Icon = "",
            //    Title = "Tipo Vehiculos",
            //    PageName = "TipoVehiculoView",
            //});

        }
        #endregion

        #region Comandos
        public ICommand NewCarCommand
        {
            get
            {
                return new RelayCommand(GoNewCar);
            }
        }
        async void GoNewCar()
        {
            AccionCarros = new AccionVehiculoViewModel("Agregar");
            
            await navigationService.NavigateOnMaster("AccionVehiculoView");
        }


        public ICommand NewObservationCommand
        {
            get
            {
                return new RelayCommand(GoNewObservation);
            }
        }
        async void GoNewObservation()
        {
            ActionObservation = new AccionObservacionViewModel();
            ActionObservation.TitleBtn = "Agregar";
            await navigationService.NavigateOnMaster("AccionObservacionView");
        }


        public ICommand NewParkingCommand
        {
            get
            {
                return new RelayCommand(GoNewParking);
            }
        }
        async void GoNewParking()
        {
            ActionParking = new AccionParqueoViewModel();
            ActionParking.TitleBtn = "Agregar";
            await navigationService.NavigateOnMaster("AccionParqueoView");
        }

        public ICommand NewZonaCommand
        {
            get
            {
                return new RelayCommand(GoNewZona);
            }
        }
        async void GoNewZona()
        {
            var mainViewModel = MainViewModel.GetInstance();
            ActionZona = new AccionZonaViewModel();
            ActionZona.TitleBtn = "Agregar";
            mainViewModel.Parking.Descripcion = mainViewModel.Parking.Descripcion;
            await navigationService.NavigateOnMaster("AccionZonaView");
        }

        public ICommand NewTipoVehiculoCommand
        {
            get
            {
                return new RelayCommand(GoNewTipoVehiculo);
            }
        }
        async void GoNewTipoVehiculo()
        {
            AccionTipoVehiculo = new AccionTipoVehiculoViewModel();
            AccionTipoVehiculo.TitleBtn = "Agregar";
            await navigationService.NavigateOnMaster("AccionTipoVehiculoView");
        }
        #endregion
    }
}
