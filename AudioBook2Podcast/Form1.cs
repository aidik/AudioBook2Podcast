﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Web;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Gma.QrCodeNet.Encoding.Windows.Forms;

namespace AudioBook2Podcast
{
    public partial class Form1 : Form
    {
        public static DateTime date = DateTime.UtcNow;

        public static string path = "";
        public static string adresa = "http://" + LocalhostIP() + ":";
        public static Size s = new Size(144, 144);
        public static string cesta = Application.StartupPath;
        public static string aport;
        public static Process LightTPD;
        



        public Form1()
        {
            InitializeComponent();

            

        }


        public static string LocalhostIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public static string ObracenaLomitka(string s)
        {
            s = s.Replace("\\", "/");
            return s;
        }

        public static void RunLightTPD()
        {


            
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = cesta + @"\LightTPD\lighttpd.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = "-f /LightTPD/conf/lighttpd-inc.conf -m lib -D"; // -D shows LightTPD window

            LightTPD = Process.Start(startInfo);
            

        }


       


        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();


            if (fbd.ShowDialog() == DialogResult.OK)
            {
                label5.Text = fbd.SelectedPath;
                path = label5.Text;
                

            }   
        }



        private void ObrCesta_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Soubor obrázku(*.bmp;*.jpg;*.gif;*.png)|*.bmp;*.jpg;*.gif;*.png";

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = resizeImage(Image.FromFile(ofd.FileName), s);
                    
                }
                catch
                {
                    MessageBox.Show("While loading image " + ofd.FileName + " an error occured. File is probably damaged.", "Image load", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            
        }



        private void podcast()
        {

            string[] args = { "", //slozka
                              textBox1.Text, //jmeno podcastu - knihy
                              textBox2.Text, //popisek
                              "podcast.jpg", //obrazek
                              "144", //vyska obrazku drzet na 144
                              "144", //sirka drzet tez na 144
                              textBox3.Text, // autor
                              "podcast.xml", //vystup podcast feed
                            };
            DirectoryInfo di = new DirectoryInfo(path);

            FileInfo[] fi = null;
            if (comboBox1.Text == "")
            {
                fi = di.GetFiles();
            }
            else
            {
                fi = di.GetFiles("*." + comboBox1.Text);
            }

            Array.Sort<FileInfo>(fi, new Comparison<FileInfo>(delegate(FileInfo d1, FileInfo d2)
            {
                return string.Compare(d1.Name, d2.Name);
            }));

          //  FileInfo[] fi = di.GetFiles();
            StreamWriter podcast = new StreamWriter(path + "\\" + args[7]);
            podcast.WriteLine("<?xml version=\"1.0\"?>");
            podcast.WriteLine("<rss xmlns:content=\"http://purl.org/rss/1.0/modules/content/\" xmlns:itunes=\"http://www.itunes.com/dtds/podcast-1.0.dtd\" version=\"2.0\"  xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            podcast.WriteLine();
            podcast.WriteLine("  <channel>");
            podcast.WriteLine("   <title>" + args[1] + "</title>");
            podcast.WriteLine("   <link>" + adresa + aport +"/"+ args[7] + "</link>");
            podcast.WriteLine("   <atom:link href=\"" + adresa + aport + "/" + args[7] + "\" rel=\"self\" type=\"application/rss+xml\" />");
            podcast.WriteLine("   <description>" + args[2] + "</description>");
            podcast.WriteLine("   <webMaster>AudioBook2Podcast</webMaster>");
            podcast.WriteLine("   <managingEditor>AudioBook2Podcast</managingEditor>");
            podcast.WriteLine("   <pubDate>" + date.ToString("r", new CultureInfo("en-GB")).Substring(0, date.ToString("r", new CultureInfo("en-GB")).Length - 3) + "+0100" + "</pubDate>");
            podcast.WriteLine();
            podcast.WriteLine("   <image>");
            podcast.WriteLine("     <url>" + adresa + aport + "/" + args[3] + "</url>");
            podcast.WriteLine("     <width>" + args[4] + "</width>");
            podcast.WriteLine("     <height>" + args[5] + "</height>");
            podcast.WriteLine("     <link>" + adresa + aport + "/" + args[7] + "</link>");
            podcast.WriteLine("     <title>" + args[1] + "</title>");
            podcast.WriteLine("   </image>");
            podcast.WriteLine();
            podcast.WriteLine("   <copyright>AudioBook2Podcast</copyright>");
            podcast.WriteLine("   <language>en-GB</language>");
            podcast.WriteLine("   <docs>http://blogs.law.harvard.edu/tech/rss</docs>");
            podcast.WriteLine("   <generator>AudioBook2Podcast</generator>");
            podcast.WriteLine("   <itunes:image href=\"" + adresa + aport + "/" + args[3] + "\"/>");
            podcast.WriteLine();
            podcast.WriteLine();

            int cislo = 1;
            foreach (FileInfo f in fi)
            {
                string jmeno = f.ToString();
                string titulek = jmeno.Substring(0, jmeno.Length - 4);
                
                date = date.AddHours(1.0);
                string gooddate = date.ToString("r", new CultureInfo("en-GB"));
                gooddate = gooddate.Substring(0, gooddate.Length - 3) + "+0100";


                podcast.WriteLine();
                podcast.WriteLine("   <item>");
                podcast.WriteLine("    <title>" + titulek + "</title>");
                podcast.WriteLine("    <link>" + HttpUtility.UrlPathEncode(adresa + aport + "/" + f.ToString()) + "</link>");
                podcast.WriteLine("    <comments>http://www.aidik.com/</comments>");
                podcast.WriteLine("    <description>" + cislo.ToString() + ". part of book " + args[1] + " from " + args[6] + ".</description>");
                podcast.WriteLine("    <pubDate>" + gooddate + "</pubDate>");
                podcast.WriteLine("    <guid>" + HttpUtility.UrlPathEncode(adresa + aport + "/" + f.ToString()) + "</guid>");
                podcast.WriteLine("   <author>" + args[6] + "</author>");
                podcast.WriteLine("   <enclosure url=\"" + HttpUtility.UrlPathEncode(adresa + aport + "/" + f.ToString()) + "\" " + "length=\"" + f.Length + "\"  type=\"audio/mpeg\" />");
                podcast.WriteLine("   </item>");
                cislo++;


            }

            podcast.WriteLine("  </channel>");
            podcast.WriteLine("</rss>");







            podcast.Close();

        }



        private void button3_Click(object sender, EventArgs e)
        {
            ImageFormat format = ImageFormat.Jpeg;

            path = label5.Text;
            Image i = resizeImage(pictureBox1.Image,  s);
            i.Save(path + "\\" + "podcast.jpg", format);
           
            
            UpravConfig();
            podcast();
            RunLightTPD();
            textBox4.Text = adresa + aport + "/podcast.xml";
            qrCodeGraphicControl1.Text = textBox4.Text;
            
        }

        private static void UpravConfig()
        {
            aport = DejPort();
            StreamReader sr = new StreamReader("lighttpd-inc.conf");
            string config = sr.ReadToEnd();
            config = config.Replace("{{DR}}", ObracenaLomitka(path));
            config = config.Replace("{{TMP}}", ObracenaLomitka(cesta) + "/LightTPD/TMP/");
            config = config.Replace("{{PORT}}", aport);
            StreamWriter sw = new StreamWriter(@"LightTPD/conf/lighttpd-inc.conf", false);
            sw.Write(config);
            sw.Close();
            sw.Dispose(); 



        }

        private static string DejPort()
        {
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set"))
            {
                StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"/port.set");
                string port = sr.ReadLine();
                sr.Close();
                return port;
            }
            else
            {
                Random rnd = new Random();
                int nport = rnd.Next(49153, 65534);
                return nport.ToString();
            }
        }

        private static Image resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        private static void KillWS()
        {
            Process[] prs = Process.GetProcesses();
            foreach (Process pr in prs)
            {
                
                if (pr.ProcessName == "LightTPD")
                {
                    pr.Kill();
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(textBox4.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox4.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                KillWS();
                LightTPD.Kill();
            }
            catch
            { }
            Environment.Exit(0);

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1_FormClosing(null, null);

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab1 = new AboutBox1();
            ab1.ShowDialog();
        }




    }
}
