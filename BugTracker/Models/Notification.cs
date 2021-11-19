using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Notification //Id, title, message, created, viewed - bool, navigation - notifcationType, TicketId?, projectid?, senderid, recipientid 
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Subject")]
        public string Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Message { get; set; }

        [DisplayName("Date Created")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Has been viewed")]
        public bool IsViewed { get; set; }

        [DisplayName("Ticket")]
        public int? TicketId { get; set; }

        [DisplayName("Project")]
        public int? ProjectId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string SenderId { get; set; }

        [Required]
        [DisplayName("Recipient")]
        public string RecipientId { get; set; }

        //--Navigational Properties--//

        public virtual Project Project { get; set; }
        public virtual BTUser Sender { get; set; }
        public virtual BTUser Recipient { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual NotificationType NotificationType { get; set; }



    }
}
