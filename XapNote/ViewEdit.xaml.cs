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
using System.IO.IsolatedStorage;
using System.IO;

namespace XapNote
{
    public partial class ViewEdit : PhoneApplicationPage
    {
        private string fileName = "";

        public ViewEdit()
        {
            InitializeComponent();
        }

        private void AppBar_Back_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void AppBar_Edit_Click(object sender, EventArgs e)
        {
            if (displayTextBlock.Visibility==Visibility.Visible)
            {
                editTextBox.Text = displayTextBlock.Text;
                displayTextBlock.Visibility = Visibility.Collapsed;
                editTextBox.Visibility = Visibility.Visible;
            }
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            if (editTextBox.Visibility == Visibility.Visible)
            {
                // Save 
                var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
                using (var file = appStorage.OpenFile(fileName,FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        sw.WriteLine(editTextBox.Text);
                    }
                }

                displayTextBlock.Text = editTextBox.Text;
                editTextBox.Visibility = Visibility.Collapsed;
                displayTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void AppBar_Delete_Click(object sender, EventArgs e)
        {
            confirmDialog.Visibility = Visibility.Visible;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string context;
            if (settings.TryGetValue<string>("context", out context))
            {
                displayTextBlock.Text = context;
                settings.Clear();
            }
        }

        private void navigateBack()
        {
            NavigationService.GoBack();
            //NavigationService.Navigate(new Uri("/XapNote;component/MainPage.xaml", UriKind.Relative));
            //NavigationService.RemoveBackEntry();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            confirmDialog.Visibility = Visibility.Collapsed;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            appStorage.DeleteFile(fileName);
            navigateBack();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            fileName = NavigationContext.QueryString["id"];
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            try
            {
                using (var file = appStorage.OpenFile(fileName, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        displayTextBlock.Text = sr.ReadToEnd();
                    }
                }
            }
            catch (IsolatedStorageException ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            Uri uri = new Uri(NavigationService.CurrentSource.ToString());
            string context = displayTextBlock.Text == ""? displayTextBlock.Text:editTextBox.Text;
            if (settings.TryGetValue<Uri>("currentUri", out uri))
            {
                settings["currentUri"] = uri;

            }
            else
            {
                settings.Add("currentUri", NavigationService.CurrentSource);
            }

            if (settings.TryGetValue<string>("context", out context))
            {
                settings["context"] = context;
            }
            else
            {
                settings.Add("context", displayTextBlock.Text == "" ? displayTextBlock.Text : editTextBox.Text);
            }
        }
    }
}