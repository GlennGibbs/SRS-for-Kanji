using System;
using System.Windows;
using System.IO;
using Path = System.IO.Path;

namespace KanjiSRS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int index { get; set; } = 0;
        private ItemDb item = new ItemDb("URI=file:" + Path.Combine(Directory.GetCurrentDirectory(), @"SRS_Items.db"));
        private int reviewOffset { get; set; } = 0;

        public MainWindow()
        { 
            InitializeComponent();
            checkKanji();
        }
        private void checkKanji()
        {
            if (newKanji() || reviewKanji())
            {
                Data data = item.ReadRow(index);
                kanjiLabel.Content = data.kanji;
                answerText.Text = "";
                displayButton.IsEnabled = true;
                Next.IsEnabled = false;
                Again.IsEnabled = false;
            }
            else
            {
                answerText.Text = "You have finished your new and old kanji reviews for today.";
                kanjiLabel.Content = "";
                displayButton.IsEnabled = false;
                Next.IsEnabled = false;
                Again.IsEnabled = false;
            }
        }
        private void displayButton_Click(object sender, RoutedEventArgs e)
        {
            Data data = item.ReadRow(index);
            answerText.Text = data.ans;
            displayButton.IsEnabled = false;
            Next.IsEnabled = true;
            Again.IsEnabled = true;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            superMemo(5);
            checkKanji();
        }

        private void againButton_Click(object sender, RoutedEventArgs e)
        {
            superMemo(1);
            checkKanji();
        }
        private bool reviewKanji()
        {
            for (; item.RowCount(false, reviewOffset) > 0; reviewOffset++)
            {
                Data data = item.ReadRow(0, "SELECT * FROM Items WHERE LastDate != \"false\" LIMIT 1 OFFSET @offset", reviewOffset);
                DateTime date = DateTime.Parse(data.date);
                if (date.AddDays(data.interval) <= DateTime.Today)
                {
                    index = data.index;
                    return true;
                }
            }
            return false;
        }
        private bool newKanji()
        {
            if(item.RowCount(true) < 30)
            {
                Data data = item.ReadRow(0, "SELECT * FROM Items WHERE Learnt = \"false\" LIMIT 1");
                index = data.index;
                return true;
            }
            
            return false;
        }
        private void superMemo(int grade)
        {
            Data itemData = item.ReadRow(index);
            double newEasiness = itemData.easiness + (0.1 - (5 - grade) * (0.08 + (5 - grade) * 0.02));
            itemData.easiness = newEasiness > 1.3 ? newEasiness : 1.3;
            if (grade == 5)
            {
                itemData.repetition += 1;
                itemData.interval = (int)Math.Ceiling(itemData.repetition * itemData.easiness);
            }
            else
            {
                itemData.repetition = 1;
                itemData.interval = 1;
            }
            itemData.date = DateTime.Today.ToString().Replace(" 00:00:00", "");
            if (itemData.learnt.Equals("false"))
            {
                itemData.learnt = DateTime.Today.ToString().Replace(" 00:00:00", "");
            }
            item.InsertAnswer(itemData.kanji, "UPDATE Items SET Repetition = @repetition, Easiness = @easiness," +
                " Interval = @interval, LastDate = @date, Learnt = @learnt WHERE Kanji = @kanji",
                itemData.repetition, itemData.easiness, itemData.interval, "", itemData.date, itemData.learnt);
        }
    }
}
