using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
    public class ProjectViewModel
    {
        public int? Id { get; set; }
        public  ProjectViewModel()
        {
            provinces = new List<Province>();
            cities = new List<City>();
            avenues = new List<Avenue>();
            categories = new List<Category>();
        }


        [Required]
        public string ProjectName { get; set; }
        [Required]
        public string Client { get; set; }
        public string ProvinceName { get; set; }
        public List<Province> provinces { get; set; }
        public string CityName { get; set; }
        public List<City> cities { get; set; }
        public string AvenueName { get; set; }
        public List<Avenue> avenues { get; set; }
        public string CategoryName{ get; set; }
		public List<Category> categories { get; set; }

		public DateTime CreatedAt { get; set; }
        public string? ImageFileNames { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
