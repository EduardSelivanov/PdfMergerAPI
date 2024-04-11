using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PdfMergerAPI.Models.DTOs
{
    public class FileDTO
    {

        [Required]
        public IFormFile FileObj { get; set; }

        //[Required]
        //public string? FileName { get; set; }

        public string? FileDescription { get; set; }

    }
}
