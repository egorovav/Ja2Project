//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ja2DataDb
{
    using System;
    using System.Collections.Generic;
    
    public partial class DataInfo
    {
        public DataInfo()
        {
            this.SlfFile = new HashSet<SlfFile>();
        }
    
        public int DataInfoId { get; set; }
        public string ProduserName { get; set; }
        public int MajorVerson { get; set; }
        public int MinorVersion { get; set; }
        public Nullable<int> Revision { get; set; }
        public Nullable<int> Build { get; set; }
        public Nullable<System.DateTime> BuildDate { get; set; }
        public string Local { get; set; }
        public string Localizer { get; set; }
    
        public virtual ICollection<SlfFile> SlfFile { get; set; }
    }
}
