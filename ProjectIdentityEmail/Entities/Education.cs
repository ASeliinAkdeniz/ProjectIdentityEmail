namespace ProjectIdentityEmail.Entities
{
    public class Education
    {
        public int Id { get; set; }
        public string SchoolName { get; set; }
        public string Department { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string AppUserID { get; set; }
        public AppUser AppUser { get; set; }
    }
}
