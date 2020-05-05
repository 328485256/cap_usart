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

namespace uart
{
    public partial class Form2 : Form
    {
        private float X;//当前窗体的宽度
        private float Y;//当前窗体的高度
        private Queue<double> dataQueue = new Queue<double>(100);
        private int num = 5;//每次删除增加几个点
        private Queue<double> Voltage_Input = new Queue<double>(100);
        private Queue<double> Current_Input = new Queue<double>(100);
        private Queue<double> Voltage_Output = new Queue<double>(100);
        private Queue<double> Current_Output = new Queue<double>(100);
        private Queue<double> Voltage_Cap_Input = new Queue<double>(100);
        private Queue<double> Current_Cap_Input = new Queue<double>(100);
        private Queue<double> Voltage_Cap_Output = new Queue<double>(100);
        private Queue<double> power_input = new Queue<double>(100);
        private Queue<double> power_cap = new Queue<double>(100);
        private Queue<double> power_output = new Queue<double>(100);



        public Form2()
        {
            InitializeComponent();//导入设计界面设计好的参数
        }

        private void Form2_Load(object sender, EventArgs e)
        {            
            //控件按照窗体大小自动变化
            X = this.Width;//获取窗体的宽度
            Y = this.Height;//获取窗体的高度
            this.label1.Text = string.Format("输入电压:{0:N3}V",0);
            SetTag(this);//调用方法
            InitChart();
            this.timer1.Start();
            this.chart1.Titles[0].Text = string.Format("输入电容功率显示");

        }
        private void InitChart()//如果在设计界面中已经设计好，这里就不需要重复设置，如果这里设置了，会覆盖原本设置
        {

            ////定义图表区域
            //this.chart1.ChartAreas.Clear();
            //ChartArea chartArea1 = new ChartArea("C1");
            //this.chart1.ChartAreas.Add(chartArea1);
            ////定义存储和显示点的容器
            //this.chart1.Series.Clear();
            //Series series1 = new Series("S1");
            //series1.ChartArea = "C1";
            //this.chart1.Series.Add(series1);
            //this.chart1.Series.Add(series2);
            //this.chart1.Series.Add(series3);
            //this.chart1.Series.Add(series4);
            //this.chart1.Series.Add(series5);
            //this.chart1.Series.Add(series6);
            //this.chart1.Series.Add(series7);
            //this.chart1.Series.Add(series8);
            //this.chart1.Series.Add(series9);
            //this.chart1.Series.Add(series10);

            //// 画样条曲线（Spline）
            //series1.ChartType = SeriesChartType.Spline;
            //// 线宽2个像素
            //series1.BorderWidth = 2;
            //// 线的颜色：红色
            //series1.Color = System.Drawing.Color.Red;
            ////设置图表显示样式
            //this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            //this.chart1.ChartAreas[0].AxisY.Maximum = 200;
            //this.chart1.ChartAreas[0].AxisX.Interval = 5;
            //this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            //this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            ////设置标题
            //this.chart1.Titles.Clear();
            //this.chart1.Titles.Add("S01");
            //this.chart1.Titles[0].Text = "XXX显示";
            //this.chart1.Titles[0].ForeColor = Color.RoyalBlue;
            //this.chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            ////设置图表显示样式
            //this.chart1.Series[0].Color = Color.Red;

            //this.chart1.Titles[0].Text = string.Format("Spline显示");
            //this.chart1.Series[0].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[0].Points.Clear();
            //this.chart1.Series[1].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[1].Points.Clear();
            //this.chart1.Series[2].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[2].Points.Clear();
            //this.chart1.Series[3].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[3].Points.Clear();
            //this.chart1.Series[4].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[4].Points.Clear();
            //this.chart1.Series[5].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[5].Points.Clear();
            //this.chart1.Series[6].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[6].Points.Clear();
            //this.chart1.Series[7].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[7].Points.Clear();
            //this.chart1.Series[8].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[8].Points.Clear();
            //this.chart1.Series[9].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[9].Points.Clear();
            //this.chart1.Series[10].ChartType = SeriesChartType.Spline;
            //this.chart1.Series[10].Points.Clear();

        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            //更新各个变量队列
            UpdateQueueValue(Voltage_Input, PublicValue1.Voltage_Input_val);
            UpdateQueueValue(Current_Input, PublicValue1.Current_Input_val);
            UpdateQueueValue(Voltage_Output, PublicValue1.Voltage_Output_val);
            UpdateQueueValue(Current_Output, PublicValue1.Current_Output_val);
            UpdateQueueValue(Voltage_Cap_Input, PublicValue1.Voltage_Cap_Input_val);
            UpdateQueueValue(Current_Cap_Input, PublicValue1.Current_Cap_Input_val);
            UpdateQueueValue(Voltage_Cap_Output, PublicValue1.Voltage_Cap_Output_val);
            UpdateQueueValue(power_input, PublicValue1.power_input_val);
            UpdateQueueValue(power_cap, PublicValue1.power_cap_val);
            UpdateQueueValue(power_output, PublicValue1.power_output_val);
            //清空所有系列的点
            for (int i = 0; i < 10; i++)
            {
                this.chart1.Series[i].Points.Clear();
            }
            //显示所有系列的点
            for (int i = 0; i < Voltage_Input.Count; i++)
            {
                this.chart1.Series[0].Points.AddXY((i + 1), Voltage_Input.ElementAt(i));
                this.chart1.Series[1].Points.AddXY((i + 1), Current_Input.ElementAt(i));
                this.chart1.Series[2].Points.AddXY((i + 1), Voltage_Output.ElementAt(i));
                this.chart1.Series[3].Points.AddXY((i + 1), Current_Output.ElementAt(i));
                this.chart1.Series[4].Points.AddXY((i + 1), Voltage_Cap_Input.ElementAt(i));
                this.chart1.Series[5].Points.AddXY((i + 1), Current_Cap_Input.ElementAt(i));
                this.chart1.Series[6].Points.AddXY((i + 1), Voltage_Cap_Output.ElementAt(i));
                this.chart1.Series[7].Points.AddXY((i + 1), power_input.ElementAt(i));
                this.chart1.Series[8].Points.AddXY((i + 1), power_cap.ElementAt(i));
                this.chart1.Series[9].Points.AddXY((i + 1), power_output.ElementAt(i));

            }
            //在label显示各个参数的值
            this.label1.Text = string.Format("输入电压V:{0:N2}", PublicValue.PublicValue1.Voltage_Input.ToString());
            this.label2.Text = string.Format("输入电流A:{0:N2}", PublicValue.PublicValue1.Current_Input.ToString());
            this.label3.Text = string.Format("输出电压V:{0:N2}", PublicValue.PublicValue1.Voltage_Output.ToString());
            this.label4.Text = string.Format("输出电流A:{0:N2}", PublicValue.PublicValue1.Current_Output.ToString());
            this.label5.Text = string.Format("电容电压V:{0:N2}", PublicValue.PublicValue1.Voltage_Cap_Input.ToString());
            this.label6.Text = string.Format("电容电流A:{0:N2}", PublicValue.PublicValue1.Current_Cap_Input.ToString());
            this.label7.Text = string.Format("电容输出电压V:{0:N2}", PublicValue.PublicValue1.Voltage_Cap_Output.ToString());
            this.label8.Text = string.Format("输入功率W:{0:N2}", PublicValue.PublicValue1.power_input.ToString());
            this.label9.Text = string.Format("输入电容功率W:{0:N2}", PublicValue.PublicValue1.power_cap.ToString());
            this.label10.Text = string.Format("输出功率W:{0:N2}", PublicValue.PublicValue1.power_output.ToString());
            //将字符串转化为数值
            double num_temp = new double();
            if (double.TryParse(PublicValue.PublicValue1.Voltage_Input.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Voltage_Input_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.Current_Input.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Current_Input_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.Voltage_Output.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Voltage_Output_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.Current_Output.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Current_Output_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.Voltage_Cap_Input.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Voltage_Cap_Input_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.Current_Cap_Input.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Current_Cap_Input_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.Voltage_Cap_Output.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.Voltage_Cap_Output_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.power_input.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.power_input_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.power_cap.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.power_cap_val = num_temp;
            }
            if (double.TryParse(PublicValue.PublicValue1.power_output.ToString(), out num_temp))
            {
                PublicValue.PublicValue1.power_output_val = num_temp;
            }
            //清空数据，为下次更新做准备
            PublicValue.PublicValue1.Voltage_Input.Clear();
            PublicValue.PublicValue1.Current_Input.Clear();
            PublicValue.PublicValue1.Voltage_Output.Clear();
            PublicValue.PublicValue1.Current_Output.Clear();
            PublicValue.PublicValue1.Voltage_Cap_Input.Clear();
            PublicValue.PublicValue1.Current_Cap_Input.Clear();
            PublicValue.PublicValue1.Voltage_Cap_Output.Clear();
            PublicValue.PublicValue1.power_input.Clear();
            PublicValue.PublicValue1.power_cap.Clear();
            PublicValue.PublicValue1.power_output.Clear();
            PublicValue.PublicValue1.sb.Clear();
            //选择要显示的数据的波形
            if (checkBox1.Checked == false)
            {
                this.chart1.Series[0].Enabled = false;
            }
            else
            {
                this.chart1.Series[0].Enabled = true;
            }
            if (checkBox2.Checked == false)
            {
                this.chart1.Series[1].Enabled = false;
            }
            else
            {
                this.chart1.Series[1].Enabled = true;
            }
            if (checkBox3.Checked == false)
            {
                this.chart1.Series[2].Enabled = false;
            }
            else
            {
                this.chart1.Series[2].Enabled = true;
            }
            if (checkBox4.Checked == false)
            {
                this.chart1.Series[3].Enabled = false;
            }
            else
            {
                this.chart1.Series[3].Enabled = true;
            }
            if (checkBox5.Checked == false)
            {
                this.chart1.Series[4].Enabled = false;
            }
            else
            {
                this.chart1.Series[4].Enabled = true;
            }
            if (checkBox6.Checked == false)
            {
                this.chart1.Series[5].Enabled = false;
            }
            else
            {
                this.chart1.Series[5].Enabled = true;
            }
            if (checkBox7.Checked == false)
            {
                this.chart1.Series[6].Enabled = false;
            }
            else
            {
                this.chart1.Series[6].Enabled = true;
            }
            if (checkBox8.Checked == false)
            {
                this.chart1.Series[7].Enabled = false;
            }
            else
            {
                this.chart1.Series[7].Enabled = true;
            }
            if (checkBox9.Checked == false)
            {
                this.chart1.Series[8].Enabled = false;
            }
            else
            {
                this.chart1.Series[8].Enabled = true;
            }
            if (checkBox10.Checked == false)
            {
                this.chart1.Series[9].Enabled = false;
            }
            else
            {
                this.chart1.Series[9].Enabled = true;
            }
            //显示要输出的功率
            this.label11.Text = string.Format("最大输入功率:{0:N2}", this.trackBar1.Value.ToString());
        }
        private void UpdateQueueValue(Queue<double> dataQ,double datein)
        {

            if (dataQ.Count > 100)
            {
                //先出列
                for (int i = 0; i < num; i++)
                {
                    dataQ.Dequeue();
                }
            }

            for (int i = 0; i < num; i++)
            {
                ////对curValue只取[0,360]之间的值
                //curValue = curValue % 360;
                ////对得到的正玄值，放大50倍，并上移50
                //dataQueue.Enqueue((50 * Math.Sin(curValue * Math.PI / 180)) + 50);
                //curValue = curValue + 10;
                dataQ.Enqueue(datein);
             }
            
        }

        private void Form2_Resiz(object sender, EventArgs e)
        {
            float newx = (this.Width) / X; //窗体宽度缩放比例
            float newy = (this.Height) / Y;//窗体高度缩放比例
            SetControls(newx, newy, this);//随窗体改变控件大小
        }

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

        private void Label1_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 1;
            this.chart1.Titles[0].Text = string.Format("输入电压显示");
        }

