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
    public class AnimalRepository : IRepository<Animal>
    {
        private readonly string _filePath = "../../Excel/Animals.xlsx";

        public AnimalRepository()
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
                    var sheet = package.Workbook.Worksheets.Add("Animals");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "Name";
                    sheet.Cells[1, 3].Value = "Species";
                    sheet.Cells[1, 4].Value = "Age";
                    sheet.Cells[1, 5].Value = "OwnerId";
                    package.Save();
                }
            }
            else
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets["Animals"] == null)
                    {
                        package.Workbook.Worksheets.Add("Animals");
                        package.Save();
                    }
                }
            }
        }

        public List<Animal> GetAll()
        {
            var animals = new List<Animal>();
            if (!File.Exists(_filePath)) return animals;

            var ownerRepo = new OwnerRepository();
            var owners = ownerRepo.GetAll();
            var ownerDict = owners.ToDictionary(o => o.Id, o => o);

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Animals"];
                if (sheet?.Dimension == null) return animals;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    int ownerId = int.TryParse(sheet.Cells[r, 5].Text, out int oid) ? oid : 0;
                    ownerDict.TryGetValue(ownerId, out Owner owner);

                    animals.Add(new Animal
                    {
                        Id = int.TryParse(sheet.Cells[r, 1].Text, out int id) ? id : 0,
                        Name = sheet.Cells[r, 2].Text,
                        Species = sheet.Cells[r, 3].Text,
                        Age = DateTime.TryParse(sheet.Cells[r, 4].Text, out DateTime age) ? age : DateTime.MinValue,
                        Owner = owner
                    });
                }
            }
            return animals;
        }

        public void Add(Animal animal)
        {
            if (animal.Owner == null)
                throw new ArgumentException("Владелец не указан");

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Animals"];
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
                animal.Id = maxId + 1;

                sheet.Cells[newRow, 1].Value = animal.Id;
                sheet.Cells[newRow, 2].Value = animal.Name;
                sheet.Cells[newRow, 3].Value = animal.Species;
                sheet.Cells[newRow, 4].Value = animal.Age.ToString();
                sheet.Cells[newRow, 5].Value = animal.Owner.Id;
                package.Save();
            }
        }

        public void Update(Animal animal)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Animals"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int id) && id == animal.Id)
                    {
                        sheet.Cells[r, 2].Value = animal.Name;
                        sheet.Cells[r, 3].Value = animal.Species;
                        sheet.Cells[r, 4].Value = animal.Age.ToString();
                        sheet.Cells[r, 5].Value = animal.Owner?.Id;
                        package.Save();
                        return;
                    }
                }
            }
        }

        public void Delete(int id)
        {
            var appointRepo = new AppointmentRepository();
            var appointments = appointRepo.GetAll().Where(a => a.Animal?.Id == id).ToList();
            foreach (var app in appointments)
                appointRepo.Delete(app.Id);

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Animals"];
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