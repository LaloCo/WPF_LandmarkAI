using LandmarkAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Image_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png; *.jpg)|*.png;*.jpg;*jpeg|All files (*.*)|*.*";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if(dialog.ShowDialog()== true)
            {
                string fileName = dialog.FileName;
                selectedImage.Source = new BitmapImage(new Uri(fileName));

                MakePredictionWithImageAsync(fileName);
            }
        }

        private void URL_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(urlTextBox.Text))
            {
                selectedImage.Source = new BitmapImage(new Uri(urlTextBox.Text));
                MakePredictionWithURLAsync(urlTextBox.Text);
            }
        }

        private async void MakePredictionWithImageAsync(string fileName)
        {
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/61d83fd6-464d-4be3-a4b9-1f86d2e5ba5a/classify/iterations/Iteration2/image";
            string prediction_key = "ce456cdb4d0044f58c8706bb588e4d0a";
            string content_type = "application/octet-stream";
            var file = File.ReadAllBytes(fileName);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using (var content = new ByteArrayContent(file))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).Predictions;
                    predictionsListView.ItemsSource = predictions;
                }
            }
        }

        private async void MakePredictionWithURLAsync(string imageUrl)
        {
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/61d83fd6-464d-4be3-a4b9-1f86d2e5ba5a/classify/iterations/Iteration2/url";
            string prediction_key = "ce456cdb4d0044f58c8706bb588e4d0a";
            string content_type = "application/json";
            string body = $"{{\"Url\": \"{imageUrl}\"}}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Prediction-Key", prediction_key);

                using (var content = new StringContent(body, Encoding.UTF8, "application/json"))
                {
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(content_type);
                    var response = await client.PostAsync(url, content);

                    var responseString = await response.Content.ReadAsStringAsync();

                    List<Prediction> predictions = (JsonConvert.DeserializeObject<CustomVision>(responseString)).Predictions;
                    predictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}
