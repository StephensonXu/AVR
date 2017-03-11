using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealCurrentChart
{
    class JSON
    {
        public int TimerInterval { get; set; }

        public int ChartLocationX { get; set; }

        public int ChartLocationY { get; set; }

        public int ChartSizeX { get; set; }

        public int ChartSizeY { get; set; }

        public int AxisYMax { get; set; }

        public double AxisYInterval { get; set; }

        public int AxisXScaleViewSize { get; set; }

        public String FilePath { get; set; }

        public String Filename1 { get; set; }

        public String Filename2 { get; set; }

        public String Filename3 { get; set; }

        public String Filename4 { get; set; }
        
    }
}
