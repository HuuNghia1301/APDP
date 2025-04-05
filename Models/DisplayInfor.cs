using Demo.Controllers.utilities;

namespace Demo.Models
{
    public class DisplayInfor : IDisplayinfor
    {
        private readonly CSVServices _csvService;
        public DisplayInfor()
        {
            
            _csvService = CSVServices.GetInstance("Data/users.csv","Data / courses.csv", "Data/grades.csv");
        }
        public string ShowInfor()
        {
            // Lấy thông tin từ CSVServices và trả về
            var filePath = _csvService.GetCSVFilePath();  // Gọi phương thức từ CSVServices
            return $"CSV File Path: {filePath}";
        }
    }
}
