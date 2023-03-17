using System;
using System.Collections.Generic;
using System.Security.Authentication;
using OpenAI_API;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;
namespace XatGPT
{
    public partial class AuthenticationPage : ContentPage
    {	
		public AuthenticationPage ()
		{
			InitializeComponent ();

            // Detect usability of OpenAI API
            DetectOpenAIUsability();
        }

        private static ISettings AppSettings => CrossSettings.Current;

        public static string OpenAIAPIKey
        {
            get => AppSettings.GetValueOrDefault(nameof(OpenAIAPIKey), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(OpenAIAPIKey), value);
        }

        private void RequireToInputOpenAIAPI()
        {
            this.InputOpenAIAPI.Text = OpenAIAPIKey;

            this.InputOpenAIAPIHint.IsVisible = true;
            this.InputOpenAIAPI.IsVisible = true;
            this.InputOpenAIAPI.IsEnabled = true;
            this.ConfirmOpenAIAPI.IsVisible = true;
            this.ConfirmOpenAIAPI.IsEnabled = true;
        }

        private async void DetectOpenAIUsability()
        {
            if (OpenAIAPIKey == string.Empty)
            {
                RequireToInputOpenAIAPI();
                return;
            }

            var auth = new APIAuthentication(OpenAIAPIKey);

            // Verify OpenAI API key
            try
            {
                await new OpenAIAPI(auth).Models.GetModelsAsync();
            }
            catch (AuthenticationException)
            {
                // OpenAI API not valid
                RequireToInputOpenAIAPI();
                await Application.Current.MainPage.DisplayAlert("API Key not valid",
                    "Please input a valid OpenAI API Key", "Ok");

                Console.Out.WriteLine("API not valid");
                return;
            }
            catch (Exception)
            {
                // Maybe network error: offline mode
                bool cont = await Application.Current.MainPage.DisplayAlert(
                    "Offline", "Cannot connect to OpenAI API server",
                    "Offline mode", "Exit");
                if (!cont)
                {
                    // TODO: Exit the application
                }
            }

            Console.Out.WriteLine("API valid");
            // Go to the main page
            Application.Current.MainPage = new MainTabbedPage();
        }

        void confirmOpenAIAPI_Clicked(System.Object sender, System.EventArgs e)
        {
            OpenAIAPIKey = this.InputOpenAIAPI.Text;
            this.InputOpenAIAPI.IsEnabled = false;
            this.ConfirmOpenAIAPI.IsEnabled = false;

            // Retry
            DetectOpenAIUsability();
        }
    }
}

