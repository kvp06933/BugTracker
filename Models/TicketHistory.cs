using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        //Id, Property, Old Value, New Value, Created Date, Updated Date, Description, Ticket, User
        public int Id { get; set; }

        [Required]
        [DisplayName("Ticket")]
        public int TicketId { get; set; }
        [Required]
        [DisplayName("Team Member")]
        public string UserId { get; set; }

        [DisplayName("Updated Item")]
        public string Property { get; set; }

        [DisplayName("Previous")]
        public string OldValue { get; set; }

        [DisplayName("Current")]
        public string NewValue { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }
        

        [DisplayName("Description of Change")]
        public string Description { get; set; }

        //--Navigational Properties--//
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
