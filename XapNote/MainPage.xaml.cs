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
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void AppBar_Add_CLick(object sender, EventArgs e)
        {
            NavigationService.Navigate(
                new Uri(
                    "/XapNote;component/Add.xaml", 
                    UriKind.Relative
                ));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "add")
                    {
                        NavigationService.Navigate(
                            new Uri(
                                "/XapNote;component/Add.xaml",
                                UriKind.Relative
                            ));
                    }
                    else if(state == "edit")
                    {
                        NavigationService.Navigate(
                            new Uri(
                                "/XapNote;component/ViewEdit.xaml", 
                                UriKind.Relative
                            ));
                    }
                }
            }
            BindList();
        }

        private void BindList()
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            string[] fileList = appStorage.GetFileNames();

            List<Note> notes = new List<Note>();

            try
            {
                foreach (string file in fileList)
                {
                    if (file != "__ApplicationSettings")
                    {
                        string fileName = file;
                        string year = file.Substring(0, 4);
                        string month = fileName.Substring(5, 2);
                        string day = fileName.Substring(8, 2);
                        string hour = fileName.Substring(11, 2);
                        string minute = fileName.Substring(14, 2);
                        string second = fileName.Substring(17, 2);

                        DateTime dateCreate = 
                            new DateTime(
                                int.Parse(year), 
                                int.Parse(month), 
                                int.Parse(day),
                                int.Parse(hour), 
                                int.Parse(minute), 
                                int.Parse(second)
                            );

                        string location = file.Substring(20);
                        location = location.Replace("_", ", ");
                        location = location.Replace("-", " ");
                        location = location.Substring(0, location.Length - 4);

                        notes.Add(new Note()
                        {
                            Location = location,
                            DateCreated = dateCreate.ToLongDateString(),
                            FileName = fileName
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            noteListBox.ItemsSource = notes;
        }

        private void noteLocation_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton clickLink = (HyperlinkButton)sender;
            string uri = 
                String.Format(
                    "/XapNote;component/ViewEdit.xaml?id={0}",
                    clickLink.Tag
                );
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }

        private void AppBar_Help_CLick(object sender, EventArgs e)
        {
            helpCanvas.Visibility = Visibility.Visible;
        }

        private void helpCloseButton_Click(object sender, RoutedEventArgs e)
        {
            helpCanvas.Visibility = Visibility.Collapsed;
        }
    }
}