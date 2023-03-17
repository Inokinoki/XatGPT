using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API.Chat;
using Xamarin.Forms;

namespace XatGPT
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.Appearing += OnPageAppearing;

            this.ChatsList.IsRefreshing = true;
        }

        void OnPageAppearing(System.Object sender, System.EventArgs e)
        {
            // Load conversation on appearing
            LoadChatConversations();
        }

        async void LoadChatConversations()
        {
            // Read from sqlite DB
            var dbInstance = await ChatConversationDB.Instance;
            var list =
                await dbInstance.GetItemsAsync();
            this.ChatsList.ItemsSource = list;
            if (list.Count > 0) this.ChatsList_Footer.IsVisible = false;
            else this.ChatsList_Footer.IsVisible = true;

            this.ChatsList.IsRefreshing = false;
        }

        async Task<int> SaveChatConversationToDB(ChatConversation conversation)
        {
            var dbInstance = await ChatConversationDB.Instance;
            return await dbInstance.SaveItemAsync(conversation);
        }

        async void NewChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            // Pop-up an alert to ask the prompt
            string prompt = await DisplayPromptAsync("Create a conversation",
                "What's the prompt?");
            if (prompt == null)
            {
                // The user gives up
                return;
            }
            var conversation = new ChatConversation();
            conversation.SystemMessage = prompt;
            conversation.ModelName = "gpt-3.5-turbo";   // Default is gpt-3.5
            conversation.Title = "Untitled";

            // Insert and wait for the ID updating
            if (await SaveChatConversationToDB(conversation) < 1 ||
                conversation.Id == 0)
            {
                // Update failed
                await DisplayAlert("Create conversation failed",
                    "Please check the storage permission or space", "OK");
                return;
            }

            // Enter the conversation
            await Navigation.PushAsync(new ChatConversationPage(conversation));
        }

        void ChatsList_Refreshing(System.Object sender, System.EventArgs e)
        {
            this.ChatsList.IsRefreshing = true;
            LoadChatConversations();
        }

        void DeleteChatItem_Clicked(System.Object sender, System.EventArgs e)
        {
            // TODO
        }

        void ChatsList_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var conversation = e.Item as ChatConversation;
            // Enter the conversation
            Navigation.PushAsync(new ChatConversationPage(conversation));
        }
    }
}

