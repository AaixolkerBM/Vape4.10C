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
        // 定义一个方法来解压缩Properties.Resources.Core文件
        public static void UnzipCoreFile()
        {
            // 获取当前程序的目录
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // 创建一个内存流来读取Properties.Resources.Core文件的字节数据
            using (MemoryStream ms = new MemoryStream(Properties.Resources.Core))
            {
                // 创建一个ZipArchive对象来访问压缩文件中的条目
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Read))
                {
                    // 遍历每一个条目
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        // 获取条目的完整路径
                        string entryPath = Path.Combine(currentDirectory, entry.FullName);

                        // 如果条目是一个文件夹，就创建它
                        if (entryPath.EndsWith("/"))
                        {
                            Directory.CreateDirectory(entryPath);
                        }
                        else // 如果条目是一个文件，就提取它
                        {
                            // 打开条目的流
                            using (Stream entryStream = entry.Open())
                            {
                                // 创建一个文件流来写入文件
                                using (FileStream fs = new FileStream(entryPath, FileMode.Create, FileAccess.Write))
                                {
                                    // 将条目的流复制到文件流中
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

        //定义一个PictureBox来显示图片
        private PictureBox pictureBox;

        //定义一个Label来显示命令行输出
        private Label outputLabel;

        //定义一个ProgressBar来显示进度条
        private ProgressBar progressBar;

        //定义一个FolderBrowserDialog来选择文件夹
        private FolderBrowserDialog folderBrowserDialog;

        //定义一个System.Diagnostics.Process来执行命令行
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

            //设置窗口的背景颜色为RGB(30,30,30)
            this.BackColor = Color.FromArgb(30, 30, 30);
            //设置窗口的大小为800x500，并且不允许调整大小
            this.Size = new Size(800, 500);
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            //设置窗口的四角为圆角，半径为5像素
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 5, 5));
            //在窗口中心点偏上方创建一个PictureBox，用于显示图片
            pictureBox = new PictureBox();
            pictureBox.Location = new Point(this.Width / 2 - 200 / 2, this.Height / 2 - 150);
            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            //从资源文件中加载图片，并赋值给PictureBox
            pictureBox.Image = Properties.Resources.Vape1;
            //将PictureBox添加到窗口的控件集合中
            this.Controls.Add(pictureBox);
            //在进度条上方创建一个Label，用于显示命令行输出
            outputLabel = new Label();
            outputLabel.Location = new Point(this.Width / 2 - 300, this.Height / 2 + 100);
            outputLabel.Size = new Size(600, 20);
            outputLabel.ForeColor = Color.White;
            outputLabel.TextAlign = ContentAlignment.MiddleCenter;
            //将Label添加到窗口的控件集合中
            this.Controls.Add(outputLabel);
            //在窗口底部创建一个ProgressBar，用于显示进度条
            progressBar = new ProgressBar();
            progressBar.Location = new Point(this.Width / 2 - 300, this.Height - 50);
            progressBar.Size = new Size(600, 3);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.ForeColor = Color.Green;
            progressBar.BackColor = Color.FromArgb(30, 30, 30);
            //设置进度条的四角为圆角，半径为1.5像素
            progressBar.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, progressBar.Width, progressBar.Height, 1, 1));
            //将ProgressBar添加到窗口的控件集合中
            this.Controls.Add(progressBar);
            //创建一个FolderBrowserDialog，用于选择文件夹
            folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "请选择java17所在文件夹";
            //创建一个System.Diagnostics.Process，用于执行命令行
            process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            //为Process添加一个事件处理器，用于接收命令行输出
            process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_OutputDataReceived);
            //为窗口添加一个事件处理器，用于在显示后弹出文件夹选择对话框
#pragma warning disable CS8622 // 参数类型中引用类型的为 Null 性与目标委托不匹配(可能是由于为 Null 性特性)。
            this.Shown += new System.EventHandler(this.Form1_Shown);
#pragma warning restore CS8622 // 参数类型中引用类型的为 Null 性与目标委托不匹配(可能是由于为 Null 性特性)。
        }

        //定义一个方法，用于创建圆角矩形的句柄
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        //定义一个方法，用于在窗口显示后弹出文件夹选择对话框，并执行命令行
        private void Form1_Shown(object sender, EventArgs e)
        {
            //如果用户选择了文件夹，就获取文件夹路径，并拼接命令行字符串
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string folderPath = folderBrowserDialog.SelectedPath;
                string command = "set MINIMAL_VERSION=1.8.0\nset JAVA_HOME=\"" + folderPath + "\"\nset PATH=%JAVA_HOME%\\bin;%PATH%\njava --add-opens java.base/java.lang=ALL-UNNAMED -jar \"vape-loader.jar\"";
                //启动Process，并写入命令行字符串
                process.Start();
                process.StandardInput.WriteLine(command);
                process.StandardInput.WriteLine("exit");
                //开始异步读取命令行输出
                process.BeginOutputReadLine();
                //启动一个定时器，用于在5秒后将进度条滚动到结束
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 5000;
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
            }
            else
            {
                //如果用户没有选择文件夹，就关闭窗口
                this.Close();
            }
        }

        //定义一个方法，用于在定时器触发时将进度条滚动到结束，并在2秒后关闭窗口
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

        //定义一个方法，用于在定时器触发时关闭窗口
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
        //定义一个方法，用于在接收到命令行输出时更新Label的文本
        private void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputLabel.Invoke(new Action(() => outputLabel.Text = e.Data));
            }
        }
    }
}