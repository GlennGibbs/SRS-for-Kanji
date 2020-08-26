using System.Data.SQLite;
using System;


namespace KanjiSRS
{
    public class ItemDb
    {
        public string _path { get; set; } 
        public ItemDb(string path)
        {
            _path = path;
            Connect();
        }

        public void Connect()
        {
            using (SQLiteConnection db = new SQLiteConnection(_path))
            {
                try
                {
                    db.Open();
                    db.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("no connection");
                }
            }
        }
        public int RowCount (bool revOrLearn, int offset = 0)
        {
            int rowCount = 0;
            using (SQLiteConnection db = new SQLiteConnection(_path))
            {
                db.Open();
                using (SQLiteCommand sqlite_cmd = db.CreateCommand())
                {

                    if (revOrLearn == true)
                    {
                        sqlite_cmd.CommandText = "SELECT * FROM Items WHERE Learnt != \"false\" ORDER BY Learnt DESC";
                    }
                    else
                    {
                        sqlite_cmd.CommandText = "SELECT * FROM Items WHERE LastDate != \"false\" LIMIT 1 OFFSET @offset";
                        sqlite_cmd.Parameters.AddWithValue("offset", offset);
                    }
                    using (SQLiteDataReader dataReader = sqlite_cmd.ExecuteReader())
                    { 

                        if(dataReader != null && dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                if (revOrLearn == true  && dataReader.GetString(7).Equals(DateTime.Today.ToString().Replace(" 00:00:00", "")))
                                {
                                    rowCount++;
                                }
                                else if(revOrLearn == false)
                                {
                                    rowCount++;
                                }
                            }
                        }
                        return rowCount;
                    }
                }
            }
        }

        public Data ReadRow(int index, string stm = "", int offset = -1)
        {
            using (SQLiteConnection db = new SQLiteConnection(_path))
            {
                db.Open();
                using (SQLiteCommand sqlite_cmd = db.CreateCommand())
                {
                    if (stm == "")
                    {
                        sqlite_cmd.CommandText = "SELECT * FROM Items WHERE Id = @index";
                        sqlite_cmd.Parameters.AddWithValue("index", index);
                    }
                    else if (stm != "" && offset != -1)
                    {
                        sqlite_cmd.CommandText = stm;
                        sqlite_cmd.Parameters.AddWithValue("offset", offset);
                    }
                    else
                    {
                        sqlite_cmd.CommandText = stm;
                    }
                    using (SQLiteDataReader dataReader = sqlite_cmd.ExecuteReader())
                    {
                        dataReader.Read();
                        Data data = new Data(dataReader.GetInt32(0), dataReader.GetString(1), dataReader.GetInt32(2), dataReader.GetDouble(3),
                            dataReader.GetInt32(4), dataReader.GetString(5), dataReader.GetString(6), dataReader.GetString(7));
                        return data;
                    }
                }
            }
        }

        public void InsertAnswer(string kanji, string stm, int rep = 0, double easiness = 0.0, int interval = 0, string ans = "", string date = "", string learnt = "")
        {
            using (SQLiteConnection db = new SQLiteConnection(_path))
            {
                db.Open();
                using (SQLiteCommand sqlite_cmd = db.CreateCommand())
                {
                    sqlite_cmd.CommandText = stm;
                    if (stm.Contains("@ans"))
                    {
                        sqlite_cmd.Parameters.AddWithValue("@ans", ans);
                    }
                    if (stm.Contains("@repetition")){
                        sqlite_cmd.Parameters.AddWithValue("@repetition", rep);
                    }
                    if(stm.Contains("@easiness"))
                    {
                        sqlite_cmd.Parameters.AddWithValue("@easiness", easiness);
                    }
                    if(stm.Contains("@interval"))
                    {
                        sqlite_cmd.Parameters.AddWithValue("@interval", interval);
                    }
                    if (stm.Contains("@date"))
                    {
                        sqlite_cmd.Parameters.AddWithValue("@date", date);
                    }
                    if (stm.Contains("@learnt"))
                    {
                        sqlite_cmd.Parameters.AddWithValue("@learnt", learnt);
                    }
                    sqlite_cmd.Parameters.AddWithValue("@kanji", kanji);
                    sqlite_cmd.ExecuteNonQuery();
                }
            }

        }

    }

    public struct Data
    {
        public Data (int id, string K, int R, double E, int I, string A, string D, string L)
        {
            index = id;
            kanji = K;
            ans = A;
            repetition = R;
            easiness = E;
            interval = I;
            date = D;
            learnt = L;
        }

        public int index { get; }
        public string kanji { get; set; }
        public string ans { get; set; }
        public int repetition { get; set; }
        public double easiness { get; set; }
        public int interval { get; set; }
        public string date { get; set; }
        public string learnt { get; set; }
    }
    
}
