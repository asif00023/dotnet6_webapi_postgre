namespace matelso.dbmodels
{
    public class Contact
    {
        public int Id { get; set; }
        public string Salutation { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Displayname { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime CreationTimestamp { get; set; }//= DateTime.Now;
        public DateTime LastChangeTimestamp { get; set; }=DateTime.Now;
        //public string? NotifyHasBirthdaySoon { get; set; }
        public string Email { get; set; }
        public string? Phonenumber { get; set; }
    }
}
