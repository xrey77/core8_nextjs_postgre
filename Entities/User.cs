using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core8_nextjs_postgre.Entities
{
    public class User {

        [Key]
        [Column("id")]
        public int Id {get; set;}

        [Column("lastname", TypeName ="varchar")]
        public string LastName {get; set;}

        [Column("firstname", TypeName ="varchar")]
        public string FirstName {get; set;}

        [Column("username", TypeName ="varchar")]
        public string UserName {get; set;}

        [Column("password", TypeName ="varchar")]
        public string Password {get; set;}

        [Column("email", TypeName ="varchar")]
        public string Email { get; set; }

        [Column("mobile", TypeName ="varchar")]
        public string Mobile { get; set; }

        [Column("roles", TypeName ="varchar")]        
        public string Roles { get; set; }

        [Column("isactivated")]
        public int Isactivated {get; set;}

        [Column("isblocked")]
        public int Isblocked {get; set;}

        [Column("mailtoken")]
        public int Mailtoken {get; set;}

        [Column("qrcodeurl", TypeName ="varchar")]
        public string Qrcodeurl {get; set;}

        [Column("profilepic", TypeName ="varchar")]
        public string Profilepic {get; set;}

        [Column("secretkey")]        
        public string Secretkey  {get; set;}

        [Column("createdAt", TypeName ="timestamp")]
        public DateTime CreatedAt  {get; set;}

        [Column("updatedAt", TypeName ="timestamp")]
        public DateTime UpdatedAt  {get; set;}
    }
}