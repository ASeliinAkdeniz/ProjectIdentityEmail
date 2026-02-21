using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectIdentityEmail.Context;
using ProjectIdentityEmail.Entities;

namespace ProjectIdentityEmail.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _context;

        public ProfileController(UserManager<AppUser> userManager, EmailContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            // 1. Adım: Kullanıcıyı UserManager ile bul.
            // UserManager, Identity tablolarını yönettiği için Email'i KESİN getirir.
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Eğer kullanıcı bulunamazsa logine at (Güvenlik önlemi)
            if (user == null) return RedirectToAction("UserLogin", "Login");

            // 2. Adım: "Explicit Loading" ile ilişkili tabloları (Skills, Educations) bu user nesnesine yüklüyoruz.
            // Bu işlem, elimizdeki dolu user nesnesinin içine yetenekleri ve eğitimleri doldurur.
            await _context.Entry(user).Collection(x => x.Skills).LoadAsync();
            await _context.Entry(user).Collection(x => x.Educations).LoadAsync();

            // 3. Adım: View'a artık içinde hem Email'i hem de Yetenekleri olan user'ı gönderiyoruz.
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAllProfile(
            string Name,
            string SurName,
            string PhoneNumber,
            string About,
            List<Skill> Skills,
            List<Education> Educations,
            IFormFile Image)
        {
            // Veritabanındaki kullanıcıyı çek
            var user = await _userManager.Users
                .Include(x => x.Skills)
                .Include(x => x.Educations)
                .FirstOrDefaultAsync(x => x.UserName == User.Identity.Name);

            if (user == null) return RedirectToAction("UserLogin", "Login");

            // 1. Manuel Eşleştirme (Email GÜNCELLENMİYOR, Korunuyor)
            user.Name = Name;
            user.SurName = SurName;
            user.PhoneNumber = PhoneNumber;
            user.About = About;

            // 2. Resim Yükleme (Dosya Geldiyse)
            if (Image != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(Image.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = Path.Combine(resource, "wwwroot/images", imageName);

                using (var stream = new FileStream(saveLocation, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }
                user.ImageUrl = imageName;
            }

            // 3. Yetenekleri Güncelle
            if (Skills != null)
            {
                foreach (var item in Skills)
                {
                    if (item.Id > 0) // Mevcutları güncelle
                    {
                        var existing = user.Skills.FirstOrDefault(s => s.Id == item.Id);
                        if (existing != null) { existing.Title = item.Title; existing.Value = item.Value; }
                    }
                    else if (!string.IsNullOrEmpty(item.Title)) // Yeni ekle
                    {
                        item.AppUserID = user.Id;
                        _context.Skills.Add(item);
                    }
                }
            }

            // 4. Eğitimleri Güncelle
            if (Educations != null)
            {
                foreach (var item in Educations)
                {
                    if (item.Id > 0)
                    {
                        var existing = user.Educations.FirstOrDefault(e => e.Id == item.Id);
                        if (existing != null)
                        {
                            existing.SchoolName = item.SchoolName;
                            existing.Department = item.Department;
                            existing.StartDate = item.StartDate;
                            existing.EndDate = item.EndDate;
                        }
                    }
                    else if (!string.IsNullOrEmpty(item.SchoolName))
                    {
                        item.AppUserID = user.Id;
                        _context.Educations.Add(item);
                    }
                }
            }

            // 5. Değişiklikleri Kaydet
            await _userManager.UpdateAsync(user); // Identity bilgilerini güncelle
            await _context.SaveChangesAsync();    // İlişkili tabloları güncelle

            return RedirectToAction("UserProfile");
        }
    }
}