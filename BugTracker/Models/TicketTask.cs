using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class TicketTask
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int TicketId { get; set; }
        public int TicketStatusId { get; set; }
        public int TaskTypeId { get; set; }

        public virtual TicketStatus TaskStatus { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual TicketType TaskType { get; set; }
        public virtual ICollection<TicketAttachment> TaskAttachments { get; set; } = new HashSet<TicketAttachment>();
    }
}
