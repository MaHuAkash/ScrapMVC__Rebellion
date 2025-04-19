using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
    public class CategoryViewModel
    {
        public int? Id { get; set; }

        [Required]
        [DisplayName("NameOfCategory")]
        public string CategoryName { get; set; }

    }
}
