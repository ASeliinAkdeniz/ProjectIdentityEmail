using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectIdentityEmail.Context; 
using ProjectIdentityEmail.Entities;
using ProjectIdentityEmail.Models; 
using System.Linq;
using System.Threading.Tasks;

namespace ProjectIdentityEmail.Controllers
{
    public class DashboardController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;


        public DashboardController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserLogin", "Login");
            }

            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);
            var userEmail = user?.Email ?? "";


            var workCount = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.FolderName == "Is" && x.IsTrash == false);

            var familyCount = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.FolderName == "Aile" && x.IsTrash == false);

            var friendsCount = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.FolderName == "Arkadaslar" && x.IsTrash == false);

        
            var totalIncoming = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.IsTrash == false);

   
            var totalOutgoing = _context.Messages.Count(x => x.SenderEmail == userEmail && x.IsTrash == false);
            var unreadMessages = _context.Messages.Count(x => x.ReceiverEmail == userEmail && x.IsStatus == false && x.IsTrash == false);
            var starredMessages = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.IsStarred == true && x.IsTrash == false);
            var lastMessages = _context.Messages
                .Where(x => x.ReceiverEmail == userEmail && x.IsTrash == false)
                .OrderByDescending(x => x.SendDate)
                .Take(5)
                .ToList();

            var model = new DashboardViewModel
            {
                WorkCount = workCount,
                FamilyCount = familyCount,
                FriendsCount = friendsCount,
                TotalIncomingCount = totalIncoming,
                LastFiveMessages = lastMessages,

                SentCount = totalOutgoing,
                UnreadCount = unreadMessages,
                StarredCount = starredMessages
            };

            return View(model);
        }
    }
}