using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Data.Entities;
using Server.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Server
{
    internal class DatabaseUtils
    {
        private static DataContext GetContext()
        {
            return new DataContext();
        }

        public static void AddUser(User newUser)
        {
            using (var context = GetContext())
            {
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        public static List<User> GetUsers()
        {
            using (var context = GetContext())
            {
                return context.Users.ToList();
            }
        }

        public static User GetUserById(int userId)
        {
            using (var context = GetContext())
            {
                return context.Users.Find(userId);
            }
        }

        public static User GetUserByEmail(string email)
        {
            using (var context = GetContext())
            {
                return context.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        public static User GetUserByKey(string key)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.PublicIdentityKey == key);
                if (user != null)
                {
                    GeneratePublicKey(user.Email);
                }
                return user;
            }
        }

        public static string GetUserByKeyStringified(string key)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.PublicIdentityKey == key);
                if (user != null)
                {
                    return $"{user.Email}|{user.Name}|{user.Surname}|{user.ChatIdSelected}|{user.Id}";
                }
                return null;
            }
        }

        static string GenerateRandomKey(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            string randomKey = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return randomKey;
        }

        public static string GeneratePublicKey(string email)
        {
            string Pkey;
            using (var context = GetContext())
            {
                Pkey = GenerateRandomKey(16);
                while ((context.Users.FirstOrDefault(u => u.PublicIdentityKey == Pkey) != null)) {
                    Pkey = GenerateRandomKey(16);
                }
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                user.PublicIdentityKey = Pkey;
                context.SaveChanges();
            }
            return Pkey;
        }

        public static string GetPublicKey(string email, string password)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
                return user.PublicIdentityKey;
            }
        }

        public static void SendMessage(string message, string key, string chat_id)
        {
            using (var context = GetContext())
            {
                var user = context.Users.FirstOrDefault(u => u.PublicIdentityKey == key);
                var ReadyMessage = new Message()
                {
                    ChatId = chat_id,
                    AuthorId = user.Id,
                    Text = message
                };
                context.Messages.Add(ReadyMessage);
                context.SaveChanges();
            }
        }

        public static string GetMessagesFromChat(string chatid, int page)
        {
            using (var context = GetContext())
            {
                try
                {
                    var messagesQuery = context.Messages
                        .Where(m => m.ChatId == chatid)
                        .OrderByDescending(m => m.Sended);

                    if (page > 0)
                    {
                        var skippedMessages = messagesQuery.Skip((page - 1) * 10);
                        messagesQuery = (IOrderedQueryable<Message>)skippedMessages;
                    }

                    var messages = messagesQuery.Take(10).ToList();
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var message in messages)
                    {
                        var user = context.Users.FirstOrDefault(u => u.Id == message.AuthorId);
                        if (user != null)
                        {
                            stringBuilder.Append($"{user.Name}|{message.Text}|{message.Sended}|{user.Id}||");
                        }
                    }

                    return stringBuilder.ToString();
                }
                catch (Exception ex)
                {
                    return $"Error retrieving messages: {ex.Message}";
                }
            }
        }

        public static void UpdateUser(int userId, User updatedUser)
        {
            using (var context = GetContext())
            {
                var userToUpdate = context.Users.Find(userId);

                if (userToUpdate != null)
                {
                    userToUpdate.Name = updatedUser.Name;
                    userToUpdate.Surname = updatedUser.Surname;
                    userToUpdate.Password = updatedUser.Password;
                    userToUpdate.Email = updatedUser.Email;

                    context.SaveChanges();
                }
            }
        }

        public static string GetChatsForUserAsString(string identityToken)
        {
            using (var context = GetContext())
            {
                StringBuilder stringBuilder = new StringBuilder();

                try
                {
                    var user = context.Users.First(u => u.PublicIdentityKey == identityToken);
                    var chatUserEntries = context.ChatUsers.Where(cu => cu.UserId == user.Id).ToList();
                    var chatIds = chatUserEntries.Select(cu => cu.ChatId).ToList();
                    var chats = context.Chats.Where(c => chatIds.Contains(c.Id)).ToList();

                    foreach (var chat in chats)
                    {
                        stringBuilder.Append($"{chat.Name}|{chat.Id}||");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving chats for user: {ex.Message}");
                }

                stringBuilder.Append("Global|0||");

                return stringBuilder.ToString();
            }
        }

        public static void CreateChat(string identityToken, int userId2)
        {
            using (var context = GetContext())
            {
                var user = context.Users.First(u => u.PublicIdentityKey == identityToken);
                var user1 = context.Users.FirstOrDefault(cu => cu.Id == user.Id);
                var user2 = context.Users.FirstOrDefault(cu2 => cu2.Id == userId2);

                if (user1 == null || user2 == null)
                {
                    Console.WriteLine("One or both users not found.");
                    return;
                }

                var chat = new Chat { AuthorId = user.Id, Name = $"{user1.Name} & {user2.Name}" };

                context.Chats.Add(chat);
                context.SaveChanges();

                var chatUser1 = new ChatUser { ChatId = chat.Id, UserId = user.Id };
                var chatUser2 = new ChatUser { ChatId = chat.Id, UserId = userId2 };

                context.ChatUsers.AddRange(chatUser1, chatUser2);
                context.SaveChanges();
            }
        }

        public static void DeleteUser(int userId)
        {
            using (var context = GetContext())
            {
                var userToDelete = context.Users.Find(userId);

                if (userToDelete != null)
                {
                    context.Users.Remove(userToDelete);
                    context.SaveChanges();
                }
            }
        }

        public static void SwitchChat(string identityToken, int chatId)
        {
            using (var context = GetContext())
            {
                var user = context.Users.First(u => u.PublicIdentityKey == identityToken);
                if (user != null)
                {
                    user.ChatIdSelected = chatId;
                    context.SaveChanges();
                }
            }
        }
    }
}
