﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLParserDB
{
    public class JoinSchema
    {
        private string m_tableNameC;

        public string TableName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public Dictionary<string, List<Tuple<string, string>>> Data { get; set; }
        public const int EndPosition = -1;
        public int Position { get; set; }

        public JoinSchema()
        {
            TableName = "";
            Attributes = new Dictionary<string, string>();
            Data = new Dictionary<string, List<Tuple<string, string>>>();
            Position = 0;
        }

        public JoinSchema(string m_tableNameC)
        {
            TableName = m_tableNameC;
            Attributes = new Dictionary<string, string>();
            Data = new Dictionary<string, List<Tuple<string, string>>>();
            Position = 0;
        }

        public bool IsDone() { return Position == EndPosition; }
        public bool Advance()
        {
            if (Position == Data.Count - 1 || Position == EndPosition)
            {
                Position = EndPosition;
                return false;
            }
            Position++;
            return true;
        }

    }
}
