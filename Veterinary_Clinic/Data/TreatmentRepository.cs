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
    public class TreatmentRepository : IRepository<Treatment>
    {
        private readonly string _filePath = "../../Excel/Treatments.xlsx";

        public TreatmentRepository()
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
                    var sheet = package.Workbook.Worksheets.Add("Treatments");
                    sheet.Cells[1, 1].Value = "Id";
                    sheet.Cells[1, 2].Value = "AppointmentId";
                    sheet.Cells[1, 3].Value = "Diagnosis";
                    sheet.Cells[1, 4].Value = "TreatmentPlan";
                    sheet.Cells[1, 5].Value = "DateCreated";
                    sheet.Cells[1, 6].Value = "Cost";
                    package.Save();
                }
            }
            else
            {
                using (var package = new ExcelPackage(fileInfo))
                {
                    if (package.Workbook.Worksheets["Treatments"] == null)
                    {
                        package.Workbook.Worksheets.Add("Treatments");
                        package.Save();
                    }
                }
            }
        }

        public List<Treatment> GetAll()
        {
            var list = new List<Treatment>();
            if (!File.Exists(_filePath)) return list;

            var appointRepo = new AppointmentRepository();
            var appointments = appointRepo.GetAll().ToDictionary(a => a.Id, a => a);

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Treatments"];
                if (sheet?.Dimension == null) return list;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    int appId = int.TryParse(sheet.Cells[r, 2].Text, out int aid) ? aid : 0;
                    appointments.TryGetValue(appId, out Appointment app);

                    list.Add(new Treatment
                    {
                        Id = int.TryParse(sheet.Cells[r, 1].Text, out int id) ? id : 0,
                        Appointment = app,
                        Diagnosis = sheet.Cells[r, 3].Text,
                        TreatmentPlan = sheet.Cells[r, 4].Text,
                        DateCreated = DateTime.TryParse(sheet.Cells[r, 5].Text, out DateTime dc) ? dc : DateTime.MinValue,
                        Cost = int.TryParse(sheet.Cells[r, 6].Text, out int cost) ? cost : 0
                    });
                }
            }
            return list;
        }

        public void Add(Treatment treatment)
        {
            if (treatment.Appointment == null)
                throw new ArgumentException("Приём не указан");

            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Treatments"];
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
                treatment.Id = maxId + 1;
                treatment.DateCreated = DateTime.Now;

                sheet.Cells[newRow, 1].Value = treatment.Id;
                sheet.Cells[newRow, 2].Value = treatment.Appointment.Id;
                sheet.Cells[newRow, 3].Value = treatment.Diagnosis;
                sheet.Cells[newRow, 4].Value = treatment.TreatmentPlan;
                sheet.Cells[newRow, 5].Value = treatment.DateCreated.ToString();
                sheet.Cells[newRow, 6].Value = treatment.Cost;
                package.Save();
            }
        }

        public void Update(Treatment treatment)
        {
            using (var package = new ExcelPackage(new FileInfo(_filePath)))
            {
                var sheet = package.Workbook.Worksheets["Treatments"];
                if (sheet?.Dimension == null) return;

                int rows = sheet.Dimension.Rows;
                for (int r = 2; r <= rows; r++)
                {
                    if (int.TryParse(sheet.Cells[r, 1].Text, out int id) && id == treatment.Id)
                    {
                        sheet.Cells[r, 2].Value = treatment.Appointment?.Id;
                        sheet.Cells[r, 3].Value = treatment.Diagnosis;
                        sheet.Cells[r, 4].Value = treatment.TreatmentPlan;
                        sheet.Cells[r, 6].Value = treatment.Cost;
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
                var sheet = package.Workbook.Worksheets["Treatments"];
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
