using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Web.Script.Serialization;

namespace RealCurrentChart
{
    public partial class Chart : Form
    {
        private int[] Count = {11, 11, 11, 11};
        private String[] logname = { "temp1.log", "temp2.log", "temp3.log", "temp4.log" };
        private String logpath = @"D:\log\";
        public Chart()
        {
            InitializeComponent();
            InitChart();
            //json
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                StreamReader sr = File.OpenText("./Chart.json"); // \\或者@"\"或者/或者./
                string chart = sr.ReadToEnd();
                sr.Close();
                var p = serializer.Deserialize<JSON>(chart);
                this.DogTimer.Interval = p.TimerInterval;
                this.chart1.ChartAreas[0].AxisY.Maximum = p.AxisYMax;
                this.chart1.ChartAreas[0].AxisY.Interval = p.AxisYInterval;
                this.chart1.Location = new Point(p.ChartLocationX,p.ChartLocationY);
                this.chart1.Size=new Size(p.ChartSizeX,p.ChartSizeY);
                chart1.ChartAreas[0].AxisX.ScaleView.Size = p.AxisXScaleViewSize;
                logpath = p.FilePath;
                logname[0] = p.Filename1;
                logname[1] = p.Filename2;
                logname[2] = p.Filename3;
                logname[3] = p.Filename4;
            }
            catch
            {
                //no to do
            }
        }

        /// <summary>
        /// 初始化图表
        /// </summary>
        private void InitChart() {
            //定义图表区域
            this.chart1.ChartAreas.Clear();
            ChartArea chartArea1 = new ChartArea("Current");
            chart1.ChartAreas.Add(chartArea1);
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;         
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 10;
            //定义存储和显示点的容器
            this.chart1.Series.Clear();
            for (int i = 1; i < 5; i++)
            {
                Series temp = new Series(i+"电机");
                temp.ChartArea = "Current";
                this.chart1.Series.Add(temp);
            }
            
            //设置图表显示样式
            this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.chart1.ChartAreas[0].AxisY.Maximum = 30;
            this.chart1.ChartAreas[0].AxisY.Interval = 0.5;
            this.chart1.ChartAreas[0].AxisY.Title = "电流 / A";
            this.chart1.ChartAreas[0].AxisX.Interval = 1;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            //设置标题
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("Current");
            this.chart1.Titles[0].Text = "电流显示";
            this.chart1.Titles[0].ForeColor = Color.RoyalBlue;
            this.chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.chart1.Titles[0].Text = string.Format("电机电流显示", "dd");
            //设置图表显示样式
            Color[] show = new Color[] {Color.Red, Color.Black, Color.Green, Color.Blue};
            for (int i = 0; i < 4; i++)
            {
                this.chart1.Series[i].Color = show[i];
                this.chart1.Series[i].ChartType = SeriesChartType.Spline;
            }
            
            //this.chart1.Series[0].Points.Clear();
            for (int i = 1; i < 11; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.chart1.Series[j].Points.AddXY(i, 0);
                }
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            this.DogTimer.Start();
        }
        private void Dog_timer_tick(object sender, EventArgs e) 
        {
            //read log and show data
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    StreamReader sr = new StreamReader(logpath+logname[i], Encoding.Default);
                    String[] line = sr.ReadLine().Split('\t');
                    this.chart1.Series[0].Points.AddXY(Count[i]++, double.Parse(line[2]));
                    sr.Close();
                }
                catch
                {
                    //
                }
            }
            chart1.ChartAreas[0].AxisX.ScaleView.Position = Math.Max(Math.Max(Count[0], Count[1]), Math.Max(Count[2], Count[3])) - chart1.ChartAreas[0].AxisX.ScaleView.Size;
            
        }

        private void pause_Click(object sender, EventArgs e)
        {
            if (this.DogTimer.Enabled)
            {
                this.DogTimer.Enabled = false;
            }
            else
            {
                this.DogTimer.Enabled = true;
            }
            
        }

        private void Restart_Click(object sender, EventArgs e)
        {
            if (this.DogTimer.Enabled)
            {
                this.DogTimer.Enabled = false;
            }
            for (int j = 0; j < 4; j++)
            {
                this.chart1.Series[j].Points.Clear();
            }
            for (int i = 0; i < 4; i++)
            {
                Count[i] = 1;
            }

        }

        private void SaveChart_Click(object sender, EventArgs e)
        {
            try
            {
                chart1.SaveImage("./chart" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpeg", ChartImageFormat.Jpeg);
                MessageBoxOut.Show("Save Chart Success!", "提示", 500, 0);
            }
            catch
            {
                //
            }
        }
    }
}
