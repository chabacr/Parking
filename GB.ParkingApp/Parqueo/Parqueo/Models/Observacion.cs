using GalaSoft.MvvmLight.Command;
using Parqueo.Services;
using Parqueo.ViewModels;
using System.Windows.Input;

namespace Parqueo.Models
{
    public class Observacion
    {
        #region Services
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades
        public int IdObservacion { get; set; }
        public string Descripcion { get; set; }
        #endregion

        #region Constructor
        public Observacion()
        {
            navigationService = new NavigationService();
            dialogService = new DialogService();
        }
        #endregion

        #region Methods
        public override int GetHashCode()
        {
            return IdObservacion;
        }
        #endregion

        #region Commands
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }

        async void Delete()
        {
            var response = await dialogService.ShowConfirm(
                "Confirmar",
                "¿Estás seguro de borrar el registro " + this.Descripcion + "?");
            if (!response)
            {
                return;
            }

            await ObservacionViewModel.GetInstance().Delete(this);
        }


        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }

        async void Edit()
        {
            MainViewModel.GetInstance().ActionObservation = new AccionObservacionViewModel(this,"Actualizar");
            
            await navigationService.NavigateOnMaster("AccionObservacionView");
        }

        #endregion
    }
}
