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
    public class AppointmentRepository : IRepository<Appointment>
    {
        private readonly string _filePath = "../../Excel/Appointments.xlsx";

        public AppointmentRepository()
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
                    var sheet = package.Workbook.Worksheets.Add("Appointments");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "AnimalId";
                    sheet.Cells[1, 3].Value = "VeterinarianId";
                    sheet.Cells[1, 4].Value = "VisitDate";
                    sheet.Cells[1, 5].Value = "Complaint";
                    package.Save();
                }
            }
            else
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets["Appointments"] == null)
                    {
                        package.Workbook.Worksheets.Add("Appointments");
                        package.Save();
                    }
                }
            }
        }

        public List<Appointment> GetAll()
        {
            var list = new List<Appointment>();
            if (!File.Exists(_filePath)) return list;

            var animalRepo = new AnimalRepository();
            var animals = animalRepo.GetAll().ToDictionary(a => a.Id, a => a);
            var vetRepo = new VeterinarianRepository();
            var vets = vetRepo.GetAll().ToDictionary(v => v.Id, v => v);

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Appointments"];
                if (sheet?.Dimension == null) return list;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    int animalId = int.TryParse(sheet.Cells[r, 2].Text, out int aid) ? aid : 0;
                    int vetId = int.TryParse(sheet.Cells[r, 3].Text, out int vid) ? vid : 0;

                    animals.TryGetValue(animalId, out Animal animal);
                    vets.TryGetValue(vetId, out Veterinarian vet);

                    list.Add(new Appointment
                    {
                        Id = int.TryParse(sheet.Cells[r, 1].Text, out int id) ? id : 0,
                        Animal = animal,
                        Doctor = vet,
                        VisitDate = DateTime.TryParse(sheet.Cells[r, 4].Text, out DateTime dt) ? dt : DateTime.MinValue,
                        Complaint = sheet.Cells[r, 5].Text
                    });
                }
            }
            return list;
        }

        public void Add(Appointment app)
        {
            if (app.Animal == null || app.Doctor == null)
                throw new ArgumentException("Животное и врач обязательны");

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Appointments"];
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
                app.Id = maxId + 1;

                sheet.Cells[newRow, 1].Value = app.Id;
                sheet.Cells[newRow, 2].Value = app.Animal.Id;
                sheet.Cells[newRow, 3].Value = app.Doctor.Id;
                sheet.Cells[newRow, 4].Value = app.VisitDate.ToString();
                sheet.Cells[newRow, 5].Value = app.Complaint;
                package.Save();
            }
        }

        public void Update(Appointment app)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Appointments"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int id) && id == app.Id)
                    {
                        sheet.Cells[r, 2].Value = app.Animal?.Id;
                        sheet.Cells[r, 3].Value = app.Doctor?.Id;
                        sheet.Cells[r, 5].Value = app.Complaint;
                        package.Save();
                        return;
                    }
                }
            }
        }

        public void Delete(int id)
        {
            var treatRepo = new TreatmentRepository();
            var treatments = treatRepo.GetAll().Where(t => t.Appointment?.Id == id).ToList();
            foreach (var t in treatments)
                treatRepo.Delete(t.Id);

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Appointments"];
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