using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStarSearch
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 地图表格宽度
        /// </summary>
        private const byte TABLE_WIDTH = 16;
        /// <summary>
        /// 地图表格高度
        /// </summary>
        private const byte TABLE_HEIGHT = 12;
        /// <summary>
        /// 显示控件宽度
        /// </summary>
        private const byte LABEL_WIDTH = 50;
        /// <summary>
        /// 显示控件高度
        /// </summary>
        private const byte LABEL_HEIGHT = 50;
        /// <summary>
        /// 开始坐标
        /// </summary>
        Point StartPoint = new Point(0,5);
        /// <summary>
        /// 目标坐标
        /// </summary>
        Point TargetPoint = new Point(15,7);
        /// <summary>
        /// 显示控件二维数组
        /// </summary>
        MapCell[,] MapCells = new MapCell[TABLE_WIDTH, TABLE_HEIGHT];

        /// <summary>
        /// 当前可以到达的MapCell有序列表
        /// </summary>
        List<MapCell> OpenList;

        /// <summary>
        /// 已经到达的MapCell列表
        /// </summary>
        List<MapCell> CloseList;

        public MainForm()
        {
            InitializeComponent();
            MapCell.StartPoint = StartPoint;
            MapCell.TargetPoint = TargetPoint;

            //初始化显示控件
            for (byte x = 0; x < TABLE_WIDTH; x++)
                for (byte y = 0; y < TABLE_HEIGHT; y++)
                {
                    MapCell newMapCell = new MapCell(x,y,LABEL_WIDTH,LABEL_HEIGHT);
                    newMapCell.DisplayLabel.Click += new EventHandler(CellLabel_Click);
                    newMapCell.DisplayLabel.Parent = this;
                    MapCells[x, y] = newMapCell;
                }

            //设置障碍
            MapCells[7,4].IsObstacle = true;
            MapCells[7,5].IsObstacle = true;
            MapCells[7,6].IsObstacle = true;
            MapCells[8,6].IsObstacle = true;
            MapCells[8,7].IsObstacle = true;
            MapCells[8,8].IsObstacle = true;

            //绘制开始坐标和目标坐标
            MapCells[StartPoint.X, StartPoint.Y].DisplayLabel.BackColor = Color.DeepPink;
            MapCells[TargetPoint.X, TargetPoint.Y].DisplayLabel.BackColor = Color.DeepSkyBlue;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void CellLabel_Click(object sender, EventArgs e)
        {
            Point SenderLocation = (Point)(sender as Label).Tag;

            if (SenderLocation.Equals(StartPoint) || SenderLocation.Equals(TargetPoint))
            {
                //点击开始坐标或结束坐标开始寻路算法
                SearchPath(StartPoint,TargetPoint);
            }
            else
            {
                //切换障碍物状态
                MapCells[SenderLocation.X, SenderLocation.Y].IsObstacle = !MapCells[SenderLocation.X, SenderLocation.Y].IsObstacle;
            }
        }

        /// <summary>
        /// 搜索路径
        /// </summary>
        /// <param name="startPoint">开始坐标</param>
        /// <param name="targetPoint">目标坐标</param>
        private void SearchPath(Point startPoint, Point targetPoint)
        {
            for (byte x = 0; x < TABLE_WIDTH; x++)
                for (byte y = 0; y < TABLE_HEIGHT; y++)
                {
                    if (!MapCells[x, y].IsObstacle)
                        MapCells[x, y].DisplayLabel.BackColor = Color.WhiteSmoke;
                }

            OpenList = new List<MapCell>();
            CloseList = new List<MapCell>();

            OpenList.Add(MapCells[startPoint.X,startPoint.Y]);

            MapCell NowCell;
            while (OpenList.Count > 0)
            {
                //todo:使用二分法插入后，可删去下面的循环，FirstOrDefault()即为最优先cell
                NowCell = OpenList.FirstOrDefault();
                foreach (MapCell childCell in OpenList)
                {
                    if (childCell.DistanceCount < NowCell.DistanceCount)
                    {
                        NowCell = childCell;
                    }
                }

                NowCell.DisplayLabel.BackColor = Color.Orange;
                NowCell.DisplayLabel.Invalidate();

                OpenList.Remove(NowCell);

                CloseList.Add(NowCell);

                foreach (MapCell childCell in FindNearCell(NowCell.MapPoint))
                {
                    if (CloseList.IndexOf(childCell) != -1) continue;
                    if (OpenList.IndexOf(childCell)==-1)
                    {
                        childCell.ParentMapCell = NowCell;
                        //todo:使用二分法把新节点插入到列表内符合排序的位置，提高性能
                        OpenList.Add(childCell);

                        childCell.DisplayLabel.BackColor = Color.Pink;
                    }
                    if (childCell.MapPoint.Equals(targetPoint))
                    {
                        ShowPath(NowCell);
                        //找到路径
                        return;
                    }
                }
            }
            //没有找到路径
        }

        /// <summary>
        /// 寻找附近可到坐标
        /// </summary>
        /// <param name="CellPoint">当前坐标</param>
        /// <returns>附近可达坐标数组</returns>
        private MapCell[] FindNearCell(Point CellPoint)
        {
            //todo:在这里扩展允许对角线移动
            List<MapCell> NearCellPoints = new List<MapCell>();
            if (CellPoint.X>0 && !MapCells[CellPoint.X-1, CellPoint.Y].IsObstacle) NearCellPoints.Add(MapCells[CellPoint.X-1, CellPoint.Y]);
            if (CellPoint.X<TABLE_WIDTH-1 && !MapCells[CellPoint.X+1, CellPoint.Y].IsObstacle) NearCellPoints.Add(MapCells[CellPoint.X+1, CellPoint.Y]);
            if (CellPoint.Y > 0 && !MapCells[CellPoint.X , CellPoint.Y-1].IsObstacle) NearCellPoints.Add(MapCells[CellPoint.X, CellPoint.Y-1]);
            if (CellPoint.Y < TABLE_HEIGHT - 1 && !MapCells[CellPoint.X, CellPoint.Y + 1].IsObstacle) NearCellPoints.Add(MapCells[CellPoint.X, CellPoint.Y+1]);
            return NearCellPoints.ToArray();
        }

        //回溯显示路径
        private void ShowPath(MapCell TargetCell)
        {
            while (TargetCell.ParentMapCell != null)
            {
                TargetCell.DisplayLabel.BackColor = Color.Red;
                TargetCell = TargetCell.ParentMapCell;
            }

            //绘制开始坐标和目标坐标
            MapCells[StartPoint.X, StartPoint.Y].DisplayLabel.BackColor = Color.DeepPink;
            MapCells[StartPoint.X, StartPoint.Y].DisplayLabel.Text = "Start";
            MapCells[TargetPoint.X, TargetPoint.Y].DisplayLabel.BackColor = Color.DeepSkyBlue;
            MapCells[TargetPoint.X, TargetPoint.Y].DisplayLabel.Text = "Target";
        }

    }
}