using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using PdfMergerAPI.DBContexts;
using PdfMergerAPI.Models;
using System.IO;

namespace PdfMergerAPI.Repository
{
    public class PdfMerger : IPdfMerger
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly PdfMergerDBContext _pdfMergerContext;
        private readonly ILogger _logerCustom; 

        public PdfMerger(IWebHostEnvironment webHostEnvironment,IHttpContextAccessor contextAccessor,PdfMergerDBContext pdfMergerContext,ILogger logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _contextAccessor = contextAccessor;
            _pdfMergerContext = pdfMergerContext;
            _logerCustom = logger;
        }
        public async Task<string> Upload(FileDomain fileDomain)// List<>???
        {
            string localFilePath = fileDomain.FileExtension;

            if (localFilePath.Equals(".pdf"))
            {
                localFilePath = System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "FilesFolder\\PdfFiles",
               $"{fileDomain.FileName} {fileDomain.FileExtension}");
            }
            else
            {
                localFilePath = System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "FilesFolder\\ImgFiles",
             $"{fileDomain.FileName} {fileDomain.FileExtension}");
            }

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await fileDomain.FileObj.CopyToAsync(stream);

            string urlPath = null;

            if (fileDomain.FileExtension == ".pdf")
            {
                 urlPath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}" +
                   $"{_contextAccessor.HttpContext.Request.PathBase}/FilesFolder/PdfFiles/{fileDomain.FileName}{fileDomain.FileExtension}";
            }
            else
            {
                 urlPath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}" +
                    $"{_contextAccessor.HttpContext.Request.PathBase}/FilesFolder/ImgFiles/{fileDomain.FileName}{fileDomain.FileExtension}";
            }

            fileDomain.FilePath = urlPath;

            await _pdfMergerContext.FilesMergerTable.AddAsync(fileDomain);
            await _pdfMergerContext.SaveChangesAsync();

            return localFilePath;
        }
        public IActionResult MergeImgsToPdf(List<string> paths)
        {
            _logerCustom.LogInformation("Pdf File creating: START POINT");
            string uotputPath = System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "FilesFolder\\FinishedPDFs",
                $"merged.pdf");
            using (PdfWriter pdfWriter = new PdfWriter(uotputPath))
            {
                using(PdfDocument pdf=new PdfDocument(pdfWriter))
                {
                    Document document = new Document(pdf);
                    foreach(var path in paths)
                    {
                        Image img=new Image(ImageDataFactory.Create(path));
                        document.Add(img);
                        _logerCustom.LogInformation($"File: {path}");

                        System.IO.File.Delete(path); 
                    }

                    var imgs = paths.Select(path => new Image(ImageDataFactory.Create(path)));
                        
                    //return document;
                }
            }
            var fileStream = new FileStream(uotputPath, FileMode.Open, FileAccess.Read);
            _logerCustom.LogInformation("Pdf File finished: END POINT");
            _logerCustom.LogError("custom err");

            return new FileStreamResult(fileStream, "application/pdf");
        }

        public IActionResult MergePdfsToPdf(List<string> paths)
        {
            string uotputPath = System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "FilesFolder\\FinishedPDFs",
                $"merged.pdf");
            using (PdfWriter pdfWriter = new PdfWriter(uotputPath))
            {
                using (PdfDocument newPdfDoc = new PdfDocument(pdfWriter))
                {
                    Document document = new Document(newPdfDoc);
                    foreach (var path in paths)
                    {
                        using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(path)))
                        {
                            for(int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                            {
                                PdfPage pg= pdfDoc.GetPage(i);
                                newPdfDoc.AddPage(pg.CopyTo(newPdfDoc));
                            }
                        }
                        System.IO.File.Delete(path); 
                    }  
                }
            }
            var fileStream = new FileStream(uotputPath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "application/pdf");
        }

        public IActionResult MergeMixedFiles(List<string> paths) 
        {
            string uotputPath = System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "FilesFolder\\FinishedPDFs",
               $"merged.pdf");
            using (PdfWriter pdfWriter = new PdfWriter(uotputPath))
            {
                using (PdfDocument newPdfDoc = new PdfDocument(pdfWriter))
                {
                    Document document=new Document(newPdfDoc);
                    foreach (string path in paths)
                    {
                        if (path.Contains(".pdf"))
                        {
                            using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(path)))
                            {
                                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                                {
                                    PdfPage pg = pdfDoc.GetPage(i);
                                    newPdfDoc.AddPage(pg.CopyTo(newPdfDoc));  
                                }
                            }
                        }
                        else
                        {
                            ImageData img = ImageDataFactory.Create(path);
                            Image image= new Image(img);    
                            
                            PdfPage newPage = newPdfDoc.AddNewPage();
                            PdfCanvas canvas = new PdfCanvas(newPage);
                            // canvas.AddImageAt(img, 0, 0, false);
                            
                            float x = 0; 
                            float y = 0; 
                            float width = 300; 
                            float height = 200; 

                            Canvas canvasObj = new Canvas(canvas, newPage.GetPageSize());
                            canvasObj.Add(new Image(img).SetAutoScale(true)
                                            .SetFixedPosition(x, y)
                                            .SetHeight(height)
                                            .SetWidth(width));
                        }
                        System.IO.File.Delete(path);
                    }
                }
            }
            var fileStream = new FileStream(uotputPath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "application/pdf");
        }
    }
}
