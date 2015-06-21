using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DodgeXaml.CommonHelper
{
    partial class FileHelper
    {

        #region 保存/读取基本类型数据
        public static void SaveData(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        public static object GetData(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return ApplicationData.Current.LocalSettings.Values[key];
            }
            else
            {
                return 0;
            }
        }


        public static void RemoveData(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                ApplicationData.Current.LocalSettings.Values.Remove(key);
            }
        }
        #endregion  

        /// <summary>
        /// Opens a file for reading.  Will check for and 
        /// return a null stream if the file does not exist.
        /// </summary>
        static public Stream ReadFile(string path)
        {
            

#if WINDOWS_PHONE
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.FileExists(path))
                    return null;

                var stream = store.OpenFile(path, FileMode.Open);
                return stream;
            }
#elif NETFX_CORE

            var stream = Task.Run(
                async () =>
                {

                    try
                    {
                        var storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(path);
                        return await storageFile.OpenStreamForReadAsync();
                    }
                    catch (IOException)
                    {
                        return null;
                    }

                }).Result;

            return stream;

#else
            if (!File.Exists(path))
                return null;

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return stream;
#endif
        }
    }
}
