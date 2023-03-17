using System;
using System.Collections.Generic;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;

namespace XatGPT
{
    public partial class ConfigurationPage : ContentPage
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string OpenAIAPIKey
        {
            get => AppSettings.GetValueOrDefault(nameof(OpenAIAPIKey), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(OpenAIAPIKey), value);
        }

        public ConfigurationPage()
        {
            InitializeComponent();

            this.InputOpenAIAPI.Text = OpenAIAPIKey;
        }

        void ClearButton_Clicked(System.Object sender, System.EventArgs e)
        {
            this.InputOpenAIAPI.Text = "";
            OpenAIAPIKey = string.Empty;
        }

        void SaveButton_Clicked(System.Object sender, System.EventArgs e)
        {
            OpenAIAPIKey = this.InputOpenAIAPI.Text;
        }
    }
}

