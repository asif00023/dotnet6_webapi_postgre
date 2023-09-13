
using System.ComponentModel.DataAnnotations;

namespace matelso.viewmodels.RequestModel
{
    public class ContactReqestModel
    {
        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        public string Salutation { get; set; }
        [Required]
        [MinLength(2)]
        public string Firstname { get; set; }
        [Required]
        [MinLength(2)]
        public string Lastname { get; set; }
        public string? Displayname { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy-MM-dd}")]
        public DateTime? Birthdate { get; set; }
        
        public string? NotifyHasBirthdaySoon { get; set; }

        [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$")]
        public string Email { get; set; }
        public string? Phonenumber { get; set; }
    }


}
