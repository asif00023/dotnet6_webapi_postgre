using System.ComponentModel.DataAnnotations;

namespace matelso.dbmodels
{
    public class Contact
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        public string Salutation { get; set; }
        [Required]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [MinLength(2)]
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreationTimestamp { get; set; }//= DateTime.Now;
        public DateTime LastChangeTimestamp { get; set; }=DateTime.Now;
        //public string? NotifyHasBirthdaySoon { get; set; }
        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$")]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
