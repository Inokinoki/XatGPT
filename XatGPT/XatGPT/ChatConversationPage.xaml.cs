using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        async Task<int> LoadMessages()
        {
            var dbInstance = await ChatConversationDB.Instance;
            var msgs = await dbInstance.GetChatMessageFromConversation(this.refChatConversation);
            this.messages.Clear();
            foreach (var msg in msgs) {
                this.messages.AddLast(msg);
            }
            this.ChatList.ItemsSource = msgs;
            if (msgs.Count > 0)
            {
                // Scroll to the end
                this.ChatList.ScrollTo(msgs[msgs.Count - 1], ScrollToPosition.End, true);
            }
            return msgs.Count;
        }

        void PrepareRequest()
        {
            if (refChatConversation.Request != null) return;

            refChatConversation.Request =
                new ChatRequest(DefaultChatRequestArgs);
            refChatConversation.Request.Temperature =
                refChatConversation.Temperature;
            refChatConversation.Request.Model = refChatConversation.ModelName;
            refChatConversation.Request.Messages = new List<OpenAI_API.Chat.ChatMessage>();
        }

        void ChatSettingButton_Clicked(System.Object sender, System.EventArgs e)
        {
			// TODO: Open a page to configure parameters
        }

        async void ChatList_Refreshing(System.Object sender, System.EventArgs e)
		{
            // Load the messages
            await LoadMessages();

            this.ChatList.IsRefreshing = false;
        }

        async Task<string> SendCoversationAndGetResponse()
        {
            if (this.refChatConversation.Conversation == null)
            {
                var api = new OpenAIAPI(OpenAIAPIKey);
                this.refChatConversation.Request.Messages.Clear();
                var msg = new OpenAI_API.Chat.ChatMessage();
                msg.Content = this.refChatConversation.SystemMessage;
                msg.Role = ChatMessageRole.System;
                this.refChatConversation.Request.Messages.Add(msg);
                for (int i = 0; i < this.messages.Count && i < 4; i++)
                {
                    msg = new OpenAI_API.Chat.ChatMessage();
                    msg.Content = this.messages.Last.Value.Text;
                    msg.Role = this.messages.Last.Value.Role;
                    this.refChatConversation.Request.Messages.Insert(1, msg);
                }
                var res = await api.Chat.CreateChatCompletionAsync(this.refChatConversation.Request);
                if (res.Choices.Count > 0)
                {
                    return res.Choices[0].Message.Content;
                }
            }
            return "";
        }

        async void SendButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (this.MessageEditor.Text.Length > 0)
            {
                var chatMessage = new ChatMessage(this.MessageEditor.Text,
                    ChatMessageRole.User);
                chatMessage.ConversationId = this.refChatConversation.Id;

                this.MessageEditor.Text = "";

                // Save to DB
                var dbInstance = await ChatConversationDB.Instance;
                await dbInstance.SaveMessageAsync(chatMessage);
                await LoadMessages();

                var reply = await SendCoversationAndGetResponse();
                if (reply.Length > 0)
                {
                    var replyMessage = new ChatMessage(reply,
                        ChatMessageRole.Assistant);
                    replyMessage.ConversationId = this.refChatConversation.Id;
                    await dbInstance.SaveMessageAsync(replyMessage);
                    await LoadMessages();
                }
            }
        }
    }
}

