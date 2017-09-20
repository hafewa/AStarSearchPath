using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStarSearch
{
    class MapCell
    {
        public static Point StartPoint;
        public static Point TargetPoint;

        /// <summary>
        /// 上级Cell，用于回溯路径
        /// </summary>
        public MapCell ParentMapCell=null;

        /// <summary>
        /// 当前Cell到开始和目标坐标距离总和
        /// </summary>
        public int DistanceCount = 0;

        /// <summary>
        /// 当前cell到开始坐标的距离
        /// </summary>
        private int DistanceToStart = 0;

        /// <summary>
        /// 当前cell到目标坐标的距离
        /// </summary>
        private int DistanceToTarget = 0;

        /// <summary>
        /// 显示控件
        /// </summary>
        public readonly Label DisplayLabel;

        private Point mapPoint;
        /// <summary>
        /// Cell在地图里的坐标
        /// </summary>
        public Point MapPoint
        {
            get
            {
                return mapPoint;
            }
            private set
            {
                mapPoint = value;
                DisplayLabel.Location = new Point(value.X * DisplayLabel.Width,value.Y * DisplayLabel.Height);
            }
        }

        private bool isObstacle=false;
        /// <summary>
        /// 是否为障碍物
        /// </summary>
        public bool IsObstacle
        {
            get
            {
                return isObstacle;
            }

            set
            {
                isObstacle = value;
                DisplayLabel.BackColor = value ? Color.DimGray : Color.WhiteSmoke;
            }
        }

        public MapCell(byte x,byte y,byte labelWidth,byte labelHeight)
        {
            DistanceToStart = Math.Abs(x - StartPoint.X) + Math.Abs(y - StartPoint.Y);
            DistanceToTarget = Math.Abs(TargetPoint.X - x) + Math.Abs(TargetPoint.Y - y);
            DistanceCount = DistanceToTarget + DistanceToStart;

            DisplayLabel = new Label()
            {
                BorderStyle = BorderStyle.FixedSingle,
                AutoSize = false,
                Size = new Size(labelWidth, labelHeight),
                Visible = true,
                Tag=new Point(x,y),
                Text = string.Format("{4}-{0},{1}\n{2}\n{3}", x, y, DistanceToStart,DistanceToTarget, DistanceCount),
            };

            MapPoint = new Point(x,y);
        }

    }
}
