using FontChanging;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Reading;
using System.IO.Compression;

namespace CertificadorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdfWriter : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public PdfWriter(IWebHostEnvironment env)
        {
            _env = env;
        }


        [HttpPost("replace")]
        [ProducesResponseType(typeof(File), StatusCodes.Status200OK, "application/zip")]
        public IActionResult ReplaceText(IFormFile excelDosAlunos, IFormFile pdfFile)
        {
            string nomeDaColuna = "Buyer's Info";
            try
            {
                if (excelDosAlunos == null || pdfFile == null)
                {
                    return BadRequest("Os arquivos Excel e PDF são obrigatórios.");
                }

                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string excelFilePath = Path.Combine(uploadPath, excelDosAlunos.FileName);
                using (var stream = new FileStream(excelFilePath, FileMode.Create))
                {
                    excelDosAlunos.CopyTo(stream);
                }

                var userNames = ExcelReader.ReadUserNameFromExcel(excelFilePath, nomeDaColuna);

                // Diretório temporário para os PDFs individuais
                string tempPdfPath = Path.Combine(uploadPath, "temp");
                if (!Directory.Exists(tempPdfPath))
                {
                    Directory.CreateDirectory(tempPdfPath);
                }

                foreach (var nomeDoAluno in userNames)
                {
                    string outputPath = Path.Combine(tempPdfPath, $"{nomeDoAluno}.pdf");

                    GlobalFontSettings.FontResolver = new PoppinsFontResolver();

                    using (var memoryStream = new MemoryStream())
                    {
                        pdfFile.CopyTo(memoryStream);
                        memoryStream.Position = 0;
                        PdfDocument document = PdfReader.Open(memoryStream, PdfDocumentOpenMode.Modify);

                        PdfPage page = document.Pages[0];
                        XGraphics gfx = XGraphics.FromPdfPage(page);

                        XFont font = new XFont("Poppins", XUnitPt.FromPoint(24).Point, XFontStyleEx.Regular);

                        XSize textSize = gfx.MeasureString(nomeDoAluno, font);



                        double xPos = ((842 + 19 - 248) - textSize.Width) / 2;
                        double yPos = 290;

                        gfx.DrawString(nomeDoAluno, font, XBrushes.Black, new XPoint(xPos, yPos));

                        document.Save(outputPath);
                    }

                }

                // Remove o arquivo Excel do diretório temporário
                if (System.IO.File.Exists(excelFilePath))
                {
                    System.IO.File.Delete(excelFilePath);
                }

                string zipFileName = $"certificados_{DateTime.Now:yyyyMMddHHmmss}.zip";
                string zipFilePath = Path.Combine(uploadPath, zipFileName);

                ZipFile.CreateFromDirectory(tempPdfPath, zipFilePath);

                Directory.Delete(tempPdfPath, true);

                byte[] fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
                return File(fileBytes, "application/zip", zipFileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Erro ao processar os arquivos: {ex.Message}" });
            }
        }
    }
}
