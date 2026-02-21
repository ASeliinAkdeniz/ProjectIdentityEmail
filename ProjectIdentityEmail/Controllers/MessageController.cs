using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectIdentityEmail.Context; 
using ProjectIdentityEmail.Entities; 
using System.IO; 

namespace ProjectIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _environment; 


        public MessageController(EmailContext context, UserManager<AppUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }
        public async Task<IActionResult> Sendbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // SADECE ÇÖPTE OLMAYANLARI GETİR
            var values = _context.Messages
                .Where(x => x.SenderEmail == user.Email && x.IsTrash == false)
                .OrderByDescending(x => x.MessageId)
                .ToList();

            return View(values);
        }


        [HttpGet]
        public IActionResult SendMessage()
        {

            return View(new Message());
        }


        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message, IFormFile ImageFile)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (ImageFile != null)
            {
                var resource = Directory.GetCurrentDirectory(); 
                var extension = Path.GetExtension(ImageFile.FileName); 
                var imageName = Guid.NewGuid() + extension; 
                var saveLocation = resource + "/wwwroot/userimages/" + imageName; 

                using (var stream = new FileStream(saveLocation, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                message.ImageUrl = "/userimages/" + imageName; 
            }
            else
            {
                message.ImageUrl = null;
            }

            message.SenderEmail = user.Email; 
            message.SendDate = DateTime.Now; 
            message.IsStatus = false; 


            _context.Messages.Add(message);
            await _context.SaveChangesAsync();


            return RedirectToAction("Sendbox");
        }
        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // SADECE ÇÖPTE OLMAYANLARI (x.IsTrash == false) GETİR
            var values = _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && x.IsTrash == false)
                .OrderByDescending(x => x.MessageId)
                .ToList();

            return View(values);
        }
        [HttpGet]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message != null)
            {
                message.IsStatus = true; 
                await _context.SaveChangesAsync(); 
            }

            return Ok();
        }
        public IActionResult DeleteMessage(int id)
        {
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                _context.Messages.Remove(message); 
                _context.SaveChanges();
            }
            return RedirectToAction("Inbox");
        }

        public async Task<IActionResult> DeleteInboxMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                // ESKİ KOD (Bunu siliyoruz): 
                // _context.Messages.Remove(message); 

                // YENİ KOD (Bunu ekliyoruz):
                message.IsTrash = true; // Mesajı silme, sadece çöp kutusuna taşı
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Inbox");
        }

        public async Task<IActionResult> DeleteSendboxMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                // ESKİ KOD (Bunu siliyoruz):
                // _context.Messages.Remove(message);

                // YENİ KOD:
                message.IsTrash = true; // Mesajı silme, sadece çöp kutusuna taşı
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Sendbox");
        }
            [HttpGet]
        public async Task<IActionResult> ToggleStar(int id)
        {
            var message = _context.Messages.Find(id);
            if (message != null)
            {
                message.IsStarred = !message.IsStarred;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> StarredMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Login");

            var values = _context.Messages
                .Where(x => (x.ReceiverEmail == user.Email || x.SenderEmail == user.Email) && x.IsStarred == true)
                .OrderByDescending(x => x.MessageId)
                .ToList();

            return View(values);
        }
        // --- ÇÖP KUTUSU LİSTESİ ---
        [HttpGet]
        public async Task<IActionResult> Trash()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Hem Gelen hem Giden mesajlardan 'Çöp' olarak işaretlenenleri getir (IsTrash == true)
            // Not: Entity'de IsTrash alanı olduğunu varsayıyorum.
            var values = _context.Messages
                .Where(x => (x.ReceiverEmail == user.Email || x.SenderEmail == user.Email) && x.IsTrash == true)
                .OrderByDescending(x => x.MessageId)
                .ToList();

            return View(values);
        }

        // --- MESAJI GERİ YÜKLE (RESTORE) ---
        public async Task<IActionResult> RestoreMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsTrash = false; // Çöp kutusundan çıkar
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Trash");
        }

        // --- KALICI OLARAK SİL (PERMANENT DELETE) ---
        public async Task<IActionResult> DeleteFinal(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message); // Veritabanından tamamen sil
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Trash");
        }
        // --- MESAJI KLASÖRE ATAMA İŞLEMİ ---
        [HttpGet]
        public async Task<IActionResult> AssignFolder(int id, string folder)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.FolderName = folder; // "Is", "Aile" veya "Arkadaslar"
                await _context.SaveChangesAsync();
            }

            // İşlem bitince kullanıcının geldiği sayfaya (Inbox veya Sendbox) geri döndür
            string referer = Request.Headers["Referer"].ToString();
            return Redirect(string.IsNullOrEmpty(referer) ? "/Message/Inbox" : referer);
        }

        // --- İŞ MESAJLARI SAYFASI ---
        [HttpGet]
        public async Task<IActionResult> WorkMessages()
        {
            return await GetFolderMessages("Is", "İş Mesajları", "fa-briefcase", "text-primary", "#e1effe", "#1e429f");
        }

        // --- AİLE MESAJLARI SAYFASI ---
        [HttpGet]
        public async Task<IActionResult> FamilyMessages()
        {
            return await GetFolderMessages("Aile", "Aile Mesajları", "fa-home", "text-warning", "#fef3c7", "#d97706");
        }

        // --- ARKADAŞ MESAJLARI SAYFASI ---
        [HttpGet]
        public async Task<IActionResult> FriendMessages()
        {
            return await GetFolderMessages("Arkadaslar", "Arkadaş Mesajları", "fa-user-friends", "text-success", "#def7ec", "#03543f");
        }

        // KOD TEKRARINI ÖNLEMEK İÇİN YARDIMCI METOT
        private async Task<IActionResult> GetFolderMessages(string folderName, string title, string icon, string colorClass, string bgHex, string textHex)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Gelen veya giden fark etmeksizin, silinmemiş ve ilgili klasöre atanmış mesajları getir
            var values = _context.Messages
                .Where(x => (x.ReceiverEmail == user.Email || x.SenderEmail == user.Email)
                            && x.FolderName == folderName
                            && x.IsTrash == false)
                .OrderByDescending(x => x.MessageId)
                .ToList();

            // View tarafında tasarımı dinamik değiştirmek için ViewBag kullanıyoruz
            ViewBag.FolderTitle = title;
            ViewBag.FolderIcon = icon;
            ViewBag.FolderColorClass = colorClass;
            ViewBag.FolderBgHex = bgHex;
            ViewBag.FolderTextHex = textHex;

            return View("FolderMessages", values);
        }
    }
}