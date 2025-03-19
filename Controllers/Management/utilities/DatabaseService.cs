namespace Demo.Controllers.utilities
{
    public  class DatabaseService 
    {
        protected string filePath;
        protected static DatabaseService service;
        protected DatabaseService(string _filePath)
        {
            //đọc file json
            this.filePath = _filePath;
        }
        public DatabaseService GetInstance(string _filePath)
        {
            if (service == null)
            {
                return new DatabaseService(_filePath);
            }
            return service;

        }

       


    }
}
