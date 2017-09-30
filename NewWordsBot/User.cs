namespace NewWordsBot
{
    public class User
    {
        public string Username { get; }
        public long ChatId { get; }

        public User(string username, long chatId)
        {
            Username = username;
            ChatId = chatId;
        }
    }
}