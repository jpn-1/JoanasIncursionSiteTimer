using System;
using System.Windows.Forms;
using System.Threading;

namespace JoanasIncursionSiteTimer.models
    {
public class TimerItem
{
    public string TimerName { get; }
    public int CountdownSeconds { get; set; }
    public System.Windows.Forms.Timer Timer { get; }
    public ListViewItem ListViewItem { get; }

    public TimerItem(string timerName, int countdownSeconds)
    {
        TimerName = timerName;
        CountdownSeconds = countdownSeconds;
        Timer = new System.Windows.Forms.Timer();
        Timer.Interval = 1000; // 1 second
        Timer.Tick += MainForm_Tick;
        ListViewItem = new ListViewItem(TimerName);
        ListViewItem.SubItems.Add(TimeSpan.FromSeconds(countdownSeconds).ToString(@"mm\:ss"));
    }

    private void MainForm_Tick(object sender, EventArgs e)
    {
        // Empty method, the actual countdown logic is in the MainForm's CountdownTimer_Tick method
    }
}
}