using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SQLite;

namespace XatGPT
{
    /* 
     * Async lazy load helper from:
     * https://learn.microsoft.com/zh-cn/xamarin/xamarin-forms/data-cloud/data/databases
     */
    public class AsyncLazy<T>
    {
        readonly Lazy<Task<T>> instance;

        public AsyncLazy(Func<T> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public AsyncLazy(Func<Task<T>> factory)
        {
            instance = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return instance.Value.GetAwaiter();
        }
    }

    public class ChatConversationDB
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<ChatConversationDB> Instance =
            new AsyncLazy<ChatConversationDB>(async () =>
            {
                var instance = new ChatConversationDB();
                CreateTableResult result = await Database.CreateTableAsync<ChatConversation>();
                result = await Database.CreateTableAsync<ChatMessage>();
                return instance;
            });

        public ChatConversationDB()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<ChatConversation>> GetItemsAsync()
        {
            return Database.Table<ChatConversation>().ToListAsync();
        }

        public Task<ChatConversation> GetItemAsync(int id)
        {
            return Database.Table<ChatConversation>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(ChatConversation item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(ChatConversation item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> SaveMessageAsync(ChatMessage msg)
        {
            if (msg.Id != 0)
            {
                return Database.UpdateAsync(msg);
            }
            else
            {
                return Database.InsertAsync(msg);
            }
        }

        public Task<List<ChatMessage>> GetChatMessageFromConversation(ChatConversation conv)
        {
            return Database.Table<ChatMessage>().Where(i => i.ConversationId == conv.Id).ToListAsync();
        }
    }
}
