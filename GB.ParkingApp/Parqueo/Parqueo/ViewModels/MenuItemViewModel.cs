using GalaSoft.MvvmLight.Command;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Parqueo.ViewModels
{
   public class MenuItemViewModel
    {
        #region Atributos



        #endregion

        #region Servicios
        private NavigationService navigationService;
        #endregion

        #region Propiedades
        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }
        #endregion

        #region Constructores

        public MenuItemViewModel()
        {
            navigationService = new NavigationService();
        }
        #endregion

        #region Comandos
        public ICommand NavigateCommand
        {
            get
            {
                return new RelayCommand(Navigate);
            }
        }

        private async void Navigate()
        {
            await navigationService.NavigateOnMaster(PageName);
        }
        #endregion
    }
}
