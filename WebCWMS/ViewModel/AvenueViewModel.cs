using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
	public class AvenueViewModel
	{
		public int? Id { get; set; }
		public string AvenueName { get; set; }
		public AvenueViewModel()
		{
			cities = new List<City>();
			provinces = new List<Province>();

		}
		
		public int CityId { get; set; }
		public string CityName { get; set; }
		public List<City> cities { get; set; }
		public int ProvinceId { get; set; }
		public string ProvinceName { get; set; }
		public List<Province> provinces { get; set; }


	}
}
