using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace PNGSteg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bmp;
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        bool imgLoaded = false;
        public void LoadImg()
        {
            openFileDialog1.FileName = "turkishEmbassy.png";
            openFileDialog1.Filter = "Image files (*.png)|*.png";
            openFileDialog1.InitialDirectory = desktopPath;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                pictureBox1.ImageLocation = filePath;
                CreateBmpFromPng(filePath);
            }
            
        }

        public void CreateBmpFromPng(string filePath)
        {
            bmp = new Bitmap(filePath);
            imgLoaded = true;
        }

        public void ClearLSB()
        {
            Color color;
            int r,g,b;

            for(int i = 0; i < bmp.Height; i++)
            {
                for(int j = 0; j < bmp.Width; j++)
                {
                    color = bmp.GetPixel(j, i);
                    r = color.R - color.R % 2;
                    g = color.G - color.G % 2;
                    b = color.B - color.B % 2;
                    bmp.SetPixel(j,i,Color.FromArgb(r,g,b));
                }
            }
        }

        public void HideTextInImage(string text)
        {
            Color color;
            int r, g, b;
            int bitPointer = 0;
            int bytePointer = 0;
            string charBits = GetCharBits(text[bytePointer]);

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    color = bmp.GetPixel(j, i);
                    r = color.R;
                    g = color.G;
                    b = color.B;

                    for (int k = 0; k < 3; k++)
                    {
                        if(bitPointer > charBits.Length-1)
                        {
                            bytePointer++;
                            if(bytePointer > text.Length-1)
                            {
                                return;
                            }
                            bitPointer = 0;
                            charBits = GetCharBits(text[bytePointer]);
                        }
                        switch (k)
                        {
                            case 0:
                                r = color.R + Convert.ToInt32(charBits[bitPointer].ToString());
                                break;
                            case 1:
                                g = color.G + Convert.ToInt32(charBits[bitPointer].ToString());
                                break;
                            case 2:
                                b = color.B + Convert.ToInt32(charBits[bitPointer].ToString());
                                break;
                        }
                        bmp.SetPixel(j, i, Color.FromArgb(r, g, b));
                        bitPointer++;
                    }

                }
            }
        }

        public void GetTextFromImage()
        {
            Color color;
            string text = "";
            string charBits = "";
            int bitCounter = 0;

            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    color = bmp.GetPixel(j, i);

                    for (int k = 0; k < 3; k++)
                    {
                        if (bitCounter > 8)
                        {
                            if (charBits == "000000000")
                            {
                                saveFileDialog1.FileName = "text.txt";
                                saveFileDialog1.Filter = "Text file (*.txt)|*.txt";
                                saveFileDialog1.InitialDirectory = desktopPath;
                                if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                                {
                                    string filePath = saveFileDialog1.FileName;
                                    File.WriteAllText(filePath, text);
                                }
                                return;
                            }
                            bitCounter = 0;
                            text += Convert.ToChar(Convert.ToInt32(charBits, 2));
                            charBits = "";
                        }
                        switch (k)
                        {
                            case 0:
                                charBits += color.R % 2;
                                break;
                            case 1:
                                charBits += color.G % 2;
                                break;
                            case 2:
                                charBits += color.B % 2;
                                break;
                        }
                        bitCounter++;
                    }

                }
            }
        }

        public string GetCharBits(Char c)
        {
            return Convert.ToString(c, 2).PadLeft(9, '0');
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            LoadImg();
        }

        private void buttonHide_Click(object sender, EventArgs e)
        {
            if (!imgLoaded)
            {
                MessageBox.Show("Image not loaded","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            ClearLSB();
            HideTextInImage(textBox1.Text);
            saveFileDialog1.FileName = "picture.png";
            saveFileDialog1.Filter = "Image file (*.png)|*.png";
            saveFileDialog1.InitialDirectory = desktopPath;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                bmp.Save(filePath);
            }
        }

        private void buttonShow_Click(object sender, EventArgs e)
        {
            if (!imgLoaded)
            {
                MessageBox.Show("Image not loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            GetTextFromImage();
        }
    }
}
