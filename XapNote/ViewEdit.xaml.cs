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
        private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
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
                bindEdit(displayTextBlock.Text);
            }
        }

        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            if (editTextBox.Visibility == Visibility.Visible)
            {
                try
                {
                    // Save 
                    var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    using (var file = appStorage.OpenFile(fileName, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(file))
                        {
                            sw.WriteLine(editTextBox.Text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "edit")
                    {
                        string content = "";

                        if (settings.TryGetValue<string>("fileName", out content))
                        {
                            fileName = content;
                        }

                        if (settings.TryGetValue<string>("content", out content))
                        {
                            bindEdit(content);
                        }
                    }
                    else
                    {
                        bindView();
                    }
                }
            }
            else
            {
                bindView();
            }
        }

        private void bindView()
        {
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

        private void bindEdit(string content)
        {
            editTextBox.Text = content;
            displayTextBlock.Visibility = Visibility.Collapsed;
            editTextBox.Visibility = Visibility.Visible;

            editTextBox.Focus();
            editTextBox.SelectionStart = content.Length;

            settings["state"] = "edit";
            settings["content"] = content;
            if (fileName != "")
            {
                settings["fileName"] = fileName;
            }
            
        }

        private void navigateBack()
        {
            settings["state"] = "";
            settings["content"] = "";
            settings["fileName"] = "";

            NavigationService.GoBack();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            confirmDialog.Visibility = Visibility.Collapsed;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                appStorage.DeleteFile(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            navigateBack();
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
            settings["fileName"] = "";
        }
    }
}