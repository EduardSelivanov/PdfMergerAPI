using Microsoft.AspNetCore.Mvc;
using PdfMergerAPI.Models;
using PdfMergerAPI.Models.DTOs;
using PdfMergerAPI.Repository;
using PdfMergerAPI.Services;

namespace PdfMergerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDFController : ControllerBase
    {
        private readonly IPdfMerger _mergerService;
        private readonly PathsService _pathSer;

        public PDFController(IPdfMerger mergerService, PathsService path)
        {
            _mergerService = mergerService;
            _pathSer = path;
        }

        [HttpPost]
        [Route("UploadFiles")]
        public async Task<IActionResult> ImagesToPdf([FromForm] List<IFormFile> ifiles)
        {
            List<FileDTO> filesDTO = new List<FileDTO>();
            foreach (IFormFile ifile in ifiles)
            {
                FileDTO fileDto = new FileDTO()
                {
                    FileObj = ifile,

                };
                filesDTO.Add(fileDto);
            }
            List<string> paths = new List<string>();
            List<FileDomain> domainList = new List<FileDomain>();

            foreach (FileDTO fileDTO in filesDTO) {
                FileDomain fileD = new FileDomain
                {
                    FileName = fileDTO.FileObj.FileName,
                    FileObj = fileDTO.FileObj,
                    FileExtension = Path.GetExtension(fileDTO.FileObj.FileName),
                    FileSizeInBytes = fileDTO.FileObj.Length,
                    FileDescription = fileDTO.FileDescription,
                };
                domainList.Add(fileD);
                string pat = await _mergerService.Upload(fileD);
                paths.Add(pat);
            }
            _pathSer.SetPaths(paths);
            return Ok(paths);
        }

       


        [HttpGet]
        [Route("IfYouHadImages")]
        public  IActionResult DownloadFile()
        {
            List<string> paths = _pathSer.GetPaths();
            var g = _mergerService.MergeImgsToPdf(paths);
            return g;
        }

        [HttpGet]
        [Route("IfYouHadPDFS")]
        public IActionResult DownloadFileFromPDFS()
        {
            List<string> paths = _pathSer.GetPaths();
            var completed= _mergerService.MergePdfsToPdf(paths);
            return completed;
        }

        [HttpGet]
        [Route("IfYouHadMix")]
        public IActionResult DownloadFileAsMixed()
        {
            List<string> paths = _pathSer.GetPaths();
            var completed = _mergerService.MergeMixedFiles(paths);
            return completed;
        }





    }
}
