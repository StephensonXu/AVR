namespace RealCurrentChart
{
    partial class Chart
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chart));
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Start = new System.Windows.Forms.Button();
            this.DogTimer = new System.Windows.Forms.Timer(this.components);
            this.pause = new System.Windows.Forms.Button();
            this.reStart = new System.Windows.Forms.Button();
            this.SaveChart = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(38, 50);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(689, 443);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // Start
            // 
            this.Start.Location = new System.Drawing.Point(85, 12);
            this.Start.Name = "Start";
            this.Start.Size = new System.Drawing.Size(114, 32);
            this.Start.TabIndex = 1;
            this.Start.Text = "Start";
            this.Start.UseVisualStyleBackColor = true;
            this.Start.Click += new System.EventHandler(this.Start_Click);
            // 
            // DogTimer
            // 
            this.DogTimer.Interval = 1000;
            this.DogTimer.Tick += new System.EventHandler(this.Dog_timer_tick);
            // 
            // pause
            // 
            this.pause.Location = new System.Drawing.Point(251, 12);
            this.pause.Name = "pause";
            this.pause.Size = new System.Drawing.Size(114, 32);
            this.pause.TabIndex = 2;
            this.pause.Text = "Pause";
            this.pause.UseVisualStyleBackColor = true;
            this.pause.Click += new System.EventHandler(this.pause_Click);
            // 
            // reStart
            // 
            this.reStart.Location = new System.Drawing.Point(409, 12);
            this.reStart.Name = "reStart";
            this.reStart.Size = new System.Drawing.Size(114, 32);
            this.reStart.TabIndex = 3;
            this.reStart.Text = "ReStart";
            this.reStart.UseVisualStyleBackColor = true;
            this.reStart.Click += new System.EventHandler(this.Restart_Click);
            // 
            // SaveChart
            // 
            this.SaveChart.Location = new System.Drawing.Point(571, 12);
            this.SaveChart.Name = "SaveChart";
            this.SaveChart.Size = new System.Drawing.Size(114, 32);
            this.SaveChart.TabIndex = 4;
            this.SaveChart.Text = "SaveChart";
            this.SaveChart.UseVisualStyleBackColor = true;
            this.SaveChart.Click += new System.EventHandler(this.SaveChart_Click);
            // 
            // Chart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(804, 527);
            this.Controls.Add(this.SaveChart);
            this.Controls.Add(this.reStart);
            this.Controls.Add(this.pause);
            this.Controls.Add(this.Start);
            this.Controls.Add(this.chart1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Chart";
            this.Text = "Chart";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button Start;
        private System.Windows.Forms.Timer DogTimer;
        private System.Windows.Forms.Button pause;
        private System.Windows.Forms.Button reStart;
        private System.Windows.Forms.Button SaveChart;
    }
}

