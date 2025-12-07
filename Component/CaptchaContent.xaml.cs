using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using Microsoft.UI.Xaml;
using OpenNEL.Manager;

namespace OpenNEL_WinUI
{
    public sealed partial class CaptchaContent : UserControl
    {
        private string _sessionId;

        public CaptchaContent()
        {
            this.InitializeComponent();
            try
            {
                var mode = SettingManager.Instance.Get().ThemeMode?.Trim().ToLowerInvariant() ?? "system";
                ElementTheme t = ElementTheme.Default;
                if (mode == "light") t = ElementTheme.Light;
                else if (mode == "dark") t = ElementTheme.Dark;
                this.RequestedTheme = t;
            }
            catch { }
        }

        public string CaptchaText => CaptchaInput.Text;

        public string SessionId => _sessionId;

        public void SetCaptcha(string sessionId, string captchaUrl)
        {
            _sessionId = sessionId ?? string.Empty;
            CaptchaInput.Text = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(captchaUrl))
                {
                    CaptchaImage.Source = new BitmapImage(new Uri(captchaUrl));
                }
            }
            catch
            {
            }
        }
    }
}
