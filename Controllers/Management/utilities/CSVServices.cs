using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Demo.Models;

namespace Demo.Controllers.utilities
{
    public class CSVServices : DatabaseService, IReadWriteUser
    {
        private readonly string _csvFilePath;

        public CSVServices(string filePath) : base(filePath)
        {
            _csvFilePath = filePath;
        }

        public List<User> ReadUsers()
        {
            var data = ReadData();
            var users = new List<User>();
            foreach (var row in data.Skip(1)) // Bỏ qua tiêu đề
            {
                if (row.Length < 9)
                {
               
                    continue;
                }

                var user = new User
                {
                    Id = int.Parse(row[0].Trim()),
                    LastName = row[1].Trim(),
                    FirstName = row[2].Trim(),
                    UserName = row[3].Trim(),
                    Address = row[4].Trim(),
                    Password = row[5].Trim(),
                    Role = row[6].Trim(),
                    Email = row[7].Trim(),
                    PhoneNumber = row[8].Trim()
                };            
                users.Add(user);
            }
            return users;
        }


        public void WriteUsers(List<User> users)
        {
            var allData = ReadData().Where(row => row[0] != "User").ToList();

            allData.AddRange(users.Select(u => new[]
            {
                "User",
                u.Id.ToString(),
                u.FirstName,
                u.LastName,
                u.UserName,
                u.Email, // Ghi Email vào file CSV
                u.PhoneNumber, // Ghi PhoneNumber vào file CSV
                u.Address,
                u.Password,
                u.Role.ToString()
            }));

            WriteData(allData);
        }

        private List<string[]> ReadData()
        {
            var rows = new List<string[]>();
            if (!File.Exists(_csvFilePath)) return rows;

            using (var reader = new StreamReader(_csvFilePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        rows.Add(line.Split(','));
                    }
                }
            }
            return rows;
        }
        public User AuthenticateUser(string username, string password)
        {
            return ReadUsers().FirstOrDefault(user =>
            user.UserName == username && user.Password == password);
        }


        private void WriteData(List<string[]> data)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            {
                foreach (var row in data)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }
        public User GetUserByEmailOrPhoneNumber(string email, string phoneNumber)
        {
            var users = ReadUsers();
            return users.FirstOrDefault(user => user.Email == email || user.PhoneNumber == phoneNumber);
        }

        List<string[]> IReadWriteUser.ReadUser()
        {
            return ReadData().Where(row => row[0] == "User").ToList();
        }

        

        public void WriteUser(List<User> users)
        {
            throw new NotImplementedException();
        }
    }
}
