using System;
using System.Collections.Generic;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Xamarin.Forms;
namespace XatGPT
{	
	public partial class ChatConversationPage : ContentPage
	{
		private ChatConversation refChatConversation;

        private static ISettings AppSettings => CrossSettings.Current;

        public static string OpenAIAPIKey
        {
            get => AppSettings.GetValueOrDefault(nameof(OpenAIAPIKey), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(OpenAIAPIKey), value);
        }

        private LinkedList<ChatMessage> messages = new LinkedList<ChatMessage>();

        public ChatConversationPage(ChatConversation conversation)
		{
			InitializeComponent();

			this.refChatConversation = conversation;
			this.ChatTitleLabel.Text = conversation.Title;
			this.ChatList_Header.Text = conversation.SystemMessage;

            this.messages.AddLast(
                new ChatMessage("TestTestTestTestTestTestTestTestTestTestTest",
                    ChatMessageRole.User));
            this.messages.AddLast(
                new ChatMessage("Test2Test2Test2Test2Test2Test2Test2Test2Test2",
                    ChatMessageRole.Assistant));
            // Load messages
            LoadMessages();

			// Create a request if necessary
			PrepareRequest();
        }

        public ChatRequest DefaultChatRequestArgs { get; set; } =
            new ChatRequest() { Model = Model.ChatGPTTurbo };

        async void LoadMessages()
        {
            this.messages.Clear();
            var dbInstance = await ChatConversationDB.Instance;
            var msgs = await dbInstance.GetChatMessageFromConversation(this.refChatConversation);
            foreach (var msg in msgs) {
                this.messages.AddLast(msg);
            }

            this.ChatList.IsRefreshing = false;
        }

        void PrepareRequest()
        {
            if (refChatConversation.Request != null) return;

            refChatConversation.Request =
                new ChatRequest(DefaultChatRequestArgs);
            refChatConversation.Request.Temperature =
                refChatConversation.Temperature;
            refChatConversation.Request.Model = refChatConversation.ModelName;
        }

        void ChatSettingButton_Clicked(System.Object sender, System.EventArgs e)
        {
			// TODO: Open a page to configure parameters
        }

        async void ChatList_Refreshing(System.Object sender, System.EventArgs e)
		{
            // Load the messages
            LoadMessages();
        }
    }
}

