using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace First
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int index { get; set; } = 0;
        private ItemDb item = new ItemDb(@"URI=file:C:\Users\Glenn\Documents\GitHub\C# repos\SRS-for-Kanji\SRS_items.db");

        public MainWindow()
        {
            InitializeComponent();
            if(!kanjiCount())
            {
                Data data = newKanji();
                kanjiLabel.Content = data.kanji;
                displayButton.IsEnabled = true;
                Next.IsEnabled = false;
                Again.IsEnabled = false;
            }
            else
            {
                // Review kanji instead.
            }
        }

        public bool kanjiCount()
        {
            if(item.LearntCount(true) >= 30)
            {
                answerText.Text = "You have learnt all the new kanji today.";
                kanjiLabel.Content = "";
                displayButton.IsEnabled = false;
                Next.IsEnabled = false;
                Again.IsEnabled = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        public Data getNext()
        {
            Data itemData = item.ReadRow(index);
            return itemData;
        }

        private void displayButton_Click(object sender, RoutedEventArgs e)
        {
            Data data = getNext();
            answerText.Text = data.ans;
            displayButton.IsEnabled = false;
            Next.IsEnabled = true;
            Again.IsEnabled = true;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            displayButton.IsEnabled = true;
            Next.IsEnabled = false;
            Again.IsEnabled = false;
            answerText.Text = "";
            superMemo(5);
            index++;
            if (!kanjiCount())
            {
                Data data = getNext();
                kanjiLabel.Content = data.kanji;
            }
        }

        private void againButton_Click(object sender, RoutedEventArgs e)
        {
            displayButton.IsEnabled = true;
            Next.IsEnabled = false;
            Again.IsEnabled = false;
            answerText.Text = "";
            superMemo(1);
            index++;
            if (!kanjiCount())
            {
                Data data = getNext();
                kanjiLabel.Content = data.kanji;
            }
        }
        private void reviewKanji()
        {

        }
        private Data newKanji()
        {
            Data data = item.ReadRow(0, "SELECT * FROM Items WHERE Learnt = \"false\" LIMIT 1");
            index = data.index;
            return data;
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
