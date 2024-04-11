using System.ComponentModel.DataAnnotations.Schema;

namespace PdfMergerAPI.Models
{
    public class FileDomain
    {
        public Guid FileId { get; set; }

        [NotMapped]
        public IFormFile FileObj { get; set; }

        public string FileName { get; set; }
        public string? FileDescription { get;set; }
        public string FileExtension { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; }   
    }
}
