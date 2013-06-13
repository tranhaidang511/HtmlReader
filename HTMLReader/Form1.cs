using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using WinSCP;

namespace HTMLReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = null;
            string url = textBox1.Text;
            string filename = null;
            number++;
            WebResponse response = null;
            StreamReader wr = null, fr = null;
            StreamWriter writer = null;
            HttpWebRequest request1;
            FtpWebRequest request2;

            try
            {
                //Read template until <div class="noPc">
                fr = new StreamReader("column0001.html");
                filename = "Number" + DateTime.Now.ToString("yyyyMMddhhmm") + number.ToString("D2") + ".html";
                writer = new StreamWriter(filename);
                label1.Text = "Writing to file...";
                while ((result = fr.ReadLine()) != "<a href=\"https://tv.pacificleague.jp/ptv/api/copyUrl/play?movieId=YuKvI3cdyHI\"><img src=\"img/img_movie.jpg\" alt=\"\" title=\"\" /></a>")
                {
                    writer.WriteLine(result);
                }
                writer.WriteLine("<a href=\"" + textBox1.Text + "\"><img src=\"img/img_movie.jpg\" alt=\"\" title=\"\" /></a>");
/*
                //Read HTML page for Android  
                request1 = (HttpWebRequest)WebRequest.Create(url);
                request1.Method = "GET";
                request1.UserAgent = "Mozilla/5.0 (Android 4.0.3)";
                response = request1.GetResponse();
                wr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = wr.ReadToEnd();
                writer.WriteLine(result);
                wr.Close();
                response.Close();
*/
                //Continue reading template until "<div class="noSp">
                while ((result = fr.ReadLine()) != "<iframe src=\"https://tv.pacificleague.jp/ptv/api/copyUrl/play?movieId=YuKvI3cdyHI\" frameborder=\"0\" scrolling=\"no\" width=\"680\" height=\"460\"></iframe>")
                {
                    writer.WriteLine(result);
                }                
                writer.WriteLine("<iframe src=\"" + textBox1.Text + "\" frameborder=\"0\" scrolling=\"no\" width=\"680\" height=\"460\"></iframe>");
/*
                //Read HTML page for Windows
                request1 = (HttpWebRequest)WebRequest.Create(url);
                request1.Method = "GET";
                request1.UserAgent = "Mozilla/5.0 (Windows NT 6.1)";
                response = request1.GetResponse();
                wr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = wr.ReadToEnd();
                writer.WriteLine(result);
                wr.Close();
                response.Close();
*/
                ////Read remaining part of template
                result = fr.ReadToEnd();
                writer.WriteLine(result);
                fr.Close();
                writer.Close();
                label1.Text = "Complete writing";

                //Upload file with FTP
/*                label1.Text = "Uploading file...";
                request2 = (FtpWebRequest)WebRequest.Create("ftp://"+number+".html");//Fill in FTP address
                request2.Method = WebRequestMethods.Ftp.UploadFile;
                request2.Credentials = new NetworkCredential("username", "password");//Change username and password
                fr = new StreamReader(number + ".html");
                byte[] fileContents = Encoding.UTF8.GetBytes(fr.ReadToEnd());
                request2.ContentLength = fileContents.Length;
                Stream requestStream = request2.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                FtpWebResponse response2 = (FtpWebResponse)request2.GetResponse();
                label1.Text = "Success !";
                requestStream.Close();
                response2.Close();*/

                //Upload file with SSH
                SessionOptions sessionOptions = new SessionOptions
                {//Fill in HostName, PortNumber(if necessary), UserName, Password, SshHostKeyFingerprint
                    Protocol = Protocol.Scp,
                    HostName = "210.129.194.50",
                    PortNumber = 22605,
                    UserName = "spo01",
//                    Password = "",
                    SshHostKeyFingerprint = "ssh-rsa 2048 32:a4:7f:c3:2e:2c:a2:0a:1d:8e:41:cc:92:46:37:f5",
                    SshPrivateKeyPath = @"C:\Users\HaiDang\Documents\Visual Studio 2012\Projects\HTMLReader\HTMLReader\bin\Debug\id_rsa-spo01.ppk",
                };
                label1.Text = "Uploading file...";
                using (Session session = new Session())
                {
                    session.Open(sessionOptions);
//                    session.ExecutablePath = ".";
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    TransferOperationResult transferResult;
                    transferResult = session.PutFiles(filename, "/home/spo01/", false, transferOptions);
                    transferResult.Check();
                    label1.Text = "Success !";
                }
            }
            catch (Exception ex)
            {
                // handle error
                MessageBox.Show(ex.Message);
            }
        }

        public int number { get; set; }//file name

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
