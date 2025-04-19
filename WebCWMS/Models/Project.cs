using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebCWMS.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

		[Required]
        public string ProjectName { get; set; }
        [Required]
        public string Client { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int AvenueId { get; set; }
        public int CategoryId { get; set; }

		[NotMapped]
        public List<IFormFile> ImageFiles { get; set; }
        public string? ImageFileNames { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }

    }

}
