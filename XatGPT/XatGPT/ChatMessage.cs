using System;
using OpenAI_API.Chat;
using SQLite;

namespace XatGPT
{
    [Table("Messages")]
    public class ChatMessage
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Text { get; set; }
        [Ignore]
        public ChatMessageRole Role { get; set; }
        public String IntRole
        {
            set
            {
                this.Role = ChatMessageRole.FromString(value);
            }
            get
            {
                return (string)this.Role;
            }
        }

        public int ConversationId { get; set; }

        public ChatMessage()
        {
            this.ConversationId = -1;
        }
        public ChatMessage(string text, ChatMessageRole role)
        {
            this.Text = text;
            this.Role = role;
        }

        [Ignore]
        public bool IsAssistant { get => this.Role == ChatMessageRole.Assistant; }
        [Ignore]
        public bool IsUser { get => this.Role == ChatMessageRole.User; }
    }
}

