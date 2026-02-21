using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectIdentityEmail.Context;
using ProjectIdentityEmail.Entities;
using ProjectIdentityEmail.Models;
using System.Linq;
using System.Threading.Tasks;

public class SidebarMessageCount : ViewComponent
{
    private readonly EmailContext _context;
    private readonly UserManager<AppUser> _userManager;
    public SidebarMessageCount(EmailContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return View(new SidebarViewModel());
        }

        var userName = User.Identity.Name;

        var user = await _userManager.FindByNameAsync(userName);
        var userEmail = user?.Email ?? "";

        var model = new SidebarViewModel
        {

            Inbox = _context.Messages.Count(x => x.ReceiverEmail == userEmail && x.IsTrash == false),

            Sendbox = _context.Messages.Count(x => x.SenderEmail == userEmail && x.IsTrash == false),

            Starred = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.IsStarred == true && x.IsTrash == false),
            Trash = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.IsTrash == true),

            Work = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.FolderName == "Is" && x.IsTrash == false),
            Family = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.FolderName == "Aile" && x.IsTrash == false),
            Friends = _context.Messages.Count(x => (x.ReceiverEmail == userEmail || x.SenderEmail == userEmail) && x.FolderName == "Arkadaslar" && x.IsTrash == false)
        };

        return View(model);
    }
}

public class SidebarViewModel
{
    public int Inbox { get; set; }
    public int Sendbox { get; set; }
    public int Starred { get; set; }
    public int Trash { get; set; }
    public int Work { get; set; }
    public int Family { get; set; }
    public int Friends { get; set; }
}