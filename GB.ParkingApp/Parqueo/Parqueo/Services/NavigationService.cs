
using Parqueo.View;
using Parqueo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Parqueo.Services
{
   public class NavigationService
    {

        public async Task NavigateOnMaster(string pageName)
        {
            App.Master.IsPresented = false;

            switch (pageName)
            {
                case "PerfilView":
                    var mainViewModel = MainViewModel.GetInstance();
                    mainViewModel.Perfil = new PerfilViewModel(mainViewModel.Usuario);
                    await App.Navigator.PushAsync(new PerfilView());
                    break;

                case "VehiculoView":
                    var mainViewModel1 = MainViewModel.GetInstance();
                    mainViewModel1.Carros = new VehiculoViewModel(mainViewModel1.Vehiculos);
                    await App.Navigator.PushAsync(new VehiculoView());
                    break;

                case "AccionVehiculoView":
                    await App.Navigator.PushAsync(new AccionVehiculoView());
                    break;
                    
                case "ObservacionView":
                    MainViewModel.GetInstance().Observations = new ObservacionViewModel();
                    await App.Navigator.PushAsync(new ObservacionView());
                    break;

                case "AccionObservacionView":
                    await App.Navigator.PushAsync(new AccionObservacionView());
                    break;

                case "ParqueoView":
                    MainViewModel.GetInstance().Parqueos = new ParqueoViewModel();
                    await App.Navigator.PushAsync(new ParqueoView());
                    break;

                case "AccionParqueoView":
                    await App.Navigator.PushAsync(new AccionParqueoView());
                    break;

                case "ZonaView":
                    await App.Navigator.PushAsync(new ZonaView());
                    break;

                case "AccionZonaView":
                    await App.Navigator.PushAsync(new AccionZonaView());
                    break;

                case "TipoVehiculoView":
                    MainViewModel.GetInstance().TipoVehiculos = new TipoVehiculoViewModel();
                    await App.Navigator.PushAsync(new TipoVehiculoView());
                        break;

                case "AccionTipoVehiculoView":
                    await App.Navigator.PushAsync(new AccionTipoVehiculoView());
                    break;

                case "FechaReservaView":
                    MainViewModel.GetInstance().FechaReserva = new FechaReservaViewModel();
                    await App.Navigator.PushAsync(new FechaReservaView());
                    break;

                case "ZonaReservaView":
                    await App.Navigator.PushAsync(new ZonaReservaView());
                    break;

                case "ParqueoReservaView":
                    await App.Navigator.PushAsync(new ParqueoReservaView());
                    break;

                case "ReservaView":
                    var mainViewModel2 = MainViewModel.GetInstance();
                    mainViewModel2.Reservas = new ReservaViewModel(mainViewModel2.MisReservas);
                    await App.Navigator.PushAsync(new ReservaView());
                    break;

                case "AccionReservaView":
                    await App.Navigator.PushAsync(new AccionReservaView());
                    break;

                case "LogoutView":
                    SetMainPage("LoginView");
                    break;
                default:
                    break;
            }
        }

        public async Task NavigateOnLogin(string pageName)
        {
            switch (pageName)
            {
                case "RegistroView":
                    await Application.Current.MainPage.Navigation.PushAsync(
                        new RegistroView());
                    break;
            }
        }

        public void SetMainPage(string pageName)
        {
            switch (pageName)
            {
                case "LoginView":
                    Application.Current.MainPage = new NavigationPage(new LoginView());
                    break;
                case "MasterView":
                    Application.Current.MainPage = new MasterView();
                    break;
            }
        }

        public async Task BackOnMaster()
        {
            await App.Navigator.PopAsync();
        }

        public async Task BackOnLogin()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

       

    }
}
