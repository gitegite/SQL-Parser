using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinForms.Documents.TextSearch;
using Telerik.WinForms.RichTextEditor;

namespace SQLParserDB
{
    public partial class MainForm : Form
    {
        ParserProvider ParserProvider = new ParserProvider();
        public MainForm()
        {
            InitializeComponent();
            TableGridView.Columns.Clear();
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            TableGridView.Columns.Clear();
            string result = ParserProvider.ParseSQLCommand(EditorTextBox.Text);

            if (!result.Contains(","))
            {
                EditorTextBox.Text = result;
            }
            else
            {
                Schema schema = JsonConvert.DeserializeObject<Schema>(result);
                JObject json = JObject.Parse(result);
                if (schema.Data.Count != 0)
                {
                    foreach (var item in schema.Data.First().Value)
                    {
                        TableGridView.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = item.Key, Name = item.Key });

                    }
                    List<string> valueList = new List<string>();
                    foreach (var item in schema.Data.Values)
                    {
                        foreach (var attr in item)
                        {
                            valueList.Add(attr.Value);
                        }
                        TableGridView.Rows.Add(valueList.ToArray());

                    }
                }


                //if (json["Data"].Children().Count() != 0)
                //{
                //    foreach (JProperty item in (JToken)json["Data"]["1"])
                //    {
                //        TableGridView.Columns.Add(new DataGridViewTextBoxColumn() { HeaderText = item.Name, Name = item.Name });

                //    }
                //    foreach (var record in json["Data"])
                //    {
                //        List<string> valueList = new List<string>();
                //        foreach (JProperty data in record.Values())
                //        {
                //            valueList.Add(data.Value.ToString());
                //        }
                //        TableGridView.Rows.Add(valueList.ToArray());
                //    }
                //}
                else
                {
                    EditorTextBox.Text = "No Values!";
                }
            }

        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            EditorTextBox.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
