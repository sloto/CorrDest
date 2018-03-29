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
        Graphics g_static, g_failing;
        Random rng;
        int virus_count, type_amount, v_y_amount, v_x_amount, v_width;

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            g_static.DrawImage(new Bitmap(bg/*Properties.Resources.bg1*/, pictureBox1.Size), 0, 0);
            //g_static.DrawImage(new Bitmap(blocks[0], pictureBox1.Size), 0, 0);
            pictureBox1.Invalidate();
        }

        public Form1()
        {
            InitializeComponent();
            type_amount = 3;
            v_y_amount = 15;
            v_x_amount = 8;
            v_width = (pictureBox1.Width - 2) / v_x_amount;
            pictureBox1.Height = v_y_amount * v_width + 2;
            field = new int[v_x_amount, v_y_amount];
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount; j++)
                {
                    field[i, j] = -1;
                }
            }
            blocks = new Bitmap[type_amount];
            try
            {
                bg = new Bitmap(Properties.Resources.background, pictureBox1.Size);
                blocks[0] = new Bitmap( Properties.Resources.im0, v_width, v_width);
                blocks[1] = new Bitmap( Properties.Resources.im1, v_width, v_width);
                blocks[2] = new Bitmap( Properties.Resources.im2, v_width, v_width);
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
            pictureBox1.BackgroundImage = bg;
            g_static = Graphics.FromImage(pictureBox1.BackgroundImage);
            rng = new Random();
            virus_count = 15;
            SeedVirus(virus_count);


        }
        private void SeedVirus(int count) //это же по значению, верно? если сломается, то из-за этого
        {
            while (count > 0)
            {
                int type = rng.Next(type_amount); //возможно, стоит контролировать, чтобы было примерное одинаковое количество вирусов каждого типа
                int i = rng.Next(v_x_amount);
                int j = rng.Next(v_y_amount - 2);
                if (field[i, j] == -1)
                {
                    count--;
                    field[i, j] = type;
                }
            }
            Brush[] pp = { Brushes.DarkBlue, Brushes.DarkRed, Brushes.DarkOrange };
            g_static.DrawImage(new Bitmap( bg, pictureBox1.Size), 0, 0);
            for (int i = 0; i < v_x_amount; i++)
            {
                for (int j = 0; j < v_y_amount - 2; j++)
                {
                    Rectangle current_block = new Rectangle(1 + i * v_width, pictureBox1.Height - (j + 1) * v_width - 1, v_width, v_width);
                    if (field[i, j] != -1)
                    {
                        g_static.DrawImage(blocks[field[i, j]], current_block);
                    }
                    //g_static.DrawImage(blocks[rng.Next(type_amount)], i * v_width, pictureBox1.Height - j * v_width);

                    //g_static.FillRectangle(pp[rng.Next(3)], current_block);
                    //g_static.DrawImage(blocks[rng.Next(3)], current_block);
                    g_static.DrawRectangle(new Pen(Color.Black), current_block);
                    
                }
            }
            pictureBox1.Invalidate();


        }
    }
}
