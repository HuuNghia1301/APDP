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
        public void UpdateUserPassword(string email, string newPassword)
        {
            if (!File.Exists(_csvFilePath)) return;

            var tempFile = Path.GetTempFileName();
            using (var reader = new StreamReader(_csvFilePath))
            using (var writer = new StreamWriter(tempFile))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var users = csvReader.GetRecords<User>().ToList();
                csvWriter.WriteHeader<User>();
                csvWriter.NextRecord();

                foreach (var user in users)
                {
                    if (user.Email == email)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    }
                    csvWriter.WriteRecord(user);
                    csvWriter.NextRecord();
                }
            }

            File.Delete(_csvFilePath);
            File.Move(tempFile, _csvFilePath);
        }
        public void UpdateUser(string Email , string PassWord , string newPassword = null, string newFirstName = null,
                       string newLastName = null, string newEmail = null, string newAddress = null, string newPhoneNumber = null)
        {
            if (!File.Exists(_csvFilePath)) return;

            var tempFile = Path.GetTempFileName();

            using (var reader = new StreamReader(_csvFilePath))
            using (var writer = new StreamWriter(tempFile))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var users = csvReader.GetRecords<User>().ToList();
                csvWriter.WriteHeader<User>();
                csvWriter.NextRecord();

                foreach (var user in users)
                {
                    if (user.Email == Email && BCrypt.Net.BCrypt.Verify(PassWord, user.Password)) 
                    {
                        user.FirstName = newFirstName;
                        user.LastName = newLastName;
                        user.Email = newEmail;
                        user.Address = newAddress;
                        user.PhoneNumber = newPhoneNumber;
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                       
                    }

                    csvWriter.WriteRecord(user);
                    csvWriter.NextRecord();
                }
            }

            File.Delete(_csvFilePath);
            File.Move(tempFile, _csvFilePath);
        }

        public void WriteUsers(List<User> users)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(users);
            }
        }

    }
}