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
        int[] sending, falling;

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            SeedVirus(15 , 4);
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
            falling = new int[2];
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
                //blocks[0] = Properties.Resources.im0;
                //blocks[1] = Properties.Resources.im1;
                //blocks[2] = Properties.Resources.im2;
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
            rng = new Random();
            virus_count = 15;
            SeedVirus(virus_count, 4);
            for (int i = 0; i < sending.Length; i++)
            {
                sending[i] = rng.Next(type_amount);
            }


        }
        private Rectangle get_rectangle(int i, int j)
        {
            return new Rectangle(1 + i * v_width, pictureBox1.Height - (j + 1) * v_width - 1, v_width, v_width);
        }
        private void SeedVirus(int count, int free_space) //это же по значению, верно? если сломается, то из-за этого
        {
            g_static.DrawImage(new Bitmap(bg, pictureBox1.Size), 0, 0);
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
            Brush[] pp = { Brushes.DarkBlue, Brushes.DarkRed, Brushes.DarkOrange };
            
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount; j++)
                {
                    //Rectangle current_block = new Rectangle(1 + i * v_width, pictureBox1.Height - (j + 1) * v_width - 1, v_width, v_width);
                    if (field[i, j] != -1)
                    {
                        g_static.DrawImage(blocks[field[i, j]], get_rectangle(i, j));
                    }
                    //g_static.DrawRectangle(new Pen(Color.Black), current_block);
                    
                }
            }
            pictureBox1.Invalidate();


        }
        private void SpawnFalling()
        {
            falling[0] = v_x_amount / 2;
            falling[1] = v_x_amount / 2 + 1;
            sending[0] = sending[2];
            sending[1] = sending[3];
            sending[2] = rng.Next(type_amount) + type_amount;
            sending[3] = rng.Next(type_amount) + type_amount;
            pb2.DrawImage(blocks[type_amount + sending[2]], 3, 3);
            pb2.DrawImage(blocks[type_amount + sending[3]], v_width, 3);
            pictureBox2.Invalidate();
            

        }
    }
}
