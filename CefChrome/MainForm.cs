using CefSharp.WinForms;
using CefSharp;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;

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

        [DllImport("User32")]
        public static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            int fsModifiers,
            int vk
        );
        [DllImport("User32")]
        public static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id
        );

        public const int MOD_WIN = 0x8;
        public const int MOD_SHIFT = 0x4;
        public const int MOD_CONTROL = 0x2;
        public const int MOD_ALT = 0x1;
        public const int WM_HOTKEY = 0x312;
        public const int WM_DESTROY = 0x0002;

        private readonly TitleBarForm TitleBarFormInstance = new();

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckBoxH_CheckedChanged();
            CheckBoxB_CheckedChanged();
            CheckBoxT_CheckedChanged();
            TitleBarFormInstance.Show();
            TitleBarFormInstance.Visible = false;
            TitleBarFormInstance.Owner = this;
            MoveTitleBarForm();
            TitleBarFormInstance.checkBoxH.CheckedChanged += CheckBoxH_CheckedChanged;
            TitleBarFormInstance.checkBoxB.CheckedChanged += CheckBoxB_CheckedChanged;
            TitleBarFormInstance.checkBoxT.CheckedChanged += CheckBoxT_CheckedChanged;
            if (!RegisterHotKey(this.Handle, 1, MOD_WIN + MOD_ALT, (int)Keys.A))
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!RegisterHotKey(this.Handle, 0, MOD_WIN + MOD_ALT, (int)Keys.S))
                MessageBox.Show("Set hotkey failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_HOTKEY:
                    switch (m.WParam.ToInt32())
                    {
                        case 1:
                            this.Show();
                            this.Activate();
                            break;
                        case 0:
                            this.Hide();
                            break;
                    }
                    break;
                case WM_DESTROY:
                    UnregisterHotKey(this.Handle, 1);
                    UnregisterHotKey(this.Handle, 0);
                    break;
            }
            base.WndProc(ref m);
        }

        private void MoveTitleBarForm()
        {
            TitleBarFormInstance.Location = new Point(this.Location.X + this.Size.Width - TitleBarFormInstance.Size.Width, this.Location.Y + 5);
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            MoveTitleBarForm();
        }

        private void CheckBoxH_CheckedChanged(object? sender = null, EventArgs? e = null)
        {
            if (TitleBarFormInstance.checkBoxH.Checked)
            {
                _ = SetWindowDisplayAffinity(this.Handle, WDA_EXCLUDEFROMCAPTURE);
            }
            else
            {
                _ = SetWindowDisplayAffinity(this.Handle, WDA_NONE);
            }
        }

        private void CheckBoxB_CheckedChanged(object? sender = null, EventArgs? e = null)
        {
            if (TitleBarFormInstance.checkBoxB.Checked)
            {
                this.ShowInTaskbar = false;
                this.MinimizeBox = false;
            }
            else
            {
                this.ShowInTaskbar = true;
                this.MinimizeBox = true;
            }
            CheckBoxH_CheckedChanged();
        }

        private void CheckBoxT_CheckedChanged(object? sender = null, EventArgs? e = null)
        {
            this.TopMost = TitleBarFormInstance.checkBoxT.Checked;
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
            //CefBrowser.AddressChanged += Browser_AddressChanged;
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
                //if (this.Text.StartsWith("https://") || this.Text.StartsWith("http://"))
                //this.Text = $"{e.Title} - Google Chrome - {this.Text}";
                this.Text = $"{e.Title} - Google Chrome";
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