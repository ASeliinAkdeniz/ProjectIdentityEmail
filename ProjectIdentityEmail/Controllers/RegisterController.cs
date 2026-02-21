using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using ProjectIdentityEmail.Dtos;
using ProjectIdentityEmail.Models;
using Newtonsoft.Json;
using ProjectIdentityEmail.Entities;
using Microsoft.AspNetCore.Identity;

public class RegisterController : Controller
{
    private readonly UserManager<AppUser> _userManager;

    public RegisterController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRegisterDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createDto);
        }

        // --- 1. MAİL ATMADAN ÖNCE IDENTITY KONTROLLERİNİ YAP ---

        // A. Kullanıcı adının veya Email'in kullanılıp kullanılmadığını kontrol et
        var existingUserByName = await _userManager.FindByNameAsync(createDto.UserName);
        if (existingUserByName != null)
        {
            ModelState.AddModelError("", "Bu kullanıcı adı zaten alınmış. Lütfen başka bir kullanıcı adı seçin.");
            return View(createDto);
        }

        var existingUserByEmail = await _userManager.FindByEmailAsync(createDto.Email);
        if (existingUserByEmail != null)
        {
            ModelState.AddModelError("", "Bu e-posta adresi zaten kullanılıyor. Lütfen başka bir e-posta adresi girin.");
            return View(createDto);
        }

        // B. Şifrenin kurallara (Identity ayarlarına) uygun olup olmadığını test et
        foreach (var validator in _userManager.PasswordValidators)
        {
            var result = await validator.ValidateAsync(_userManager, null, createDto.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                // Şifre hatalıysa mail atma, sayfaya geri dön!
                return View(createDto);
            }
        }

        // --- 2. HER ŞEY UYGUNSA MAİL ATMA İŞLEMİNE GEÇ ---

        Random random = new Random();
        int code = random.Next(100000, 999999);

        try
        {
            MimeMessage mimeMessage = new MimeMessage();
            MailboxAddress mailboxAddressFrom = new MailboxAddress("Mendy Admin", "akdenizasliselin@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", createDto.Email);
            mimeMessage.To.Add(mailboxAddressTo);

            mimeMessage.Subject = "Mendy Panel - Kayıt Doğrulama Kodu";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Merhaba {createDto.Name},\n\nKayıt işlemini tamamlamak için doğrulama kodunuz: {code}";
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("akdenizasliselin@gmail.com", "zlzf rkzy puuk tneh");
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Mail gönderilirken bir hata oluştu. Lütfen e-posta adresinizin doğruluğunu kontrol edin.");
            return View(createDto);
        }

        // --- 3. KODU VE KULLANICIYI TEMPDATA'YA AT VE MODALI AÇ ---

        TempData["UserDto"] = JsonConvert.SerializeObject(createDto);
        TempData["Code"] = code;

        ViewBag.ShowModal = true;
        return View(createDto);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmMailCode(int InputCode)
    {
        var storedCode = (int?)TempData["Code"];
        var userDtoJson = (string)TempData["UserDto"];

        if (storedCode == null || userDtoJson == null)
        {
            return RedirectToAction("CreateUser");
        }

        var userDto = JsonConvert.DeserializeObject<CreateUserRegisterDto>(userDtoJson);

        if (InputCode == storedCode)
        {
            // --- KOD DOĞRU! KAYDI GERÇEKLEŞTİR ---
            AppUser appUser = new AppUser()
            {
                Name = userDto.Name,
                SurName = userDto.SurName,
                Email = userDto.Email,
                UserName = userDto.UserName
            };

            // Biz tüm testleri (şifre, email vs.) yukarıda yaptığımız için burası büyük ihtimalle Succeeded dönecektir.
            var result = await _userManager.CreateAsync(appUser, userDto.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("UserLogin", "Login"); // Login Controllerındaki View isminin doğruluğunu kontrol et
            }
            else
            {
                // Nadir de olsa kayıt anında bir hata çıkarsa (örn: veritabanı bağlantısı koparsa)
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View("CreateUser", userDto);
            }
        }
        else
        {
            // --- KOD YANLIŞ ---
            ViewBag.ShowModal = true;
            ViewBag.Error = "Girdiğiniz kod hatalı, lütfen tekrar deneyin.";

            // Verileri koru 
            TempData["Code"] = storedCode;
            TempData["UserDto"] = userDtoJson;

            return View("CreateUser", userDto);
        }
    }
}