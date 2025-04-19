using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCWMS.ViewModel;

namespace WebCWMS.Models
{
	public class Blog
	{
		[Key]
		public int Id { get; set; }



		[Required]
		[DisplayName("Date & Time")]
		public DateTime Date { get; set; }


		[Required]
		[MaxLength(200)]
		public string Title { get; set; }

		[Required]
		[MaxLength(5000)]
		public string Description { get; set; }

		public string? ImageName { get; set; }

		[NotMapped]
		[DisplayName("Upload File")]
		public IFormFile ImageFile { get; set; }

	}
}
