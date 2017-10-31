using System.ComponentModel.DataAnnotations;

namespace AltSourceBankAppAPI.Models
{
    /* Our user */
    public class Users
    {
        [Key]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters  long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /* We will also assume a bank collects a lot more information
         * about users, but we sure won't here 
         */
        public string API_KEY { get; set; }
        public double Balance { get; set; }
    }
}
