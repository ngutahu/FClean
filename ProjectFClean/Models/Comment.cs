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
    
    public partial class Comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            this.RepliComments = new HashSet<RepliComment>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> HID { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> CDate { get; set; }
        public Nullable<int> AccountID { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual Housekeeper Housekeeper { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RepliComment> RepliComments { get; set; }
    }
}
