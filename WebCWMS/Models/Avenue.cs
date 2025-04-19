using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebCWMS.Models
{
	public class Avenue
	{
        [Key]
        public int Id { get; set; }
        public string AvenueName { get; set; }
		[Required]
		[DisplayName("City")]
		public int CityId { get; set; }
		[Required]
		[DisplayName("Province")]

		public int ProvinceId { get; set; }


	}
}
