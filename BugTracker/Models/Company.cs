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
    public class Company
    {
        //Id, name, description, image

        //List - Members, projects, invites

        public int Id { get; set; }

        [Required]
        [DisplayName("Company Name")]
        public string Name { get; set; }

        [DisplayName("Company Description")]
        public string Description { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [DisplayName("File Name")]
        public string ImageName { get; set; }

        [DisplayName("File Extension")]
        public string ImageType { get; set; }

        [DisplayName("Company Image")]
        public byte ImageData { get; set; }

        //--Navigational Properties--//
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Project> Project { get; set; } = new HashSet<Project>();
        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();

    }
}
