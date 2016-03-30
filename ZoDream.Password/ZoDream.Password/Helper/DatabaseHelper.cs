using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Password.Helper.Database;
using ZoDream.Password.Model;

namespace ZoDream.Password.Helper
{
    public class DatabaseHelper
    {
        public static string Password;

        public static void ChooseSqlite()
        {
            string dir;
            switch (IntPtr.Size)
            {
                case 8:
                    dir = "x64";
                    break;
                case 4:
                default:
                    dir = "x86";
                    break;
            }
            var files = LocalHelper.GetAllFile(AppDomain.CurrentDomain.BaseDirectory + "\\" + dir);
            foreach (var file in files)
            {
                File.Copy(file, file.Replace($"\\{dir}", ""), true);
            }
        }

        public static void Init()
        {
            Create<PasswordItem>(
                "Id INTEGER PRIMARY KEY AUTOINCREMENT,Url VARCHAR(255), Name VARCHAR(255),Email VARCHAR(255), Number VARCHAR(255), Password VARCHAR(255), Mark Text");
        }

        public static void SetIni(string file, string password = null)
        {
            SqLiteHelper.ConnectionString = $"Data Source={file};Pooling=true;FailIfMissing=false;Password={password}";
        }

        public static SQLiteConnection Open()
        {
            var file = AppDomain.CurrentDomain.BaseDirectory + @"\ZoDream.db";
            var exists = File.Exists(file);
            SetIni(file, Password);
            SqLiteHelper.Conn.Open();
            if (!exists)
            {
                SqLiteHelper.Conn.ChangePassword(Password);
                Init();
            }
            return SqLiteHelper.Conn;
        }

        public static void Close()
        {
            SqLiteHelper.Conn.Close();
        }

        public static int Create<T>(string columns)
        {
            var table = typeof (T).Name;
            return SqLiteHelper.CreateCommand($"CREATE TABLE IF NOT EXISTS {table} ({columns});").ExecuteNonQuery();
        }

        /// <summary>
        /// 插入语句
        /// </summary>
        /// <typeparam name="T">表名</typeparam>
        /// <param name="columns">插入的列</param>
        /// <param name="tags">标签或值</param>
        /// <param name="parameters">标签对应的值</param>
        /// <returns></returns>
        public static int Insert<T>(string columns, string tags, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"INSERT INTO {table} ({columns}) VALUES ({tags});", parameters).ExecuteNonQuery();
        }

        /// <summary>
        /// 返回插入的自增id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int InsertId<T>(string columns, string tags, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return Convert.ToInt32(SqLiteHelper.CreateCommand($"INSERT INTO {table} ({columns}) VALUES ({tags});select last_insert_rowid();", parameters).ExecuteScalar());
        }

        public static int Insert<T>(string tags, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"INSERT INTO {table} VALUES ({tags});", parameters).ExecuteNonQuery();
        }

        /// <summary>
        /// 插入时，某条记录不存在则插入，存在则更新 id 会改变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int Replace<T>(string columns, string tags, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"REPLACE INTO {table} ({columns}) VALUES ({tags});", parameters).ExecuteNonQuery();
        }
        /// <summary>
        /// 插入 如果存在忽略
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int InsertOrIgnore<T>(string columns, string tags, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"INSERT OR IGNORE INTO {table} ({columns}) VALUES ({tags});", parameters).ExecuteNonQuery();
        }
        /// <summary>
        /// 返回自增id 0 表示失败
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <param name="tags"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int InsertOrIgnoreId<T>(string columns, string tags, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return Convert.ToInt32(SqLiteHelper.CreateCommand($"INSERT OR IGNORE INTO {table} ({columns}) VALUES ({tags});select last_insert_rowid();", parameters).ExecuteScalar());
        }

        public static int Update<T>(string sql, string where, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"UPDATE {table} SET {sql} WHERE {where};", parameters).ExecuteNonQuery();
        }

        public static int Delete<T>(string where)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"DELETE FROM {table} WHERE {where};").ExecuteNonQuery();
        }

        public static SQLiteDataReader Select<T>(string feild = "*", string sql = "", params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"SELECT {feild} FROM {table} {sql};", parameters).ExecuteReader();
        }

        /// <summary>
        /// 获取第一行第一列的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="feild"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Find<T>(string feild, string where, params SQLiteParameter[] parameters)
        {
            var table = typeof(T).Name;
            return SqLiteHelper.CreateCommand($"SELECT {feild} FROM {table} WHERE {where};", parameters).ExecuteScalar();
        }
    }
}
