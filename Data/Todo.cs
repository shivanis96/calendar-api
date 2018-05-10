using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Data
{
    public class Todo
    {
        public int Id { get; set; }
        [ForeignKey("Id")]
        public int UserId { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public DateTime Pubdate { get; set; }
    }
}
