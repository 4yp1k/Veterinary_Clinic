using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veterinary_Clinic.Models;

namespace Veterinary_Clinic.Data
{
    public class VeterinarianRepository : IRepository<Veterinarian>
    {
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _filePath = Path.Combine(_basePath, "Excel", "Veterinarians.xlsx");

        public VeterinarianRepository()
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
                    var sheet = package.Workbook.Worksheets.Add("Veterinarians");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "FullName";
                    sheet.Cells[1, 3].Value = "Specialty";
                    sheet.Cells[1, 4].Value = "IsActive";
                    package.Save();
                }
            }
            else
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets["Veterinarians"] == null)
                    {
                        package.Workbook.Worksheets.Add("Veterinarians");
                        package.Save();
                    }
                }
            }
        }

        public List<Veterinarian> GetAll()
        {
            var list = new List<Veterinarian>();
            if (!File.Exists(_filePath)) return list;

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Veterinarians"];
                if (sheet?.Dimension == null) return list;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    list.Add(new Veterinarian
                    {
                        Id = int.TryParse(sheet.Cells[r, 1].Text, out int id) ? id : 0,
                        FullName = sheet.Cells[r, 2].Text,
                        Specialty = sheet.Cells[r, 3].Text,
                        IsActive = sheet.Cells[r, 4].Text.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                    });
                }
            }
            return list;
        }

        public List<Veterinarian> GetActive()
        {
            return GetAll().Where(v => v.IsActive).ToList();
        }

        public void Add(Veterinarian vet)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Veterinarians"];
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
                vet.Id = maxId + 1;

                sheet.Cells[newRow, 1].Value = vet.Id;
                sheet.Cells[newRow, 2].Value = vet.FullName;
                sheet.Cells[newRow, 3].Value = vet.Specialty;
                sheet.Cells[newRow, 4].Value = "TRUE";
                package.Save();
            }
        }

        public void Update(Veterinarian vet)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Veterinarians"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int id) && id == vet.Id)
                    {
                        sheet.Cells[r, 2].Value = vet.FullName;
                        sheet.Cells[r, 3].Value = vet.Specialty;
                        package.Save();
                        return;
                    }
                }
            }
        }

        public void Delete(int id)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Veterinarians"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int currentId) && currentId == id)
                    {
                        sheet.Cells[r, 4].Value = "FALSE";
                        package.Save();
                        return;
                    }
                }
            }
        }
    }
}
