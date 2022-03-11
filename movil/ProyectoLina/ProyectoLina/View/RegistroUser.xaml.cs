using ProyectoLina.Commun;
using ProyectoLina.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoLina.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistroUser : ContentPage
    {
        public RegistroUser()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#7B97D7");
        }
        readonly FirebaseUserHelper firebaseuserhelper = new FirebaseUserHelper();
        readonly FirebaseDoctorHelper firebaseDoctorHelper = new FirebaseDoctorHelper();
        public bool verificar =false, verificar2=false, verificar3 =false;
        private async void Button_Clicked(object sender, EventArgs e)
        {

            string dia, mes, year, fecha;
            dia = DatePikerFNacimineto.Date.Day.ToString();
            mes = DatePikerFNacimineto.Date.Month.ToString();
            year = DatePikerFNacimineto.Date.Year.ToString();
            fecha = dia + "-" + mes + "-" + year;

            try
            {
                if (!IsFormaVald())
                {
                    await DisplayAlert("Error", "No se llenó un Campo", "OK");
                    return;
                }
                if (DatePikerFNacimineto.Date >= DateTime.Now.Date)
                {
                    await DisplayAlert("Error", "Fecha de nacimiento no valido", "OK");
                    return;
                }
                if (DatePikerFNacimineto.Date.AddYears(18) >= DateTime.Today)
                {
                    await DisplayAlert("Error", "No es mayor de edad", "OK");
                    return;
                }
                var usernexit = await firebaseuserhelper.ExitUserNombre(txtNombre.Text.ToUpper().Trim());
                var userpexit = await firebaseuserhelper.ExitUserApellido(txtApellido.Text.ToUpper().Trim());
                var usernexirt = await firebaseuserhelper.ExitUserMatricula(txtNroMatricula.Text.ToUpper().Trim());
                if (usernexit != null || userpexit != null || usernexirt != null)
                {
                    await DisplayAlert("Error", "Ya existe la  persona registrada  ", "OK");
                    return;
                }

                string Nombre = txtNombre.Text.ToUpper().Trim();
                string Apellido = txtApellido.Text.ToUpper().Trim();
                string CI = txtCi.Text.ToUpper().Trim();
                string FechaNacimineto = fecha;
                string Institucion = txtIstitusion.Text.ToUpper().Trim();
                string NroMantricula = txtNroMatricula.Text.ToUpper().Trim();
                await Navigation.PushModalAsync(new VerificarCorro(Nombre, Apellido, CI, fecha, Institucion, NroMantricula));
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "No se pudo registrar", "OK");
            }

        }

        private bool IsFormaVald() => IsNomnbreVald() && IsApellidoVald() &&
        IsCiVall() && IsIstitucionVall() && IsNroMatriculaVall();
        private bool IsNomnbreVald() => !string.IsNullOrWhiteSpace(txtNombre.Text);
        private bool IsApellidoVald() => !string.IsNullOrWhiteSpace(txtApellido.Text);
        private bool IsCiVall() => !string.IsNullOrWhiteSpace(txtCi.Text);
        private bool IsIstitucionVall() => !string.IsNullOrWhiteSpace(txtIstitusion.Text);
        private bool IsNroMatriculaVall() => !string.IsNullOrWhiteSpace(txtNroMatricula.Text);

        private async void txtNombre_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var m = await firebaseDoctorHelper.GetNombreM(txtNombre.Text.ToUpper().Trim());
                if (m != null)
                {
                    frNombre.BorderColor = Color.FromHex("#6C75B0");
                    frNombre.BackgroundColor = Color.FromHex("#8BA5DF");
                    verificar = true;
                }
                else
                {
                    frNombre.BorderColor = Color.FromHex("#6F2B32");
                    frNombre.BackgroundColor = Color.FromHex("#CA3846");
                }
                Veficasionbuton(verificar, verificar2, verificar3);
            }
            catch (Exception)
            {

                await DisplayAlert("Error", "Debe tener conexión a internet", "OK");
                return;
            }


        }

        private async void txtApellido_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var m = await firebaseDoctorHelper.GetApelidoM(txtApellido.Text.ToUpper().Trim());
                if (m != null)
                {
                    frApellido.BorderColor = Color.FromHex("#6C75B0");
                    frApellido.BackgroundColor = Color.FromHex("#8BA5DF");
                    verificar2 = true;
                }
                else
                {
                    frApellido.BorderColor = Color.FromHex("#6F2B32");
                    frApellido.BackgroundColor = Color.FromHex("#CA3846");
                }
                Veficasionbuton(verificar, verificar2, verificar3);
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Debe tener conexión a internet", "OK");
                return;
            }


        }

        private async void txtNroMatricula_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var m = await firebaseDoctorHelper.GetMatricula(txtNroMatricula.Text.ToUpper().Trim());
                if (m != null)
                {
                    frNroMatricula.BorderColor = Color.FromHex("#6C75B0");
                    frNroMatricula.BackgroundColor = Color.FromHex("#8BA5DF");
                    verificar3 = true;
                }
                else
                {
                    frNroMatricula.BorderColor = Color.FromHex("#6F2B32");
                    frNroMatricula.BackgroundColor = Color.FromHex("#CA3846");
                }

                Veficasionbuton(verificar, verificar2, verificar3);
            }
            catch (Exception)
            {

                await DisplayAlert("Error", "Debe tener conexión a internet", "OK");
                return;
            }

        }
        public void Veficasionbuton(bool uno, bool dos, bool tres)
        {
            if (uno==true && dos==true && tres==true)
            {
                BtnNext.IsVisible = true;
            }
        }
    }
}