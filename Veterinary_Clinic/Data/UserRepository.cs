using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class UserRepository
    {
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _filePath = Path.Combine(_basePath, "Excel", "Users.xlsx");

        public UserRepository()
        {
            EnsureFileAndSheetExist();
        }

        private void EnsureFileAndSheetExist()
        {
            var fileInfo = new FileInfo(_filePath);
            if (!fileInfo.Exists)
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    var sheet = package.Workbook.Worksheets.Add("Users");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "Username";
                    sheet.Cells[1, 3].Value = "Password";
                    sheet.Cells[1, 4].Value = "Role";
                    package.Save();
                }


            }
            else
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets["Users"] == null)
                    {
                        package.Workbook.Worksheets.Add("Users");
                        package.Save();
                    }
                }
            }
        }

        public List<User> GetAll()
        {
            var list = new List<User>();
            if (!File.Exists(_filePath)) return list;

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Users"];
                if (sheet?.Dimension == null) return list;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    list.Add(new User
                    {
                        Id = int.TryParse(sheet.Cells[r, 1].Text, out int id) ? id : 0,
                        Username = sheet.Cells[r, 2].Text,
                        Password = sheet.Cells[r, 3].Text,
                        Role = Enum.TryParse(sheet.Cells[r, 4].Text, out UserRole role) ? role : UserRole.Doctor
                    });
                }
            }
            return list;
        }

        public User Authenticate(string username, string password)
        {
            return GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public void Add(User user)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Users"];
                int newRow = (sheet.Dimension?.Rows ?? 1) + 1;

                int maxId = 0;
                if (sheet.Dimension != null)
                {
                    for (int r = 2; r < newRow; r++)
                    {
                        int id = int.TryParse(sheet.Cells[r, 1].Text, out int val) ? val : 0;
                        if (id > maxId) maxId = id;
                    }
                }
                user.Id = maxId + 1;

                sheet.Cells[newRow, 1].Value = user.Id;
                sheet.Cells[newRow, 2].Value = user.Username;
                sheet.Cells[newRow, 3].Value = user.Password;
                sheet.Cells[newRow, 4].Value = user.Role.ToString();
                package.Save();
            }
        }
    }
}
