//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectFClean.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Feedback
    {
        public int FbID { get; set; }
        public string Comment { get; set; }
        public Nullable<int> HID { get; set; }
        public Nullable<int> RID { get; set; }
        public string Rate { get; set; }
        public System.DateTime DateTimeCreated { get; set; }
        public bool IsCompleted { get; set; }
        public byte[] ImageURL { get; set; }
        public string FeedbackType { get; set; }
        public Nullable<int> CID { get; set; }
    
        public virtual Compact Compact { get; set; }
    }
}
