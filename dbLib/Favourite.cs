using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbLib
{
    public class Favourite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PostId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User MyUser { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual Post MyPost { get; set; }
    }
}
