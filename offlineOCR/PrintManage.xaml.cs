using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace offlineOCR
{
    public partial class PrintManage : Window
    {
        private BitmapImage bitmaptest;
        private bool _started;
        private Point _downPoint1;
        private Point _downPoint;
        private Canvas canvas = new Canvas();
        private ImageBrush ib = new ImageBrush();
        private double angle=0;
        private string FilePath;
        private int ClickNum=0;
        public PrintManage(BitmapImage image,string Imagepath)
        {
            InitializeComponent();
            lastImage.Source = image;
            bitmaptest = image;
            FilePath = Imagepath;
            transBtn.Click += TransBtn_Click;
            MouseDown += MainWindow_MouseDown;
            MouseMove += MainWindow_MouseMove;
            MouseUp += MainWindow_MouseUp;
            confirm.Click += ConfirmClick;
            cancel.Click += cancelClick;
            int w = 4000;//Canvas的宽度
            int h = 4000;//Canvas的高度
            Size size = new Size(w, h);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));
            DealCanvas();
        }
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _started = true;
            _downPoint = e.GetPosition(Grid);
        }
        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ClickNum++;
            _started = false;
            
            var point = e.GetPosition(Grid);
            Window window =  Window.GetWindow(lastImage);  
            Point  point2  =  lastImage.TransformToAncestor(window).Transform(new Point(0, 0));  
            var rect = new Rect(_downPoint, point);
            var left = _downPoint.X - point2.X;
            var top = _downPoint.Y - point2.Y;
            Rectangle rectangle = new Rectangle();
            rectangle.Width = rect.Width*5;
            rectangle.Height = rect.Height * 6.6;
            rectangle.Margin = new Thickness(left*5.1, top*6.5, 0, 0);
            rectangle.Fill = Brushes.White;
            if ((angle % 360) ==90)
            {
                point2  =  lastImage.TransformToAncestor(window).Transform(new Point(0, 0));
                left= _downPoint.X - (point2.X-600);
                // top = _downPoint.Y - (point2.Y - 800);
                Console.WriteLine(point2.X);
                rectangle.Margin = new Thickness(left*6.6, top*5.1, 0, 0);
                rectangle.Width = rect.Width*6.6;
                rectangle.Height = rect.Height * 5.1;
            }if ((angle % 360)==180)
            {
                point2  =  lastImage.TransformToAncestor(window).Transform(new Point(0, 0));
                left= _downPoint.X - (point2.X-800);
                top = _downPoint.Y - (point2.Y - 600);
                Console.WriteLine(point2.X);
                rectangle.Margin = new Thickness(left*5, top*6.6, 0, 0);
                rectangle.Width = rect.Width*5.1;
                rectangle.Height = rect.Height * 6.6;
            }

            if ((angle % 360) == 270)
            {
                point2  =  lastImage.TransformToAncestor(window).Transform(new Point(0, 0));
                // left= _downPoint.X - (point2.X-800);
                top = _downPoint.Y - (point2.Y - 800);
                Console.WriteLine(point2.X);
                rectangle.Margin = new Thickness(left*6.6, top*5, 0, 0);
                rectangle.Width = rect.Width*6.6;
                rectangle.Height = rect.Height * 5;
            }
            DealCanvas(rectangle);
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_started)
            {
                var point = e.GetPosition(Grid);
                Rectangle rectangle = new Rectangle();
                
                var rect = new Rect(_downPoint, point);
                Rectangle.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
                Rectangle.Width = rect.Width;
                Rectangle.Height = rect.Height;
                Rectangle.Fill=Brushes.White;
                Point getPoint = PointToScreen(new Point());
                // Console.WriteLine(getPoint);
            }
        }


        private void TransBtn_Click(object sender, RoutedEventArgs e)
        {
            ClickNum++;
            Console.WriteLine(ClickNum);
            TransformGroup tg = lastImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                RotateTransform rt = tgnew.Children[2] as RotateTransform;
                lastImage.RenderTransformOrigin = new Point(0.5, 0.5);
                rt.Angle += 90;
                this.angle = rt.Angle;
            }
            // 重新给图像赋值Transform变换属性
            lastImage.RenderTransform = tgnew;
            DealCanvas();
            
            
        }

        private void ConfirmClick(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rb = new RenderTargetBitmap(4000, 4000, 96d, 96d, PixelFormats.Pbgra32);
            rb.Render(canvas);
            
            string path = FilePath;
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                
                encoder.Frames.Add(BitmapFrame.Create(rb));
                
                encoder.Save(fs);
                MainWindow main = new MainWindow();
                (this.Owner as MainWindow).SaveBtn.IsEnabled = true;
                (this.Owner as MainWindow).PrintBtn.IsEnabled = true;
                (this.Owner as MainWindow).printcanvas = canvas;
            }
        }

        private void DealCanvas(Rectangle rectangle=null)
        {
            Console.WriteLine(ClickNum);
            ib.ImageSource = lastImage.Source;
            var anglechange = new RotateTransform();
            anglechange.CenterX = 0.5;
            anglechange.CenterY = 0.5;
            anglechange.Angle += this.angle;
            ib.RelativeTransform = anglechange;
            Console.WriteLine(ib);
            canvas.Background =ib ;
            if (rectangle != null)
            {
                canvas.Children.Add(rectangle);
            }
            
        }

        private void cancelClick(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);//关闭父窗体
            window.Close();
        }
    }
    
}