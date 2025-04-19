using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebCWMS.Models
{
	public class Province
	{
        [Key]
        public int Id { get; set; }

		[Required]
		[DisplayName("NameOfProvience")]
		public string ProvinceName { get; set; }
	}
}
