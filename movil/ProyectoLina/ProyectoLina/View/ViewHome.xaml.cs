using ProyectoLina.Commun;
using ProyectoLina.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Markup;
using Xamarin.Forms.Xaml;

namespace ProyectoLina.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewHome : ContentPage
    {
        public ViewHome(string idkey)
        {
            InitializeComponent();
            Useridentity(idkey);
        }
        readonly FirebaseUserHelper firebaseuserhelper = new FirebaseUserHelper();
        private List<Usuario> ListUsario = new List<Usuario>();
        private Usuario usuario;
        private double previousScrollPosition = 0;
        private async void Useridentity(string id)
        {
            try
            {
                usuario = new Usuario();
                ListUsario = await firebaseuserhelper.GetUserAll(id);
                usuario.KeyId = ListUsario.FirstOrDefault().KeyId.ToString();
                usuario.Nombre = ListUsario.FirstOrDefault().Nombre.ToString();
                labetTitle.Text = usuario.Nombre;
                publicacionesView();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void publicacionesView()
        {
            stackView.Children.Clear();
            stackView.Children.Add(new PublicacionesView(usuario.KeyId));
        }

        private void Handle_Scrolled(object sender, ScrolledEventArgs e)
        {
            previousScrollPosition = MyScrollView.ContentSize.Height - MyScrollView.Height;
            double buffer = 32;
            stMenu.IsVisible = previousScrollPosition > e.ScrollY + buffer;
        }

        private void btnNuevaPublicacion_Clicked(object sender, EventArgs e)
        {
            stackView.Children.Clear();
            stackView.Children.Add(new PublicarView(usuario.KeyId));
        }

        private void btnPublicado_Clicked(object sender, EventArgs e)
        {
            publicacionesView();
        }
    }
}