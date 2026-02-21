using Microsoft.AspNetCore.Http;

namespace ProjectIdentityEmail.Dtos
{
    public class UserEditDto
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }

        // Hakkımda alanını buraya ekliyoruz, ? ile boş geçilebilir (nullable) yapıyoruz.
        public string? About { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // Veritabanındaki dosya adını tutar
        public string? ImageUrl { get; set; }

        // Dosyanın kendisini transfer etmek için kullanılır
        public IFormFile? Image { get; set; }
    }
}