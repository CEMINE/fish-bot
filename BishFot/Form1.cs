using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
//using IronOcr;
using System.Drawing.Imaging;
using System.Threading;
using Tesseract;
using OpenCvSharp;
using System.Runtime.InteropServices;
using System.Collections;
using WindowsInput.Native;
using WindowsInput;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Text.RegularExpressions;
using Point = System.Drawing.Point;
using OpenTK.Audio.OpenAL;
using Timer = System.Windows.Forms.Timer;
using System.Diagnostics;
using OpenCvSharp.ML;
using BishFot.Properties;

namespace BishFot
{
    public partial class Form1 : Form
    {
        string path = @"A:\c# apps\BishFot\BishFot\Img\";
        string pathTess = @"./tessdata";
        public static string process = string.Empty;
        string rez = "";
        int counter_pesti = 0;
        int uptime = 0;
        float rez_uptime = 0;
        public Form1()
        {
            InitializeComponent();
            this.TopMost = true;
            if (Properties.Settings.Default.path is not null)
                path = Properties.Settings.Default.path;
            
            
        }

        public static class PositionWindowDemo
        {

            // P/Invoke declarations.

            [DllImport("user32.dll", SetLastError = true)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true)]
            static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            const uint SWP_NOSIZE = 0x0001;
            const uint SWP_NOZORDER = 0x0004;

            public static void SetWindowPos()
            {
                // Find (the first-in-Z-order) Notepad window.
                IntPtr hWnd = FindWindow(process ,null);

                // If found, position it.
                if (hWnd != IntPtr.Zero)
                {
                    // Move the window to (0,0) without changing its size or position
                    // in the Z order.
                    SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pictureBox1.ImageLocation = @"A:\c# apps\BishFot\BishFot\Img\test3.jpeg";
            List<Color> colorList2 = new List<Color>();
            colorList2.Add(Color.FromArgb(119, 163, 128));
            this.TopMost = true;
            PositionWindowDemo.SetWindowPos();
            if (Properties.Settings.Default.path is not null)
            path = Properties.Settings.Default.path;
            counter_pesti = Properties.Settings.Default.pesti;
            uptime = Properties.Settings.Default.uptime;
        }
        private void ImgProcessing()
        {
            Mat test = new Mat();
            test = Cv2.ImRead(path + "retine2.jpeg");
            Cv2.MedianBlur(test, test, 3);
            Cv2.CvtColor(test, test, ColorConversionCodes.RGB2GRAY);
            Cv2.Resize(test, test, new OpenCvSharp.Size(700, 500));
            
            Cv2.BitwiseNot(test, test);
            Cv2.Threshold(test, test, 140, 255, ThresholdTypes.Binary);
            //Cv2.ImShow("test", test);
            Cv2.ImWrite(path + "retine3.jpeg", test);
            //if (puzzle > 0)
            Cv2.ImWrite(path + "retinetest.jpeg", test);
        }

        
    
        
        
        private void ReadText()
        {         
            using (var engine = new TesseractEngine(@"./tessdata", "ron", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(path + "retine3.jpeg"))
                {
                    
                    using (var page = engine.Process(img))
                    {
                        var text = page.GetText();
                        rez = text;
                        textBox1.Text = text;           
                    }
                    
                }
            }
        }

        int puzzle = 0;
        int number_puzzle;
        string b = string.Empty;

        private void SolvePuzzleModel()
        {
            GetListOfColors();
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            static extern bool SetCursorPos(int X, int Y);
            InputSimulator sim = new InputSimulator();
            if (x1 != 0 && y1 != 0 && x2 != 0 && y2 != 0 && x3 != 0 && y3 != 0)
            {
                lblXY.Text = $"XY1 ({x1},{y1}), XY2 ({x2},{y2}, X3Y3 ({x3},{y3}))";
                SetCursorPos(x1, y1);
                var t = Task.Delay(300);
                var zet = Task.Delay(500);
                sim.Mouse.LeftButtonClick();
                t.Wait();
                SetCursorPos(x2, y2);
                t.Wait(); ;
                sim.Mouse.LeftButtonClick();
                t.Wait();
                SetCursorPos(x3, y3);
                Debug.WriteLine($"Am gasit patrate la coordonatele X,Y({x1},{y1}), X,Y({x2},{y2}), X,Y({x3},{y3})");
                t.Wait();
                sim.Mouse.LeftButtonClick();
                zet.Wait();
                SendKeys.Send("{F4}");
                SetCursorPos(0, 0);

                puzzle = 0;

            }
        }
        
        private void SendPuzzleNumberInput()
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            static extern bool SetCursorPos(int X, int Y);
            InputSimulator sim = new InputSimulator();
            FindWindow();
            SetCursorPos(950, 580);
            sim.Mouse.LeftButtonClick();
            var t = Task.Delay(200);
            var zet = Task.Delay(500);
            t.Wait();
            SendKeys.Send(b);
            t.Wait();
            SendKeys.Send("{ENTER}");
            SendKeys.Send("{ENTER}");
            zet.Wait();
            SendKeys.Send("{F4}");
            SetCursorPos(0, 0);
            b = string.Empty;
        }

