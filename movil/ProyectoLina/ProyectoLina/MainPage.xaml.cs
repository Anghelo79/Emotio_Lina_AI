using ProyectoLina.Commun;
using ProyectoLina.Database;
using ProyectoLina.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoLina
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        readonly FirebaseUserHelper firebaseuserhelper = new FirebaseUserHelper();
        public MainPage()
        {
            InitializeComponent();
            btnOnForgotPwd.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnForgotPassword()),
            });
        }
        List<Usuario> ListUsario = new List<Usuario>();
        private void btnRegistrar_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegistroUser());
        }

        private void OnForgotPassword()
        {
           Navigation.PushAsync(new ViewRecuper());
        }

        private async void btnIniaciar_Clicked(object sender, EventArgs e)
        {
            try
            {
                
                if (!IsFormaVald())
                {
                    await DisplayAlert("Error", "Existen campos vacios", "OK");
                    return;
                }
                ListUsario = await firebaseuserhelper.GetAllPersons(txtLogin.Text.ToUpper().Trim(),txtPassword.Text.Trim());
                if (ListUsario.Count==1)
                {
                    string idkey = ListUsario.FirstOrDefault().KeyId;
                  await Navigation.PushModalAsync(new ViewHome(idkey));
                }
                else
                {
                    await DisplayAlert("Error", "Error al ingresar su correo o contraseña", "OK");
                    return;
                }

            }
            catch (Exception ex )
            {
                await DisplayAlert("Error", "Debe tener conexión a internet", "OK");
                return;
            }
          

        }
        private bool IsFormaVald() => IsEmailVald() && IspassVald();
        private bool IsEmailVald() => !string.IsNullOrWhiteSpace(txtLogin.Text);
        private bool IspassVald() => !string.IsNullOrWhiteSpace(txtPassword.Text);
    }
}
