using iText.Layout;
using Microsoft.AspNetCore.Mvc;
using PdfMergerAPI.Models;

namespace PdfMergerAPI.Repository
{
    public interface IPdfMerger
    {
        Task<string> Upload(FileDomain fileDomain);
        IActionResult MergeImgsToPdf(List<string> paths);
        IActionResult MergePdfsToPdf(List<string> paths);
        IActionResult MergeMixedFiles(List<string> paths);
    }
}
