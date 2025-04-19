using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
	public class CityViewModel
	{
		public CityViewModel()
		{
			provinces = new List<Province>();
		}
		public int? Id { get; set; }

		[Required]
		[DisplayName("City")]
		public string CityName { get; set; }
		public int ProvinceId { get; set; }
		public string ProvinceName { get; set; }
		public List<Province> provinces { get; set; }




	}
}