        private void SolvePuzzleNumar()
        {
            
            for (int i = 0; i < rez.Length; i++)
            {
                
                if (char.IsDigit(rez[i]))
                {
                    b+= rez[i];
                }
            }
            if (b.Length > 0 && b.Length < 4)
            {
                number_puzzle = int.Parse(b);

                     
                //MessageBox.Show("numarul este "+ b);
                SendPuzzleNumberInput();
                Debug.WriteLine("Codul pentru puzzle a fost " + b);
                puzzle = 0;
            }
            if (b.Length > 3)
            {
                SolvePuzzleNumar();
            }
                                
        }

        bool enabled = true;
        private void CheckForPuzzle(bool enabled)
        {
            if (enabled == true)
            {
                //Create a new bitmap.
                var bmpScreenshot = new Bitmap(600,
                                               400,
                                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Create a graphics object from the bitmap.
                using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
                {
                    // Take the screenshot from the upper left corner to the right bottom corner.
                    gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                            Screen.PrimaryScreen.Bounds.Y,
                                            -610,
                                            -320,
                                            Screen.PrimaryScreen.Bounds.Size,
                                            CopyPixelOperation.SourceCopy);
                }
                new Bitmap(bmpScreenshot).Save(path + "retine2.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //bmpScreenshot.Save(@"A:\c# apps\BishFot\BishFot\Img\retine2.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                bmpScreenshot.Dispose();
                //pictureBox1.ImageLocation = @"A:\c# apps\BishFot\BishFot\Img\test3.jpeg";
                ImgProcessing();
                var dilei = Task.Delay(200);
                //dilei.Wait();
                ReadText();
                //var dilei = Task.Delay(4000);
                //var dileiut = Task.Delay(2000);
                if (textBox1.Text.Contains("numarul"))
                {
                    //MessageBox.Show("pUZZLE cu numar detectat");
                    enabled = false;
                    puzzle = 1;                                 
                }
                if (textBox1.Text.Contains("model"))
                {
                    //MessageBox.Show("puzzle cu model detectat");
                    enabled = false;
                    puzzle = 2;
                    

                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            PositionWindowDemo.SetWindowPos();
            timer1.Enabled = true;
            timer2.Enabled = true; 
            FindWindow(); 
            SendInput();
        }
        int x = 0;

        private void TakeScreenshotOfGreen()
        {
            
            
            var bmpScreenshot = new Bitmap(200,
                                           200,
                                           System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
            {
                // Take the screenshot from the upper left corner to the right bottom corner.
                gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        -800,
                                        -400,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);
            }

            bmpScreenshot.Save(path + "test4.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
            pictureBox1.ImageLocation = path + "test4.jpeg";
        }

        Color current;
        private void GetColorAt(int x, int y)
        {
            Bitmap bmp = new Bitmap(1, 1);
            Rectangle bounds = new Rectangle(x, y, 1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            colorlist.Add(bmp.GetPixel(0, 0));
            current = bmp.GetPixel(0, 0);
        }
        List<int> x_pos = new List<int>() {850, 906, 961, 1015, 1070 };
        List<int> y_pos = new List<int>() { 470, 530, 580, 630, 695 };
        List<Color> colorlist = new List<Color>();
        int x1 = 0;
        int y1 = 0;
        int x2 = 0;
        int y2 = 0;
        int x3 = 0;
        int y3 = 0;
        int count = 0;
        private void GetListOfColors()
        {
            for (int i = 0; i < x_pos.Count; i++)
            {
                for (int x = 0; x< y_pos.Count; x++)
                {
                    //colorlist.Add(GetColorAt(x_pos[x], y_pos[i]));
                    GetColorAt(x_pos[i], y_pos[x]);
                    if (current.R >= 55 && current.R <= 65 && current.G >= 65 && current.G <= 75 && current.B >= 86 && current.B <= 100)
                    {
                        count++;
                        if (count == 1)
                        {
                            x1 = x_pos[i];
                            y1 = y_pos[x];
                        }
                        if (count == 2)
                        {
                            x2 = x_pos[i];
                            y2 = y_pos[x];
                        }
                        if (count == 3)
                        {
                            x3 = x_pos[i];
                            y3 = y_pos[x];
                            count = 0;
                        }
                    }
                    //MessageBox.Show("x axis: " + x_pos[i] + " y axis: " + y_pos[x]);
                    //MessageBox.Show("x1y1: " +x1 + "," + y1 + "x2y2: " + x2 + "," + y2 + " x3y3: " + x3 + "," + y3);

                }
            }

 
            //MessageBox.Show(GetColorAt(853, 477)ToString());
            //listBox1.DataSource = colorlist;
        }



        bool isGreen = false;
        private void GetPixels()
        {
            var b = (Bitmap)Image.FromFile(path + "test4.jpeg");
            

            List<Color> colorList = new List<Color>
            {

            };

            

            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    colorList.Add(b.GetPixel(x, y));
                }
            }
            
            for (int i = 0; i < colorList.Count; i++)
            {
                /*
                Color g = Color.GreenYellow;
                if (colorList[i] == g) 
                {
                    MessageBox.Show("Am gasit verdili la " + i);
                }
                */
                if (colorList[i].R >= 100 && colorList[i].R <= 120)
                {
                    if (colorList[i].G >= 150 && colorList[i].G <= 200)
                    {
                        if (colorList[i].B >= 100 && colorList[i].B <= 130)
                        {
                            isGreen = true;
                            lblOutput.Text = "lam gasit box";
                        }
                    }
                }

                //lblOutput.Text = "e greseala";

                
            }
            b.Dispose();
            //colorList.Sort();
            //listBox1.DataSource = colorList;

        }


        //Color g = 

        void button2_Click(object sender, EventArgs e)
        {

            
        }
        string pesti = "Sturgeon, perch, puffer, angel, small, tuna, carp";


        public async Task Execute(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            action();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            var t = Task.Delay(4000);
            var tt = Task.Delay(200);
            if (lblOutput.Text != "lam gasit box")
            {
                FindWindow();
                tt.Wait();
                TakeScreenshotOfGreen();
                tt.Wait();
                GetPixels();
            }
            
            if (lblOutput.Text == "lam gasit box")
            {
                counter_pesti++;
                timer1.Enabled = false;
                lblOutput.Text = "cacatelul";
                SendInput();
                SendInput();
                Execute(SendInput, 4200);
                Execute(timer1.Start, 4200);              

            }




        }
        System.Timers.Timer runonce = new System.Timers.Timer(4000);
        bool windowIsActive = false;
        void FindWindow()
        {
            [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
            static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("USER32.DLL")]
            static extern bool SetForegroundWindow(IntPtr hWnd);
            IntPtr calcWindow = FindWindow(process, null);

            SetForegroundWindow(calcWindow);
            windowIsActive = true;

        }       

        void SendInput()
        {
            InputSimulator sim = new InputSimulator();
            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            /*
            System.Timers.Timer runonce = new System.Timers.Timer(4000);
            runonce.Elapsed += (s, e) => { SendInput(); };
            runonce.AutoReset = false;
            runonce.Start();    
            */
            PositionWindowDemo.SetWindowPos();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Enabled = !timer2.Enabled;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (enabled == true)
            {
                CheckForPuzzle(enabled);
            }
            
            lblPuzzle.Text = puzzle.ToString();
            if (puzzle == 1)
            {
                enabled = false;
                Debug.WriteLine("am detectat puzzle de tip numar");
                timer1.Enabled = false;
                timer2.Enabled = false;
                Debug.WriteLine("asteptam 5 s");
                Thread.Sleep(6500);
                Debug.WriteLine("am asteptat 5s");
                Debug.WriteLine("incep rezolvarea puzzle-ului");
                Debug.WriteLine("Codul pentru puzzle a fost " + b);
                var trc = Task.Delay(3000);
                SolvePuzzleNumar();
                Debug.WriteLine("am rezolvat puzzle-ul");
                puzzle = 0;
                trc.Wait();
                SendInput();
                timer1.Enabled = true;
                timer2.Enabled = true;
                enabled = true;

            }
            if (puzzle == 2)
            {
                puzzle = 0;
                enabled = false;
                GetListOfColors();
                Debug.WriteLine("am detectat puzzle de tip model");          
                timer1.Enabled = false;
                timer2.Enabled = false;
                Debug.WriteLine("asteptam 5 s");
                var mcn = Task.Delay(6500);
                Thread.Sleep(6500);
                var trc = Task.Delay(2500);
                Debug.WriteLine("am asteptat 5s");
                Debug.WriteLine("incep rezolvarea puzzle-ului");
                Debug.WriteLine($"Am gasit patrate la coordonatele X,Y({x1},{y1}), X,Y({x2},{y2}), X,Y({x3},{y3})");
                SolvePuzzleModel();
                Debug.WriteLine("am rezolvat puzzle-ul");            
                trc.Wait();
                SendInput();
                timer1.Enabled = true;
                timer2.Enabled = true;
                enabled = true;
                x1 = 0;
                x2 = 0;
                x3 = 0;
                y1 = 0;
                y2 = 0;
                y3 = 0;

            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            lblXY.Text = "Timer 2 este " + timer2.Enabled;
            lblPuzzle.Text = "Timer 1 este " + timer1.Enabled;
            Settings.Default.pesti = counter_pesti;
            Properties.Settings.Default.Save();
            lblPesti.Text = "Pesti prinsi: " + counter_pesti;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            path = textBox1.Text;
            Properties.Settings.Default.path = path;
            Properties.Settings.Default.Save();
            MessageBox.Show("path " + path);
            textBox1.Text = string.Empty;
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            
            process = textBox1.Text;
            MessageBox.Show("process " + process);
            textBox1.Text = string.Empty;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer2.Enabled = false;
        }

        private void btnResetPesti_Click(object sender, EventArgs e)
        {
            counter_pesti = 0;
            Properties.Settings.Default.Save();
        }


    }
}
