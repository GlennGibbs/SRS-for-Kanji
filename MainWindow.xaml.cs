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
        private string kanji = System.IO.File.ReadAllText(@"C:\Users\Glenn\source\repos\First\test kanji.txt");
        private int index { get; set; } = 0;



        public MainWindow()
        {
            InitializeComponent();
        }



        private void displayButton_Click(object sender, RoutedEventArgs e)
        {
            string stm = string.Format("SELECT * FROM Items LIMIT 1 OFFSET {0}", index);
            Item item = new Item();
            kanjiLabel.Content = item.ReadData(stm);
            index++;
            //superMemo();
            //textBox1.Text = kanji.Substring(1,1);
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            string[] kanji2 = kanji.Split('\n');
            index++;
            string[] ans = kanji2[0].Split(' ');
            string kan = ans[0];
            string answ = ans[1];
            kanjiLabel.Content = kan;
            textBox1.Text = answ;
        }

        private void againButton_Click(object sender, RoutedEventArgs e)
        {
            index++;
            textBox1.Text = char.ToString(kanji[index]);
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
        public string ReadData(string stm)
        {
            return db.ReadData(stm);
        }
        public void InsertData()
        {
            string kanji = System.IO.File.ReadAllText(@"C:\Users\Glenn\source\repos\First\wanikani sorted kanji.txt");
            foreach (var item in kanji)
            {
                db.InsertData(char.ToString(item));
            }            
        }
    }
}
