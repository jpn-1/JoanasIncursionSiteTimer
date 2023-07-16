
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tesseract;
using JoanasIncursionSiteTimer.models;
using JoanasIncursionSiteTimer.helpers;
using JoanasIncursionSiteTimer.utility;
using System.Reflection;

namespace JoanasIncursionSiteTimer
{
    public partial class JIST : Form
    {

        private Rectangle selectedRectangle;
        private Point mousePositionStart;
        private Point mousePositionEnd;
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
        private List<TimerItem> timers;
        private string executableDirectoryPath;
        public JIST()
        {
            InitializeComponent();
            timers = new List<TimerItem>();
            this.TopMost = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            executableDirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string tessdataPath = Path.Combine(executableDirectoryPath, "tessdata");

            tessa = new TesseractEngine(tessdataPath, "eng");
            logger = new Logger(logFilePath);
            logger.Log($"Currrent Tesseract Data Path: {tessdataPath}");

            lblVersion.Text = Application.ProductVersion;
        }

        private void btnSetup_Click(object Sender, EventArgs e)
        {
            btnSetup.Enabled = false;
            btnStartStop.Enabled = false;
            mousePositionStart = Point.Empty;
            mousePositionEnd = Point.Empty;
            InitializeMouseHook();
        }

        private void AddTimerItem(string timerName, int countdownSeconds)
        {
            // Create a new TimerItem
            TimerItem timerItem = new TimerItem(timerName, countdownSeconds, lvTimers);

            // Add the TimerItem to the list
            timers.Add(timerItem);

            // Create a new ListViewItem for the TimerItem
            ListViewItem item = new ListViewItem(timerName);
            item.Text = (TimeSpan.FromSeconds(countdownSeconds).ToString(@"mm\:ss"));
            item.Tag = timerItem;

            // Add the item to the ListView
            lvTimers.Invoke(new Action(() =>
            {


                timerItem.StartTimer();

            }));

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

                HandleSiteCountChange(UpdateActiveSitesFromParsedData(data, 4));

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
                            try
                            {

                                // Start a timer and play the ding sound

                                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Path.Combine(executableDirectoryPath, @"Sounds\ding.wav"));
                                player.Play();
                                AddTimerItem("Timer!", 435);
                            }
                            catch (Exception ex)
                            {
                                LogException(ex);
                                throw;
                            }
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
            try
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
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void LogException(Exception ex)
        {
            logger.Log($"Error: {ex.Message} ,  Stack: {ex.StackTrace}");
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


        private void RemoveTimer(ListViewItem selectedItem)
        {
            if (selectedItem != null)
            {
                foreach (TimerItem item in timers)
                {
                    if (item.listViewItem == selectedItem)
                    {
                        item.timer.Stop();
                        item.timer.Dispose();


                    }


                }


                lvTimers.Items.Remove(selectedItem);
            }
        }
        private void lvTimers_MouseClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                RemoveTimer(lvTimers.GetItemAt(e.X, e.Y));

             
            }
        }
    }


}
