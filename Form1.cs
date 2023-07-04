using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace VapeV4


{
    public partial class Form1 : Form
    {
        // ����һ����������ѹ��Properties.Resources.Core�ļ�
        public static void UnzipCoreFile()
        {
            // ��ȡ��ǰ�����Ŀ¼
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // ����һ���ڴ�������ȡProperties.Resources.Core�ļ����ֽ�����
            using (MemoryStream ms = new MemoryStream(Properties.Resources.Core))
            {
                // ����һ��ZipArchive����������ѹ���ļ��е���Ŀ
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read))
                {
                    // ����ÿһ����Ŀ
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        // ��ȡ��Ŀ������·��
                        string entryPath = Path.Combine(currentDirectory, entry.FullName);

                        // �����Ŀ��һ���ļ��У��ʹ�����
                        if (entryPath.EndsWith("/"))
                        {
                            Directory.CreateDirectory(entryPath);
                        }
                        else // �����Ŀ��һ���ļ�������ȡ��
                        {
                            // ����Ŀ����
                            using (Stream entryStream = entry.Open())
                            {
                                // ����һ���ļ�����д���ļ�
                                using (FileStream fs = new FileStream(entryPath, FileMode.Create, FileAccess.Write))
                                {
                                    // ����Ŀ�������Ƶ��ļ�����
                                    entryStream.CopyTo(fs);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Opacity = 0;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler((sender2, e2) =>
            {
                if ((this.Opacity += 0.05d) == 1) timer.Stop();
            });
            timer.Start();
        }

        //����һ��PictureBox����ʾͼƬ
        private PictureBox pictureBox;

        //����һ��Label����ʾ���������
        private Label outputLabel;

        //����һ��ProgressBar����ʾ������
        private ProgressBar progressBar;

        //����һ��FolderBrowserDialog��ѡ���ļ���
        private FolderBrowserDialog folderBrowserDialog;

        //����һ��System.Diagnostics.Process��ִ��������
        private System.Diagnostics.Process process;
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_DROPSHADOW = 0x00020000;
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }


        public Form1()
        {


            InitializeComponent();

            //���ô��ڵı�����ɫΪRGB(30,30,30)
            this.BackColor = Color.FromArgb(30, 30, 30);
            //���ô��ڵĴ�СΪ800x500�����Ҳ����������С
            this.Size = new Size(800, 500);
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //���ô��ڵ��Ľ�ΪԲ�ǣ��뾶Ϊ5����
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 5, 5));
            //�ڴ������ĵ�ƫ�Ϸ�����һ��PictureBox��������ʾͼƬ
            pictureBox = new PictureBox();
            pictureBox.Location = new Point(this.Width / 2 - 200 / 2, this.Height / 2 - 150);
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            //����Դ�ļ��м���ͼƬ������ֵ��PictureBox
            pictureBox.Image = Properties.Resources.Vape1;
            //��PictureBox��ӵ����ڵĿؼ�������
            this.Controls.Add(pictureBox);
            //�ڽ������Ϸ�����һ��Label��������ʾ���������
            outputLabel = new Label();
            outputLabel.Location = new Point(this.Width / 2 - 300, this.Height / 2 + 100);
            outputLabel.Size = new Size(600, 20);
            outputLabel.ForeColor = Color.White;
            outputLabel.TextAlign = ContentAlignment.MiddleCenter;
            //��Label��ӵ����ڵĿؼ�������
            this.Controls.Add(outputLabel);
            //�ڴ��ڵײ�����һ��ProgressBar��������ʾ������
            progressBar = new ProgressBar();
            progressBar.Location = new Point(this.Width / 2 - 300, this.Height - 50);
            progressBar.Size = new Size(600, 3);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.ForeColor = Color.Green;
            progressBar.BackColor = Color.FromArgb(30, 30, 30);
            //���ý��������Ľ�ΪԲ�ǣ��뾶Ϊ1.5����
            progressBar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, progressBar.Width, progressBar.Height, 1, 1));
            //��ProgressBar��ӵ����ڵĿؼ�������
            this.Controls.Add(progressBar);
            //����һ��FolderBrowserDialog������ѡ���ļ���
            folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "��ѡ��java17�����ļ���";
            //����һ��System.Diagnostics.Process������ִ��������
            process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            //ΪProcess���һ���¼������������ڽ������������
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_OutputDataReceived);
            //Ϊ�������һ���¼�����������������ʾ�󵯳��ļ���ѡ��Ի���
#pragma warning disable CS8622 // �����������������͵�Ϊ Null ����Ŀ��ί�в�ƥ��(����������Ϊ Null ������)��
            this.Shown += new System.EventHandler(this.Form1_Shown);
#pragma warning restore CS8622 // �����������������͵�Ϊ Null ����Ŀ��ί�в�ƥ��(����������Ϊ Null ������)��
        }

        //����һ�����������ڴ���Բ�Ǿ��εľ��
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        //����һ�������������ڴ�����ʾ�󵯳��ļ���ѡ��Ի��򣬲�ִ��������
        private void Form1_Shown(object sender, EventArgs e)
        {
            //����û�ѡ�����ļ��У��ͻ�ȡ�ļ���·������ƴ���������ַ���
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;
                string command = "set MINIMAL_VERSION=1.8.0\nset JAVA_HOME=\"" + folderPath + "\"\nset PATH=%JAVA_HOME%\\bin;%PATH%\njava --add-opens java.base/java.lang=ALL-UNNAMED -jar \"vape-loader.jar\"";
                //����Process����д���������ַ���
                process.Start();
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
                //��ʼ�첽��ȡ���������
                process.BeginOutputReadLine();
                //����һ����ʱ����������5��󽫽���������������
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 5000;
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
            }
            else
            {
                //����û�û��ѡ���ļ��У��͹رմ���
                this.Close();
            }
        }

        //����һ�������������ڶ�ʱ������ʱ������������������������2���رմ���
        private void Timer_Tick(object sender, EventArgs e)
        {
            progressBar.Value = 100;
            System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
            timer.Stop();
            timer.Dispose();
            System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
            timer2.Interval = 2000;
            timer2.Tick += new EventHandler(Timer2_Tick);
            timer2.Start();
        }

        //����һ�������������ڶ�ʱ������ʱ�رմ���
        private void Timer2_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
        private Point mousePoint = new Point();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint.X = e.X;
            mousePoint.Y = e.Y;
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - mousePoint.X;
                this.Top += e.Y - mousePoint.Y;
            }
        }
        //����һ�������������ڽ��յ����������ʱ����Label���ı�
        private void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputLabel.Invoke(new Action(() => outputLabel.Text = e.Data));
            }
        }
    }
}