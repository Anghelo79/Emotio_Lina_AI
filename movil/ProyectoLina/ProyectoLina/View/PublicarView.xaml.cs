using ProyectoLina.Database;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoLina.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublicarView : ContentView
    {
        private string KeyId { set; get; }

        public PublicarView(string id)
        {
            InitializeComponent();
            InitApp();
            KeyId = id;
        }
        readonly FirebasePublicationHelper firebasePublicationHelper = new FirebasePublicationHelper();
        void InitApp()
        {
            List<String> emotions = new List<string>();
            emotions.Add("ENOJADO");
            emotions.Add("DISGUSTADO");
            emotions.Add("TEMEROSO");
            emotions.Add("FELIZ");
            emotions.Add("NEUTRAL");
            emotions.Add("TRISTE");
            emotions.Add("SORPRENDIDO");
            foreach (var emotion in emotions)
            {
                pickerEmotion.Items.Add(emotion);
            }
            List<String> ages = new List<string>();
            ages.Add("Menor a 10");
            ages.Add("Entre 10 a 18");
            ages.Add("Entre 19 a 30");
            ages.Add("Entre 31 a 50");
            ages.Add("Mayores a 50");
            foreach (var age in ages)
            {
                pickerAgeRange.Items.Add(age);
            }
        }

        private bool IsFormaVald() => IsEmotionVald() && IsAgeRangeVald() && IsPublicationVald();
        private bool IsEmotionVald() => pickerEmotion.SelectedIndex != -1;
        private bool IsAgeRangeVald() => pickerAgeRange.SelectedIndex != -1;
        private bool IsPublicationVald() => !string.IsNullOrWhiteSpace(txtpublicacion.Text);

        private async void BtnPublicar_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!IsFormaVald())
                {
                    await App.Current.MainPage.DisplayAlert("Error", "No se llenó un Campo", "OK");
                    return;
                }
                string Publication = txtpublicacion.Text.Trim();
                string Emotion = pickerEmotion.SelectedItem.ToString();
                string AgeRange = pickerAgeRange.SelectedItem.ToString();
                await firebasePublicationHelper.AgregarPublicasion(KeyId, Publication, Emotion, AgeRange);
                await App.Current.MainPage.DisplayAlert("Confirmación", "Se publicó correctamente", "OK");
                pickerEmotion.SelectedIndex = -1;
                pickerAgeRange.SelectedIndex = -1;
                txtpublicacion.Text = string.Empty;
            }
            catch (Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo publicar", "OK");
            }
        }
    }
}