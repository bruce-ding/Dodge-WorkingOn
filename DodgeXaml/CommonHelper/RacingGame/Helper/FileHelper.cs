#region File Description
//-----------------------------------------------------------------------------
// FileHelper.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using directives

using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace DodgeXaml.CommonHelper.RacingGameHelper
{
    /// <summary>
    /// File helper class to get text lines, number of text lines, etc.
    /// Update: Now also supports the XNA Storage classes :)
    /// </summary>
    public static partial class FileHelper
    {
        private static async Task<Stream> OpenStreamAsync(string name)
        {
            Package package = Package.Current;
            Stream stream;
            try
            {
                StorageFile storageFile = await package.InstalledLocation.GetFileAsync(name);
                IRandomAccessStreamWithContentType randomAccessStream = await storageFile.OpenReadAsync();
                stream = WindowsRuntimeStreamExtensions.AsStreamForRead((IInputStream)randomAccessStream);
            }
            catch (IOException ex)
            {
                stream = (Stream)null;
            }
            return stream;
        }

        #region LoadGameContentFile
        /// <summary>
        /// Load game content file, returns null if file was not found.
        /// </summary>
        /// <param name="relativeFilename">Relative filename</param>
        /// <returns>File stream</returns>
        /// 在Windows8上的位置是程序安装根目录
        public static Stream LoadGameContentFile(string relativeFilename)
        {
            return TitleContainer.OpenStream(relativeFilename);
        }
        #endregion

        #region StorageDevice

        public static ManualResetEvent StorageContainerMRE = new ManualResetEvent(true);


        /// <summary>
        /// XNA user device, asks for the saving location on the Xbox360,
        /// theirfore remember this device for the time we run the game.
        /// </summary>
        static StorageDevice xnaUserDevice = null;

        /// <summary>
        /// Xna user device
        /// </summary>
        /// <returns>Storage device</returns>
        public static StorageDevice XnaUserDevice
        {
            get
            {
                if ((xnaUserDevice != null) && !xnaUserDevice.IsConnected)
                {
                    xnaUserDevice = null;
                }
                // Create if not created yet.
                if (xnaUserDevice == null)
                {
                    // 确定用户界面屏幕是否处于活动状态。
                    //if (Guide.IsVisible)
                    //{
                    //    return null;
                    //}
                    IAsyncResult async = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);

                    async.AsyncWaitHandle.WaitOne();

                    xnaUserDevice = StorageDevice.EndShowSelector(async);

                }
                return xnaUserDevice;
            }
        }

        #endregion

        #region Get text lines
        /// <summary>
        /// Returns the number of text lines we got in a file.
        /// </summary>
        static public string[] GetLines(string filename)
        {
            try
            {
#if NETFX_CORE
                Stream result =
                //Task.Run<Stream>((Func<Stream>) (() => OpenStreamAsync("1.txt").Result)).Result;
                Task.Run<Stream>(() => OpenStreamAsync(filename)).Result;

                StreamReader reader = new StreamReader(result,
                    System.Text.Encoding.UTF8);
#else
                StreamReader reader = new StreamReader(
                    new FileStream(filename, FileMode.Open, FileAccess.Read),
                    System.Text.Encoding.UTF8);
#endif
                // Generic version
                List<string> lines = new List<string>();

                //do
                //{
                //    lines.Add(reader.ReadLine());
                //} while (reader.Peek() > -1);
                // 上面的写法遇到所读文件为空的时候会抛出异常。
                // 在此使用do while这种句型确实有值得商榷的地方：
                // 当所读文件为空时，reader.ReadLine()的返回值为null。
                // 而这个没有被初始化的string对象就这样被添加到lines中，
                // 又返回到调用GetLines函数的地方。带来一些潜在的危险。
                // 还是采用MSDN中的写法比较妥当。
                while (reader.Peek() >= 0)
                {
                    lines.Add(reader.ReadLine());
                }

                reader.Dispose();
                return lines.ToArray();
            }
            catch (FileNotFoundException)
            {
                // Failed to read, just return null!
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }
        #endregion

        #region Write Helpers
        /// <summary>
        /// Write vector3 to stream
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="vec">Vector3</param>
        public static void WriteVector3(BinaryWriter writer, Vector3 vec)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }

        /// <summary>
        /// Write vector4 to stream
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="vec">Vector4</param>
        public static void WriteVector4(BinaryWriter writer, Vector4 vec)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
            writer.Write(vec.W);
        }

        /// <summary>
        /// Write matrix to stream
        /// </summary>
        /// <param name="writer">Writer</param>
        /// <param name="matrix">Matrix</param>
        public static void WriteMatrix(BinaryWriter writer, Matrix matrix)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(matrix.M11);
            writer.Write(matrix.M12);
            writer.Write(matrix.M13);
            writer.Write(matrix.M14);
            writer.Write(matrix.M21);
            writer.Write(matrix.M22);
            writer.Write(matrix.M23);
            writer.Write(matrix.M24);
            writer.Write(matrix.M31);
            writer.Write(matrix.M32);
            writer.Write(matrix.M33);
            writer.Write(matrix.M34);
            writer.Write(matrix.M41);
            writer.Write(matrix.M42);
            writer.Write(matrix.M43);
            writer.Write(matrix.M44);
        }
        #endregion

        #region Read Helpers
        /// <summary>
        /// Read vector3 from stream
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Vector3</returns>
        public static Vector3 ReadVector3(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }

        /// <summary>
        /// Read vector4 from stream
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Vector4</returns>
        public static Vector4 ReadVector4(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Vector4(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }

        /// <summary>
        /// Read matrix from stream
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Matrix</returns>
        public static Matrix ReadMatrix(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Matrix(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }
        #endregion
    }
}
