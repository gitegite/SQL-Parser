using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
namespace SQLParserDB
{
    public class ParserProvider
    {
        string[] GetWhereClause(string p_command)
        {
            string temp = MySubString.SubString(p_command, p_command.IndexOf("where"), p_command.Length - 1);
            temp = temp.Replace("where", "");
            temp = temp.Replace(" ", "");
            return temp.Split('=');

        }
        void SortMergeJoin(JoinSchema target, Schema table1, Schema table2, string attr1, string attr2)
        {
            table1.Data = new Dictionary<string, Dictionary<string, string>>(table1.Data.OrderBy(x => x.Value[attr1]).ToDictionary(x => x.Key, x => x.Value));
            table2.Data = new Dictionary<string, Dictionary<string, string>>(table2.Data.OrderBy(x => x.Value[attr2]).ToDictionary(x => x.Key, x => x.Value));

            foreach (var record in table1.Data)
            {
                
            }
            
            Cartesian(target, table1, table2);
            foreach (var record in target.Data.Reverse())
            {
                var xx = record.Value.ToLookup(x => x.Item1, x => x.Item2)[attr1];
                var yy = record.Value.ToLookup(y => y.Item1, x => x.Item2)[attr2];
                var table1Attribute = xx.ElementAt(0);
                var table2Attribute = yy.Count() > 1 ? yy.ElementAt(1) : yy.ElementAt(0);
                if (!table1Attribute.Equals(table2Attribute))
                {
                    target.Data.Remove(record.Key);
                }

            }

        }

        bool ValidateValueAndType(string typeName, string value, int typeLength)
        {
            switch (typeName)
            {
                case "char":
                    if (value.Length - 2 != typeLength)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                case "varchar":
                    return value.Length - 2 > typeLength ? false : true;

                case "numeric":
                    int n;
                    return value.Length > typeLength && !int.TryParse(value, out n) ? false : true;
                case "bit":
                    int bit;
                    return value.Length > 1 && !int.TryParse(value, out bit) && (bit != 1 || bit != 0) ? false : true;
            }
            return false;
        }
        int GetAttributeLength(string attribute)
        {
            attribute = MySubString.SubString(attribute, attribute.IndexOf('(') + 1, attribute.IndexOf(')') - 1);

            return Convert.ToInt32(attribute);
        }
        string GetAttributeName(string attribute)
        {
            return MySubString.SubString(attribute, 0, attribute.IndexOf('(') - 1);

        }
        void Cartesian(JoinSchema target, Schema table1, Schema table2)
        {
            var cartesianed = new Dictionary<string, List<Tuple<string, string>>>();
            int surrogatekey = 1;
            foreach (var record1 in table1.Data)
            {
                var temp = new Dictionary<string, string>();
                var temp2 = new Dictionary<string, string>();
                var temp3 = new List<Tuple<string, string>>();
                foreach (var attribute in record1.Value)
                {
                    temp.Add(attribute.Key, attribute.Value);
                }
                foreach (var record2 in table2.Data)
                {
                    foreach (var item in temp)
                    {
                        temp3.Add(new Tuple<string, string>(item.Key, item.Value));
                        //temp3.Add(item.Key, item.Value);
                    }
                    foreach (var attribute2 in record2.Value)
                    {
                        temp2.Add(attribute2.Key, attribute2.Value);
                    }

                    foreach (var item in temp2)
                    {
                        temp3.Add(new Tuple<string, string>(item.Key, item.Value));

                        //temp3.Add(item.Key, item.Value);
                    }

                    cartesianed.Add(surrogatekey.ToString(), new List<Tuple<string, string>>(temp3));
                    surrogatekey++;
                    temp3.Clear();
                    temp2.Clear();
                }
            }
            foreach (var item in cartesianed)
            {
                target.Data.Add(item.Key, item.Value);
            }
        }

        public string ParseSQLCommand(string p_command)
        {
            Schema _oldSchema = new Schema();
            Schema _newSchema = new Schema();
            JoinSchema _joinSchema = new JoinSchema();
            string parseResult = "";
            string _tableName = "";
            string[] SplittedCommand = p_command.Split(' ');
            SplittedCommand = SplittedCommand.Where(x => x != "").Select(x => x.Trim()).ToArray();
            var _dir = @"C:\DBparser\Schemas";
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);


