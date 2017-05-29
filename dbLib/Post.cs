using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbLib
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(200)]
        public string ImagePath { get; set; }

        [Required]
        [StringLength(4000)]
        public string Text { get; set; }

        [Required]
        public DateTime PostDate { get; set; }
    }
}
