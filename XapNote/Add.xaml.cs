using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Text;
using System.IO;

namespace XapNote
{
    public partial class Add : PhoneApplicationPage
    {
        private string location = "";
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        public Add()
        {
            InitializeComponent();

            GeoCoordinateWatcher myWatcher = new GeoCoordinateWatcher();

            var myPosition = myWatcher.Position;

            double latitude = 47.674;
            double longitude = -122.12;

            if (!myPosition.Location.IsUnknown)
            {
                latitude = myPosition.Location.Latitude;
                longitude = myPosition.Location.Longitude;
            }

            myTerraService.TerraServiceSoapClient client = new myTerraService.TerraServiceSoapClient();
            client.ConvertLonLatPtToNearestPlaceCompleted += new EventHandler<myTerraService.ConvertLonLatPtToNearestPlaceCompletedEventArgs>(client_ConvertLonLatPtToNearestPlaceCompleted);
            client.ConvertLonLatPtToNearestPlaceAsync(new myTerraService.LonLatPt() { Lat = latitude, Lon = longitude });
        }

        void client_ConvertLonLatPtToNearestPlaceCompleted(object sender, myTerraService.ConvertLonLatPtToNearestPlaceCompletedEventArgs e)
        {
            location = e.Result;
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            if (location.Trim().Length == 0)
            {
                location = "Unknow";
            }
            
            // Construct the name of the file
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_"));
            location = location.Replace(" ", "-");
            location = location.Replace(", ", "_");
            sb.Append(location);
            sb.Append(".txt");

            // Write file into isolatedStorage
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var fileStream = appStorage.OpenFile(sb.ToString(), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.WriteLine(editTextBox.Text);
                }
            }

            navigateBack();
        }

        private void AppBar_Cancel_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void navigateBack()
        {
            // Reset state in appSettings
            settings["state"] = "";
            settings["content"] = "";

            NavigationService.GoBack();
            //NavigationService.Navigate(new Uri("/XapNote;component/MainPage.xaml",UriKind.Relative));
            //NavigationService.RemoveBackEntry();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string state = "";

            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "add")
                    {
                        string content = "";
                        if (settings.Contains("content"))
                        {
                            if (settings.TryGetValue<string>("content", out content))
                            {
                                editTextBox.Text = content;
                            }
                        }
                    }
                }
            }

            settings["state"] = "add";
            settings["content"] = editTextBox.Text;
            editTextBox.Focus();
            editTextBox.SelectionStart = editTextBox.Text.Length;
            //string content;
            //if (settings.TryGetValue<string>("content", out content))
            //{
            //    editTextBox.Text = content;
            //    settings.Clear();
            //}
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            //base.OnNavigatingFrom(e);
            //IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            //Uri uri = new Uri(NavigationService.CurrentSource.ToString());
            //string content = editTextBox.Text;
            //if (settings.TryGetValue<Uri>("currentUri", out uri))
            //{
            //    settings["currentUri"] = uri;
            //}
            //else
            //{
            //    settings.Add("currentUri", NavigationService.CurrentSource);
            //}

            //if (settings.TryGetValue<string>("content", out content))
            //{
            //    settings["content"] = content;
            //}
            //else
            //{
            //    settings.Add("content", content);
            //}
        }

        private void editTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings["content"] = editTextBox.Text;
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            settings["state"] = "";
            settings["content"] = "";
        }
    }
}