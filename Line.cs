using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextComparer
{
    public class Line
    {
        public enum LineAction
        {
            Added,
            Deleted,
            Modified
        }

        public string text { get; set; }
        public int? lineNumber { get; set; }
        public LineAction lineAction { get; set; }
    }
}
