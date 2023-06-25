
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace JoanasIncursionSiteTimer
{
    public partial class JIST : Form
    {

        private Rectangle selectedRectangle;
        private Point mousePositionStart;
        private Point mousePositionEnd;
        private bool isDrawing = false;
        private bool isActive = false;
        List<string> cachedSites = new List<string>();
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private Thread activeThread;
        private IntPtr mouseHookHandle;
        private NativeMethods.HookProc mouseHookProc;
        private TesseractEngine tessa;
        private string logFilePath = "log.txt";
        private Logger logger;
        private bool isFirstScan = true;
        private int timerIDTracker = 0;
        private List<SiteTimer> timers = new List<SiteTimer>();
        public JIST()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Code to be executed when the form is loaded
            tessa = new TesseractEngine(Path.GetFullPath(@"..\..\tessdata"), "eng");
            logger = new Logger(logFilePath);
        }

        private void btnSetup_Click(object Sender, EventArgs e)
        {
            btnSetup.Enabled = false;
            btnStartStop.Enabled = false;
            mousePositionStart = Point.Empty;
            mousePositionEnd = Point.Empty;
            InitializeMouseHook();
            this.isDrawing = true;
        }

        private void CreateSiteTimer()
        {
            timers.Add(new SiteTimer(this.rtbTimers));
            timers[timers.Count - 1].timer.Start();
            timers[timers.Count - 1].timer.Enabled = true;

        }

        private void InitializeMouseHook()



        {
            // Set up the mouse hook procedure
            mouseHookProc = MouseHookCallback;

            // Install the mouse hook
            mouseHookHandle = NativeMethods.SetWindowsHookEx(WH_MOUSE_LL, mouseHookProc, IntPtr.Zero, 0);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Handle mouse events captured by the hook
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)

            {
                // Check if the mouse click occurred outside the form's bounds
                Point clickPosition = GetMousePosition();
                if (!Bounds.Contains(clickPosition))
                {
                    if (mousePositionStart != Point.Empty) { mousePositionEnd = clickPosition; }
                    if (mousePositionEnd == Point.Empty && mousePositionStart == Point.Empty) { mousePositionStart = clickPosition; };
                    if (mousePositionStart != Point.Empty && mousePositionEnd != Point.Empty)
                    {

                        UnhookMouse();

                        cbIsSetup.Checked = true;
                        this.isDrawing = false;
                        btnStartStop.Enabled = true;
                        return new IntPtr(1);
                        // Call the next hook in the hook chain

                    }

                }
            }

            // Call the next hook in the hook chain
            return NativeMethods.CallNextHookEx(mouseHookHandle, nCode, wParam, lParam);
        }

        private Point GetMousePosition()
        {
            NativeMethods.POINT point;
            NativeMethods.GetCursorPos(out point);
            return new Point(point.X, point.Y);
        }

        private void UnhookMouse()
        {
            // Uninstall the mouse hook
            if (mouseHookHandle != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(mouseHookHandle);
                mouseHookHandle = IntPtr.Zero;
            }
        }

        private static Rectangle CreateRectangle(Point point1, Point point2)
        {
            // Calculate the rectangle properties
            int x = Math.Min(point1.X, point2.X);
            int y = Math.Min(point1.Y, point2.Y);
            int width = Math.Abs(point2.X - point1.X);
            int height = Math.Abs(point2.Y - point1.Y);

            // Create and return the rectangle
            return new Rectangle(x, y, width, height);
        }

        private Tuple<int, string> CompareToHQSites(string text)
        {

            int length = text.Length;

            string NRF = HQSites.NRF.Substring(0, Math.Min(length, HQSites.NRF.Length));

            string TCRC = HQSites.TCRC.Substring(0, Math.Min(length, HQSites.TCRC.Length));

            string TPPH = HQSites.TPPH.Substring(0, Math.Min(length, HQSites.TPPH.Length));

            List<Tuple<int, string>> results = new List<Tuple<int, string>>();

            results.Add(new Tuple<int, string>(ComputeLevenshteinDistance(text, NRF), "NRF"));
            results.Add(new Tuple<int, string>(ComputeLevenshteinDistance(text, TCRC), "TCRC"));
            results.Add(new Tuple<int, string>(ComputeLevenshteinDistance(text, TPPH), "TPPH"));

            foreach (var entry in results)
            {

                logger.Log($"Text: {text} - Distance: {entry.Item1} - ComparedTo: {entry.Item2}");
            }

            return GetTupleWithLowestInt(results);

        }

        static Tuple<int, string> GetTupleWithLowestInt(List<Tuple<int, string>> tupleList)
        {
            Tuple<int, string> lowestTuple = null;
            int lowest = int.MaxValue;

            foreach (var tuple in tupleList)
            {
                int currentValue = tuple.Item1;
                if (currentValue < lowest)
                {
                    lowest = currentValue;
                    lowestTuple = tuple;
                }
            }

            return lowestTuple;
        }

        private Pix PreparePixFromRectanglePoints(Point Start, Point End)
        {
            selectedRectangle = CreateRectangle(Start, End);
            Bitmap bitmap = new Bitmap(selectedRectangle.Width, selectedRectangle.Height);

            // Create a graphics object from the bitmap
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Copy the specified region from the screen to the bitmap
                graphics.CopyFromScreen(selectedRectangle.Location, Point.Empty, selectedRectangle.Size);
            }
            Pix result = PixConverter.ToPix(bitmap);
            bitmap.Dispose();
            return result;

        }

        private void HandleStart()
        {
            while (isActive)
            {
                if (selectedRectangle.Contains(GetMousePosition()))
                {
                    lblMouseWarning.Invoke(new Action(() =>
                    {
                        this.lblMouseWarning.Text = "Mouse in Area! No processing!";
                    }));
                    Thread.Sleep(3000);
                    continue;
                }
                else
                {

                    lblMouseWarning.Invoke(new Action(() =>
                    {
                        this.lblMouseWarning.Text = "";
                    }));
                }
                Pix img = PreparePixFromRectanglePoints(mousePositionStart, mousePositionEnd);
                var page = tessa.Process(img);
                var text = page.GetText();
                var iterator = page.GetIterator();
                iterator.Begin();

                List<string> data = new List<string>();
                do
                {
                    do
                    {
                        do
                        {
                            string line = "";
                            do
                            {

                                line += iterator.GetText(PageIteratorLevel.Word) + " ";
                            } while (iterator.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                            if (line.Length > 5) { data.Add(line); }

                        } while (iterator.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                    } while (iterator.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                } while (iterator.Next(PageIteratorLevel.Block));

                page.Dispose();

                HandleSiteCountChange(UpdateActiveSitesFromParsedData(data, 3));

                img.Dispose();
                Thread.Sleep(500);
            }



        }

        private List<string> UpdateActiveSitesFromParsedData(List<string> data, int levenshteinDistance)
        {
            List<string> activeSites = new List<string>();
            foreach (string parsedText in data)
            {
                Tuple<int, string> comparedTuple = CompareToHQSites(parsedText);
                if (comparedTuple.Item1 < levenshteinDistance)
                {
                    activeSites.Add(comparedTuple.Item2);
                }
            }
            return activeSites;
        }

        private void HandleSiteCountChange(List<string> activeSites)
        {
            if (activeSites.Count > 0)

            {
                if (Math.Abs(activeSites.Count - cachedSites.Count) == 1 || isFirstScan)
                {
                    isFirstScan = false;
                    if (activeSites.Count != cachedSites.Count)
                    {
                        if (activeSites.Count + 1 == cachedSites.Count)
                        {
                            // Start a timer
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"..\..\Sounds\ding.wav");
                            player.Play();
                        }

                        cachedSites.Clear();
                        cachedSites.AddRange(activeSites);

                        rtbOutput.Invoke(new Action(() =>
                        {
                            this.rtbOutput.Text = AggregateStrings(cachedSites, ';');
                        }));
                    }
                }

            }
        }

        private void btnStartStop_Click(object Sender, EventArgs e)
        {
            // isActive is used to check if we are running the thread+loop
            if (!isActive)
            {
                isActive = true;
                // Create a new thread to run the loop in there seperatly
                Thread thread = new Thread(HandleStart);
                activeThread = thread;
                activeThread.Start();
                btnStartStop.Text = "Stop";

            }
            else
            {
                isActive = false;
                activeThread.Join();
                btnStartStop.Text = "Start";
                isFirstScan = true;
                btnSetup.Enabled = true;
            }
        }

        static string AggregateStrings(List<string> stringList, char separator)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < stringList.Count; i++)
            {
                builder.Append(stringList[i]);

                if (i != stringList.Count - 1)
                {
                    builder.Append(separator);
                }
            }

            return builder.ToString();
        }

        static int ComputeLevenshteinDistance(string s, string t)
        {
            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= t.Length; j++)
                d[0, j] = j;

            for (int j = 1; j <= t.Length; j++)
            {
                for (int i = 1; i <= s.Length; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }

        private void JIST_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookMouse();
        }
    }
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int X;
            public int Y;
        }

        internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetCursorPos(out POINT lpPoint);
    }
    internal static class HQSites
    {

        public static string TPPH = "True Power Provisional Headquaters";
        public static string NRF = "Nation Rebirth Facility";
        public static string TCRC = "True Creation Research Center";
    };
    public class Logger
    {
        private readonly string logFilePath;

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    string logEntry = $"{DateTime.Now} - {message}";
                    writer.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during logging
                Console.WriteLine($"An error occurred while logging: {ex.Message}");
            }
        }
    }

    public class SiteTimer{
        public  int remainingSeconds = 435;
        public System.Windows.Forms.Timer timer;
        public RichTextBox internalRTBox;

        public SiteTimer(RichTextBox rtb)
        {
           timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            internalRTBox = rtb;
            timer.Tick += Timer_Tick;
          


        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            remainingSeconds--;
           
            if (remainingSeconds >= 0)
            {
                internalRTBox.Invoke(new Action(() =>
                {
                    internalRTBox.Text += remainingSeconds.ToString();
                }));
                    
                   
            }
            else
            {
                timer.Stop();
                
            }
        }

    

    }


}
