using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLParserDB
{
    public class Schema
    {
        private string m_tableNameC;

        public string TableName { get; set; }
        public Dictionary<string,string> Attributes { get; set; }
        public Dictionary<string, Dictionary<string,string>> Data { get; set; }

        public Schema()
        {
            TableName = "";
            Attributes = new Dictionary<string, string>();
            Data = new Dictionary<string, Dictionary<string,string>>();
        }

        public Schema(string m_tableNameC)
        {
            TableName = m_tableNameC;
            Attributes = new Dictionary<string, string>();
            Data = new Dictionary<string, Dictionary<string,string>>();
        }

    }
}
