using CefSharp.WinForms;
using CefSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpBrowser
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            var args = Environment.GetCommandLineArgs();
            string url = "www.google.com";
            string? proxy = null;
            if (args.Length > 1)
            {
                url = args[1];
                if (args.Length > 2)
                    proxy = args[2];
            }
            InitBrowser(url, proxy);
            _ = SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE);
        }

        private readonly TitleBarForm TitleBarFormInstance = new();

        private void Form1_Load(object sender, EventArgs e)
        {
            _ = SetWindowDisplayAffinity(TitleBarFormInstance.Handle, WDA_EXCLUDEFROMCAPTURE);
            TitleBarFormInstance.Show();
            TitleBarFormInstance.Visible = false;
            TitleBarFormInstance.Owner = this;
            MoveTitleBarForm();
            TitleBarFormInstance.checkBoxH.CheckedChanged += CheckBoxH_CheckedChanged;
            TitleBarFormInstance.checkBoxB.CheckedChanged += CheckBoxB_CheckedChanged;
            TitleBarFormInstance.checkBoxT.CheckedChanged += CheckBoxT_CheckedChanged;
        }

        private void MoveTitleBarForm()
        {
            TitleBarFormInstance.Location = new Point(this.Location.X + this.Size.Width - TitleBarFormInstance.Size.Width, this.Location.Y + 5);
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            MoveTitleBarForm();
        }

        private void CheckBoxH_CheckedChanged(object? sender, EventArgs e)
        {
            if (((CheckBox)sender!).Checked)
            {
                _ = SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE);
            }
            else
            {
                _ = SetWindowDisplayAffinity(this.Handle, WDA_NONE);
            }
        }

        private void CheckBoxB_CheckedChanged(object? sender, EventArgs e)
        {
            this.ShowInTaskbar = !((CheckBox)sender!).Checked;
            CheckBoxH_CheckedChanged(sender, e);
        }

        private void CheckBoxT_CheckedChanged(object? sender, EventArgs e)
        {
            this.TopMost = ((CheckBox)sender!).Checked;
        }

        [DllImport("user32.dll")]
        static extern uint SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        const uint WDA_NONE = 0x00000000;
        //const uint WDA_MONITOR = 0x00000001;
        const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        public ChromiumWebBrowser? CefBrowser;

        public void InitBrowser(string url, string? proxy)
        {
            var settings = new CefSettings();
            if (proxy != null)
                settings.CefCommandLineArgs.Add("proxy-server", proxy);
            Cef.Initialize(settings);
            CefBrowser = new ChromiumWebBrowser(url);
            CefBrowser.AddressChanged += Browser_AddressChanged;
            CefBrowser.TitleChanged += Browser_TitleChanged;
            this.Controls.Add(CefBrowser);
            CefBrowser.Dock = DockStyle.Fill;
            CefBrowser.KeyboardHandler = new KeyboardHandler(this);
            KeyboardHandler.AddHotKey(this, RefreshActiveTab, Keys.F5);
        }

        private void Browser_AddressChanged(object? sender, AddressChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                Debug.WriteLine(e.Address);
                this.Text = e.Address;
                var url = "https://" + new Uri(e.Address).Host + "/favicon.ico";
                try
                {
                    var httpClient = new HttpClient();
                    var httpResult = httpClient.GetByteArrayAsync(url).Result;
                    var img = new Icon(new System.IO.MemoryStream(httpResult));
                    this.Icon = img;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }));
            if (e.Address.StartsWith("https://translate.google.com/"))
                CefBrowser!.AddressChanged -= Browser_AddressChanged;
        }

        private void Browser_TitleChanged(object? sender, TitleChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                Debug.WriteLine($"Title changed: {e.Title}");
                if (this.Text.StartsWith("https://") || this.Text.StartsWith("http://"))
                    this.Text = $"{e.Title} - Google Chrome - {this.Text}";
            }));
            if (e.Title == "Google Translate")
                CefBrowser!.TitleChanged -= Browser_TitleChanged;
        }

        public void RefreshActiveTab()
        {
            CefBrowser!.Load(CefBrowser.Address);
        }

        private void Form1_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            MoveTitleBarForm();
            TitleBarFormInstance.Visible = !TitleBarFormInstance.Visible;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            MoveTitleBarForm();
        }

    }

}