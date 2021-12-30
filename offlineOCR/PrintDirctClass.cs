using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace offlineOCR
{
    public class PrintDirectClass
    {
        private int printNum = 0;//多页打印
        private string imageFile = "";//单个图片文件
        private ArrayList fileList = new ArrayList();//多个图片文件
        private PrintDialog printDialog;
        private string outputDic { get { return $"{AppDomain.CurrentDomain.BaseDirectory}\\Temp\\"; } }
        public void PrintPreview(string filename)
        {
            this.imageFile = filename;

            PrintDocument docToPrint = new PrintDocument();
            docToPrint.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.docToPrint_BeginPrint);
            docToPrint.EndPrint += new System.Drawing.Printing.PrintEventHandler(this.docToPrint_EndPrint);
            docToPrint.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.docToPrint_PrintPage);
            docToPrint.DefaultPageSettings.Landscape = false;
 
             printDialog = new PrintDialog();
            printDialog.PageRangeSelection = PageRangeSelection.AllPages;
            if (printDialog.ShowDialog() == true)
            {
                docToPrint.Print();
            }
        }
        private void docToPrint_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (fileList.Count == 0)
                fileList.Add(imageFile);
        }
        private void docToPrint_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
 
        }
        private void docToPrint_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //图片抗锯齿
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var height1 = printDialog.PrintableAreaHeight;
            var width1 = printDialog.PrintableAreaWidth;
            Stream fs = new FileStream(fileList[printNum].ToString().Trim(), FileMode.Open, FileAccess.Read);
            System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
            var newImage = RezizeImage(image, (int)width1, (int)height1);
            image = newImage;
            int x = e.MarginBounds.X;
            int y = e.MarginBounds.Y;
            int width = image.Width;
            int height = image.Height;
            if ((width / e.MarginBounds.Width) > (height / e.MarginBounds.Height))
            {
                width = e.MarginBounds.Width;
                height = image.Height * e.MarginBounds.Width / image.Width;
            }
            else
            {
                height = e.MarginBounds.Height;
                width = image.Width * e.MarginBounds.Height / image.Height;
            }
            var vis = new DrawingVisual();
            using (var dc = vis.RenderOpen())
            {
                // dc.DrawImage(img, new Rect { Width = bi.Width, Height = bi.Height });
            }

            if (printNum < fileList.Count - 1)
            {
                printNum++;
                e.HasMorePages = false;//HasMorePages为true则再次运行PrintPage事件
                return;
            }
            e.HasMorePages = false;
        }

        private System.Drawing.Image RezizeImage(System.Drawing.Image img, int maxWidth, int maxHeight)
        {
            using (img)
            {
                Console.WriteLine(img.Width+" "+img.Height);
                Double xRatio = (double) img.Width / maxWidth;
                Double yRatio = (double) img.Height / maxHeight;
                Console.WriteLine("xrtio"+xRatio);
                Console.WriteLine("yrt"+yRatio);
                Double ratio = Math.Max(xRatio, yRatio);
                int nnx = (int) Math.Floor(img.Width / ratio);
                int nny = (int) Math.Floor(img.Height / ratio);
                Console.WriteLine("nnx="+nnx);
                Console.WriteLine("nny="+nny);
                Bitmap cpy = new Bitmap(nnx, nny, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics gr = Graphics.FromImage(cpy))
                {
                    System.Drawing.Rectangle rotateRec = GetRotateRectangle(nnx, nny, 90);
                    int rotateWidth = rotateRec.Width;
                    int rotateHeight = rotateRec.Height;
                    gr.Clear(System.Drawing.Color.Transparent);
                    System.Drawing.Drawing2D.Matrix matrix = gr.Transform;
                    matrix.RotateAt(90, new System.Drawing.PointF((float)(rotateWidth/2 ), (float)(rotateHeight/2)));
                    gr.Transform = matrix;

                    
                    
                    // This is said to give best quality when resizing images
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    gr.DrawImage(img,
                        new System.Drawing.Rectangle(0, 0, rotateWidth, rotateHeight),
                        new System.Drawing.Rectangle(0, 0, img.Width, img.Height),
                        GraphicsUnit.Pixel);
                }
                
                return cpy;
            }
        }
        public void Print(string outputPath)
        {
            try
            {
                
                
                ResizeImageAndPrint(outputPath);
            }
            finally
            {
                Console.WriteLine("1111");
            }
        }

        /// <summary>
        /// 根据打印设置重新计算图片高宽后打印
        /// </summary>
        /// <param name="outputPath"></param>
        private void ResizeImageAndPrint(string outputPath)
        {
            var pdialog = new PrintDialog();
            if (pdialog.ShowDialog() == true)
            {
                //根据纸张大小和纸张方向，读取图片，修改图片大小
                var width = pdialog.PrintableAreaHeight;
                var height = pdialog.PrintableAreaWidth;
                
                Console.WriteLine(width+" "+height);
                // return;
                Bitmap bmp = new Bitmap(outputPath);
                var newImage = RezizeImage(bmp, (int)width, (int)height);
                var newPath = $"{outputDic}\\{Guid.NewGuid().ToString()}.jpg";
                // newImage.Save(newPath, ImageFormat.Png);
                bool x=SaveImageForSpecifiedQuality(newImage, newPath, 100, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (!x) return;
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(newPath);
                bi.EndInit();

                var vis = new DrawingVisual();
                using (var dc = vis.RenderOpen())
                {
                    dc.DrawImage(bi, new Rect { Width = bi.Height, Height = height});
                    Console.WriteLine(bi.Width+" "+bi.Height);
                    
                }

                Console.WriteLine(bi.Width+" "+bi.Height);
                
                pdialog.PrintVisual(vis, "My Image");
                
                // File.Delete(newPath);
            }
        }
        private System.Drawing.Rectangle GetRotateRectangle(int width, int height, float angle)
        {
            double radian = angle * Math.PI / 180; ;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
 
            int newWidth = (int)(Math.Max(Math.Abs(width * cos - height * sin), Math.Abs(width * cos + height * sin)));
            int newHeight = (int)(Math.Max(Math.Abs(width * sin - height * cos), Math.Abs(width * sin + height * cos)));
            Console.WriteLine(newHeight+" "+newWidth);
            return new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
        }
        public bool SaveImageForSpecifiedQuality(System.Drawing.Image sourceImage, string savePath, int imageQualityValue,System.Drawing.Imaging.ImageFormat format)
        {
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters encoderParameters = new EncoderParameters();
            EncoderParameter encoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, imageQualityValue);
            encoderParameters.Param[0] = encoderParameter;
            try
            {
                ImageCodecInfo[] ImageCodecInfoArray = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegImageCodecInfo = null;
                for (int i = 0; i < ImageCodecInfoArray.Length; i++)
                {
                    // Console.WriteLine(ImageCodecInfoArray[i].FormatDescription.);
                    Console.WriteLine(format.ToString());
                    if (ImageCodecInfoArray[i].FormatDescription.Equals(format.ToString().ToUpper()))
                    {
                        jpegImageCodecInfo = ImageCodecInfoArray[i];
                        break;
                    }
                }   
                sourceImage.Save(savePath, jpegImageCodecInfo, encoderParameters);              
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }

}