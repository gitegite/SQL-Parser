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
        Schema _OldSchema = new Schema();
        public string ParseSQLCommand(string p_command)
        {
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
                    if (SplittedCommand[1] == "*")
                    {

                        _tableName = @SplittedCommand[3];
                        if (File.Exists(Path.Combine(_dir, _tableName + ".txt")))
                        {
                            m_selected = File.ReadAllText(Path.Combine(_dir, _tableName + ".txt"));
                            parseResult = m_selected;
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

                    _tableName = @SplittedCommand[1].Replace(System.Environment.NewLine,"").Replace("set","");
                    string m_toUpdate = File.ReadAllText(Path.Combine(_dir, _tableName + ".txt"));
                    string m_fieldsToUpdate = MySubString.SubString(p_command, p_command.IndexOf("set") + 3, p_command.Length - 1);
                    Console.WriteLine(m_fieldsToUpdate);
                    string[] m_fields = m_fieldsToUpdate.Trim().Split(',');
                     _OldSchema = JsonConvert.DeserializeObject<Schema>(m_toUpdate);
                                                                    
                    foreach (var item in m_fields)
                    {
                        string[] temp = item.Trim().Split('=');
                        string attrName = temp[0].Trim();
                        string updateValue = temp[1].Replace("'","").Trim();
                        foreach (var record in _OldSchema.Data.Values)
                        {
                            record[attrName] = updateValue;
                        }
                        
                    }
                    
                    //Console.WriteLine(toUpdateJson);
                    File.Delete(Path.Combine(_dir, _tableName + ".txt"));
                    File.WriteAllText(Path.Combine(_dir, _tableName + ".txt"), JsonConvert.SerializeObject(_OldSchema,Formatting.Indented));


                    break;
                case "INSERT":
                    string m_tableNameI = @SplittedCommand[2];
                    string m_value = MySubString.SubString(p_command, p_command.IndexOf('(') + 1, p_command.IndexOf(')') - 1);
                    Console.WriteLine(m_value);
                    bool IsInsertError = false;
                    string[] m_valueSplitted = m_value.Split(',');
                    _OldSchema = JsonConvert.DeserializeObject<Schema>(File.ReadAllText(Path.Combine(_dir, m_tableNameI + ".txt")));
                    int insertValueIndex = 0;
                    int surrogateKey = _OldSchema.Data.Count;
                    Dictionary<string,string> newRecord = new Dictionary<string,string>();
                    foreach (var item in _OldSchema.Attributes)
                    {
                   
                       int attrLength =  GetAttributeLength(item.Value);
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
                    _OldSchema.Data.Add(surrogateKey.ToString(), newRecord);

                    if (!IsInsertError)
                    {
                        File.Delete(Path.Combine(_dir, m_tableNameI + ".txt"));
                        File.WriteAllText(Path.Combine(_dir, m_tableNameI + ".txt"),JsonConvert.SerializeObject(_OldSchema,Formatting.Indented));
                    }

                    break;
                case "CREATE":

                    string m_tableNameC = SplittedCommand[2];

                    string m_schema = MySubString.SubString(p_command, p_command.IndexOf('(') + 1, p_command.LastIndexOf(';') - 2);

                    Schema newSchema = new Schema(m_tableNameC);
                    string[] properties = m_schema.Replace(System.Environment.NewLine, "").Split(',');
                    foreach (var item in properties)
                    {
                        string[] singleProperty = item.Split(' ');
                        newSchema.Attributes.Add(singleProperty[0], singleProperty[1]);
                    }

                    _dir = @"C:\DBparser\Schemas";  // folder location

                    if (!Directory.Exists(_dir))  // if it doesn't exist, create
                        Directory.CreateDirectory(_dir);

                    // use Path.Combine to combine 2 strings to a path
                    File.WriteAllText(Path.Combine(_dir, m_tableNameC + ".txt"), JsonConvert.SerializeObject(newSchema, Formatting.Indented));


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
