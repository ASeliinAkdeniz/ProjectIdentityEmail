using Microsoft.AspNetCore.Identity;

        namespace ProjectIdentityEmail.Entities
    {
        public class AppUser : IdentityUser
        {
            public string? Name { get; set; }
            public string? SurName { get; set; }
            public string? ImageUrl { get; set; }
            public string? About { get; set; } // Uzun hakkımda yazısı
            public string? Title { get; set; } // Örn: Software Developer
            public DateTime BirthDate { get; set; } // Yaş hesabı için
            public string? City { get; set; }
            public string? Website { get; set; }
            public string? ConfirmCode { get; set; }

            // İlişkili tablolar (Birden fazla olabileceği için bunlar ayrı sınıf)
            public List<Skill> Skills { get; set; } //
            public List<Education> Educations { get; set; }
        }
    }


