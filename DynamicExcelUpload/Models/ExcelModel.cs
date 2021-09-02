using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DynamicExcelUpload.Models
{
    public class ExcelModel
    {
        [Display(Name = "UploadFile")]
        [Required]
        public IFormFile ExcelFile { get; set; }
        public string FileName { get; set; }
    }
}
