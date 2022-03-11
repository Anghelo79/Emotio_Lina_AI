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
    public partial class VerificarCorro : ContentPage
    {
        public string Nombre, Apellido, CI, fecha, Institucion, NroMantricula;

        private async void btnVerificarSend_Clicked(object sender, EventArgs e)
        {

            try
            {
                if (!IsFormaVald())
                {
                    await DisplayAlert("Error", "No se puso un Correo electrónico", "OK");
                    return;
                }
                var usercexirt = await firebaseuserhelper.ExitUserCorreo(txtCorreo.Text.ToUpper().Trim());

                if (usercexirt != null)
                {
                    await DisplayAlert("Error", "El correo electrónico ya fue Registrado", "OK");
                    await DisplayAlert("Error", "Utilice otro correo electrónico", "OK");
                    return;
                }
                string sujet = Nombre;
                AddressEmail.SendEmail(txtCorreo.Text.Trim().ToString(), sujet);
                await DisplayAlert("Confirmación", "Se envió correctamente su código a su correo electrónico", "OK");
                StaclayautConfirmate.IsVisible = true;
                btnVerificarSend.Text = "Volver a enviar correo";
                txtCorreo.IsEnabled = false;
                btnCanselar.IsVisible = true;
            }
            catch (Exception ex)
            {

                await DisplayAlert("Error", "Su correo electrónico no exite o use otro", "OK");
                return;
            }
        }

        readonly AddressEmail AddressEmail = new AddressEmail();
        readonly FirebaseUserHelper firebaseuserhelper = new FirebaseUserHelper();
        public VerificarCorro(string Nombre, string Apellido, string CI, string fecha, string Institucion, string NroMantricula)
        {
            InitializeComponent();
            this.Nombre = Nombre;
            this.Apellido = Apellido;
            this.CI = CI;
            this.fecha = fecha;
            this.Institucion = Institucion;
            this.NroMantricula = NroMantricula;
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#7B97D7");

        }

        private void btnCanselar_Clicked(object sender, EventArgs e)
        {
            StaclayautConfirmate.IsVisible = false;
            txtCorreo.IsEnabled = true;
            btnCanselar.IsVisible = false;
            btnVerificarSend.IsVisible = true;
            btnVerificarSend.Text = "Confirmar";
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
            if (!IsFormapassVald())
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
            if (!IsFormapassVald())
            {
                await DisplayAlert("Error", "No tiene que dejar vacío la contraseña", "OK");
                return;
            }
            string FechaNacimineto = fecha;
            string Correo = txtCorreo.Text.Trim();
            string Password = txtPasswor.Text.Trim();
            await firebaseuserhelper.AgregarUsuario(Nombre.ToUpper(), Apellido.ToUpper(), FechaNacimineto, CI, NroMantricula.ToUpper(), Institucion.ToUpper(), Correo.ToUpper(), Password);
            await Navigation.PushModalAsync(new MainPage());
        }


    }
}