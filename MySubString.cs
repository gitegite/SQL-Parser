using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLParserDB
{
    public partial class MySubString
    {
        public static string SubString(string s, int start, int end)
        {
            return s.Substring(start, end - start + 1);
        }
    }
}
