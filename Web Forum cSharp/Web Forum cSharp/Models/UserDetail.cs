//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Web_Forum_cSharp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    
    public partial class UserDetail
    {
        public UserDetail()
        {
            this.Threads = new HashSet<Thread>();
        }
    
        public string UserName { get; set; }
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public string ImgPath { get; set; }
        public string Interest { get; set; }
    
        public virtual ICollection<Thread> Threads { get; set; }
        public virtual User User { get; set; }
    }
}
