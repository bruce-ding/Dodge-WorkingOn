using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;

namespace DodgeXaml.CommonHelper
{
    public class SQLiteHelper
    {
        private static SQLiteConnection conn = null;

        private static async Task<string> CreateOrOpenSQLiteFile()
        {
            string databasePath = string.Empty;
            var root = Windows.Storage.ApplicationData.Current.LocalFolder;
            var path = await root.CreateFolderAsync("sql", CreationCollisionOption.OpenIfExists);
            StorageFile file = null;
            try
            {
                file = await path.CreateFileAsync("Database.wdb", CreationCollisionOption.OpenIfExists);
                databasePath = file.Path;
            }
            catch (Exception e)
            {
                throw new Exception("Fail to create or open SQLite file!");
            }

            return databasePath;

        }

        // 连接数据库和创建表T
        private static async void ConnectAndCreateTable<T>()
        {

            //数据库文件保存的位置
            string dbPath = await CreateOrOpenSQLiteFile();

            //打开创建数据库和创建表
            using (conn = new SQLiteConnection(dbPath))
            {
                //创建表
                conn.CreateTable<T>();

            }

        }

        public static void InsertSingle<T>(T type)
        {
            conn.Insert(type);
        }
        //多条插入集合
        public static void InsertAll<T>(ObservableCollection<T> collection)
        {
            conn.InsertAll(collection);
        }

        public static void Update(string cmd)
        {
            //更新语句
            conn.CreateCommand(cmd).ExecuteNonQuery();
        }

        public static void DeleteAll<T>()
        {

            conn.DeleteAll<T>();
        }

        public static List<object> Query<T>(string cmd)
        {

            List<object> list = conn.Query(new TableMapping(typeof(T)), cmd);
            return list;
        }
    }
}
