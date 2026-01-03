using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core8_nextjs_postgre.Entities
{
    [Table("users")]
    public class User {

        [Key]
        [Column("id")]
        public int Id {get; set;}

        public string LastName {get; set;}
        public string FirstName {get; set;}
        public string UserName {get; set;}
        public string Password {get; set;}
        public string Email { get; set; }
        public string Mobile { get; set; }

        [Column("roles_id")] 
        public int RolesId { get; set; } 

        public int Isactivated {get; set;}
        public int Isblocked {get; set;}
        public int Mailtoken {get; set;}

        [Column("qrcodeurl", TypeName ="text")]
        public string Qrcodeurl {get; set;}

        public string Profilepic {get; set;}
        public string Secretkey  {get; set;}

        [Column("created_at")]
        public DateTime? CreatedAt {get; set;}

        [Column("updated_at")]
        public DateTime? UpdatedAt {get; set;}

        public ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}