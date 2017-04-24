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
    
    public partial class SlfFile
    {
        public SlfFile()
        {
            this.SlfRecord = new HashSet<SlfRecord>();
            this.SlfRecordJsd = new HashSet<SlfRecordJsd>();
            this.SlfRecordStci = new HashSet<SlfRecordStci>();
            this.SlfRecordText = new HashSet<SlfRecordText>();
        }
    
        public int SlfFileId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string LibName { get; set; }
        public string PathToLibrary { get; set; }
        public int EntriesCount { get; set; }
        public int Used { get; set; }
        public short Sort { get; set; }
        public short Version { get; set; }
        public byte ContainsSubDirectories { get; set; }
        public int Reserved { get; set; }
        public int DataInfoId { get; set; }
    
        public virtual ICollection<SlfRecord> SlfRecord { get; set; }
        public virtual ICollection<SlfRecordJsd> SlfRecordJsd { get; set; }
        public virtual ICollection<SlfRecordStci> SlfRecordStci { get; set; }
        public virtual DataInfo DataInfo { get; set; }
        public virtual ICollection<SlfRecordText> SlfRecordText { get; set; }
    }
}
