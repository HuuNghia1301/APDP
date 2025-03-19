
using Demo.Models;

namespace Demo.Controllers.utilities
{
    public interface IReadWriteUser
    {
        public List<string[]> ReadUser();
        public void WriteUser(List<User> users);
    }
}
