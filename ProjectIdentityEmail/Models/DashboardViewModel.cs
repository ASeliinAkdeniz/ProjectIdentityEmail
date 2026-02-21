using ProjectIdentityEmail.Entities;
using System.Collections.Generic;

namespace ProjectIdentityEmail.Models
{
    public class DashboardViewModel
    {
        public int WorkCount { get; set; }
        public int FamilyCount { get; set; }
        public int FriendsCount { get; set; }
        public int TotalIncomingCount { get; set; }
        public int InboxCount { get; set; }
        public int SentCount { get; set; }
        public int StarredCount { get; set; }
        public int TrashCount { get; set; }
        public int UnreadCount { get; set; }
        public List<Message> LastFiveMessages { get; set; }
    }
}