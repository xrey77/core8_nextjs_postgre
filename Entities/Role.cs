using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core8_nextjs_postgre.Entities
{

    [Table("roles")]
    public class Role {

        [Key]
        [Column("id")]
        public int Id {get; set;}

        public string Name {get; set;}

        public ICollection<User> Users { get; set; } = new List<User>();
    }
    

}