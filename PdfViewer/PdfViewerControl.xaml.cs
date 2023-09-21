using Microsoft.Win32;
using PdfViewer.Attributes;
using PdfViewer.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PdfViewer
{
    /// <summary>
    /// PdfViewerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PdfViewerControl : UserControl, INotifyPropertyChanged
    {
        #region 프로퍼티
        public event PropertyChangedEventHandler PropertyChanged;
        private Process CurrentProcess { get; }
        private CancellationTokenSource Cts { get; }
        private PdfSearchManager SearchManager { get; }
        public string InfoText { get; set; }
        public string SearchTerm { get; set; }
        public PdfBookmarkCollection Bookmarks { get; set; }
        public bool ShowBookmarks { get; set; }
        public PdfBookmark SelectedBookIndex { get; set; }
        public double ZoomPercent
        {
            get => Renderer.Zoom * 100;
            set => Renderer.SetZoom(value / 100);
        }
        public bool IsSearchOpen { get; set; }
        public int SearchMatchItemNo { get; set; }
        public int SearchMatchesCount { get; set; }
        public int Page
        {
            get => Renderer.PageNo + 1;
            set => Renderer.GotoPage(Math.Min(Math.Max(value - 1, 0), Renderer.PageCount - 1));
        }
        public FlowDirection IsRtl
        {
            get => Renderer.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            set => Renderer.IsRightToLeft = value == FlowDirection.RightToLeft ? true : false;
        }
        #endregion

        public PdfViewerControl()
        {
            InitializeComponent();
            CurrentProcess = Process.GetCurrentProcess();
            Cts = new CancellationTokenSource();
            Renderer.PropertyChanged += delegate
            {
                OnPropertyChanged(nameof(Page));
                OnPropertyChanged(nameof(ZoomPercent));
            };
            SearchManager = new PdfSearchManager(Renderer);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                Title = "Open PDF File"
            };

            if (dialog.ShowDialog() == true)
            {
                byte[] bytes = File.ReadAllBytes(dialog.FileName);
                var mem = new MemoryStream(bytes);
                Renderer.OpenPdf(mem);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
