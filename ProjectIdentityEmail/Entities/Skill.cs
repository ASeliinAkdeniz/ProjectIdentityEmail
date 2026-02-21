namespace ProjectIdentityEmail.Entities
{
    public class Skill
    {
        public int Id { get; set; } 
        public string Title { get; set; } 
        public int Value { get; set; }
        public string AppUserID { get; set; }
        public AppUser AppUser { get; set; }


    }
}
