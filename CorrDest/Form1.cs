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
        int[,] field;
        Bitmap[] blocks;
        Bitmap bg;
        Graphics g_static, g_failing, pb2;
        Random rng;
        int virus_count, type_amount, v_y_amount, v_x_amount, v_width, period;
        int[] sending, falling_x, falling_y;
        bool left, right, up, down;

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right) { right = false; }
            if (e.KeyCode == Keys.Left) { left = false; }
            move_timer.Enabled = left || right || up || down;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            right = e.KeyCode == Keys.Right;
            left = e.KeyCode == Keys.Left;
            move_timer.Enabled = left || right || up || down;

        }

        private void move_timer_Tick(object sender, EventArgs e)
        {
            bool move_right = right;
            bool move_left = left;
            for (int i = 0; i < falling_x.Length; i++)
            {
                move_right &= (falling_x[i] < v_x_amount - 1) && (field[falling_x[i] + 1, falling_y[i]] == -1);
            }
            for (int i = 0; i < falling_x.Length; i++)
            {
                move_left &= (falling_x[i] > 0) && (field[falling_x[i] - 1, falling_y[i]] == -1);
            }
            if (move_right)
            {
                for (int i = 0; i < falling_x.Length; i++)
                    g_failing.DrawImage(bg, get_rectangle(falling_x[i], falling_y[i]));

                for (int i = 0; i < falling_x.Length; i++)
                {
                    falling_x[i]++;
                    g_failing.DrawImage(blocks[sending[i]], get_rectangle(falling_x[i], falling_y[i])); 
                }
                pictureBox1.Invalidate();
            }
            if (move_left)
            {
                for (int i = 0; i < falling_x.Length; i++)
                    g_failing.DrawImage(bg, get_rectangle(falling_x[i], falling_y[i]));

                for (int i = 0; i < falling_x.Length; i++)
                {
                    falling_x[i]--;
                    g_failing.DrawImage(blocks[sending[i]], get_rectangle(falling_x[i], falling_y[i]));
                }
                pictureBox1.Invalidate();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            SpawnFalling();
            timer2.Enabled = false;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (isFallingPossible())
            {
                FallDown();
            }
            else
            {
                for (int i = 0; i < falling_x.Length; i++)
                {
                    field[falling_x[i], falling_y[i]] = sending[i];
                }
                FieldInvalidate();
                timer2.Enabled = true;
                timer1.Enabled = false;
                move_timer.Enabled = false;
                move_timer.Interval = 35;
            }
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
            field = new int[v_x_amount, v_y_amount];
            blocks = new Bitmap[2*type_amount];
            falling_x = new int[2];
            falling_y = new int[2];
            sending = new int[4];
            try
            {
                bg = Properties.Resources.background;
                blocks[0] = new Bitmap( Properties.Resources.im0, v_width, v_width);
                blocks[1] = new Bitmap( Properties.Resources.im1, v_width, v_width);
                blocks[2] = new Bitmap( Properties.Resources.im2, v_width, v_width);
                blocks[3] = new Bitmap(Properties.Resources.im3, v_width, v_width);
                blocks[4] = new Bitmap(Properties.Resources.im4, v_width, v_width);
                blocks[5] = new Bitmap(Properties.Resources.im5, v_width, v_width);
            }
            catch (Exception except)
            {
                MessageBox.Show(except.ToString(), "Flyght", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].MakeTransparent();
            }
            pictureBox1.BackgroundImage = new Bitmap(bg, pictureBox1.Size);
            pictureBox2.BackgroundImage = new Bitmap(bg, pictureBox2.Size);
            pb2 = Graphics.FromImage(pictureBox2.BackgroundImage);
            g_static = Graphics.FromImage(pictureBox1.BackgroundImage);
            g_failing = Graphics.FromImage(pictureBox1.BackgroundImage); 
            rng = new Random();
            virus_count = 15;
            SeedVirus(virus_count, 6);
            for (int i = 0; i < sending.Length; i++)
            {
                sending[i] = rng.Next(type_amount) + type_amount;
            }
            SpawnFalling();
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer2.Enabled = false;
            timer2.Interval = timer1.Interval;
            right = left = up = down = false;


        }
        private Rectangle get_rectangle(int i, int j)
        {
            return new Rectangle(1 + i * v_width, pictureBox1.Height - (j + 1) * v_width - 1, v_width, v_width);
        }

        private void FieldInvalidate()
        {
            g_static.DrawImage(new Bitmap(bg, pictureBox1.Size), 0, 0);
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount; j++)
                {
                    if (field[i, j] != -1)
                    {
                        g_static.DrawImage(blocks[field[i, j]], get_rectangle(i, j));
                    }

                }
            }
            pictureBox1.Invalidate();
        }

        private void SeedVirus(int count, int free_space) //это же по значению, верно? если сломается, то из-за этого
        {
            
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount; j++)
                {
                    field[i, j] = -1;
                }
            }
            while (count > 0)
            {
                int type = rng.Next(type_amount); //возможно, стоит контролировать, чтобы было примерное одинаковое количество вирусов каждого типа
                int i = rng.Next(v_x_amount);
                int j = rng.Next(v_y_amount - free_space);
                if (field[i, j] == -1)
                {
                    count--;
                    field[i, j] = type;
                }
            }
            FieldInvalidate();

        }
        private bool isFallingPossible()
        {
            bool falling_is_possible = true;
            for (int i = 0; i < falling_y.Length; i++)
            {
                falling_is_possible &= (falling_y[i] - 1 >= 0) && (field[falling_x[i], falling_y[i] - 1] == -1);
            }
            return falling_is_possible;
        }
        private void FallDown()
        {
            //if (!isFallingPossible())
            //{
            //    return;
            //}
            for (int i = 0; i < falling_y.Length; i++) //перерисовывать несколько маленьких кусочков или сразу все поле?Ы
            {
                g_failing.DrawImage(bg, get_rectangle(falling_x[i], falling_y[i]));
                falling_y[i]--;
                g_failing.DrawImage(blocks[sending[i]], get_rectangle(falling_x[i], falling_y[i]));
            }
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
            sending[2] = rng.Next(type_amount) + type_amount;
            sending[3] = rng.Next(type_amount) + type_amount;
            pb2.DrawImage(new Bitmap(bg, pictureBox2.Size), 0, 0);
            pb2.DrawImage(blocks[sending[2]], 3, 3);
            pb2.DrawImage(blocks[sending[3]], v_width + 3, 3);
            pictureBox2.Invalidate();
            for (int i = 0; i < 2; i++)
            {
                g_failing.DrawImage(blocks[sending[i]], get_rectangle(falling_x[i], falling_y[i]));
                pictureBox1.Invalidate(get_rectangle(falling_x[i], falling_y[i]));
            }
        }
    }
}
