using System;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;

namespace QuranAuthor.Helps
{
    public class ClipboardHelper
    {
        private IntPtr hWndNextViewer;
        private HwndSource hWndSource;
        private bool isViewing;
        private Window window;
        private bool firstTime;
        private bool secondCapture = false;

        public bool IsViewing
        {
            get
            {
                return this.isViewing;
            }
        }

        public ClipboardHelper(Window window)
        {
            this.window = window;
            isViewing = false;
        }

        public void Start()
        {
            WindowInteropHelper wih = new WindowInteropHelper(window);
            hWndSource = HwndSource.FromHwnd(wih.Handle);

            firstTime = true;
            hWndSource.AddHook(this.WinProc);   // start processing window messages
            hWndNextViewer = Win32.SetClipboardViewer(hWndSource.Handle);   // set this window as a viewer
            isViewing = true;
        }

        public void Stop()
        {
            // remove this window from the clipboard viewer chain
            Win32.ChangeClipboardChain(hWndSource.Handle, hWndNextViewer);

            hWndNextViewer = IntPtr.Zero;
            hWndSource.RemoveHook(this.WinProc);
            isViewing = false;
        }

        private IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_CHANGECBCHAIN:
                    if (wParam == hWndNextViewer)
                    {
                        // clipboard viewer chain changed, need to fix it.
                        hWndNextViewer = lParam;
                    }
                    else if (hWndNextViewer != IntPtr.Zero)
                    {
                        // pass the message to the next viewer.
                        Win32.SendMessage(hWndNextViewer, msg, wParam, lParam);
                    }
                    break;

                case Win32.WM_DRAWCLIPBOARD:
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    else
                    {
                        if (secondCapture)
                        {
                            this.ExtractText();
                        }
                        
                        secondCapture = !secondCapture;
                    }
                    // pass the message to the next viewer.
                    Win32.SendMessage(hWndNextViewer, msg, wParam, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private void ExtractText()
        {
            IDataObject iData = new DataObject();

            try
            {
                iData = Clipboard.GetDataObject();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            
            if (iData.GetDataPresent(DataFormats.Rtf))
            {
                var rtf = (string)iData.GetData(DataFormats.Rtf);
                var bitmap = WindowCapturer.Capture();

                var selection = BitmapHelper.GetSnippetSelection(bitmap);
                File.WriteAllText("E://test.txt", selection.ToString());
                bitmap.Save("E://test.png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
