using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers.Management.CrudManagement
{
    public class AddUser : DatabaseService, IReadWriteUser
    {
        private readonly string addUser;

        protected AddUser(string addUser) : base(addUser)
        {
            this.addUser = addUser;
        }

        public List<string[]> ReadUser()
        {
            throw new NotImplementedException();
        }

        public void WriteUser(List<User> users)
        {
            throw new NotImplementedException();
        }
    }
}
