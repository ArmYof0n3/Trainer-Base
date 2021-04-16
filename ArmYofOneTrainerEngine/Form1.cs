using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Cheats;

namespace ArmYofOneTrainerEngine
{
    public partial class Form1 : Form
    {
        #region DllImports
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        SoundPlayer splayer = new SoundPlayer();

        Dictionary<string, uint> dictionary = new Dictionary<string, uint>();

        myCheats ch = new myCheats();       

        private bool mouseDown;
        private Point lastLocation;
        private bool Muted = false;
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;

        //Trainer
        private uint[] HotkeyB = new uint[12];
        //End Trainer

        public Form1()
        {
            InitializeComponent();

            loadDictionary();

            Label[] labels = new Label[]
            {
                Hotkey0,
                Hotkey1,
                Hotkey2,
                Hotkey3,
                Hotkey4,
                Hotkey5,
                Hotkey6,
                Hotkey7,
                Hotkey8,
                Hotkey9,
                Hotkey10,
                Hotkey11
            };

            try
            {

                foreach (KeyValuePair<string, uint> pair in dictionary)
                {
                    for (int k = 0; k < labels.Length; k++)
                    {

                        HotkeyB[k] = Convert.ToUInt32(Convert.ToString(Properties.Settings.Default["HotkeyB" + k]), 16);

                        if (HotkeyB[k] == pair.Value)
                        {
                            labels[k].Text = pair.Key;
                        }
                    }

                }
            }
            catch
            {

                int i = 0;
                
                for (uint ui = 0x60; ui <= 0x6A; ui++)  //Virtual keys NUM's buttons
                {
                    HotkeyB[i] = ui;
                    i++;

                }


                foreach (KeyValuePair<string, uint> pair in dictionary)
                {

                    for (int k = 0; k < labels.Length; k++)
                    {

                        if (HotkeyB[k] == pair.Value)
                            labels[k].Text = pair.Key;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (backgroundWorker1.IsBusy == false)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            RegisterHotKey(this.Handle, 0, 0x0000 | 0x4000, HotkeyB[0]);
            RegisterHotKey(this.Handle, 1, 0x0000 | 0x4000, HotkeyB[1]);
            RegisterHotKey(this.Handle, 2, 0x0000 | 0x4000, HotkeyB[2]);
            RegisterHotKey(this.Handle, 3, 0x0000 | 0x4000, HotkeyB[3]);
            RegisterHotKey(this.Handle, 4, 0x0000 | 0x4000, HotkeyB[4]);
            RegisterHotKey(this.Handle, 5, 0x0000 | 0x4000, HotkeyB[5]);
            RegisterHotKey(this.Handle, 6, 0x0000 | 0x4000, HotkeyB[6]);
            RegisterHotKey(this.Handle, 7, 0x0000 | 0x4000, HotkeyB[7]);
            RegisterHotKey(this.Handle, 8, 0x0000 | 0x4000, HotkeyB[8]);
            RegisterHotKey(this.Handle, 9, 0x0000 | 0x4000, HotkeyB[9]);
            RegisterHotKey(this.Handle, 10, 0x0000 | 0x4000, HotkeyB[10]);
            RegisterHotKey(this.Handle, 11, 0x0000 | 0x4000, HotkeyB[11]);
        }

        //Customize Form Start

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            this.Cursor = Cursors.SizeAll;
            lastLocation = e.Location;
        }


        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            this.Cursor = Cursors.Default;
        }

        private void pictureMinimizeForm_MouseClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void pictureMinimizeForm_MouseEnter(object sender, EventArgs e)
        {
            this.pictureMinimizeForm.BackColor = Color.WhiteSmoke;
            this.Cursor = Cursors.Hand;
        }

        private void pictureMinimizeForm_MouseLeave(object sender, EventArgs e)
        {
            this.pictureMinimizeForm.BackColor = Color.Transparent;
            this.Cursor = Cursors.Default;
        }

        private void pictureCloseForm_MouseEnter(object sender, EventArgs e)
        {
            this.pictureCloseForm.BackColor = Color.WhiteSmoke;
            this.Cursor = Cursors.Hand;
        }

        private void pictureCloseForm_MouseLeave(object sender, EventArgs e)
        {
            this.pictureCloseForm.BackColor = Color.Transparent;
            this.Cursor = Cursors.Default;

        }

        private void pictureCloseForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (WindowState != FormWindowState.Minimized && WindowState != FormWindowState.Maximized)
            {
                this.Close();
            }
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (ch.openProc)
            {

                if (!Muted)
                {

                    for (int i = 0; i < ch.CodeCave.Length; i++)
                    {
                        if (ch.CodeCave[i] != 0 || ch.AoBScan[0] != 0)
                        {
                            splayer.Stream = Properties.Resources.beep_off;
                            splayer.PlaySync();
                            break;
                        }

                    }
                }

                ch.TrainerClose();
            }
        }

