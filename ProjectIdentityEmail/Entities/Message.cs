namespace ProjectIdentityEmail.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string ReceiverEmail { get; set; }
        public string SenderEmail { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsStatus { get; set; } // Bunu "Okundu/Okunmadı" bilgisi olarak kullanırız.

        // --- YENİ EKLENMESİ GEREKENLER ---

        // 1. Timeline için Resim Yolu (Boş geçilebilir olsun, belki resimsiz mail atılır)
        public string? ImageUrl { get; set; }

        // 2. Mesajı Yıldızlamak için (Favori)
        public bool IsStarred { get; set; }

        // 3. Mesajı Sildiğimizde Çöp Kutusuna taşımak için
        public bool IsTrash { get; set; }
        public string? FolderName { get; set; }
    }
}