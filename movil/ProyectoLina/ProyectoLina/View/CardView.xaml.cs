using ProyectoLina.Commun;
using ProyectoLina.Database;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoLina.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardView : ContentView
    {
        private Publicacion Publicacion { set; get; }
        private bool mostrar = true;
        public CardView(Object send)
        {
            InitializeComponent();
            Publicacion = (Publicacion)send;
            InitApp();
        }
        readonly FirebasePublicationHelper firebasePublicationHelper = new FirebasePublicationHelper();
        private async void InitApp()
        {
            try
            {
                lblEmocion.Text = Publicacion.TipoEstadoAnimo;
                lblRangoEdad.Text = Publicacion.RangoEdad;
                if (Publicacion.Descripcion.Length >= 50)
                {
                    btnMostrarOcultar.IsVisible = true;
                    lblPublicacion.Text = Publicacion.Descripcion.Substring(0,49) + "...";
                }
                else
                {
                    lblPublicacion.Text = Publicacion.Descripcion;
                }
                lblVistas.Text = Publicacion.Vistas;
                lblMeGusta.Text = Publicacion.Megusta;
                lblNoMeGusta.Text = Publicacion.NoMegusta;
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo cargar la publicación", "OK");
                return;
            }
        }

        private void btnMostrarOcultar_Clicked(object sender, EventArgs e)
        {
            if (mostrar)
            {
                btnMostrarOcultar.RotateTo(-180);
                lblPublicacion.Text = Publicacion.Descripcion;
            }
            else
            {
                btnMostrarOcultar.RotateTo(0);
                lblPublicacion.Text = Publicacion.Descripcion.Substring(0,49) + "...";
            }
            mostrar = !mostrar;
        }

        private async void btnBorrar_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool answer = await App.Current.MainPage.DisplayAlert("Eliminar", "¿Seguro que quiere eliminar esta publicación?", "Si", "No");
                if (answer)
                {
                    await firebasePublicationHelper.DelatePublicasion(Publicacion.IdKeyP);
                    this.IsVisible = false;
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la publicación", "OK");
                return;
            }
        }
    }
}