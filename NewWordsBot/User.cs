using System;
using MongoDB.Bson.Serialization.Attributes;

namespace NewWordsBot
{
    public class User
    {
        [BsonId]
        public Guid Id { get; }
        public string Username { get; }
        public long ChatId { get; }
        public DateTime RegisteredDate { get; }

        public User(Guid id, string username, long chatId, DateTime registeredDate)
        {
            Id = id;
            Username = username;
            ChatId = chatId;
            RegisteredDate = registeredDate;
        }

        protected bool Equals(User other)
        {
            return Id.Equals(other.Id) && string.Equals(Username, other.Username) && ChatId == other.ChatId 
                   && Math.Abs(RegisteredDate.Ticks - other.RegisteredDate.Ticks) < 10000;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ChatId.GetHashCode();
                hashCode = (hashCode * 397) ^ RegisteredDate.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Username)}: {Username}, {nameof(ChatId)}: {ChatId}, {nameof(RegisteredDate)}: {RegisteredDate}";
        }
    }
}