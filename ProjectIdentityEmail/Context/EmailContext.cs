using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectIdentityEmail.Entities;

namespace ProjectIdentityEmail.Context
{
    public class EmailContext : IdentityDbContext<AppUser>
    {
        // 1. ÖNEMLİ: Constructor ekleyin (EF Core'un nesne oluşturabilmesi için)
        public EmailContext()
        {
        }

        public EmailContext(DbContextOptions<EmailContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Bağlantı dizesini güvenli hale getirdik
            optionsBuilder.UseSqlServer("Server=DESKTOP-BMACQ1D\\SQLEXPRESS;initial catalog=Project2EmailDb;integrated security=true;TrustServerCertificate=True;");
        }
        
        // 2. ÇOK ÖNEMLİ: Identity yapılandırmasını sisteme dahil edin
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Bu satırı sakın silmeyin! 
            // base.OnModelCreating(builder) Identity tablolarının (AspNetUsers vb.) 
            // temel yapısını kuran koddur.
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Education> Educations { get; set; }
    }
}
