using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AudioBook2Podcast
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set"))
            {
                radioButton2.Checked = true;
                StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set");
                textBox2.Text = sr.ReadLine();
                sr.Close();
            }
            else
            {
                radioButton1.Checked = true;
            }
 

        }



        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set");
                    
                }
                this.Close();
            }
            if (radioButton2.Checked)
            {
                try
                {
                   int pn = Convert.ToInt32(textBox2.Text);
                   if (pn >= 0 && pn <= 65535)
                   {
                       StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set", false);
                       sw.WriteLine(textBox2.Text);
                       sw.Close();
                       this.Close();
                   }
                   if (pn < 0 || pn > 65535)
                   {
                       MessageBox.Show("Port number must be in range from 0 - 65535", "Port number is not in range", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   }

                }
                catch
                {
                    MessageBox.Show("Port number must be a number", "Port number is not number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            
        }
    }
}
