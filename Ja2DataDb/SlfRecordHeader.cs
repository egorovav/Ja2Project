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
    
    public partial class SlfRecordHeader
    {
        public SlfRecordHeader()
        {
            this.SlfRecord = new HashSet<SlfRecord>();
            this.SlfRecordJsd = new HashSet<SlfRecordJsd>();
            this.SlfRecordStci = new HashSet<SlfRecordStci>();
            this.SlfRecordText = new HashSet<SlfRecordText>();
        }
    
        public long SlfRecordHeaderId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public long Offset { get; set; }
        public long Length { get; set; }
        public byte State { get; set; }
        public byte Reserved { get; set; }
        public System.DateTime FileTime { get; set; }
        public int Reserved2 { get; set; }
        public string FileNameExtention { get; set; }
        public string FileNameWithoutExtention { get; set; }
    
        public virtual ICollection<SlfRecord> SlfRecord { get; set; }
        public virtual ICollection<SlfRecordJsd> SlfRecordJsd { get; set; }
        public virtual ICollection<SlfRecordStci> SlfRecordStci { get; set; }
        public virtual ICollection<SlfRecordText> SlfRecordText { get; set; }
    }
}
