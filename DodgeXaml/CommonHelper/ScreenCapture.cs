using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace DodgeXaml.CommonHelper
{
    public class ScreenCapture
    {
        public static async void Capture(UIElement element)
        {

            MessageDialog msgDialog = new MessageDialog("此方法只能在Windows8.1及以上版本中实现", "警告");

            msgDialog.ShowAsync();


            //    var bitmap = new RenderTargetBitmap();
            //    await bitmap.RenderAsync(element);
            //    IBuffer buffer = await bitmap.GetPixelsAsync();
            //    var stream = buffer.AsStream();

            //    FileSavePicker savePicker = new FileSavePicker();
            //    savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
            //    // Dropdown of file types the user can save the file as
            //    savePicker.FileTypeChoices.Add("Bitmap", new List<string>() { ".png" });
            //    // Default file name if the user does not type one in or select a file to replace
            //    savePicker.SuggestedFileName = "New Bitmap";

            //    // Windows.Graphics.Imaging.BitmapDecoder decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, stream.AsRandomAccessStream());


            //    //StorageFile file = await savePicker.PickSaveFileAsync();
            //    //if (file != null)
            //    //{
            //    //    // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
            //    //    CachedFileManager.DeferUpdates(file);
            //    //    // write to file
            //    //    await FileIO.WriteBufferAsync(file, buffer);
            //    //    // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
            //    //    // Completing updates may require Windows to ask for user input.
            //    //    await CachedFileManager.CompleteUpdatesAsync(file);

            //    //}
            //    StorageFile savedItem = await savePicker.PickSaveFileAsync();

            //    try
            //    {

            //        Guid encoderId = BitmapEncoder.PngEncoderId;
            //        IRandomAccessStream fileStream = await savedItem.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

            //        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, fileStream);

            //        Stream pixelStream = stream; //src.PixelBuffer.AsStream();

            //        byte[] pixels = new byte[pixelStream.Length];

            //        pixelStream.Read(pixels, 0, pixels.Length);

            //        //pixal format shouldconvert to rgba8

            //        for (int i = 0; i < pixels.Length; i += 4)
            //        {

            //            byte temp = pixels[i];

            //            pixels[i] = pixels[i + 2];

            //            pixels[i + 2] = temp;

            //        }

            //        encoder.SetPixelData(

            //         BitmapPixelFormat.Rgba8,

            //         BitmapAlphaMode.Straight,

            //         (uint)bitmap.PixelWidth,//src.PixelWidth,

            //         (uint)bitmap.PixelHeight,

            //         96, // Horizontal DPI

            //         96, // Vertical DPI

            //         pixels

            //         );

            //        await encoder.FlushAsync();

            //    }

            //    catch (Exception ex)
            //    {

            //        throw ex;

            //    }


        }
    }
}