            foreach (var item in SplittedCommand)
            {
                Console.WriteLine(item);

            }
            Console.WriteLine();
            switch (SplittedCommand[0].ToUpper())
            {
                case "SELECT":
                    string m_selected = "";
                    var singleTableName = "";
                    string[] fromTwoTables = new string[2];
                    if (SplittedCommand[1] == "*")
                    {
                        if (!@SplittedCommand[3].Contains(","))
                        {
                            singleTableName = @SplittedCommand[3];
                        }
                        else
                        {
                            fromTwoTables = MySubString.SubString(p_command, p_command.IndexOf("from") + 5, p_command.Length - 1).Split(',');
                            for (int i = 0; i < fromTwoTables.Length; i++)
                            {
                                fromTwoTables[i] = fromTwoTables[i].Trim();
                            }
                        }

                        if (File.Exists(Path.Combine(_dir, singleTableName + ".txt")) || File.Exists(Path.Combine(_dir, fromTwoTables[0] + ".txt")))
                        {
                            _newSchema = new Schema();

                            if (!p_command.Contains("where"))
                            {

                                if (fromTwoTables[1] != null)
                                {
                                    Schema table1 = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, fromTwoTables[0] + ".txt")));
                                    Schema table2 = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, fromTwoTables[1] + ".txt")));
                                    //Cartesian(_newSchema, table1, table2);                                    
                                    //parseResult = JsonConvert.SerializeObject(_newSchema);
                                    Cartesian(_joinSchema, table1, table2);
                                    parseResult = JsonConvert.SerializeObject(_joinSchema);

                                }
                                else
                                {
                                    m_selected = File.ReadAllText(Path.Combine(_dir, singleTableName + ".txt"));
                                    var q = JsonConvert.DeserializeObject<Schema>(m_selected);
                                    foreach (var item in q.Data)
                                    {
                                        _joinSchema.Data.Add(item.Key, new List<Tuple<string, string>>());
                                        foreach (var item2 in item.Value)
                                        {
                                            _joinSchema.Data[item.Key].Add(new Tuple<string, string>(item2.Key, item2.Value));

                                        }
                                    }
                                    parseResult = JsonConvert.SerializeObject(_joinSchema);
                                }
                            }
                            else
                            {
                                string[] whereClauseKeyValue = new string[2];
                                whereClauseKeyValue = GetWhereClause(p_command);

                                if (fromTwoTables[1] == null)
                                {
                                    _oldSchema = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, singleTableName + ".txt")));


                                    foreach (var record in _oldSchema.Data)
                                    {
                                        List<Tuple<string, string>> temp = new List<Tuple<string, string>>();
                                        foreach (var attribute in record.Value)
                                        {
                                            if (attribute.Key == whereClauseKeyValue[0].Trim() && attribute.Value == whereClauseKeyValue[1].Trim().Replace("'", ""))
                                            {
                                                foreach (var item in record.Value)
                                                {
                                                    temp.Add(new Tuple<string, string>(item.Key, item.Value));
                                                }

                                            }
                                        }
                                        _joinSchema.Data.Add(record.Key, temp);
                                    }
                                }
                                else
                                {
                                    fromTwoTables[1] = MySubString.SubString(fromTwoTables[1], 0, fromTwoTables[1].IndexOf(' ') - 1);
                                    Schema table1 = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, fromTwoTables[0] + ".txt")));
                                    Schema table2 = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, fromTwoTables[1] + ".txt")));

                                    //Cartesian(_joinSchema, table1, table2);
                                    if (whereClauseKeyValue.Any(x => x.Contains(".")))
                                    {
                                        var table1WhereAttribute = whereClauseKeyValue[0].Split('.');
                                        var table2WhereAttribute = whereClauseKeyValue[1].Split('.');
                                        SortMergeJoin(_joinSchema, table1, table2, table1WhereAttribute[1], table2WhereAttribute[1]);
                                        
                                    }
                                    else
                                    {
                                        //WRECKED
                                        foreach (var record in _newSchema.Data.Reverse())
                                        {
                                            if (record.Value.Any(x => x.Key == whereClauseKeyValue[0].Trim() && x.Value != whereClauseKeyValue[1].Trim().Replace("'", "")))
                                            {
                                                _joinSchema.Data.Remove(record.Key);
                                            }

                                        }
                                    }
                                }
                                parseResult = JsonConvert.SerializeObject(_joinSchema);

                            }
                        }
                        else
                        {
                            parseResult = "No Schema!!";
                        }
                    }
                    else
                    {
                        string m_selectedField = MySubString.SubString(p_command, p_command.IndexOf("select") + 7, p_command.LastIndexOf("from") - 2);
                        string[] selectedFieldsArray = m_selectedField.Split(',');

                        selectedFieldsArray = selectedFieldsArray.Select(x => x.Trim()).ToArray();
                        _tableName = MySubString.SubString(p_command, p_command.LastIndexOf("from") + 5, p_command.Length - 1);

                        if (File.Exists(Path.Combine(_dir, _tableName + ".txt")))
                        {
                            m_selected = File.ReadAllText(Path.Combine(_dir, _tableName + ".txt"));

                            JObject json = JObject.Parse(m_selected);
                            List<JProperty> removeList = new List<JProperty>();
                            foreach (var record in json["Data"])
                            {
                                foreach (JProperty data in record.Values().Reverse())
                                {
                                    if (!selectedFieldsArray.Contains(data.Name))
                                    {
                                        removeList.Add(data);
                                        data.Remove();
                                    }

                                }

                            }

                            m_selected = json.ToString();
                            parseResult = m_selected;
                        }
                        else
                        {
                            parseResult = "No Schema!";
                        }

                    }



                    break;
                case "UPDATE":

                    _tableName = @SplittedCommand[1].Replace(System.Environment.NewLine, "").Replace("set", "");
                    string m_toUpdate = File.ReadAllText(Path.Combine(_dir, _tableName + ".txt"));
                    string m_fieldsToUpdate = MySubString.SubString(p_command, p_command.IndexOf("set") + 3, p_command.Length - 1);
                    string[] m_fields = m_fieldsToUpdate.Trim().Split(',');
                    _oldSchema = JsonConvert.DeserializeObject<Schema>(m_toUpdate);

                    foreach (var item in m_fields)
                    {
                        string[] temp = item.Trim().Split('=');
                        string attrName = temp[0].Trim();
                        string updateValue = temp[1].Replace("'", "").Trim();
                        foreach (var record in _oldSchema.Data.Values)
                        {
                            record[attrName] = updateValue;
                        }

                    }

                    File.Delete(Path.Combine(_dir, _tableName + ".txt"));
                    File.WriteAllText(Path.Combine(_dir, _tableName + ".txt"), JsonConvert.SerializeObject(_oldSchema, Formatting.Indented));


                    break;
                case "INSERT":
                    string m_tableNameI = @SplittedCommand[2];
                    string m_value = MySubString.SubString(p_command, p_command.IndexOf('(') + 1, p_command.IndexOf(')') - 1);
                    Console.WriteLine(m_value);
                    bool IsInsertError = false;
                    string[] m_valueSplitted = m_value.Split(',');
                    _oldSchema = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, m_tableNameI + ".txt")));
                    int insertValueIndex = 0;
                    int surrogateKey = _oldSchema.Data.Count;
                    Dictionary<string, string> newRecord = new Dictionary<string, string>();
                    foreach (var item in _oldSchema.Attributes)
                    {

                        int attrLength = GetAttributeLength(item.Value);
                        if (insertValueIndex < m_valueSplitted.Length)
                        {
                            //if (ValidateValueAndType(GetAttributeName(item.Value), m_valueSplitted[insertValueIndex], attrLength))
                            //{
                            //    newRecord.Add(item.Key, m_valueSplitted[insertValueIndex]);
                            //}
                            newRecord.Add(item.Key, m_valueSplitted[insertValueIndex].Replace("'", ""));
                        }
                        else
                        {
                            newRecord.Add(item.Key, "");
                        }


                        insertValueIndex++;
                    }
                    surrogateKey += 1;
                    _oldSchema.Data.Add(surrogateKey.ToString(), newRecord);

                    if (!IsInsertError)
                    {
                        File.Delete(Path.Combine(_dir, m_tableNameI + ".txt"));
                        File.WriteAllText(Path.Combine(_dir, m_tableNameI + ".txt"), JsonConvert.SerializeObject(_oldSchema, Formatting.Indented));
                    }

                    break;
                case "CREATE":

                    string m_tableNameC = SplittedCommand[2];

                    string m_schema = MySubString.SubString(p_command, p_command.IndexOf('(') + 1, p_command.LastIndexOf(';') - 2);

                    _newSchema = new Schema(m_tableNameC);
                    string[] properties = m_schema.Replace(System.Environment.NewLine, "").Split(',');
                    foreach (var item in properties)
                    {
                        string[] singleProperty = item.Split(' ');
                        _newSchema.Attributes.Add(singleProperty[0], singleProperty[1]);
                    }

                    _dir = @"C:\DBparser\Schemas";  // folder location

                    if (!Directory.Exists(_dir))  // if it doesn't exist, create
                        Directory.CreateDirectory(_dir);

                    // use Path.Combine to combine 2 strings to a path
                    File.WriteAllText(Path.Combine(_dir, m_tableNameC + ".txt"), JsonConvert.SerializeObject(_newSchema, Formatting.Indented));


                    parseResult = "Create Table Successfully!";

                    break;
                case "DROP":
                    _tableName = SplittedCommand[2];
                    File.Delete(Path.Combine(_dir, _tableName + ".txt"));
                    parseResult = "Drop " + _tableName;
                    //File.WriteAllText(Path.Combine(_dir, m_tableNameI + ".txt"), schemaJson.ToString())
                    break;
                case "DELETE":
                    if (SplittedCommand[1] == "*")
                    {
                        _tableName = SplittedCommand[3];
                    }
                    else
                    {
                        _tableName = SplittedCommand[2];
                    }
                    string m_toDelete = File.ReadAllText(Path.Combine(_dir, _tableName + ".txt"));
                    JObject deleteJson = JObject.Parse(m_toDelete);
                    deleteJson["Data"] = null;
                    Console.WriteLine(deleteJson);
                    File.Delete(Path.Combine(_dir, _tableName + ".txt"));
                    File.WriteAllText(Path.Combine(_dir, _tableName + ".txt"), deleteJson.ToString());
                    parseResult = "Delete records from " + _tableName;
                    break;
            }
            return parseResult;
        }
    }
}
