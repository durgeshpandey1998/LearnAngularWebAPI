using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnAngularWebAPI.Model
{
    public class UserModel
    {
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phoneno { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
