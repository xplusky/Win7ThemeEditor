using System;
using System.Net;
using System.Net.Mail;
using System.Windows;

namespace Win7ThemeEditor.Helper
{
    /// <summary>
    /// ErrorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ErrorWindow
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow(string message)
        {
            InitializeComponent();
            ErrorTextbox.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SendFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            var mailMessage = new MailMessage
            {
                Body = "联系方式： " + InfoTextbox.Text + Environment.NewLine + SystemInfo.SystemName() + "  " + SystemInfo.OsBit() + "  " + SystemInfo.SystemVersion() + Environment.NewLine + ErrorTextbox.Text, 
                From = new MailAddress("skysgod@126.com"), 
                Subject = "Win7主题生成器" + App.AppVersion + "反馈"
            };
            mailMessage.To.Add("plusky@126.com");

            var client =
                new SmtpClient
                {
                    Host = "smtp.126.com",
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("skysgod", "feedback"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 6000
                };
            client.SendCompleted +=
                (o, ev) =>
                {
                    if (ev.Error == null)
                        MessageBox.Show("发送成功,谢谢你的反馈，我会尽快处理的~~");
                    else
                    {
                        MessageBox.Show(ev.Error.ToString());
                        SendFeedbackButton.IsEnabled = true;
                    }
                };
            client.SendAsync(mailMessage, null);
            SendFeedbackButton.IsEnabled = false;
        }
    }
}
