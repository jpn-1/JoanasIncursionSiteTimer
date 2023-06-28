using System;
using System.Windows.Forms;

namespace JoanasIncursionSiteTimer.models
{
    public class TimerItem
    {
        public string timerName { get; }
        public int countdownSeconds { get; set; }
        public System.Windows.Forms.Timer timer { get; set; }
        public ListViewItem listViewItem { get; set; }

        public ListView listView { get; set; }

        public TimerItem(string _timerName, int _countdownSeconds, ListView _listView)
        {
            timerName = _timerName;
            countdownSeconds = _countdownSeconds;
            listView = _listView;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000; // 1 second
            timer.Tick += CountdownTimer_Tick;
            listViewItem = new ListViewItem(timerName);
            listViewItem.SubItems.Add(TimeSpan.FromSeconds(countdownSeconds).ToString(@"mm\:ss"));
            // Create a delegate that represents the code to be invoked on the UI thread
            // This really feels clunky and can probably be done better!
            Action<ListView, ListViewItem> addItemDelegate = new Action<ListView, ListViewItem>(AddItemToListView);
            listView.Invoke(addItemDelegate, listView, listViewItem);

        }

        public void StartTimer()
        {
            timer.Start();
        }

        // The method to add an item to the ListView
        private void AddItemToListView(ListView listView, ListViewItem item)
        {
   

            // Add the new item to the ListView
            listView.Items.Add(item);
        }



        // The method to check ListView items
        private void CheckListViewItems(ListView listView)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (item == listViewItem)
                {
                    item.Text = TimeSpan.FromSeconds(countdownSeconds).ToString(@"mm\:ss");

                }


            }
        }


        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            if (countdownSeconds > 0)
            {
                countdownSeconds--;
                // Create a delegate that represents the code to be invoked on the UI thread
                Action<ListView> checkItemsDelegate = new Action<ListView>(CheckListViewItems);

                // Update the corresponding ListViewItem with the new countdown value
                listViewItem.ListView.Invoke(checkItemsDelegate, listViewItem.ListView);

            }
            else
            {

                timer.Stop();
                timer.Dispose();
                listViewItem.ListView.Invoke(new Action(
              () =>
              {
                  listViewItem.Remove();
              }
              ));


            }
        }

    }
}