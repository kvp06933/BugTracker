using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Invite
    {
        //Id, Invitedate- like created, joindate, companytoken - type Guid, companyId, projectid, invitorId{BTUser}, inviteeId, inviteeEmail, inviteeFirstNAme, inviteeLasName, MEssage, IsValid

        public int Id { get; set; }
        [DisplayName("Date Sent")]
        [DataType(DataType.Date)]
        public DateTimeOffset InviteDate { get; set; }

        [DisplayName("Date Joined")]
        [DataType(DataType.Date)]
        public DateTimeOffset? JoinDate { get; set; }

        [DisplayName("Company Code")]
        public Guid CompanyToken { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string InviteeId { get; set; }

        [DisplayName("Invitee Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Invitee First Name")]
        public string FirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string LastName { get; set; }

        [DisplayName("Invite Message")]
        public string Message { get; set; }

        public bool IsValid { get; set; } //Once the invite is used the boolean will be false

        //--Navigational Properties--//

        public virtual Company Company { get; set; }
        public virtual Project Project { get; set; }
        public virtual BTUser Invitor { get; set; }
        public virtual BTUser Invitee { get; set; }

    }
}
