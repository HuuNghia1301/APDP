using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Demo.Models;
using CsvHelper;
using System.Globalization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Demo.Controllers.utilities
{
    public class CSVServices : DatabaseService, IReadWriteUser
    {
        private readonly string _csvFilePath;

        public CSVServices(string csvFilePath) : base(csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        public List<User> GetAllUsers()
        {
            if (!File.Exists(_csvFilePath)) return new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<User>().ToList();
        }

        public void WriteUsers(User user)
        {
            var users = GetAllUsers();
            user.IdUser = users.Count > 0 ? users.Max(u => u.IdUser) + 1 : 1;
            user.CodeUser = CodeUser();
            user.Created = DateTime.Now;
            users.Add(user);
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(users);
        }

        public string CodeUser()
        {
            var users = GetAllUsers();
            HashSet<string> existingCodes = new HashSet<string>(users.Select(u => u.CodeUser ?? string.Empty));

            Random random = new Random();
            string codename;
            do
            {
                int num = random.Next(1000, 9999);
                codename = "BH" + num;
            } while (existingCodes.Contains(codename)); // Lặp lại nếu mã đã tồn tại

            return codename;
        }

        public List<User> ReadUser()
        {
            if (!File.Exists(_csvFilePath)) return new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<User>().ToList();
        }

        public void WriteUser(List<User> users)
        {
            var existingUsers = GetAllUsers();
            foreach (var user in users)
            {
                user.IdUser = existingUsers.Count > 0 ? existingUsers.Max(u => u.IdUser) + 1 : 1;
                user.CodeUser = CodeUser();
                user.Created = DateTime.Now;
                existingUsers.Add(user);
            }
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(existingUsers);
        }

        public User? GetUserByEmailOrPhoneNumber(string email, string phoneNumber)
        {
            var users = ReadUser();
            return users.FirstOrDefault(u => u.Email == email || u.PhoneNumber == phoneNumber);
        }

        List<string[]> IReadWriteUser.ReadUser()
        {
            if (!File.Exists(_csvFilePath)) return new List<string[]>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<dynamic>()
               .Select(record => ((IDictionary<string, object>)record)
               .Values
               .Select(v => v?.ToString() ?? string.Empty)
               .ToArray())
               .ToList();
        }
    }
}