using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using PublicValue;
using System.Collections;



    namespace uart
{
    
    public partial class Form1 : Form
    {
       
        private long receive_count = 0; //接收字节计数, 作用相当于全局变量 
        private long send_count = 0;    //发送字节计数, 作用相当于全局变量 
        //字符串构造类型（StringBuilder）它不仅允许任意改动内容，还提供了Append，Remove，Replace，Length，ToString等等有用的方法
        private StringBuilder sb = new StringBuilder();     //为了避免在接收处理函数中反复调用，依然声明为一个全局变量
        private DateTime current_time = new DateTime();    //为了避免在接收处理函数中反复调用，依然声明为一个全局变量
        private float X;//当前窗体的宽度
        private float Y;//当前窗体的高度
        private Queue<double> dataQueue = new Queue<double>(100);
        private int num = 5;//每次删除增加几个点
        double num2 = 0;//字符串转化为数字结果的存储
        List<byte> buffer = new List<byte>();//数据缓存
        private volatile bool is_serial_listening = false;//串口正在监听标记
        private volatile bool is_serial_closing = false;//串口正在关闭标记


        public Form1()
        {
            
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //获取电脑当前可用串口并添加到选项列表中
            comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            int i;
            //单个添加
            for (i = 300; i <= 38400; i = i*2)
            {
                comboBox2.Items.Add(i.ToString());  //添加波特率列表
            }
            for (i = 7; i <= 8; i = i+1)
            {
                comboBox3.Items.Add(i.ToString());  //添加数据位
            }


            comboBox4.Items.Add("None");  //添加校验位
            comboBox4.Items.Add("Odd");
            comboBox4.Items.Add("Even");
            comboBox4.Items.Add("Mark");
            comboBox4.Items.Add("Space");

            comboBox5.Items.Add("1");  //添加停止位
            comboBox5.Items.Add("1.5");
            comboBox5.Items.Add("2");

            //批量添加波特率列表
            string[] baud = { "43000", "56000", "57600", "115200", "128000", "230400", "256000", "460800" };
            comboBox2.Items.AddRange(baud);

            //设置默认值
            comboBox1.Text = "COM1";
            comboBox2.Text = "115200";
            comboBox3.Text = "8";
            comboBox4.Text = "None";
            comboBox5.Text = "1";
            //首先为了避免发送出错，启动时我们将发送按钮失能，只有成功打开后才使能，关闭后失能
            button2.Enabled = false;
            //控件按照窗体大小自动变化
            X = this.Width;//获取窗体的宽度
            Y = this.Height;//获取窗体的高度
            SetTag(this);//调用方法
            //InitChart();
            this.timer1.Start();
        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                //将可能产生异常的代码放置在try块中
                //根据当前串口属性来判断是否打开
                if (serialPort1.IsOpen)
                {
                    //串口已经处于打开状态，点击后关闭串口
                    is_serial_closing = true;//关闭窗口时，置位is_serial_closing标记
                    while (is_serial_listening) Application.DoEvents();
                    serialPort1.Close();    //关闭串口
                    //设置打开串口按键
                    button1.Text = "打开串口";
                    button1.BackColor = Color.ForestGreen;
                    //设置下拉组合框控件，使能
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    //textBox_receive.Text = "";  //清空接收区
                    textBox_send.Text = "";     //清空发送区
                    //串口状态显示
                    label6.Text = "串口已关闭";
                    label6.ForeColor = Color.Red;
                    //让发送按键失能
                    button2.Enabled = false;
                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开，点击后打开串口
                    //设置下拉组合框控件
                    is_serial_listening = false;//串口正在监听标记
                    is_serial_closing = false;//串口正在关闭标记
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    //设置串口控件
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);
                    //校验位判断
                    if (comboBox4.Text.Equals("None"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("Odd"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("Even"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;
                    else if (comboBox4.Text.Equals("Mark"))
                        serialPort1.Parity = System.IO.Ports.Parity.Mark;
                    else if (comboBox4.Text.Equals("Space"))
                        serialPort1.Parity = System.IO.Ports.Parity.Space;
                    //停止位判断
                    if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open();     //打开串口
                    button1.Text = "关闭串口";
                    button1.BackColor = Color.Firebrick;
                    //串口状态显示
                    label6.Text = "串口已打开";
                    label6.ForeColor = Color.Green;
                    //让发送按键使能
                    button2.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                //捕获可能发生的异常并进行处理

                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            /*串口发送有两种方法，一种是字符串发送WriteLine，一种是Write（），
            可以发送一个字符串或者16进制发送（见下篇），其中字符串发送WriteLine
            默认已经在末尾添加换行符；*/
            //try
            //{
            //    //首先判断串口是否开启
            //    if (serialPort1.IsOpen)
            //    {
            //        //串口处于开启状态，将发送区文本发送
            //        serialPort1.Write(textBox_send.Text);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //捕获到异常，创建一个新的对象，之前的不可以再用
            //    serialPort1 = new System.IO.Ports.SerialPort();
            //    //刷新COM口选项
            //    comboBox1.Items.Clear();
            //    comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            //    //响铃并显示异常给用户
            //    System.Media.SystemSounds.Beep.Play();
            //    button1.Text = "打开串口";
            //    button1.BackColor = Color.ForestGreen;
            //    MessageBox.Show(ex.Message);
            //    comboBox1.Enabled = true;
            //    comboBox2.Enabled = true;
            //    comboBox3.Enabled = true;
            //    comboBox4.Enabled = true;
            //    comboBox5.Enabled = true;
            //}
            byte[] temp = new byte[1];
            try
            {
                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    int num = 0;   //获取本次发送字节数
                                   //串口处于开启状态，将发送区文本发送

                    //判断发送模式
                    if (radioButton4.Checked)
                    {
                        //以HEX模式发送
                        //首先需要用正则表达式将用户输入字符中的十六进制字符匹配出来
                        string buf = textBox_send.Text;
                        string pattern = @"\s";
                        string replacement = "";
                        Regex rgx = new Regex(pattern);
                        string send_data = rgx.Replace(buf, replacement);

                        //不发送新行
                        num = (send_data.Length - send_data.Length % 2) / 2;
                        for (int i = 0; i < num; i++)
                        {
                            temp[0] = Convert.ToByte(send_data.Substring(i * 2, 2), 16);
                            serialPort1.Write(temp, 0, 1);  //循环发送
                        }
                        //如果用户输入的字符是奇数，则单独处理
                        if (send_data.Length % 2 != 0)
                        {
                            temp[0] = Convert.ToByte(send_data.Substring(textBox_send.Text.Length - 1, 1), 16);
                            serialPort1.Write(temp, 0, 1);
                            num++;
                        }
                        //判断是否需要发送新行
                        if (checkBox3.Checked)
                        {
                            //自动发送新行
                            serialPort1.WriteLine("");
                        }
                    }
                    else
                    {
                        //以ASCII模式发送
                        //判断是否需要发送新行
                        if (checkBox3.Checked)
                        {
                            //自动发送新行
                            serialPort1.WriteLine(textBox_send.Text);
                            num = textBox_send.Text.Length + 2; //回车占两个字节
                        }
                        else
                        {
                            //不发送新行
                            serialPort1.Write(textBox_send.Text);
                            num = textBox_send.Text.Length;
                        }
                    }

                    send_count += num;      //计数变量累加
                    label8.Text = "Tx:" + send_count.ToString() + "Bytes";   //刷新界面
                }
            }
            catch (Exception ex)
            {
                serialPort1.Close();
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;
            }
        }

        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            /* 先为串口注册一个Receive事件，相当于单片机中的串口接收中断，
             * 然后在中断内部对缓冲区的数据进行读取*/
            /* 串口接收也有两种方法，一种是16进制方式读（下篇介绍），一种是字符串方式读*/
            /* 这个串口接收处理函数属于一个单独的线程，不属于main的主线程，而接收区的TextBox
             * 是在主线程中创建的，所以当我们直接用serialPort1.ReadExisting()读取回来字符串，
             * 然后用追加到textBox_receive.AppendText（）追加到接收显示文本框中的时候，串口
             * 助手在运行时没有反应，甚至报异常，所以，这个时候我们就需要用到invoke方式，
             * 这种方式专门被用于解决从不是创建控件的线程访问它，加入了invoke方式后，串口助手
             * 就可以正常接收到数据了*/
            /* 简单说明：非常典型的应用就是在子线程里刷新主线程的控件比如说 我按一个按钮去删除
             * 一个文件，然后删除完了在一个文本框内提示 删除完成。文件小删除快，那么不会有问题。
             * 如果文件 太大，界面就会卡死，这样效果太不好了，于是我们可以使用多线程。点击按钮
             * 的时候开一个线程去删除文件，然后给个等待提示。这样整个界面不至于卡死（没有响应）。
             * 但是问题又来了。为微软不让你在子线程里访问主线程创建的控件，他认为这个是不安全的。
             * 所以就开了一个委托来异步操作一下。就向你前面写的东西。 */
            if (is_serial_closing)
            {
                is_serial_listening = false; //准备关闭串口时，reset串口侦听标记
                return;
            }

            int num = serialPort1.BytesToRead;      //获取接收缓冲区中的字节数
            byte[] received_buf = new byte[num];    //声明一个大小为num的字节数据用于存放读出的byte型数据
            byte[] received_buf2 = new byte[200] ;
            receive_count += num;                   //接收字节计数变量增加nun
            serialPort1.Read(received_buf, 0, num);   //读取接收缓冲区中num个字节到byte数组中
            //1.缓存数据
            buffer.AddRange(received_buf);//不断地将接收到的数据加入到buffer链表中
            //2.完整性判断
            while (buffer.Count >= 5) //至少包含帧头（2字节）、长度（1字节）、校验位（1字节）,功能位（1字节）；根据设计不同而不同
            {
                //2.1 查找数据头
                if (buffer[0] == 0x0AA) //传输数据有帧头，用于判断. 找到帧头  AA AA 0A 
                {
                    int len = buffer[2];
                    //int len = 79;
                    if (buffer.Count < len + 5) //数据区尚未接收完整，
                    {
                        break;//跳出接收函数后之后继续接收数据
                    }
                    //得到完整的数据，复制到ReceiveBytes中进行校验
                    buffer.CopyTo(0, received_buf2, 0, len + 5);//
                    byte jiaoyan; //开始校验
                    jiaoyan = 0x01;//jiaoyan = this.JY(ReceiveBytes);

                    if (jiaoyan != received_buf2[3]) //验证功能位失败    if (jiaoyan != ReceiveBytes[len+3])
                    {
                        buffer.RemoveRange(0, len + 5);//从链表中移除接收到的校验失败的数据，
                        //MessageBox.Show("数据包不正确！");//显示数据包不正确,
                        continue;//继续执行while循环程序,
                    }
                    //数据分类
                    byte[] received_buf3 = new byte[len];//截取取有用数据
                    buffer.CopyTo(5, received_buf3, 0, len);
                    switch (received_buf2[4])
                    {
                        case 0x11 :
                            PublicValue.PublicValue1.Voltage_Input.Clear();
                            PublicValue.PublicValue1.Voltage_Input.Append(Encoding.ASCII.GetString(received_buf3));
                            break;
                        case 0x12:
                            PublicValue.PublicValue1.Current_Input.Clear();
                            PublicValue.PublicValue1.Current_Input.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x13:
                            PublicValue.PublicValue1.Voltage_Output.Clear();
                            PublicValue.PublicValue1.Voltage_Output.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x14:
                            PublicValue.PublicValue1.Current_Output.Clear();
                            PublicValue.PublicValue1.Current_Output.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x15:
                            PublicValue.PublicValue1.Voltage_Cap_Input.Clear();
                            PublicValue.PublicValue1.Voltage_Cap_Input.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x16:
                            PublicValue.PublicValue1.Current_Cap_Input.Clear();
                            PublicValue.PublicValue1.Current_Cap_Input.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x17:
                            PublicValue.PublicValue1.Voltage_Cap_Output.Clear();
                            PublicValue.PublicValue1.Voltage_Cap_Output.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x18:
                            PublicValue.PublicValue1.power_input.Clear();
                            PublicValue.PublicValue1.power_input.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x19:
                            PublicValue.PublicValue1.power_cap.Clear();
                            PublicValue.PublicValue1.power_cap.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0x1A:
                            PublicValue.PublicValue1.power_output.Clear();
                            PublicValue.PublicValue1.power_output.Append(Encoding.ASCII.GetString(received_buf3)); 
                            break;
                        case 0xFF:
                            PublicValue.PublicValue1.Usart_sent_flage=1;
                            System.Threading.Thread.Sleep(1);

                            break;
                        default:;
                            break;
                    }
                    buffer.RemoveRange(0, len + 5);
                    double num3 = new double();
                    if (double.TryParse(PublicValue.PublicValue1.power_input.ToString(), out num3))
                    {
                        num2 = num3;
                    }

                    //执行其他代码，对数据进行处理。
                    //解析5 6， 7 8字节的经纬度.
                    //DataProgress();
                }
                else //帧头不正确时，记得清除
                {
                    buffer.RemoveAt(0);//清除第一个字节，继续检测下一个。

                }
            }
            //接第二步中的代码
            sb.Clear();     //防止出错,首先清空字符串构造器
            if (radioButton2.Checked)
            {
                //选中HEX模式显示
                //遍历数组进行字符串转化及拼接
                foreach (byte b in received_buf)
                {
                    sb.Append(b.ToString("X2") + ' ');    //将byte型数据转化为2位16进制文本显示,用空格隔开
                }
                
            }
            else
            {
                //选中ASCII模式显示
                sb.Append(Encoding.ASCII.GetString(received_buf));  //将整个数组解码为ASCII数组
                //string str = sb;   
                //string[] condition = { ":" };
                
                //字符串转化为数字

                

            }

            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                if (serialPort1.IsOpen)
                {
                    is_serial_listening = true;
                }
                this.Invoke((EventHandler)(delegate
                {
                    if (checkBox1.Checked)
                    {
                        //显示时间
                        current_time = System.DateTime.Now;     //获取当前时间
                        textBox_receive.AppendText(current_time.ToString("HH:mm:ss") + "  " + sb.ToString());

                    }
                    else
                    {
                        //不显示时间 
                        textBox_receive.AppendText(sb.ToString());
                    }
                    label7.Text = "Rx:" + receive_count.ToString() + "Bytes";

                }
                  )
                );
            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);

            }
            finally
            {
                is_serial_listening = false;//串口调用完毕后，reset串口侦听标记
            }

        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Label7_Click(object sender, EventArgs e)
        {

        }

        private void TextBox_receive_TextChanged(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            textBox_receive.Text = "";  //清空接收文本框
            textBox_send.Text = "";     //清空发送文本框
            receive_count = 0;          //计数清零
            label7.Text = "Rx:" + receive_count.ToString() + "Bytes";   //刷新界面
        }

        private void Form1_Resiz(object sender, EventArgs e)
        {
            float newx = (this.Width) / X; //窗体宽度缩放比例
            float newy = (this.Height) / Y;//窗体高度缩放比例
            SetControls(newx, newy, this);//随窗体改变控件大小
        }


        /// 将控件的宽，高，左边距，顶边距和字体大小暂存到tag属性中
        /// </summary>
        /// <param name="cons">递归控件中的控件</param>
        private void SetTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    SetTag(con);
            }
        }
        private void SetControls(float newx, float newy, Control cons)
        {
            //遍历窗体中的控件，重新设置控件的值
            foreach (Control con in cons.Controls)
            {

                if (con.Tag == null) continue;//源码中没有，不加报错
                string[] mytag = con.Tag.ToString().Split(new char[] { ':' });//获取控件的Tag属性值，并分割后存储字符串数组
                float a = System.Convert.ToSingle(mytag[0]) * newx;//根据窗体缩放比例确定控件的值，宽度
                con.Width = (int)a;//宽度
                a = System.Convert.ToSingle(mytag[1]) * newy;//高度
                con.Height = (int)(a);
                a = System.Convert.ToSingle(mytag[2]) * newx;//左边距离
                con.Left = (int)(a);
                a = System.Convert.ToSingle(mytag[3]) * newy;//上边缘距离
                con.Top = (int)(a);
                Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字体大小
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    SetControls(newx, newy, con);
                }
            }
        }

        private void Chart1_Click(object sender, EventArgs e)
        {

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            UpdateQueueValue();
            this.chart1.Series[0].Points.Clear();
            for (int i = 0; i < dataQueue.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY((i + 1), dataQueue.ElementAt(i));
            }
            if (radioButton5.Checked)
            {
                this.chart1.Titles[0].Text = string.Format("输入功率 {0} 显示", radioButton5.Text);
                this.chart1.Series[0].ChartType = SeriesChartType.Line;
            }
            if (radioButton6.Checked)
            {
                this.chart1.Titles[0].Text = string.Format("输入功率 {0} 显示", radioButton6.Text);
                this.chart1.Series[0].ChartType = SeriesChartType.Spline;
            }
            if (PublicValue1.Usart_sent_flage == 1)//发送指令数据Usart_power_input_val_sent
            {
                this.serialPort1.Write(string.Format("power_set{0}", PublicValue1.Usart_power_input_val_sent.ToString()));
                PublicValue1.Usart_sent_flage = 0;
            }
            else if(PublicValue1.Usart_sent_flage == 0) { }



        }
        /// <summary>
        /// 初始化图表
        /// </summary>
        //private void InitChart()
        //{

        //    //定义图表区域
        //    this.chart1.ChartAreas.Clear();
        //    ChartArea chartArea1 = new ChartArea("C1");
        //    this.chart1.ChartAreas.Add(chartArea1);
        //    //定义存储和显示点的容器
        //    this.chart1.Series.Clear();
        //    Series series1 = new Series("S1");
        //    series1.ChartArea = "C1";
        //    this.chart1.Series.Add(series1);
        //    // 画样条曲线（Spline）
        //    series1.ChartType = SeriesChartType.Spline;
        //    // 线宽2个像素
        //    series1.BorderWidth = 2;
        //    // 线的颜色：红色
        //    series1.Color = System.Drawing.Color.Red;
        //    //设置图表显示样式
        //    this.chart1.ChartAreas[0].AxisY.Minimum = 0;
        //    this.chart1.ChartAreas[0].AxisY.Maximum = 140;
        //    this.chart1.ChartAreas[0].AxisX.Interval = 5;
        //    this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
        //    this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
        //    //设置标题
        //    this.chart1.Titles.Clear();
        //    this.chart1.Titles.Add("S01");
        //    this.chart1.Titles[0].Text = "XXX显示";
        //    this.chart1.Titles[0].ForeColor = Color.RoyalBlue;
        //    this.chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
        //    //设置图表显示样式
        //    this.chart1.Series[0].Color = Color.Red;
        //    if (radioButton5.Checked)
        //    {
        //        this.chart1.Titles[0].Text = string.Format("XXX {0} 显示", radioButton5.Text);
        //        this.chart1.Series[0].ChartType = SeriesChartType.Line;
        //    }
        //    if (radioButton6.Checked)
        //    {
        //        this.chart1.Titles[0].Text = string.Format("XXX {0} 显示", radioButton6.Text);
        //        this.chart1.Series[0].ChartType = SeriesChartType.Spline;
        //    }
        //    this.chart1.Series[0].Points.Clear();
        //}
        //更新队列中的值
        private void UpdateQueueValue()
        {

            if (dataQueue.Count > 100)
            {
                //先出列
                for (int i = 0; i < num; i++)
                {
                    dataQueue.Dequeue();
                }
            }
            for (int i = 0; i < num; i++)
            {
                ////对curValue只取[0,360]之间的值
                //curValue = curValue % 360;
                ////对得到的正玄值，放大50倍，并上移50
                //dataQueue.Enqueue((50 * Math.Sin(curValue * Math.PI / 180)) + 50);
                //curValue = curValue + 10;
                dataQueue.Enqueue(num2);
            }
            

        }


        private void RadioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Label9_Click(object sender, EventArgs e)
        {

        }

        private void Chart1_Click_1(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();//新窗口显现
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void TextBox_send_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label10_Click(object sender, EventArgs e)
        {

        }
    }
}            
