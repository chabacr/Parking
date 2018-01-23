using Parqueo.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Parqueo
{
    public partial class App : Application
    {
        #region Propiedades
        public static NavigationPage Navigator { get; internal set; }

        public static MasterView Master { get; internal set; }

        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginView());
        }
        #endregion

        #region Metodos

        protected override void OnStart()
        {
            // Handle when your app starts
        }


        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }


        protected override void OnResume()
        {
            // Handle when your app resumes
        } 
        #endregion

    }
}
