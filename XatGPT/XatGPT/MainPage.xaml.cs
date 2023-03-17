using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XatGPT
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.ChatsList.IsRefreshing = true;
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

        void NewChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            // TODO
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

        void ChatCell_Tapped(System.Object sender, System.EventArgs e)
        {
            // TODO
        }
    }
}

