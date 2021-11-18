using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Project
    {
        //Id, name, description, createddate, startdate, enddate, image, company, archived, company, project priority, collections - members, tickets

        public int Id { get; set; }
        [Required]
        [DisplayName("Project Name")]
        [StringLength(50)]
        public string Name { get; set; }

        
        [DisplayName("Description")]
        [StringLength(2500)]
        public string Description { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset? StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset? EndDate { get; set; }

        [DisplayName("Created Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [DisplayName("File Name")]
        public string ImageName { get; set; }

        [DisplayName("File Extension")]
        public string ImageType { get; set; }

        [DisplayName("Project Image")]
        public byte ImageData { get; set; }

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Priority")]
        public int? ProjectPriorityId { get; set; }

        public bool Archived { get; set; }

        //--Navigational Properties--//
        public virtual Company Company { get; set; }
        public virtual ProjectPriority ProjectPriority { get; set; }
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
