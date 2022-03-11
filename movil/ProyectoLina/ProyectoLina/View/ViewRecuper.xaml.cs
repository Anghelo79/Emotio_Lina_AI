using ProyectoLina.Commun;
using ProyectoLina.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoLina.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewRecuper : ContentPage
    {
        
        readonly AddressEmail AddressEmail = new AddressEmail();
        readonly FirebaseUserHelper firebaseuserhelper = new FirebaseUserHelper();
        public ViewRecuper()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#7B97D7");
        }
        List<Usuario> usuariolist = new List<Usuario>();
        string idkeyuser;
        public string Nombre, Apellido, CI, fecha, Institucion, NroMantricula,Correo;
        private async void btnVerificarSend_Clicked(object sender, EventArgs e)
        {
            try
            {
                string sujet;
               
                if (!IsFormaVald())
                {
                    await DisplayAlert("Error", "No se puso un Correo electrónico", "OK");
                    return;
                }
                usuariolist = await firebaseuserhelper.EmailRecuperait(txtCorreo.Text.ToUpper().Trim());
                if (usuariolist.Count == 1)
                {
                    idkeyuser = usuariolist.FirstOrDefault().KeyId;
                    Nombre = usuariolist.FirstOrDefault().Nombre;
                    Apellido = usuariolist.FirstOrDefault().Apellido;
                    CI = usuariolist.FirstOrDefault().CI;
                    fecha = usuariolist.FirstOrDefault().FechaNacimineto;
                    Institucion = usuariolist.FirstOrDefault().Institucion;
                    NroMantricula = usuariolist.FirstOrDefault().NroMantricula;
                    Correo= usuariolist.FirstOrDefault().Correo;
                    sujet = usuariolist.FirstOrDefault().Nombre;
                }
                else
                {
                    await DisplayAlert("Error", "Su correo electronico no enta registrado en nuetros servicios", "OK");
                    return;
                }

            
                AddressEmail.SendEmail(txtCorreo.Text.Trim().ToString(), sujet);
                await DisplayAlert("Confirmación", "Se envió correctamente su código a su correo", "OK");
                StaclayautConfirmate.IsVisible = true;
                btnVerificarSend.Text = "Volver a enviar correo";
                txtCorreo.IsEnabled = false;
                btnCanselar.IsVisible = true;
               
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", "Su comreo electronico no exite", "OK");
                return;
            }

        }
        private bool IsFormaVald() => IsEmailVald();
        private bool IsEmailVald() => !string.IsNullOrWhiteSpace(txtCorreo.Text);

        private void txtCodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (AddressEmail.verifycode(int.Parse(txtCodigo.Text.Trim())) == true)
                {
                    btnCanselar.IsVisible = true;
                    StaclayautConfirmate.IsVisible = false;
                    Staclayautpasswort.IsVisible = true;
                }

            }
            catch (Exception)
            {


            }
        }

        private void txtPasswor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Regex.IsMatch(txtPasswor.Text.Trim(), @"^(?=.{8,}$)(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*\W).*$"))
                {
                    msgPassword.IsVisible = false;
                    frPassword.BorderColor = Color.FromHex("#6C75B0");
                    frPassword.BackgroundColor = Color.FromHex("#8BA5DF");
                }
                else
                {
                    msgPassword.IsVisible = true;
                    frPassword.BorderColor = Color.FromHex("#6F2B32");
                    frPassword.BackgroundColor = Color.FromHex("#CA3846");
                }
            }
            catch (Exception)
            {


            }
        }

        private async void txtPassworVeri_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!IsFormapassVald())
            {
                await DisplayAlert("Error", "No se puede dejar vacío la contraseña", "OK");
                return;
            }
            if (txtPasswor.Text.Trim() == txtPassworVeri.Text.Trim())
            {
                btnFinis.IsVisible = true;
                frPasswordVeri.BorderColor = Color.FromHex("#6C75B0");
                frPasswordVeri.BackgroundColor = Color.FromHex("#8BA5DF");
            }
            else
            {
                frPasswordVeri.BorderColor = Color.FromHex("#6F2B32");
                frPasswordVeri.BackgroundColor = Color.FromHex("#CA3846");
            }
        }
        private bool IsFormapassVald() => IsPaswordVald() && IsPasword();
        private bool IsPaswordVald() => !string.IsNullOrWhiteSpace(txtPassworVeri.Text);
        private bool IsPasword() => !string.IsNullOrWhiteSpace(txtPasswor.Text);
        private async void btnFinis_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!IsFormapassVald())
                {
                    await DisplayAlert("Error", "No se puede dejar vacío la contraseña", "OK");
                    return;
                }
                await firebaseuserhelper.UpdatePaswword(idkeyuser, Nombre, Apellido, fecha, CI, NroMantricula, Institucion,Correo
                    ,txtPasswor.Text.Trim());
                await DisplayAlert("Confirmación", "Su contraseña se cambio correctamente", "OK");
                await Navigation.PushModalAsync(new MainPage());

            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", "Asegúrese de estar conectado a internet", "OK");
                return;
            }
        }

        private void btnCanselar_Clicked(object sender, EventArgs e)
        {
            StaclayautConfirmate.IsVisible = false;
            txtCorreo.IsEnabled = true;
            btnCanselar.IsVisible = false;
            btnVerificarSend.IsVisible = true;
            btnVerificarSend.Text = "Confirmar";
        }
    }
}