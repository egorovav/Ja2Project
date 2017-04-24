using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ja2DataDb
{
    public class JsdCell : IComparable
    {
        public long JsdTileId { get; set; } 
        public byte RowNumber { get; set; }
        public byte CellNumber { get; set; }
        public byte Value { get; set; }


        public int CompareTo(object obj)
        {
            JsdCell _cell = obj as JsdCell;
            int _result = 0;

            if(_cell != null)
            {
                _result = this.RowNumber.CompareTo(_cell.RowNumber);
                if(_result == 0)
                    _result = this.CellNumber.CompareTo(_cell.CellNumber);
            }

            return _result;
        }
    }
}
