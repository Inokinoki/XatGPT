using System;
using OpenAI_API.Chat;
using SQLite;

namespace XatGPT
{
    [Table("Chats")]
    public class ChatConversation
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public string Title { get; set; }

        // Conversation related fields
        public string SystemMessage { get; set; }
        public string ModelName { set; get; }
        public double Temperature { set; get; }

        [Ignore]
        public Conversation Conversation { get; set; }
        [Ignore]
        public ChatRequest Request { get; set; }

        public ChatConversation()
        {
            Conversation = null;
        }

        public ChatConversation(Conversation conv)
        {
            Conversation = conv;
        }
    }
}

