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
    
    public partial class StciRecordStciFlag
    {
        public long StciRecordsFlagsId { get; set; }
        public long RecordId { get; set; }
        public byte FlagId { get; set; }
    
        public virtual SlfRecordStci SlfRecordStci { get; set; }
        public virtual StciFlags StciFlags { get; set; }
    }
}
