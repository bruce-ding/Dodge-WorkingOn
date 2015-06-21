using DodgeXaml.CommonHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace DodgeXaml
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StorePage : Page
    {
        private LicenseInformation licenseInfo;
        private ListingInformation listingInfo;
        public StorePage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            await GetLicenseInfo();
        }

        private void BtnBack_OnClick(object sender, RoutedEventArgs e)
        {
            Window.Current.Content = StartPage.Current;
        }

        private async void ImgLighting_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
//            string receipt = "";
//#if DEBUG
//            receipt = await CurrentAppSimulator.RequestProductPurchaseAsync("product1", true);//第一个参数是产品ID 第二个是是否返回发票（一些信息） 一般为false
//#else
//                    await CurrentApp.RequestProductPurchaseAsync("product1", false);
//#endif
            //应用内购买功能
//            if (licenseInfo.ProductLicenses.ContainsKey("product1"))
//            {
//                var product1 = licenseInfo.ProductLicenses["product1"]; //取product1的信息 
//                if (!product1.IsActive)
//                {
//                    MessageDialog dlg = new MessageDialog("还没买呢？");
//                    dlg.ShowAsync();
//                    tb_Info.Text = "购买产品中";
//                    try
//                    {

//                        string receipt = "";
//#if DEBUG
//                        receipt = await CurrentAppSimulator.RequestProductPurchaseAsync("product1", true);//第一个参数是产品ID 第二个是是否返回发票 一般为false
//#else
//                    await CurrentApp.RequestProductPurchaseAsync("product1", false);
//#endif
//                        if (licenseInfo.ProductLicenses["product1"].IsActive)
//                        {
//                            tb_Info.Text = "购买成功!";
//                            Debug.WriteLine("打印发票:" + receipt);
//                        }

//                    }
//                    catch (Exception)
//                    {
//                        tb_Info.Text = "没有购买成功!";
//                    }
//                }
//                else
//                {
//                    tb_Info.Text = "您已经购买了该功能!";
//                }
//            }

            
        }

        private async System.Threading.Tasks.Task GetLicenseInfo()
        {

#if DEBUG
            //如果是调试状态则调用模拟器否则就xxoo
            licenseInfo = CurrentAppSimulator.LicenseInformation;
#else
            licenseInfo = CurrentApp.LicenseInformation;
#endif
#if DEBUG
            listingInfo = await CurrentAppSimulator.LoadListingInformationAsync();
#else
            listingInfo = await CurrentApp.LoadListingInformationAsync();
#endif

            appInfo.Text = "当前应用程序状态:";
            appInfo.Text += (licenseInfo.IsActive == true ? "已激活," : "未激活,");//概要信息 能得到当前应用是否激活
            appInfo.Text += (licenseInfo.IsTrial == true ? "试用期." : "非试用期.");//概要信息 得到当前应用是否处于试用期


            var product1 = listingInfo.ProductListings["product1"];//详细信息  取product1的信息 这个配置在商店里面'高级功能'配置

            productInfo.Text = "内购产品:";
            productInfo.Text += "\"" + product1.Name + "\"";
            productInfo.Text += "价格:" + product1.FormattedPrice;//10块钱你买不了吃亏，10块钱你买不了上当
            productInfo.Text += "状态:" + (licenseInfo.ProductLicenses["product1"].IsActive == true ? "已购买" : "未购买");
        }

        private void BtnTest_OnClick(object sender, RoutedEventArgs e)
        {

            XamlHelper.MakeToast("测试", "这是消息弹出框测试！", "ms-appx:///Assets/rain.jpg");

           
        }
    }
}
