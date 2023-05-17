namespace ClientApplication.Services
{
    using DatabaseAccess.Models;
    using Microsoft.AspNetCore.Authorization;
    using OfficeOpenXml;
    using DAModels = DatabaseAccess.Models;
    public interface IDocumentContact : System.IDisposable
    {
        public abstract Task<byte[]> GetDocument(List<DAModels::Contact> contacts);
    }
    public partial class DocumentContact : System.Object, IDocumentContact
    {
        protected ILogger<DocumentContact> Logger { get; set; } = default!;
        public DocumentContact(ILogger<DocumentContact> logger) { this.Logger = logger; }
        public void Dispose() => this.Logger.LogInformation("DocumentContact Disposed");

        public async Task<byte[]> GetDocument(List<Contact> contactList)
        {
            using var excelDocument = new ExcelPackage();
            var workSheet = excelDocument.Workbook.Worksheets.Add("Контакты");

            workSheet.DefaultRowHeight = 12;
            workSheet.DefaultColWidth = 20;
            for(var index = 1; index <= 9; index++) workSheet.Column(index).AutoFit();

            workSheet.Cells[1, 1].Value = "Имя"; workSheet.Cells[1, 2].Value = "Фамилия";
            workSheet.Cells[1, 3].Value = "Телефон"; workSheet.Cells[1, 4].Value = "Email";
            workSheet.Cells[1, 5].Value = "Пол"; workSheet.Cells[1, 6].Value = "Изменение";
            workSheet.Cells[1, 7].Value = "День рождения";
            workSheet.Cells[1, 8].Value = "Семейный статус";
            workSheet.Cells[1, 9].Value = "Тип контакта";

            workSheet.Row(1).Style.Font.Bold = true; workSheet.Row(1).Height = 20; 
            for(var index = 0; index < contactList.Count; index++)
            {
                workSheet.Cells[index + 2, 1].Value = contactList[index].Name;
                workSheet.Cells[index + 2, 2].Value = contactList[index].Surname;
                workSheet.Cells[index + 2, 3].Value = contactList[index].Phonenumber ?? "Не указано";
                workSheet.Cells[index + 2, 4].Value = contactList[index].Emailaddress;

                workSheet.Cells[index + 2, 5].Value = contactList[index].Gendertype.Gendertypename;
                workSheet.Cells[index + 2, 6].Value = contactList[index].Lastupdate.ToString();
                workSheet.Cells[index + 2, 7].Value = contactList[index].Birthday.ToString();
                workSheet.Cells[index + 2, 8].Value = contactList[index].Familystatus;
                workSheet.Cells[index + 2, 9].Value = contactList[index].Authorization == null ?
                    "Созданный контакт" : "Профиль пользователя";
            }
            this.Logger.LogInformation($"Document created\n");
            return await excelDocument.GetAsByteArrayAsync();
        }
    }
}
