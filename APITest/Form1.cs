using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace APITest
{
    public partial class Form1 : Form
    {
        JsonParse JP = new JsonParse();
        public void JsonTextUpdated(Object sender, EventArgs e)
        {
            RichTextBox RTB = sender as RichTextBox;
            JP.LJV.Clear();
            JP.JsonString = RTB.Text;
            JP.ParseString(RTB.Text);
            if (JP.JTU.LBL != null || JP.JTU.TBL != null)
            {
                try
                {
                    foreach (Label V in JP.JTU.LBL)
                    {
                        JP.REC.Controls.Remove(V);
                        ((IDisposable)V).Dispose();
                    }
                }
                catch (Exception E1)
                {

                    Console.Write(E1);
                }
                try
                {
                    foreach (TextBox C in JP.JTU.TBL)
                    {
                        JP.REC.Controls.Remove(C);
                        ((IDisposable)C).Dispose();
                    }
                }
                catch (Exception E2)
                {

                    Console.Write(E2);
                }
                JP.JTU.TBL.Clear();
                JP.JTU.LBL.Clear();
            }
            JP.JTU.CreateTextObjects(JP.LJV);
            foreach (TextBox NTB in JP.JTU.TBL)
            {
                 JP.REC.Controls.Add(NTB);
            }
            foreach (Label LBL in JP.JTU.LBL)
            {
                 JP.REC.Controls.Add(LBL);
            }
        }
        public void SizeChange(Object sender, EventArgs e)
        {
            Form F1 = sender as Form1;
            JP.REC.Width = this.Width;
            JP.RichBox.Width = this.Width;
            JP.RichBox.Height = this.Height / 2;
            
        }
        public Form1()
        {
            this.AutoSize = true;
            this.Controls.Add(JP.REC);
            JP.REC.Width = 500;
            JP.REC.Height = 350;
            JP.REC.Controls.Add(JP.JsonError);
            JP.REC.Location = new Point(Left,Top);
            JP.JsonError.Location = new Point(Right, Top);
            JP.JsonError.AutoSize = true;
            JP.REC.AutoScroll = true;
            Button BTN = new Button();
            JP.JTU = new JsontoUI();
            JP.RichBox.Name = "InputJson";
            JP.RichBox.Text = @"{""test"":""InputJson""}";
            JP.RichBox.Location = new Point(Left,Bottom);
            JP.RichBox.Width = 1000;
            JP.RichBox.Height = 500;
            JP.RichBox.Click += JsonTextUpdated;
            BTN.Text = "Generate Json";
            BTN.AutoSize=true;
            BTN.Location = new Point(0,JP.RichBox.Height+15);
            BTN.Click += JP.GenerateJson;
            this.SizeChanged += new EventHandler(SizeChange);
            this.Controls.Add(BTN);
            this.Controls.Add(JP.RichBox);
            InitializeComponent();

        }
    }
}
