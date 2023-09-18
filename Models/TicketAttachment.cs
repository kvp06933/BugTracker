using BugTracker.Extensions;
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
    public class TicketAttachment
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Ticket Task")]
        public int? TicketTaskId { get; set; }

        
        [DisplayName("Team Member")]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Attachment Date")]
        public DateTimeOffset Created { get; set; }

        [NotMapped]
        [DisplayName("Select a file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile Attachment { get; set; }

        public byte[] FileData { get; set; }

        [DisplayName("File Name")]
        public string FileName { get; set; }

        [DisplayName("Attachment Extension")]
        public string FileType { get; set; }

        [DisplayName("Attachment Description")]
        public string Description { get; set; }

        //--Navigational Properties--//

        public virtual Ticket Ticket { get; set; }

        public virtual BTUser User { get; set; }






    }
}
