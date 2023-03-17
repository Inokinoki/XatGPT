using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace XatGPT
{	
	public partial class ChatConversationPage : ContentPage
	{
		private ChatConversation refChatConversation;

		public ChatConversationPage(ChatConversation conversation)
		{
			InitializeComponent();

			this.refChatConversation = conversation;
			this.ChatTitleLabel.Text = conversation.Title;
			this.ChatList_Header.Text = conversation.SystemMessage;
        }

        void ChatSettingButton_Clicked(System.Object sender, System.EventArgs e)
        {
			// TODO
        }

        async void ChatList_Refreshing(System.Object sender, System.EventArgs e)
		{
			// TODO: Load the messages
		}
    }
}