        private void Label2_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 2;
            this.chart1.Titles[0].Text = string.Format("输入电流显示");
        }

        private void Label3_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 3;
            this.chart1.Titles[0].Text = string.Format("输出电压显示");
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Label4_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 4;
            this.chart1.Titles[0].Text = string.Format("输出电流显示");
        }

        private void Label5_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 5;
            this.chart1.Titles[0].Text = string.Format("电容电压显示");
        }

        private void Label6_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 6;
            this.chart1.Titles[0].Text = string.Format("电容电流显示");
        }

        private void Label7_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 7;
            this.chart1.Titles[0].Text = string.Format("电容输出电压显示");
        }

        private void Label8_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 8;
            this.chart1.Titles[0].Text = string.Format("输入功率显示");
        }

        private void Label9_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 9;
            this.chart1.Titles[0].Text = string.Format("输入电容功率显示");
        }

        private void Label10_Click(object sender, EventArgs e)
        {
            PublicValue1.chart_flage = 10;
            this.chart1.Titles[0].Text = string.Format("输出电压显示");
        }

        private void Chart1_Click(object sender, EventArgs e)
        {

        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CheckBox7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Label11_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            PublicValue1.Usart_power_input_val_sent = this.trackBar1.Value;
            PublicValue1.Usart_sent_flage = 1;


        }
    }
}
