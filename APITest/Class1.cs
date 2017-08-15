using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Drawing;
namespace APITest
{
    public class JsonParse
    {
        public String JsonString { get; set; }
        public JObject JsonObject { get; set; }
        public List<string> Paths = new List<string>();
        public List<JsonValues> LJV = new List<JsonValues>();
        public IEnumerable<TextBox> KeysTB { get; set; }
        public IEnumerable<TextBox> ValuesTB { get; set; }
        public JsontoUI JTU = new JsontoUI();
        public Label JsonError = new Label();
        public RichTextBox RichBox = new RichTextBox();
        public ContainerControl REC = new ContainerControl();
        public void ParseString(String JsonText)
        {
            try
            {
                JsonString = JsonText;
                JsonObject = JObject.Parse(JsonString);
                ToDict(JsonObject.First);
                Paths.Clear();
                JsonError.Text = "Successfully Parsed Json";
            }
            catch (JsonException JSE)
            {
                JsonError.Text = JSE.Message.ToString();
                Console.Write(JSE.ToString());
            }

        }
        public void GenerateJson(Object sender, EventArgs e)
        {
            UpdateObject();
            RichBox.Text = JsonObject.ToString();
        }

        public void UpdateObject()
        {
            JTU.CreateTextObjects(LJV);
            KeysTB = JTU.TBL.Where(t => LJV.Select(z => z.Path).Contains(t.Name));
            ValuesTB = JTU.TBL.Where(t => LJV.Select(z => "V"+z.Path).Contains(t.Name));
            for (int T=0; T < Paths.Count();T++)
            {
                JsonObject.Property(Paths[T]).Value = KeysTB.Where(t => t.Name == Paths[T]).Select(v => v.Text).Single();
                JsonObject.Property(Paths[T]).Value = ValuesTB.Where(t => t.Name == "V"+Paths[T]).Select(v => v.Text).Single();
            }
            try
            {
                if (JsonString != null && JsonObject != null)
                {
                    JsonString = JsonObject.ToString();
                }
            }
            catch (JsonException NJE)
            {
                
                JsonError.Text = NJE.ToString();
            }
        }
        private void ToDict(JToken Next)
        {
            if (Next != null)
            {
                Paths.Add(Next.Path);
                JsonValues JV = new JsonValues();
                if (Next.Parent.Path != null)
                {
                    JV.ChildOf = Next.Parent.Path;
                }
                JV.ObjectValue = Next.Values();
                foreach (JToken J in JV.ObjectValue)
                {

                    if (J.HasValues)
                    {
                        ToDict(J);
                    }
                    else
                    {
                        Dictionary < string, string>  ND = new Dictionary<string, string>();
                        string NJVP;
                        if (J.Path == null)
                        {
                            NJVP = JV.Path;
                        }
                        else
                        {
                            try
                            {
                                NJVP = J.Path.Substring(J.Path.LastIndexOf(".")+1);

                            }
                            catch
                            {
                                NJVP = J.Path;
                            }
                        }
                        if (J.Value<string>() == null)
                        {
                            ND.Add(NJVP,"Value was Null");
                            JV.Path = Next.Path;
                            JV.Value.Add(ND);
                        }
                        else
                        {
                            ND.Add(NJVP, J.Value<string>());
                            JV.Path = Next.Path;
                            JV.Value.Add(ND);
                        }
                    }
                }
                LJV.Add(JV);
                if (Next.HasValues)
                {
                    ToDict(Next.Next);
                }
            }
        }
    }
    public class JsonValues
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<Dictionary<string,string>> Value = new List<Dictionary<string, string>>();
        public IJEnumerable<JToken> ObjectValue { get; set; }
        public string ChildOf { get; set; }
        public JsonValues()
        {

        }
    }
    public class JsontoUI
    {
        public List<TextBox> TBL = new List<TextBox>();
        public List<Label> LBL = new List<Label>();
        public void CreateTextObjects(List<JsonValues> InObject)
        {
            int Count = InObject.Count;
            Point OriginalPosition = new Point(5,0);
            for (int I=0;I < Count;I++)
            {
                String NameText = InObject[I].Path;

                if (NameText != null)
                {
                    TextBox Name = new TextBox();
                    TextBox Value = new TextBox();
                    Label NL = new Label();
                    NL.Text = NameText;
                    NL.AutoSize = true;

                    Name.Name = NameText;

                    Value.Name = "V" + NameText;

                    int NTextHeight = Name.Height;
                    int NTextWidth = Name.Width;

                    Name.Location = OriginalPosition;
                    Value.Location = new Point(Name.Width + Name.Width - 70, Name.Location.Y);
                    NL.Location = new Point(Name.Width + Value.Width + Value.Width, Name.Location.Y);

                    string OriginalNameText = NameText;

                    if (InObject[I].Value.Count > 1) {
                        foreach (Dictionary<String, String> KV in InObject[I].Value)
                        {
                            try
                            {
                                Name.Text = InObject[I].ChildOf.Substring(InObject[I].ChildOf.LastIndexOf("."));
                            }
                            catch
                            {
                                Name.Text = InObject[I].ChildOf;
                            }
                            foreach (KeyValuePair<String, String> KV1 in KV)
                            {
                                TextBox CTB = new TextBox();
                                TextBox CTN = new TextBox();
                                CTB.Text = KV1.Value;
                                OriginalPosition.Y += 25;
                                CTN.Location = new Point(OriginalPosition.X + CTN.Width, OriginalPosition.Y);
                                CTB.Location = new Point(OriginalPosition.X + CTN.Width + CTN.Width + 15, OriginalPosition.Y);
                                CTN.Text = KV1.Key;
                                TBL.Add(CTN);
                                TBL.Add(CTB);
                            }
                        }
                    }
                    else
                    {
                        Name.Text = NameText;
                        Value.Text = InObject[I].Value.First().First().Value;
                        TBL.Add(Value);
                    }
                    OriginalPosition.Y += 25;
                    TBL.Add(Name);
                    LBL.Add(NL);
                    foreach (TextBox I1 in TBL)
                    {
                       // Console.Write(I1.Text+"\r\n");
                    }
                }
            }
        }
    }

}
