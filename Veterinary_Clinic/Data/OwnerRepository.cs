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
    public class OwnerRepository : IRepository<Owner>
    {
        private readonly string _filePath = "../../Excel/Owners.xlsx";

        public OwnerRepository()
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
                    var sheet = package.Workbook.Worksheets.Add("Owners");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "FullName";
                    sheet.Cells[1, 3].Value = "Phone";
                    sheet.Cells[1, 4].Value = "Address";
                    sheet.Column(3).Style.Numberformat.Format = "@";
                    package.Save();
                }
            }
            else
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets["Owners"] == null)
                    {
                        package.Workbook.Worksheets.Add("Owners");
                        package.Save();
                    }
                }
            }
        }

        public List<Owner> GetAll()
        {
            var list = new List<Owner>();
            if (!File.Exists(_filePath)) return list;

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Owners"];
                if (sheet?.Dimension == null) return list;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    list.Add(new Owner
                    {
                        Id = int.TryParse(sheet.Cells[r, 1].Text, out int id) ? id : 0,
                        FullName = sheet.Cells[r, 2].Text,
                        Phone = sheet.Cells[r, 3].Text,
                        Address = sheet.Cells[r, 4].Text
                    });
                }
            }
            return list;
        }

        public void Add(Owner owner)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Owners"];
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
                owner.Id = maxId + 1;

                sheet.Cells[newRow, 1].Value = owner.Id;
                sheet.Cells[newRow, 2].Value = owner.FullName;
                sheet.Cells[newRow, 3].Value = owner.Phone;
                sheet.Cells[newRow, 4].Value = owner.Address;
                package.Save();
            }
        }

        public void Update(Owner owner)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Owners"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int id) && id == owner.Id)
                    {
                        sheet.Cells[r, 2].Value = owner.FullName;
                        sheet.Cells[r, 3].Value = owner.Phone;
                        sheet.Cells[r, 4].Value = owner.Address;
                        package.Save();
                        return;
                    }
                }
            }
        }

        public void Delete(int id)
        {
            var animalRepo = new AnimalRepository();
            var animals = animalRepo.GetAll().Where(a => a.Owner?.Id == id).ToList();
            foreach (var animal in animals)
                animalRepo.Delete(animal.Id);

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Owners"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int currentId) && currentId == id)
                    {
                        sheet.DeleteRow(r);
                        package.Save();
                        return;
                    }
                }
            }
        }
    }
}