        private void m_Enter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void m_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void Fearless_MouseClick(object sender, MouseEventArgs e)
        {
            var targetURL = "https://fearlessrevolution.com";
            
            var psi = new ProcessStartInfo
            {
                FileName = targetURL,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void picturePayPal_MouseClick(object sender, MouseEventArgs e)
        {

            var targetURL = "https://www.paypal.me/ArmYof0n3";

            var psi = new ProcessStartInfo
            {
                FileName = targetURL,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void picturePatreon_MouseClick(object sender, MouseEventArgs e)
        {

            var targetURL = "https://www.patreon.com/ArmYof0n31";

            var psi = new ProcessStartInfo
            {
                FileName = targetURL,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void pictureMute_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Muted)
            {
                Muted = true;
                pictureMute.Image = Properties.Resources.muted;
                this.toolTip1.SetToolTip(this.pictureMute, "Unmute Sound");

            }
            else
            {
                Muted = false;
                pictureMute.Image = Properties.Resources.mute;
                this.toolTip1.SetToolTip(this.pictureMute, "Mute Sound");
            }
        }

        private uint textToVKey(string text)
        {
            if (dictionary.ContainsKey(text))
                return dictionary[text];
            else
                return 0x00; // I don't know.
        }

        private void loadDictionary()
        {
            int i = 0;
            uint ui = 0x00;

            //functions
            i = 1;
            for (ui = 0x70; ui <= 0x7A; ui++)
            {
                dictionary.Add("F" + i.ToString(), ui);
                i++;
            }

            //num pad
            i = 0;
            for (ui = 0x60; ui <= 0x69; ui++)
            {
                dictionary.Add("NUM " + i.ToString(), ui);
                i++;
            }

            dictionary.Add("NUM *", 0x6A);
            dictionary.Add("NUM +", 0x6B);
            dictionary.Add("NUM -", 0x6D);
            dictionary.Add("NUM .", 0x6E);
            dictionary.Add("NUM /", 0x6F);

            dictionary.Add("PGDN", 0x22);
            dictionary.Add("PGUP", 0x21);
            dictionary.Add("INSERT", 0x2D);
            dictionary.Add("HOME", 0x24);
            dictionary.Add("END", 0x23);
            dictionary.Add("DEL", 0x2E);

            //numbers
            i = 0;
            for (ui = 0x30; ui <= 0x39; ui++)
            {
                dictionary.Add(i.ToString(), ui);
                i++;
            }

            HotkeyBox.DataSource = new BindingSource(dictionary, null);
            HotkeyBox.DisplayMember = "Key";
            HotkeyBox.ValueMember = "Value";

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (HotkeyPanel.Visible == false)
            {
                this.pictureGameCover.Height = 220;
                this.HotkeyPanel.Visible = true;

                CheatNumBox.SelectedIndex = -1;
                HotkeyBox.SelectedIndex = -1;
            }
            else
            {
                this.pictureGameCover.Height = 304;
                this.HotkeyPanel.Visible = false;
            }
        }

        private void SetHotkeyButton_MouseClick(object sender, MouseEventArgs e)
        {

            Label[] labels = new Label[]
            {
                Hotkey0,
                Hotkey1,
                Hotkey2,
                Hotkey3,
                Hotkey4,
                Hotkey5,
                Hotkey6,
                Hotkey7,
                Hotkey8,
                Hotkey9,
                Hotkey10,
                Hotkey11
            };

            for (int i = 0; i < labels.Length; i++)
            {
                if (labels[i].Text == HotkeyBox.Text || HotkeyBox.Text == labels[i].Text)
                {
                    MessageBox.Show("This hotkey is already registered !", "Information", 0, MessageBoxIcon.Information);
                    break;
                }
                else if (CheatNumBox.SelectedIndex != -1 && HotkeyBox.SelectedIndex != -1)
                {
                    switch (CheatNumBox.SelectedIndex)
                    {

                        case 0:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[9] = textToVKey(HotkeyBox.Text);
                                Hotkey9.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 9);
                                RegisterHotKey(this.Handle, 9, 0x0000 | 0x4000, HotkeyB[9]);
                            }
                            break;

                        case 1:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[10] = textToVKey(HotkeyBox.Text);
                                Hotkey10.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 10);
                                RegisterHotKey(this.Handle, 10, 0x0000 | 0x4000, HotkeyB[10]);
                            }
                            break;

                        case 2:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[11] = textToVKey(HotkeyBox.Text);
                                Hotkey11.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 11);
                                RegisterHotKey(this.Handle, 11, 0x0000 | 0x4000, HotkeyB[11]);
                            }
                            break;

                        case 3:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[0] = textToVKey(HotkeyBox.Text);
                                Hotkey0.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 0);
                                RegisterHotKey(this.Handle, 0, 0x0000 | 0x4000, HotkeyB[0]);
                            }
                            break;

