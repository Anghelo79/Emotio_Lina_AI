using ProyectoLina.Commun;
using ProyectoLina.Database;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoLina.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublicacionesView : ContentView
    {
        private string KeyIdU { set; get; }
        public PublicacionesView(string id)
        {
            InitializeComponent();
            KeyIdU = id;
            InitApp();
        }
        readonly FirebasePublicationHelper firebasePublicationHelper = new FirebasePublicationHelper();
        private async void InitApp()
        {
            try
            {
                List<Publicacion> ListPublicaciones = await firebasePublicationHelper.GetAllPublications(KeyIdU);
                foreach (var publicacion in ListPublicaciones)
                {
                    publicacionesStack.Children.Add(new CardView(publicacion));
                }
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo cargar las publicaciones", "OK");
                return;
            }
        }
    }
}