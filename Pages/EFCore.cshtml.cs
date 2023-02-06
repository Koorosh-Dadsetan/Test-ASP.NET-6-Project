using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SelectPdf;
using Test_Project.Models;

namespace Test_Project.Pages
{
    public class EFCoreModel : PageModel
    {
        private readonly DBCtx _context;

        public EFCoreModel(DBCtx context)
        {
            _context = context;
        }

        public IEnumerable<Employee> Employee { get; set; } = default!;

        public int TotalRecords { get; set; }

        public int PageNo { get; set; }

        public int PageSize { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchBox { get; set; }

        public IActionResult OnGet(int p = 1, int s = 5)
        {
            if (_context.Employees != null && string.IsNullOrEmpty(SearchBox))
            {
                Employee = _context.Employees.Skip((p - 1) * s).Take(s);

                TotalRecords = _context.Employees.Count();

                PageNo = p;

                PageSize = s;
            }
            else if (_context.Employees != null && SearchBox != null)
            {
                Employee = from employee in _context.Employees
                           where employee.FullName.Contains(SearchBox)
                           select employee;

                TotalRecords = Employee.Count();

                Employee = Employee.Skip((p - 1) * s).Take(s);

                PageNo = p;

                PageSize = s;
            }
            return Page();
        }

        public FileResult OnPostExportExcel()
        {
            if (_context.Employees != null)
            {
                Employee = _context.Employees.AsEnumerable();
            }

            using (XLWorkbook workbook = new XLWorkbook { RightToLeft = true })
            {
                var worksheet = workbook.Worksheets.Add("Employees Sheet");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "ردیف";
                worksheet.Cell(currentRow, 2).Value = "نام و نام خانوادگی";
                worksheet.Cell(currentRow, 3).Value = "موبایل";
                worksheet.Cell(currentRow, 4).Value = "سن";
                worksheet.Cell(currentRow, 5).Value = "آدرس";

                foreach (var item in Employee)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.id;
                    worksheet.Cell(currentRow, 2).Value = item.FullName;
                    worksheet.Cell(currentRow, 3).Value = item.Mobile;
                    worksheet.Cell(currentRow, 4).Value = item.Age;
                    worksheet.Cell(currentRow, 5).Value = item.Address;
                }

                var myCustomStyle = XLWorkbook.DefaultStyle;
                myCustomStyle.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                myCustomStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.RangeUsed(XLCellsUsedOptions.AllContents).Style = myCustomStyle;

                worksheet.Column(1).AdjustToContents();
                worksheet.Column(2).AdjustToContents();
                worksheet.Column(3).AdjustToContents();
                worksheet.Column(4).AdjustToContents();
                worksheet.Column(5).AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }


        public FileResult OnPostExportPDF()
        {
            string htmlString = "<html><body><h1>Koorosh Dadsetan</h1></body></html>";

            using (MemoryStream stream = new MemoryStream())
            {
                HtmlToPdf converter = new HtmlToPdf();

                converter.Options.PdfPageSize = PdfPageSize.A4;
                converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
                converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
                converter.Options.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;

                PdfDocument doc = converter.ConvertHtmlString(htmlString);

                //PdfDocument doc = converter.ConvertUrl("https://localhost:7109/sqldataadapter");

                doc.Save(stream);
                doc.Close();

                return File(stream.ToArray(), "application/pdf", "Employees.pdf");
            }
        }


    }
}
