using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace DodgeXaml.CommonHelper
{
     class XamlHelper
    {
        public static void MakeToast(string text0, string text1, string imgInfo)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(text0));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(text1));

            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");

            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", imgInfo);
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "ms-appx:///Assets/smallLogo.png");

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.Default");
            toastNode.AppendChild(audio);

            //((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
