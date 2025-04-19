using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCWMS.Models;

namespace WebCWMS.ViewModel
{
	public class BlogViewModel
	{
		public int? Id { get; set; }

		public BlogViewModel()
		{
			Comments= new List<CommentViewModel>();
		}

		[Required]
		[DisplayName("Date & Time")]
		public DateTime Date { get; set; }
		
		public string Title { get; set; }
		public string Description { get; set; }
		public int CommentId { get; set; }
		public string? ImageName { get; set; }
		public IFormFile ImageFile { get; set; }
		public List<CommentViewModel> Comments { get; set; }

	}
}
