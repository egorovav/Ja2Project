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
    
    public partial class SlfRecordStci
    {
        public SlfRecordStci()
        {
            this.StciIndexed = new HashSet<StciIndexed>();
            this.StciRecordStciFlag = new HashSet<StciRecordStciFlag>();
            this.StciRgb = new HashSet<StciRgb>();
        }
    
        public long SlfRecordId { get; set; }
        public long SlfRecordHeaderId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int UserId { get; set; }
        public int SlfFileId { get; set; }
        public string ID { get; set; }
        public long OriginalSize { get; set; }
        public long StoredSize { get; set; }
        public long TransparentValue { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public byte Depth { get; set; }
        public int AppDataSize { get; set; }
        public byte[] Unused { get; set; }
    
        public virtual SlfFile SlfFile { get; set; }
        public virtual ICollection<StciIndexed> StciIndexed { get; set; }
        public virtual ICollection<StciRecordStciFlag> StciRecordStciFlag { get; set; }
        public virtual ICollection<StciRgb> StciRgb { get; set; }
        public virtual SlfRecordHeader SlfRecordHeader { get; set; }
    }
}
