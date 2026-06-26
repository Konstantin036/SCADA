using DataConcentrator.Database;
using DataConcentrator.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DataConcentrator.Services
{
    public class UserService
    {
        private static UserService _instance;
        private static readonly object _lock = new object();

        public User CurrentUser { get; private set; }

        public static UserService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new UserService();
                    return _instance;
                }
            }
        }

        public bool Login(string username, string password)
        {
            string hash = HashPassword(password);
            using (var ctx = new ScadaContext())
            {
                var user = ctx.Users.FirstOrDefault(u =>
                    u.Username == username && u.PasswordHash == hash);
                if (user == null) return false;
                CurrentUser = user;
                return true;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public bool Register(string username, string password, UserRole role)
        {
            if (!ValidatePassword(password)) return false;

            string hash = HashPassword(password);
            using (var ctx = new ScadaContext())
            {
                // Ne sme postojati ista lozinka
                if (ctx.Users.Any(u => u.PasswordHash == hash)) return false;
                // Ne sme postojati isti username
                if (ctx.Users.Any(u => u.Username == username)) return false;

                ctx.Users.Add(new User
                {
                    Username = username,
                    PasswordHash = hash,
                    Role = role
                });
                ctx.SaveChanges();
                return true;
            }
        }

        public bool ValidatePassword(string password)
        {
            if (password.Length < 15) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[a-z]")) return false;
            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]")) return false;
            return true;
        }

        public bool IsAdmin() => CurrentUser?.Role == UserRole.Admin;

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}