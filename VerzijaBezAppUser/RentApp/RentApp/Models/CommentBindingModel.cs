using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RentApp.Models
{
    public class CommentBindingModel
    {
        public class CreateCommentBindingModel
        {
            [Required]
            [Display(Name = "ServiceId")]
            public int ServiceId { get; set; }
            
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Text")]
            public string Text { get; set; }
            
        }

        public class ClientComment
        {         
            public string User { get; set; }

            public string Text { get; set; }

            public DateTime DateTime { get; set; }

        }
    }
}