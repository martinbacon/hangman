/*
Copyright (C) 2016  Martin Slanina

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Media;

namespace obesenec
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            metodaButtony();           
        }

        private void metodaButtony()
        {
            for (int i = 0; i < 26; i++)
            {
                polebuttonu[i] = new Button();
                polebuttonu[i].Top = 400;
                polebuttonu[i].Left = i * 30 + 10;
                polebuttonu[i].Text = Convert.ToChar(65 + i).ToString();
                polebuttonu[i].Size = new Size(30, 30);
                polebuttonu[i].Font = new Font(this.Font.FontFamily, 16);
                polebuttonu[i].Click += new System.EventHandler(letterClick);
                this.Controls.Add(polebuttonu[i]);
            }

            for (int i = 0; i < polelabelu.Length; i++)
            {
                polelabelu[i] = new Label();
            }
        }

        Label[] polelabelu = new Label[32];
        Button[] polebuttonu = new Button[26];
        Label label1 = new Label();
        Label label2 = new Label();
        SoundPlayer prehravac = new SoundPlayer();
        WMPLib.WindowsMediaPlayer wmp = new WMPLib.WindowsMediaPlayer();
        WMPLib.WindowsMediaPlayer wmp2 = new WMPLib.WindowsMediaPlayer();
        string[] zvukywin = new string[3] { "win_1.wav", "win_2.wav", "win_3.wav" };
        string[] zvukynew = new string[5] { "new_1.wav", "new_2.wav", "new_3.wav", "new_4.wav", "new_5.wav" };
        string[] zvukylose = new string[3] { "lose_1.wav", "lose_2.wav", "lose_3.wav" };
        string[] zvukycorrect = new string[3] { "correct_1.wav", "correct_3.wav", "correct_3.wav" };
        string[] poleslov = new string[110000];
        string slovo,puvodnislovo;
        Random kostka = new Random();
        int nahoda;
        public int nastaveni;
        int zivoty = 5;
        int skore;
        int nasobic = 1;
        int pocet = 0;

        private void letterClick(object sender, EventArgs e)
        {
            Button send = sender as Button;
            int index = 0;
            bool nalezeno = false;
            foreach (char c in slovo)
            {
                if (c==Convert.ToChar(send.Text.ToLower()))
                {
                    polelabelu[index].Text = send.Text;
                    pocet++;
                    nalezeno = true;
                    skore += 8 * nasobic;
                    nasobic += 1;
                    send.Visible = false;
                    wmp2.URL = zvukycorrect[kostka.Next(zvukycorrect.Length)];
                }
                index++;
            }
            if (!nalezeno)
            {
                zivoty -= 1;
                label3.Text = label3.Text.Substring(0, label3.Text.Length - 1);
                nasobic = 1;
                prehravac.SoundLocation = "wrong.wav";
                prehravac.Play();
            }
            if (zivoty==0)
            {
                wmp.controls.stop();
                wmp2.URL = zvukylose[kostka.Next(zvukylose.Length)];
                MessageBox.Show("Prohra" + Environment.NewLine + slovo);
                for (int i = 0; i < polelabelu.Length; i++)
                {
                    this.Controls.Remove(polelabelu[i]);
                }
                metodaLabely();
                skore = 0;
                nasobic = 1;
                metodaZivoty();
                pocet = 0;
                wmp.controls.stop();
                wmp2.URL = zvukynew[kostka.Next(zvukynew.Length)];
            }
            index = 0;
            if (pocet==slovo.Length)
            {
                wmp.controls.stop();
                wmp2.URL = zvukywin[kostka.Next(zvukywin.Length)];
                MessageBox.Show("Výhra");
                for (int i = 0; i < polelabelu.Length; i++)
                {
                    this.Controls.Remove(polelabelu[i]);
                }               
                metodaLabely();
                pocet = 0;
                metodaZivoty();
                wmp.controls.stop();
                wmp2.URL = zvukynew[kostka.Next(zvukynew.Length)];
            }
            label1.Text = "Skóre: " + skore.ToString();
            label2.Text = nasobic.ToString() + "x";
            label2.Font = new Font(this.Font.FontFamily, nasobic * 6+10);
        }

        private void metodaZivoty()
        {
            if (nastaveni == 1)
            {
                zivoty = 12;
                label3.Text = "♥♥♥♥♥♥♥♥♥♥♥♥";
            }
            if (nastaveni == 2)
            {
                zivoty = 8;
                label3.Text = "♥♥♥♥♥♥♥♥";
            }
            if (nastaveni == 3)
            {
                zivoty = 5;
                label3.Text = "♥♥♥♥♥";
            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
            metodaZivoty();
            wmp2.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wmp2_PlayStateChange);
            wmp.settings.setMode("loop", true);
            wmp.URL = "loop.wav";
            metodaLabely();
            pictureBox1.Image = Image.FromFile("bulb.gif");
        }

        private void wmp2_PlayStateChange(int NewState)
        {
            //MessageBox.Show(NewState.ToString());
            if (NewState==8)
            {
                wmp.URL = "loop.wav";
            }
        }

        private void metodaLabely()
        {
            FileStream fs = new FileStream("slova.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            int k = 0;
            while (!sr.EndOfStream)
            {
                poleslov[k] = sr.ReadLine();
                k++;
            }
            nahoda = kostka.Next(0, k);
            slovo = poleslov[nahoda];
            puvodnislovo = poleslov[nahoda];
            for (int i = 0; i < polelabelu.Length; i++)
            {
                polelabelu[i].Text = "";
            }
            for (int i = 0; i < slovo.Length; i++)
            {
                polelabelu[i].Top = 340;
                polelabelu[i].Left = i * 40 + 230;
                polelabelu[i].Text = "__";
                polelabelu[i].Size = new Size(40, 40);
                polelabelu[i].Font = new Font(this.Font.FontFamily, 20);

                this.Controls.Add(polelabelu[i]);
            }
            sr.Close();
            fs.Close();
            foreach (Button c in polebuttonu)
            {
                c.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(slovo);
        }

    }
}
