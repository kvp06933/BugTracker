using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool Archived { get; set; }
        public bool ArchivedByProject { get; set; }

        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketStatusId { get; set; }
        public int TicketPriorityId { get; set; }
        public string UserId { get; set; }
        public string DevloperId { get; set; }

        //--Navigational Properties--//
        public virtual ICollection<TicketComment> Comment { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachment { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<TicketTask> Task { get; set; } = new HashSet<TicketTask>();

    }
}
