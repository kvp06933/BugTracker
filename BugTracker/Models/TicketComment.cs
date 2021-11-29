using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        [Required]
        public string UserId { get; set; }

        [DisplayName("Member Comment")]
        public string Comment { get; set; }

        [DisplayName("Comment Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        //--Navigation Properties--//

        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
