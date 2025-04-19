using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebCWMS.Models
{

	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<Blog> Blogs { get; set; }
		public DbSet<City> Cities { get; set; }
		public DbSet<Province> Provinces { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Reply> Replys { get; set; }
		public DbSet<Avenue> Avenues { get; set; }
		public DbSet<Project> Projects { get; set; }
		public DbSet<Category> Categories { get; set; }
        public DbSet<TimeRecord> TimeRecords { get; set; }


    }
}
