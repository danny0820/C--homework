using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hw7
{
    public partial class Form1 : Form
    {
        public class MyPictureBox : PictureBox
        {
            public bool CanClick { get; set; }
            public MyPictureBox()
            {
                CanClick = true; // 默认情况下允许点击
            }
            private Color borderColor = Color.Black;
            public Color BorderColor
            {
                get { return borderColor; }
                set
                {
                    borderColor = value;
                    Invalidate(); // 使控件无效，以便重绘
                }
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                // 绘制边框
                using (Pen pen = new Pen(BorderColor, 8))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, Width, Height);
                }
            }

        }
        private PictureBox lastClickedPictureBox;
        private MyPictureBox[] p = new MyPictureBox[64];
        public int[,] Map = new int[10, 10];// 0 = empty ,1 = black ,2 = white  
        public int MyColor;
        string[] Info = new string[61];
        int[] cs = new int[64];
        private int selectedCount = 0;
        private int cannum = 0;
        public Form1()
        {
            InitializeComponent();
            CreatPictureBox();
            lastClickedPictureBox = p[0];
        }

        private void CreatPictureBox()
        {
            toolStripStatusLabel1.Text = "黑色棋子先走";
            MyColor = 1;
            int k = 0;
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Map[i, j] = 0;
                    p[k] = new MyPictureBox();
                    p[k].Width = 50;
                    p[k].Height = 50;
                    p[k].Location = new Point(50 + i * 50, 50 + j * 50);
                    p[k].BackColor = Color.White;
                    p[k].BorderStyle = BorderStyle.FixedSingle;
                    this.Controls.Add(p[k]);
                    p[k].Click += PictureBox_Click;
                    if ((i == 4 && j == 4) || (i == 5 && j == 5))
                    {
                        Map[i, j] = 2;
                        p[k].BackgroundImage = hw7.Properties.Resources.w; //i是行 j是列 從0開始算
                        p[k].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    if ((i == 4 && j == 5) || (i == 5 && j == 4))
                    {
                        Map[i, j] = 1;
                        p[k].BackgroundImage = hw7.Properties.Resources.b;
                        p[k].BackgroundImageLayout = ImageLayout.Stretch;
                    }
                    /*if (i == 1 && j == 1)
                    {
                        p[k].BackColor = Color.Gray;
                    }*/
                    if ((i == 4 && j == 4) || (i == 4 && j == 5) || (i == 5 && j == 4) || (i == 5 && j == 5))
                    {
                        p[k].CanClick = false;
                    }
                    k++;
                }

            }
            Show_Can_Position();
            p[cs[1]].BackColor = Color.Gray;
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            MyPictureBox clickedPictureBox = (MyPictureBox)sender;
            if (!clickedPictureBox.CanClick)
            {
                return; // 不执行任何操作
            }
            if (lastClickedPictureBox != null)
            {
                lastClickedPictureBox.BackColor = Color.White; // 恢复上一个灰色PictureBox的颜色
            }

            clickedPictureBox.BackColor = Color.Gray; // 将当前点击的PictureBox转为灰色
            lastClickedPictureBox = clickedPictureBox; // 更新lastClickedPictureBox为当前点击的PictureBox
            int clickedIndex = Array.IndexOf(p, clickedPictureBox);
            // 根据索引计算Map数组中的坐标
            int x1 = 1 + clickedIndex / 8; // 行数
            int y1 = 1 + clickedIndex % 8; // 列数
            int k = clickedIndex;
            p[k].BorderColor = Color.Black;
            if (!Can_go(x1, y1))
            {
                toolStripStatusLabel1.Text = "此處不能下棋";
                return;
            }

            if (MyColor == 2)
            {
                Map[x1, y1] = 2;
                p[k].BackgroundImage = hw7.Properties.Resources.w;
                p[k].BackgroundImageLayout = ImageLayout.Stretch;
            }
            if (MyColor == 1)
            {
                Map[x1, y1] = 1;
                p[k].BackgroundImage = hw7.Properties.Resources.b;
                p[k].BackgroundImageLayout = ImageLayout.Stretch;
            }
            //从左，左上，上，右上，右，右下，下，左下八个方向翻转 
            if (CheckDirect(x1, y1, -1, 0) == true) //向左方向形成夹击之势
                DirectReverse(x1, y1, -1, 0);
            if (CheckDirect(x1, y1, -1, -1) == true) //向左上方向形成夹击之势
                DirectReverse(x1, y1, -1, -1);
            if (CheckDirect(x1, y1, 0, -1) == true) //向上方向形成夹击之势
                DirectReverse(x1, y1, 0, -1);
            if (CheckDirect(x1, y1, 1, -1) == true) //向右上方向形成夹击之势
                DirectReverse(x1, y1, 1, -1);

            if (CheckDirect(x1, y1, 1, 0) == true)
                DirectReverse(x1, y1, 1, 0);
            if (CheckDirect(x1, y1, 1, 1) == true)
                DirectReverse(x1, y1, 1, 1);
            if (CheckDirect(x1, y1, 0, 1) == true)
                DirectReverse(x1, y1, 0, 1);
            if (CheckDirect(x1, y1, -1, 1) == true)
                DirectReverse(x1, y1, -1, 1);
            Cls_Can_Position();
            if (MyColor == 1)
            {
                //状态行提示该对方走棋 
                MyColor = 2;
                toolStripStatusLabel1.Text = "白色棋子走";
            }
            else
            {
                MyColor = 1;
                toolStripStatusLabel1.Text = "黑色棋子走";
            }
            Show_Can_Position();
            if (Show_Can_Num() == 0)
            {
                MessageBox.Show("對方無可走位置請繼續", "提示!");
                if (MyColor == 1)
                {
                    MyColor = 2;
                    toolStripStatusLabel1.Text = "白色棋子继续走";
                }
                else
                {
                    MyColor = 1;
                    toolStripStatusLabel1.Text = "黑色棋子继续走";
                }

                Show_Can_Position(); //显示提示 
            }
            int whitenum = 0;
            int blacknum = 0;
            int n = 0;
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    if (Map[x, y] != 0)
                    {
                        n++;
                        if (Map[x, y] == 2)
                            whitenum += 1;
                        if (Map[x, y] == 1)
                            blacknum += 1;
                    }
                }
            }
            if (n == 64) //在棋盘下满时， 
            {
                if (blacknum > whitenum)
                {
                    MessageBox.Show("遊戲結束黑方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                }
                else
                {
                    MessageBox.Show("遊戲結束白方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                }
                // button1.Enabled = true; //"开始游戏"按钮有效 
                return;
            }
            if (whitenum == 0)
            {
                MessageBox.Show("遊戲結束黑方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                //button1.Enabled = true; //"开始游戏"按钮有效 
            }

            if (blacknum == 0)
            {
                MessageBox.Show("遊戲結束白方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                //button1.Enabled = true; //"开始游戏"按钮有效 
            }
        }
        private void Show_Can_Position()
        {
            int i;
            int j;
            int n=0;
            cannum = 0;
            for (i = 1; i <= 8; i++)
            {
                for (j = 1; j <= 8; j++)
                {
                    if (Map[i, j] == 0 & Can_go(i, j))
                    {
                        int k = (i - 1) * 8 + (j - 1);
                        Info[n] = i + "|" + j;
                        n = n + 1;
                        if (MyColor == 1)
                        {
                         //p[k].BackColor = Color.Blue;
                         p[k].BorderColor = Color.Blue;
                         cs[cannum] = k;
                         cannum++;
                        }
                        else
                        //p[k].BackColor = Color.Blue;
                        p[k].BorderColor = Color.Blue;
                        cs[cannum] = k;
                        cannum++;
                    }
                }
            }
        }
        private int Show_Can_Num()
        {
            int i, j;
            int n = 0;
            for (i = 1; i <= 8; i++)
            {
                for (j = 1; j <= 8; j++)
                {
                    if (Can_go(i, j))
                    {
                        Info[n] = i + "|" + j;
                        n++;
                    }
                }
            }

            return n;
            //可以落子的位置个数 
        }

        private void Cls_Can_Position()
        {
            //關閉顯示
            int n;
            string a;
            string b;
            int x;
            int y;
            string s;
            //背景图片 
            for (n = 0; n <= 60; n++)
            {
                s = Info[n];
                if (string.IsNullOrEmpty(s)) break;

                a = s.Substring(0, 1);
                //b = s.Substring(Strings.InStr(s, "|"), 1);
                b = s.Substring(s.IndexOf('|', 1) + 1);
                x = Convert.ToInt16(a);
                y = Convert.ToInt16(b);
                if (Map[x, y] == 0)
                {
                    int k = (x - 1) * 8 + (y - 1);
                    // p[k].BackColor = Color.White;
                    p[k].BorderColor = Color.Black;

                }

                //Me.Text = CInt(x) & y 
            }
        }
        private bool InBoard(int x, int y)
        {
            if (x >= 1 & x < 9 & y >= 1 & y < 9)
                return true;
            else
                return false;
        }
        private bool CheckDirect(int x1, int y1, int dx, int dy)
        {
            int x, y;
            bool flag;
            x = x1 + dx;
            y = y1 + dy;
            flag = false;

            while (InBoard(x, y) & !Ismychess(x, y) & Map[x, y] != 0)
            {
                x += dx;
                y += dy;
                flag = true;
            }

            if (InBoard(x, y) & Ismychess(x, y) & flag == true)
            {
                return true;
            }
            return false;
        }
        private void DirectReverse(int x1, int y1, int dx, int dy)
        {
            int x, y;
            bool flag;
            x = x1 + dx;
            y = y1 + dy;
            flag = false;
            while (InBoard(x, y) & !Ismychess(x, y) & Map[x, y] != 0)
            {
                x += dx;
                y += dy;
                flag = true; //构成夹击之势 
            }

            if (InBoard(x, y) & Ismychess(x, y) & flag == true)
            {
                do
                {
                    x -= dx;
                    y -= dy;
                    if ((x != x1 || y != y1)) FanQi(x, y);
                } while ((x != x1 || y != y1));
            }
        }
        private bool Ismychess(int x, int y)
        {
            if (Map[x, y] == MyColor)
                return true;
            else
                return false;
        }
        private bool Can_go(int x1, int y1)
        {
            if (CheckDirect(x1, y1, -1, 0) == true)
                return true;
            if (CheckDirect(x1, y1, -1, -1) == true)
                return true;
            if (CheckDirect(x1, y1, 0, -1) == true)
                return true;
            if (CheckDirect(x1, y1, 1, -1) == true)
                return true;
            if (CheckDirect(x1, y1, 1, 0) == true)
                return true;
            if (CheckDirect(x1, y1, 1, 1) == true)
                return true;
            if (CheckDirect(x1, y1, 0, 1) == true)
                return true;
            if (CheckDirect(x1, y1, -1, 1) == true)
                return true;
            return false;
        }
        private void FanQi(int x, int y)
        {
            int k = (x - 1) * 8 + (y - 1);
            if (Map[x, y] == 1)
            {
                Map[x, y] = 2;
                p[k].BackgroundImage = hw7.Properties.Resources.w;
                p[k].BackgroundImageLayout = ImageLayout.Stretch;
            }
            else
            {
                Map[x, y] = 1;
                p[k].BackgroundImage = hw7.Properties.Resources.b;
                p[k].BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int x1, y1;
            int i = cannum;
            if (e.KeyCode == Keys.Left)
            {
                selectedCount--;
                if (selectedCount < 0)
                {
                    selectedCount = i - 1; // 循环到最右边
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                selectedCount++;
                if (selectedCount >= i)
                {
                    selectedCount = 0; // 循环到最左边
                }
            }
            else if (e.KeyCode == Keys.Enter) // 当用户按下Enter键时
            {
                x1 =1+ cs[selectedCount] / 8 ;
                y1 =1+ cs[selectedCount] % 8 ;
                p[cs[selectedCount]].BorderColor = Color.Black;
                if (!Can_go(x1, y1))
                {
                    toolStripStatusLabel1.Text = "此處不能下棋";
                    return;
                }
                if (MyColor == 2)
                {
                    Map[x1, y1] = 2;
                    p[cs[selectedCount]].BackgroundImage = hw7.Properties.Resources.w;
                    p[cs[selectedCount]].BackgroundImageLayout = ImageLayout.Stretch;
                }
               if (MyColor == 1)
                {
                    Map[x1, y1] = 1;
                    p[cs[selectedCount]].BackgroundImage = hw7.Properties.Resources.b;
                    p[cs[selectedCount]].BackgroundImageLayout = ImageLayout.Stretch;
                }
                //从左，左上，上，右上，右，右下，下，左下八个方向翻转 
                if (CheckDirect(x1, y1, -1, 0) == true) //向左方向形成夹击之势
                    DirectReverse(x1, y1, -1, 0);
                if (CheckDirect(x1, y1, -1, -1) == true) //向左上方向形成夹击之势
                    DirectReverse(x1, y1, -1, -1);
                if (CheckDirect(x1, y1, 0, -1) == true) //向上方向形成夹击之势
                    DirectReverse(x1, y1, 0, -1);
                if (CheckDirect(x1, y1, 1, -1) == true) //向右上方向形成夹击之势
                    DirectReverse(x1, y1, 1, -1);

                if (CheckDirect(x1, y1, 1, 0) == true)
                    DirectReverse(x1, y1, 1, 0);
                if (CheckDirect(x1, y1, 1, 1) == true)
                    DirectReverse(x1, y1, 1, 1);
                if (CheckDirect(x1, y1, 0, 1) == true)
                    DirectReverse(x1, y1, 0, 1);
                if (CheckDirect(x1, y1, -1, 1) == true)
                    DirectReverse(x1, y1, -1, 1);
                Cls_Can_Position();
                if (MyColor == 1)
                {
                    //状态行提示该对方走棋 
                    MyColor = 2;
                    toolStripStatusLabel1.Text = "白色棋子走";
                }
                else
                {
                    MyColor = 1;
                    toolStripStatusLabel1.Text = "黑色棋子走";
                }
                Show_Can_Position();
                if (Show_Can_Num() == 0)
                {
                    MessageBox.Show("對方無可走位置請繼續", "提示!");
                    if (MyColor == 1)
                    {
                        MyColor = 2;
                        toolStripStatusLabel1.Text = "白色棋子继续走";
                    }
                    else
                    {
                        MyColor = 1;
                        toolStripStatusLabel1.Text = "黑色棋子继续走";
                    }

                    Show_Can_Position(); //显示提示 
                }
                int whitenum = 0;
                int blacknum = 0;
                int n = 0;
                for (int x = 1; x <= 8; x++)
                {
                    for (int y = 1; y <= 8; y++)
                    {
                        if (Map[x, y] != 0)
                        {
                            n++;
                            if (Map[x, y] == 2)
                                whitenum += 1;
                            if (Map[x, y] == 1)
                                blacknum += 1;
                        }
                    }
                }
                label1.Text = "黑棋:" + blacknum;
                label2.Text = "白棋:" + whitenum;
                if (n == 64) //在棋盘下满时， 
                {
                    if (blacknum > whitenum)
                    {
                        MessageBox.Show("遊戲結束黑方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                    }
                    else
                    {
                        MessageBox.Show("遊戲結束白方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                    }
                    // button1.Enabled = true; //"开始游戏"按钮有效 
                    return;
                }
                if (whitenum == 0)
                {
                    MessageBox.Show("遊戲結束黑方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                    //button1.Enabled = true; //"开始游戏"按钮有效 
                }

                if (blacknum == 0)
                {
                    MessageBox.Show("遊戲結束白方勝利", "黑方:" + blacknum + "白方:" + whitenum);
                    //button1.Enabled = true; //"开始游戏"按钮有效 
                }

            }

            // 清除所有背景色
            foreach (MyPictureBox pictureBox in p)
            {
                pictureBox.BackColor = Color.White;
            }

            // 设置选定位置的背景色
            p[cs[selectedCount]].BackColor = Color.Gray;
        }
    }
}
