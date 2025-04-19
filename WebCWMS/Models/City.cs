using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebCWMS.Models
{
	public class City
	{
        [Key]
        public int Id { get; set; }

		[Required]
		[DisplayName("NameofCity")] 
		public string CityName { get; set; }
		public int ProvinceId { get; set; }



	}
}