                        case 4:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[1] = textToVKey(HotkeyBox.Text);
                                Hotkey1.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 1);
                                RegisterHotKey(this.Handle, 1, 0x0000 | 0x4000, HotkeyB[1]);
                            }
                            break;

                        case 5:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[2] = textToVKey(HotkeyBox.Text);
                                Hotkey2.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 2);
                                RegisterHotKey(this.Handle, 2, 0x0000 | 0x4000, HotkeyB[2]);
                            }
                            break;

                        case 6:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[3] = textToVKey(HotkeyBox.Text);
                                Hotkey3.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 3);
                                RegisterHotKey(this.Handle, 3, 0x0000 | 0x4000, HotkeyB[3]);
                            }
                            break;

                        case 7:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[4] = textToVKey(HotkeyBox.Text);
                                Hotkey4.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 4);
                                RegisterHotKey(this.Handle, 4, 0x0000 | 0x4000, HotkeyB[4]);
                            }
                            break;

                        case 8:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[5] = textToVKey(HotkeyBox.Text);
                                Hotkey5.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 5);
                                RegisterHotKey(this.Handle, 5, 0x0000 | 0x4000, HotkeyB[5]);
                            }
                            break;

                        case 9:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[6] = textToVKey(HotkeyBox.Text);
                                Hotkey6.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 6);
                                RegisterHotKey(this.Handle, 6, 0x0000 | 0x4000, HotkeyB[6]);

                            }
                            break;

                        case 10:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[7] = textToVKey(HotkeyBox.Text);
                                Hotkey7.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 7);
                                RegisterHotKey(this.Handle, 7, 0x0000 | 0x4000, HotkeyB[7]);
                            }
                            break;

                        case 11:
                            if (i == labels.Length - 1)
                            {
                                HotkeyB[8] = textToVKey(HotkeyBox.Text);
                                Hotkey8.Text = HotkeyBox.Text;

                                UnregisterHotKey(this.Handle, 8);
                                RegisterHotKey(this.Handle, 8, 0x0000 | 0x4000, HotkeyB[8]);
                            }
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("A cheat or a hotkey isn't selected !", "Information", 0, MessageBoxIcon.Information);
                    break;
                }
            }
        }

        private void SaveHotkeysButton_MouseClick(object sender, MouseEventArgs e)
        {
            Label[] labels = new Label[]
            {
                Hotkey0,
                Hotkey1,
                Hotkey2,
                Hotkey3,
                Hotkey4,
                Hotkey5,
                Hotkey6,
                Hotkey7,
                Hotkey8,
                Hotkey9,
                Hotkey10,
                Hotkey11
            };

            for (int i = 0; i < labels.Length; i++)
            {
                Properties.Settings.Default["HotkeyB" + i] = "0x" + HotkeyB[i].ToString("X");
            }

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        //Customize Form End

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (true)
            {
                ch.OpenGame();

                if (ch.openProc)
                {

                    this.ProcessID.Invoke((MethodInvoker)delegate
                    {     
                        this.ProcessID.Text = "PROCESS ID: " + ch.pID.ToString();
                    });

                    if (!ch.UWP)
                        Platform.Text = "PLATFORM: STEAM";
                    else
                        Platform.Text = "PLATFORM: UWP";

                    this.GameOnOff.ForeColor = Color.SpringGreen;
                    this.GameOnOff.Text = "ON";


                    if (ch.CodeCave[1] > 0)
                    {
                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x269).ToString("X")) == 0)
                        {
                            this.Cheat1.ForeColor = Color.White;
                        }

                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x26D).ToString("X")) == 0)
                        {
                            this.Cheat2.ForeColor = Color.White;
                        }

                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x271).ToString("X")) == 0)
                        {
                            this.Cheat3.ForeColor = Color.White;
                        }

                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x28D).ToString("X")) == 0)
                        {
                            this.Cheat5.ForeColor = Color.White;
                        }

                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x291).ToString("X")) == 0)
                        {
                            this.Cheat6.ForeColor = Color.White;
                        }

                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x295).ToString("X")) == 0)
                        {
                            this.Cheat7.ForeColor = Color.White;
                        }

                        if (ch.LibMem.ReadInt((ch.CodeCave[1] + 0x299).ToString("X")) == 0)
                        {
                            this.Cheat8.ForeColor = Color.White;
                        }
                    }

                    if (ch.CodeCave[3] > 0)
                    {
                        if (!ch.UWP)
                        {
                            if (ch.LibMem.ReadInt((ch.CodeCave[3] + 0x94).ToString("X")) == 0)
                                this.Cheat9.ForeColor = Color.White;
                        }
                        else
                        {
                            if (ch.LibMem.ReadInt((ch.CodeCave[3] + 0x98).ToString("X")) == 0)
                                this.Cheat9.ForeColor = Color.White;
                        }
                    }

                    if (ch.CodeCave[5] > 0)
                    {
                        if (!ch.UWP)
                        {
                            if (ch.LibMem.ReadInt((ch.CodeCave[5] + 0x7A).ToString("X")) == 0)
                                this.Cheat10.ForeColor = Color.White;
                        }
                        else
                        {
                            if (ch.LibMem.ReadInt((ch.CodeCave[5] + 0x7E).ToString("X")) == 0)
                                this.Cheat10.ForeColor = Color.White;
                        }
                    }

                    if (ch.CodeCave[6] > 0)
                        if (ch.LibMem.ReadInt((ch.CodeCave[6] + 0x98).ToString("X")) == 0)
                            this.Cheat11.ForeColor = Color.White;

                }
                else
                {

                    this.ProcessID.Text = "PROCESS ID: 0";

                    Platform.Text = "PLATFORM: ?";

                    if (GameOnOff.Text != "OFF")
                    {

                        this.GameOnOff.ForeColor = Color.Red;
                        this.GameOnOff.Text = "OFF";

                    }
                    else
                    {
                        this.GameOnOff.Text = "";
                    }

                    Label[] lbs = new Label[]
                    {
                        Cheat0,
                        Cheat1,
                        Cheat2,
                        Cheat3,
                        Cheat4,
                        Cheat5,
                        Cheat6,
                        Cheat7,
                        Cheat8,
                        Cheat9,
                        Cheat10,
                        Cheat11
                    };

                    for (int i = 0; i < lbs.Length; i++) // check all labels
                    {
                        if (lbs[i].ForeColor == Color.RoyalBlue)
                        {
                            lbs[i].ForeColor = Color.White;
                        }
                    }
                }

                if (!ch.openProc)
                    Thread.Sleep(500);
                else
                    Thread.Sleep(1000);
            }
        }

        protected override void WndProc(ref Message m) //hotbuttons
        {
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();

                if (id == 0)         
                    MyCheat0();

                if (id == 1)
                    MyCheat1();

                if (id == 2)
                   MyCheat2();

                if (id == 3)
                    MyCheat3();

                if (id == 4)
                    MyCheat4();
                
                if (id == 5)
                    MyCheat5();
                
                if (id == 6)
                    MyCheat6();

                if (id == 7)
                    MyCheat7();

                if (id == 8)    
                    MyCheat8();

                if (id == 9)
                    MyCheat9();

                if (id == 10)
                    MyCheat10();

                if (id == 11)
                    MyCheat11();

            }
            base.WndProc(ref m);
        }

        private async void MyCheat9() // Money add after trainer finish !!!
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[3] == 0)
                {
                    if (!ch.UWP)
                        await ch.CheatMoney();
                    else
                        await ch.CheatMoneyUwp();
                }

                if (ch.CodeCave[3] > 0)
                {
                    //if (ch.LibMem.ReadInt((ch.CodeCave[3] + 0x94).ToString("X")) == 0)
                    //{
                     
                    if(!ch.UWP)
                        ch.LibMem.WriteMemory((ch.CodeCave[3] + 0x94).ToString("X"), "int", "1");
                    else
                        ch.LibMem.WriteMemory((ch.CodeCave[3] + 0x98).ToString("X"), "int", "1");

                    this.Cheat9.ForeColor = Color.RoyalBlue;
   
                    if (!Muted)  
                    {       
                        splayer.Stream = Properties.Resources.beep_on;  
                        splayer.PlaySync(); 
                    }
                    //}
                    /*else if (ch.LibMem.ReadInt((ch.CodeCave[3] + 0x7F).ToString("X")) == 1)
                    {
                        ch.LibMem.WriteMemory((ch.CodeCave[3] + 0x7F).ToString("X"), "int", "0");

                        this.Cheat9.ForeColor = Color.White;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_off;
                            splayer.PlaySync();

                        }
                    }*/
                }
            }
        }

        private async void MyCheat10() // Wheelspins add after trainer finish !!!
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[5] == 0)
                {
                    if (!ch.UWP)
                        await ch.CheatWheelSpins();
                    else
                        await ch.CheatWheelSpinsUwp();
                }

                if (ch.CodeCave[5] > 0)
                {
                    //if (ch.LibMem.ReadInt((ch.CodeCave[5] + 0x7A).ToString("X")) == 0)
                    //{
                    if(!ch.UWP)    
                        ch.LibMem.WriteMemory((ch.CodeCave[5] + 0x7A).ToString("X"), "int", "1");
                    else
                        ch.LibMem.WriteMemory((ch.CodeCave[5] + 0x7E).ToString("X"), "int", "1");

                    this.Cheat10.ForeColor = Color.RoyalBlue;

                        
                    if (!Muted)   
                    {
                            splayer.Stream = Properties.Resources.beep_on;
                            splayer.PlaySync();  
                    }
                    //}
                    /*else if (ch.LibMem.ReadInt((ch.CodeCave[5] + 0x4E).ToString("X")) == 1)
                    {
                        ch.LibMem.WriteMemory((ch.CodeCave[5] + 0x4E).ToString("X"), "int", "0");

                        this.Cheat10.ForeColor = Color.White;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_off;
                            splayer.PlaySync();

                        }
                    }*/
                }
            }
        }

        private async void MyCheat11() // Perk Points add after trainer finish !!!
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[6] == 0)
                {
                    if (!ch.UWP)
                        await ch.CheatPerkPoints();
                    else
                        await ch.CheatPerkPointsUwp();
                }

                if (ch.CodeCave[6] > 0)
                {
                    //if (ch.LibMem.ReadInt((ch.CodeCave[6] + 0x98).ToString("X")) == 0)
                    //{
                        ch.LibMem.WriteMemory((ch.CodeCave[6] + 0x98).ToString("X"), "int", "1");

                        this.Cheat11.ForeColor = Color.RoyalBlue;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_on;
                            splayer.PlaySync();

                        }
                    //}
                    /*else if (ch.LibMem.ReadInt((ch.CodeCave[5] + 0x4E).ToString("X")) == 1)
                    {
                        ch.LibMem.WriteMemory((ch.CodeCave[5] + 0x4E).ToString("X"), "int", "0");

                        this.Cheat10.ForeColor = Color.White;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_off;
                            splayer.PlaySync();

                        }
                    }*/
                }
            }
        }

        private async void MyCheat0()
        {
            if (ch.openProc)
            {    
                await ch.CheatTimer();

                if (ch.AoBScan[0] > 0)
                {
                    if (ch.TimerChange == 1)
                    {
                        this.Cheat0.ForeColor = Color.RoyalBlue;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_on;
                            splayer.PlaySync();

                        }
                    }
                    else if (ch.TimerChange == 2)
                    {
                        this.Cheat0.ForeColor = Color.White;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_off;
                            splayer.PlaySync();

                        }
                    }
                }
            }
        }

        private async void MyCheat1()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x269).ToString("X"), "int", "1");

                    this.Cheat1.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }

        private async void MyCheat2()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x26D).ToString("X"), "int", "1");

                    this.Cheat2.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }

        private async void MyCheat3()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x271).ToString("X"), "int", "1");

                    this.Cheat3.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }

        private async void MyCheat4()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[2] == 0)
                    await ch.CheatFreezeAI();

                if (ch.CodeCave[2] > 0)
                {
                    if (ch.LibMem.ReadInt((ch.CodeCave[2] + 0x39).ToString("X")) == 0)
                    {
                        ch.LibMem.WriteMemory((ch.CodeCave[2] + 0x39).ToString("X"), "int", "1");

                        this.Cheat4.ForeColor = Color.RoyalBlue;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_on;
                            splayer.PlaySync();

                        }
                    }
                    else if (ch.LibMem.ReadInt((ch.CodeCave[2] + 0x39).ToString("X")) == 1)
                    {
                        ch.LibMem.WriteMemory((ch.CodeCave[2] + 0x39).ToString("X"), "int", "0");

                        this.Cheat4.ForeColor = Color.White;

                        if (!Muted)
                        {
                            splayer.Stream = Properties.Resources.beep_off;
                            splayer.PlaySync();

                        }
                    }
                }
            }
        }

        private async void MyCheat5()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x28D).ToString("X"), "int", "1");

                    this.Cheat5.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }

        private async void MyCheat6()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x291).ToString("X"), "int", "1");

                    this.Cheat6.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }

        private async void MyCheat7()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x295).ToString("X"), "int", "1");

                    this.Cheat7.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }

        private async void MyCheat8()
        {
            if (ch.openProc)
            {
                if (ch.CodeCave[1] == 0)
                    await ch.CheatVelocity();

                if (ch.CodeCave[1] > 0)
                {
                    ch.LibMem.WriteMemory((ch.CodeCave[1] + 0x299).ToString("X"), "int", "1");

                    this.Cheat8.ForeColor = Color.RoyalBlue;

                    if (!Muted)
                    {
                        splayer.Stream = Properties.Resources.beep_on;
                        splayer.PlaySync();

                    }
                }
            }
        }
    }
}
