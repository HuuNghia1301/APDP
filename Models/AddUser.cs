using CsvHelper;
using Demo.Models;
using System.Globalization;

public class AddNewUser
{
    private readonly string _csvFilePath;

    public AddNewUser(string csvFilePath)
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

    public void AddUser(User user)
    {
        var users = GetAllUsers();
        user.IdUser = users.Count > 0 ? users.Max(u => u.IdUser) + 1 : 1;
        
     

        user.CreatedAt = DateTime.Now;
        users.Add(user);
        using var writer = new StreamWriter(_csvFilePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(users);
    }

    public User? GetUserByEmailOrPhoneNumber(string email, string phoneNumber)
    {
        var users = GetAllUsers();
        return users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) || x.PhoneNumber == phoneNumber);
    }
}