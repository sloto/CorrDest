using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorrDest
{
    public partial class Form1 : Form
    {
        delegate Tuple<int, int> ChangeIndex(Tuple<int,int> xy);
        ChangeIndex[] GoToNeighbor = new ChangeIndex[4];
        List<Tuple<int, int>> toDestroy;
        List<int>[] positionsToFall; 
        Tuple<int,int>[,] field;
        Bitmap[] positions;
        Brush[] types;
        Bitmap bg;
        //Graphics g_static, g_failing, pb2;
        Random rng;
        int virus_count, type_amount, v_y_amount, v_x_amount, v_width, period;
        int[] falling_x, falling_y;
        Tuple<int,int>[] sending;
        bool left, right, up, down, moved_r, moved_l;

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                if (!moved_r && move_timer.Enabled)
                {
                    bool move_right = right;
                    for (int i = 0; i < falling_x.Length; i++)
                    {
                        move_right &= sending[i].Item2 == 2 || ((falling_x[i] < v_x_amount - 1) && (field[falling_x[i] + 1, falling_y[i]].Item1 == -1));
                    }
                    if (move_right)
                    {
                        for (int i = 0; i < falling_x.Length; i++)
                            field[falling_x[i], falling_y[i]] = new Tuple<int, int>(-1, -1);

                        for (int i = 0; i < falling_x.Length; i++)
                        {
                            falling_x[i]++;
                            field[falling_x[i], falling_y[i]] = sending[i];
                        }
                        pictureBox1.Invalidate();
                        
                    }

                }
                right = false;
            }
            if (e.KeyCode == Keys.Left)
            {
                if (!moved_l && move_timer.Enabled)
                {
                    bool move_left = left;
                    for (int i = 0; i < falling_x.Length; i++)
                    {
                        move_left &= sending[i].Item2 == 0 || ((falling_x[i] > 0) && (field[falling_x[i] - 1, falling_y[i]].Item1 == -1));
                    }
                    if (move_left)
                    {
                        for (int i = 0; i < falling_x.Length; i++)
                            field[falling_x[i], falling_y[i]] = new Tuple<int, int>(-1, -1);

                        for (int i = 0; i < falling_x.Length; i++)
                        {
                            falling_x[i]--;
                            field[falling_x[i], falling_y[i]] = sending[i];
                        }
                        pictureBox1.Invalidate();
                    }
                }
                left = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                if (up && move_timer.Enabled)
                {
                    Rotate();
                }
                up = false;
            }
            moved_r = false;
            moved_l = false;
            //move_timer.Enabled = left || right || up || down;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            right = e.KeyCode == Keys.Right;
            left = e.KeyCode == Keys.Left;
            up = e.KeyCode == Keys.Up;
            //move_timer.Enabled = left || right || up || down;
            if (e.KeyCode == Keys.Space)
            {
                timer1.Enabled = !timer1.Enabled;
            }

        }

        private void move_timer_Tick(object sender, EventArgs e)
        {
            bool move_right = right;
            bool move_left = left;
            for (int i = 0; i < falling_x.Length; i++)
            {
                move_right &= sending[i].Item2 == 2 || ((falling_x[i] < v_x_amount - 1) && (field[falling_x[i] + 1, falling_y[i]].Item1 == -1));
            }
            for (int i = 0; i < falling_x.Length; i++)
            {
                move_left &= sending[i].Item2 == 0 || ((falling_x[i] > 0) && (field[falling_x[i] - 1, falling_y[i]].Item1 == -1));
            }
            if (move_right)
            {
                for (int i = 0; i < falling_x.Length; i++)
                    field[falling_x[i], falling_y[i]] = new Tuple<int, int>(-1, -1);

                for (int i = 0; i < falling_x.Length; i++)
                {
                    falling_x[i]++;
                    field[falling_x[i], falling_y[i]] = sending[i];
                }
                pictureBox1.Invalidate();
                moved_r = true;
            }
            if (move_left)
            {
                for (int i = 0; i < falling_x.Length; i++)
                    field[falling_x[i], falling_y[i]] = new Tuple<int, int>(-1, -1);

                for (int i = 0; i < falling_x.Length; i++)
                {
                    falling_x[i]--;
                    field[falling_x[i], falling_y[i]] = sending[i];
                }
                pictureBox1.Invalidate();
                moved_l = true;
            }
            
        }
        
        private void timer2_Tick(object sender, EventArgs e)
        {
            bool isFallingOver = true;
            for (int i = 0; i < v_y_amount - 1; i++)
            {
                foreach(int t in positionsToFall[i])
                {
                    field[t, i] = field[t, i + 1];
                    field[t, i + 1] = new Tuple<int, int>(-1, -1);
                    
                }
                foreach (int t in positionsToFall[i])
                {
                    if (i + 2 < v_y_amount && field[t,i+2].Item1 != -1)
                    {
                        if(field[t, i + 2].Item2 == 0 && field[t - 1, i + 1].Item1 == -1)
                        {
                            positionsToFall[i + 1].Add(t);
                        }
                        else if(field[t, i + 2].Item2 == 2 && field[t + 1, i + 1].Item1 == -1)
                        {
                            positionsToFall[i + 1].Add(t);
                        }
                        else if(field[t,i+2].Item2 != 5)
                        {
                            positionsToFall[i + 1].Add(t);
                        }
                    }
                    if (i > 0 && field[t,i-1].Item1 == -1)
                    {
                        if (field[t, i].Item2 == 0 && field[t - 1, i - 1].Item1 == -1)
                        {
                            positionsToFall[i - 1].Add(t);
                            isFallingOver = false;
                        }
                        else if (field[t, i].Item2 == 2 && field[t + 1, i - 1].Item1 == -1)
                        {
                            positionsToFall[i - 1].Add(t);
                            isFallingOver = false;
                        }
                        else if (field[t, i].Item2 != 5)
                        {
                            positionsToFall[i - 1].Add(t);
                            isFallingOver = false;
                        }
                    }
                }
                positionsToFall[i] = new List<int>();
            }
            pictureBox1.Invalidate();
            if (isFallingOver)
            {
                timer2.Enabled = false;
                timer1.Enabled = true;
                move_timer.Enabled = true;
                SpawnFalling();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (isFallingPossible())
            {
                FallDown();
            }
            else
            {
                timer1.Enabled = false;
                move_timer.Enabled = false;
                //for (int i = 0; i < falling_x.Length; i++)
                //{
                //    field[falling_x[i], falling_y[i]] = sending[i];
                //}
                toDestroy = new List<Tuple<int, int>>();
                SelectDestructionField(falling_x[0], falling_y[0], ref toDestroy);
                SelectDestructionField(falling_x[1], falling_y[1], ref toDestroy);
                //toDestroy.Sort((Tuple<int, int> x, Tuple<int, int> y) => { return x.Item2 - y.Item2; });
                for (int i = 0; i < v_y_amount; i++)
                {
                    positionsToFall[i] = new List<int>();
                }
                foreach (Tuple<int, int> t in toDestroy)
                {
                    if (field[t.Item1, t.Item2].Item2 < 4 && field[t.Item1, t.Item2].Item2 >= 0)
                    {
                        Tuple<int, int> neighbor = GoToNeighbor[field[t.Item1, t.Item2].Item2](t);
                        if(neighbor.Item2 > 0 && field[neighbor.Item1, neighbor.Item2 - 1].Item1 == -1)
                        {
                            positionsToFall[neighbor.Item2 - 1].Add(neighbor.Item1);
                        }
                        field[neighbor.Item1, neighbor.Item2] = new Tuple<int, int>(field[neighbor.Item1, neighbor.Item2].Item1, 4);
                    }
                    field[t.Item1, t.Item2] = new Tuple<int, int>(-1, -1);
                    
                }
                foreach (Tuple<int, int> t in toDestroy)
                {
                    if (t.Item2 < v_y_amount - 1 && field[t.Item1, t.Item2 + 1].Item1 != -1)    
                    {
                        if(field[t.Item1, t.Item2 + 1].Item2 == 0 && field[t.Item1 - 1, t.Item2].Item1 == -1)
                        {
                            positionsToFall[t.Item2].Add(t.Item1);
                            positionsToFall[t.Item2].Add(t.Item1 - 1);
                        }
                        else if (field[t.Item1, t.Item2 + 1].Item2 == 2 && field[t.Item1 + 1, t.Item2].Item1 == -1)
                        {
                            positionsToFall[t.Item2].Add(t.Item1);
                            positionsToFall[t.Item2].Add(t.Item1 + 1);
                        }
                        else if (field[t.Item1, t.Item2 + 1].Item2 != 5)
                        {
                            positionsToFall[t.Item2].Add(t.Item1);
                        }
                    }
                }

                pictureBox1.Invalidate();
                timer2.Enabled = true;
                
                
                //move_timer.Interval = 35;
            }
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {          
            e.Graphics.DrawImage(new Bitmap(bg, pictureBox2.Size), 0, 0);
            e.Graphics.FillRectangle(types[sending[2].Item1], new Rectangle(3, 3, v_width, v_width));
            e.Graphics.DrawImage(positions[sending[2].Item2], new Rectangle(3, 3, v_width, v_width));
            e.Graphics.FillRectangle(types[sending[3].Item1], new Rectangle(3 + v_width, 3, v_width, v_width));
            e.Graphics.DrawImage(positions[sending[3].Item2], new Rectangle(3 + v_width, 3, v_width, v_width));
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            FieldInvalidate(e.Graphics);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //SeedVirus(15 , 4);
            //SpawnFalling();
            
        }

        public Form1()
        {
            InitializeComponent();
            type_amount = 3;
            v_y_amount = 15;
            v_x_amount = 8;
            period = 80;
            v_width = (pictureBox1.Width - 2) / v_x_amount;
            pictureBox1.Height = v_y_amount * v_width + 2;
            field = new Tuple<int, int>[v_x_amount, v_y_amount];
            positions = new Bitmap[6];
            types = new Brush[]{ Brushes.DarkRed, Brushes.DarkSeaGreen, Brushes.Gold};
            falling_x = new int[2];
            falling_y = new int[2];
            sending = new Tuple<int, int>[4];
            List<Tuple<int, int>> burning_blocks = new List<Tuple<int, int>>();
            positionsToFall = new List<int>[v_y_amount];
            try
            {
                bg = Properties.Resources.background;
                positions[0] = new Bitmap(Properties.Resources.msk0, v_width, v_width);
                positions[1] = new Bitmap(Properties.Resources.msk1, v_width, v_width);
                positions[2] = new Bitmap(Properties.Resources.msk2, v_width, v_width);
                positions[3] = new Bitmap(Properties.Resources.msk3, v_width, v_width);
                positions[4] = new Bitmap(Properties.Resources.msk4, v_width, v_width);
                positions[5] = new Bitmap(Properties.Resources.msk5, v_width, v_width);
            }
            catch (Exception except)
            {
                MessageBox.Show(except.ToString(), "Flyght", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            pictureBox1.BackgroundImage = new Bitmap(bg, pictureBox1.Size);
            pictureBox2.BackgroundImage = new Bitmap(bg, pictureBox2.Size);
            rng = new Random();
            virus_count = 15;
            SeedVirus(virus_count, 6);
            for (int i = 0; i < sending.Length; i++)
            {
                sending[i] = new Tuple<int, int>(rng.Next(type_amount), ((i + 1) % 2) * 2);
            }
            GoToNeighbor[0] = (Tuple<int, int> xy) => { return new Tuple<int, int>(xy.Item1 - 1, xy.Item2); };
            GoToNeighbor[1] = (Tuple<int, int> xy) => { return new Tuple<int, int>(xy.Item1, xy.Item2 + 1); };
            GoToNeighbor[2] = (Tuple<int, int> xy) => { return new Tuple<int, int>(xy.Item1 + 1, xy.Item2); };
            GoToNeighbor[3] = (Tuple<int, int> xy) => { return new Tuple<int, int>(xy.Item1, xy.Item2 - 1); };
            SpawnFalling();
            move_timer.Interval = 63;
            timer1.Enabled = true;
            timer1.Interval = move_timer.Interval * v_x_amount;
            move_timer.Enabled = true;
            
            timer2.Enabled = false;
            timer2.Interval = timer1.Interval;
            right = left = up = down = false;


        }
        private Rectangle get_rectangle(int i, int j)
        {
            return new Rectangle(1 + i * v_width, pictureBox1.Height - (j + 1) * v_width - 1, v_width, v_width);
        }
        private void DrawBlock(Graphics g ,Tuple<int, int> block, int x, int y)
        {
            g.FillRectangle(types[block.Item1], get_rectangle(x, y));
            g.DrawImage(positions[block.Item2], get_rectangle(x, y));
        }
        private void SelectDestructionField(int i, int j, ref List<Tuple<int, int>> burned)
        {
            List<Tuple<int, int>> horizontal = new List<Tuple<int, int>>(), vertical = new List<Tuple<int, int>>();
            for (int k = i + 1; k < v_x_amount && field[k, j].Item1 == field[i, j].Item1; k++)
            {
                Tuple<int, int> t = new Tuple<int, int>(k,j);
                if (!horizontal.Contains(t))
                {
                    horizontal.Add(t);
                }
            }
            for (int k = i - 1; k >= 0 && field[k, j].Item1 == field[i, j].Item1; k--)
            {
                Tuple<int, int> t = new Tuple<int, int>(k, j);
                if (!horizontal.Contains(t))
                {
                    horizontal.Add(t);
                }
            }


            for (int m = j + 1; m < v_y_amount && field[i, m].Item1 == field[i, j].Item1; m++)
            {
                Tuple<int, int> t = new Tuple<int, int>(i, m);
                if (!vertical.Contains(t))
                {
                    vertical.Add(t);
                }
            }
            for (int m = j - 1; m >= 0 && field[i, m].Item1 == field[i, j].Item1; m--)
            {
                Tuple<int, int> t = new Tuple<int, int>(i, m);
                if (!vertical.Contains(t))
                {
                    vertical.Add(t);
                }
            }
            if (horizontal.Count >= 2 || vertical.Count >= 2)
            {
                if (!burned.Contains(new Tuple<int, int>(i, j)))
                    burned.Add(new Tuple<int, int>(i, j));
            }
            if (horizontal.Count >= 2)
            {
                foreach (Tuple<int,int> t in horizontal)
                {
                    if (!burned.Contains(t))
                        burned.Add(t);
                }
                
            }
            if (vertical.Count >= 2)
            {
                foreach (Tuple<int, int> t in vertical)
                {
                    if (!burned.Contains(t))
                        burned.Add(t);
                }

            }

        }

       
        private void FieldInvalidate(Graphics g)
        {
            g.DrawImage(new Bitmap(bg, pictureBox1.Size), 0, 0);
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount; j++)
                {
                    if (field[i, j].Item1 != -1)
                    {
                        g.FillRectangle(types[field[i, j].Item1], get_rectangle(i, j));//заменить функцией
                        g.DrawImage(positions[field[i, j].Item2], get_rectangle(i, j));
                    }

                }
            }
            //pictureBox1.Invalidate();
        }

        private void SeedVirus(int count, int free_space) 
        {
            
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount; j++)
                {
                    field[i, j] = new Tuple<int, int>(-1,-1);
                }
            }
            while (count > 0)
            {
                int type = rng.Next(type_amount); //возможно, стоит контролировать, чтобы было примерное одинаковое количество вирусов каждого типа
                int i = rng.Next(v_x_amount);
                int j = rng.Next(v_y_amount - free_space);
                if (field[i, j].Item1 == -1)
                {
                    count--;
                    field[i, j] = new Tuple<int, int>(type, 5);
                }
            }
            pictureBox1.Invalidate();

        }
        private bool isFallingPossible()
        {
            bool falling_is_possible = true;
            for (int i = 0; i < falling_y.Length; i++)
            {
                falling_is_possible &= sending[i].Item2 == 3 || ( (falling_y[i] - 1 >= 0) && (field[falling_x[i], falling_y[i] - 1].Item1 == -1));
            }
            return falling_is_possible;
        }
        private void FallDown()
        {
            //if (!isFallingPossible())
            //{
            //    return;
            //}
            for (int i = 0; i < falling_y.Length; i++)
                field[falling_x[i], falling_y[i]] = new Tuple<int, int>(-1, -1);

            for (int i = 0; i < falling_y.Length; i++) 
            {
                
                falling_y[i]--;
                field[falling_x[i], falling_y[i]] = sending[i];
            }
            pictureBox1.Invalidate();
        }

        private void Rotate()
        {
            if (sending[1].Item2 == 0 && falling_y[1] + 1 < v_y_amount && field[falling_x[1], falling_y[1] + 1].Item1 == -1)
            {
                field[falling_x[0], falling_y[0]] = new Tuple<int, int>(-1, -1);
                field[falling_x[1], falling_y[1]] = new Tuple<int, int>(-1, -1);
                falling_x[0] = falling_x[1];
                falling_y[0] = falling_y[1] + 1;

            }
            else if(sending[1].Item2 == 1 && falling_x[1] + 1 < v_x_amount && field[falling_x[1] + 1, falling_y[1]].Item1 == -1)
            {
                field[falling_x[0], falling_y[0]] = new Tuple<int, int>(-1, -1);
                field[falling_x[1], falling_y[1]] = new Tuple<int, int>(-1, -1);
                falling_x[0] = falling_x[1] + 1;
                falling_y[0] = falling_y[1];
            }
            else if (sending[1].Item2 == 2 && falling_y[1] -1 >= 0 && field[falling_x[1], falling_y[1] - 1].Item1 == -1)
            {
                field[falling_x[0], falling_y[0]] = new Tuple<int, int>(-1, -1);
                field[falling_x[1], falling_y[1]] = new Tuple<int, int>(-1, -1);
                falling_x[0] = falling_x[1];
                falling_y[0] = falling_y[1] - 1;

            }
            else if (sending[1].Item2 == 3 && falling_x[1] - 1 >= 0 && field[falling_x[1] - 1, falling_y[1]].Item1 == -1)
            {
                field[falling_x[0], falling_y[0]] = new Tuple<int, int>(-1, -1);
                field[falling_x[1], falling_y[1]] = new Tuple<int, int>(-1, -1);
                falling_x[0] = falling_x[1] - 1;
                falling_y[0] = falling_y[1];
            }
            else
            {
                return;
            }
            sending[0] = new Tuple<int, int>(sending[0].Item1, (sending[0].Item2 + 1) % 4);
            sending[1] = new Tuple<int, int>(sending[1].Item1, (sending[1].Item2 + 1) % 4);
            field[falling_x[0], falling_y[0]] = sending[0];
            field[falling_x[1], falling_y[1]] = sending[1];

            pictureBox1.Invalidate();
        }

        private void SpawnFalling()
        {
            falling_x[0] = v_x_amount / 2 - 1;
            falling_x[1] = v_x_amount / 2;
            falling_y[0] = v_y_amount - 1;
            falling_y[1] = v_y_amount - 1;
            sending[0] = sending[2];
            sending[1] = sending[3];
            sending[2] = new Tuple<int, int>( rng.Next(type_amount),2);
            sending[3] = new Tuple<int, int>(rng.Next(type_amount), 0);
            //pb2.DrawImage(new Bitmap(bg, pictureBox2.Size), 0, 0);
            //pb2.FillRectangle(types[sending[2].Item1], new Rectangle(3, 3, v_width, v_width));
            //pb2.DrawImage(positions[sending[2].Item2], new Rectangle(3, 3, v_width, v_width));
            //pb2.FillRectangle(types[sending[3].Item1], new Rectangle(3 + v_width, 3, v_width, v_width));
            //pb2.DrawImage(positions[sending[3].Item2], new Rectangle(3 + v_width, 3, v_width, v_width));
            
            pictureBox2.Invalidate();
            for (int i = 0; i < 2; i++)
            {
                //g_failing.FillRectangle(types[sending[i].Item1], get_rectangle(falling_x[i], falling_y[i]));
                //g_failing.DrawImage(positions[sending[i].Item2], get_rectangle(falling_x[i], falling_y[i]));
                field[falling_x[i], falling_y[i]] = sending[i];
                
            }
            pictureBox1.Invalidate();
        }
    }
}
