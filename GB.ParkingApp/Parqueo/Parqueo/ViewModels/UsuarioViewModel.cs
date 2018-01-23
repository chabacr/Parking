using Parqueo.Models;
using Parqueo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parqueo.ViewModels
{
    public class UsuarioViewModel:INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        ApiService apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        List<Usuario> usuarios;
        ObservableCollection<Usuario> _usuarios;
        bool _isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<Usuario> Usuarios
        {
            get
            {
                return _usuarios;
            }
            set
            {
                if (_usuarios != value)
                {
                    _usuarios = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(Usuarios)));
                }
            }
        }

        public bool IsRefreshing
        {
            get
            {
                return _isRefreshing;
            }
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(
                        this,
                        new PropertyChangedEventArgs(nameof(IsRefreshing)));
                }
            }
        }
        #endregion

        #region Constructor
        public UsuarioViewModel()
        {
            instance = this;
            

            apiService = new ApiService();
            dialogService = new DialogService();
            
        }
        #endregion

        #region Sigleton
        static UsuarioViewModel instance;

        public static UsuarioViewModel GetInstance()
        {
            if(instance == null)
            {
                return new UsuarioViewModel();
            }
            return instance;
        }
        #endregion

        #region Methods
        public void Add(Usuario usuario)
        {
            IsRefreshing = true;
            usuarios.Add(usuario);
            Usuarios = new ObservableCollection<Usuario>(
                usuarios.OrderBy(u => u.Nombre));
            IsRefreshing = false;
        }

        //public void Update(Product product)
        //{
        //    IsRefreshing = true;
        //    var oldProduct = products
        //        .Where(p => p.ProductId == product.ProductId)
        //        .FirstOrDefault();
        //    oldProduct = product;
        //    Products = new ObservableCollection<Product>(
        //        products.OrderBy(c => c.Description));
        //    IsRefreshing = false;
        //}

        //public async Task Delete(Product product)
        //{
        //    IsRefreshing = true;

        //    var connection = await apiService.CheckConnection();
        //    if (!connection.IsSuccess)
        //    {
        //        IsRefreshing = false;
        //        await dialogService.ShowMessage("Error", connection.Message);
        //        return;
        //    }

        //    var mainViewModel = MainViewModel.GetInstance();

        //    var response = await apiService.Delete(
        //        "http://productszuluapi.azurewebsites.net",
        //        "/api",
        //        "/Products",
        //        mainViewModel.Token.TokenType,
        //        mainViewModel.Token.AccessToken,
        //        product);

        //    if (!response.IsSuccess)
        //    {
        //        IsRefreshing = false;
        //        await dialogService.ShowMessage(
        //            "Error",
        //            response.Message);
        //        return;
        //    }

        //    products.Remove(product);
        //    Products = new ObservableCollection<Product>(
        //        products.OrderBy(c => c.Description));

        //    IsRefreshing = false;
        //}
        #endregion
    }
}
