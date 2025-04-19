using Microsoft.CodeAnalysis;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
	public class ProvinceViewModel
	{
		public int? Id { get; set; }

		[Required]
		[DisplayName("NameOfProvience")]
		public string ProvinceName { get; set; }


	}
}
