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


        public MainWindow()
        {
            InitializeComponent();
            string kanjiAns = getNext("");
            kanjiLabel.Content = kanjiAns.Split(';')[0];
            displayButton.IsEnabled = true;
            Next.IsEnabled = false;
            Again.IsEnabled = false;
        }


        public string getNext(string ansType)
        {
            string stm = string.Format("SELECT * FROM Items LIMIT 1 OFFSET {0}", index);
            Item item = new Item();
            KeyValuePair<string, string> kanjiAns = item.ReadData(stm);
            if(ansType == "k")
            {
                return kanjiAns.Key;
            }
            else if (ansType == "a")
            {
                return kanjiAns.Value;
            }
            else
            {
                return string.Format("{0};{1}", kanjiAns.Key, kanjiAns.Value);
            }
        }

        private void displayButton_Click(object sender, RoutedEventArgs e)
        {
            string ans = getNext("a");
            answerText.Text = ans;
            displayButton.IsEnabled = false;
            Next.IsEnabled = true;
            Again.IsEnabled = true;
            index++;
            //superMemo();
            //answerText.Text = kanji.Substring(1,1);
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            string kanji = getNext("k");
            kanjiLabel.Content = kanji;
            displayButton.IsEnabled = true;
            Next.IsEnabled = false;
            Again.IsEnabled = false;
            answerText.Text = "";
        }

        private void againButton_Click(object sender, RoutedEventArgs e)
        {
            index++;
            
        }

        private void superMemo()
        {
            int grade = 0;
            Item item = new Item();
            item.InsertData();


            //item.easiness += 0.1 - (5 - grade) * (0.08 + (5 - grade) * 0.02);
        }
    }

    public partial class Item
    {
        public int rep { get; set; } = 0;
        public double easiness { get; set; } = 0;
        public int interval { get; set; } = 0;
        public int grade { get; set; } = 0;

        public SQLiteDb db { get; set; } = new SQLiteDb(@"URI=file:C:\Users\Glenn\source\repos\First\SRS_items.db");

        public Item()
        {
            
        }
        public KeyValuePair<string, string> ReadData(string stm)
        {
            return db.ReadData(stm);
        }

        public void InsertAnswer(string kanji, string ans)
        {
            db.InsertAnswer(kanji, ans);
        }
        public void InsertData()
        {
            string kanjiAns = System.IO.File.ReadAllText(@"C:\Users\Glenn\source\repos\First\kanji answer.txt");
            string[] kanjiList = kanjiAns.Split('\n');
            foreach (var item in kanjiList)
            { 
                if(item.Length == 0)
                {
                    continue;
                }
                string[] pair = item.Trim().Split(';');
                db.InsertData(pair[0],pair[1]);
            }            
        }
    }
}
