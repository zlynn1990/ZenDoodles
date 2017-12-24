using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Color = System.Drawing.Color;

namespace ZenDoodles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int LineSpeed = 1200;

        private bool _isRunning = true;
        private readonly Thread _updateThread;

        private Bitmap _frameBuffer;
        private Bitmap _lineCache;
        private WriteableBitmap _writeableBitmap;
        private static object _bufferLock = new object();

        private List<LineRenderer> _lineRenderers;

        public MainWindow()
        {
            InitializeComponent();

            Initialize((int)Width, (int)Height);

            _updateThread = new Thread(GameLoop);
            _updateThread.Start();
        }

        private void Initialize(int width, int height)
        {
            lock (_bufferLock)
            {
                if (_frameBuffer != null)
                {
                    _frameBuffer.Dispose();
                }

                if (_lineCache != null)
                {
                    _lineCache.Dispose();
                }

                Canvas.Width = width;
                Canvas.Height = height;

                _frameBuffer = new Bitmap(width, height);
                _lineCache = new Bitmap(width, height);

                _writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
                Canvas.Source = _writeableBitmap;

                GenerateRenderers(width, height);
            }
        }

        private void GenerateRenderers(int width, int height)
        {
            List<LineSection> lineSections = PatternGenerator.Simple(width, height);

            _lineRenderers = new List<LineRenderer>();

            foreach (LineSection section in lineSections)
            {
                _lineRenderers.Add(new LineRenderer(LineSpeed, section.Fill()));
            }
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        private void GameLoop()
        {
            float dt = 1.0f / DisplayUtilities.GetMonitorRefreshRate();
            int sleepTimeout = (int)Math.Max(dt * 1000, 1);

            var renderTimer = new Stopwatch();

            while (_isRunning)
            {
                renderTimer.Start();

                Update(dt);
                DrawScreen();

                renderTimer.Stop();

                Thread.Sleep(Math.Max(sleepTimeout - (int)renderTimer.ElapsedMilliseconds, 0));
                renderTimer.Reset();
            }
        }

        /// <summary>
        /// Do all the update work.
        /// </summary>
        private void Update(float dt)
        {
            lock (_bufferLock)
            {
                using (Graphics graphics = Graphics.FromImage(_frameBuffer))
                {
                    graphics.Clear(Color.White);

                    graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

                    graphics.DrawImage(_lineCache, 0, 0);

                    graphics.InterpolationMode = InterpolationMode.Bilinear;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.Default;

                    foreach (LineRenderer renderer in _lineRenderers)
                    {
                        renderer.Draw(graphics, dt);
                    }
                }

                using (Graphics graphics = Graphics.FromImage(_lineCache))
                {
                    graphics.InterpolationMode = InterpolationMode.Bilinear;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.Default;

                    foreach (LineRenderer renderer in _lineRenderers)
                    {
                        renderer.Cache(graphics);
                    }
                }
            }
        }

        /// <summary>
        /// Draw the frame to the screen.
        /// </summary>
        private void DrawScreen()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (_bufferLock)
                {
                    var data = _frameBuffer.LockBits(new Rectangle(0, 0, _frameBuffer.Width, _frameBuffer.Height), ImageLockMode.ReadOnly, _frameBuffer.PixelFormat);

                    _writeableBitmap.WritePixels(new Int32Rect(0, 0, _frameBuffer.Width, _frameBuffer.Height), data.Scan0, data.Stride * data.Height, data.Stride);

                    _frameBuffer.UnlockBits(data);
                }

            }), DispatcherPriority.Render, null);
        }

        private void OnResize(object sender, SizeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Initialize((int) e.NewSize.Width - 17, (int) e.NewSize.Height - 40);

            }), DispatcherPriority.Render, null);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                _isRunning = false;

                _updateThread.Join(100);
            }

            _frameBuffer.Dispose();
        }
    }
}
