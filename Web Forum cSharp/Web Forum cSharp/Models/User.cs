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
    
    public partial class User
    {
        [Required]
        [Display (Name="User Name")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    
        public virtual UserDetail UserDetail { get; set; }
    }
}