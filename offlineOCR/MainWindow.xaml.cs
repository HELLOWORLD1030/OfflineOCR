using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System.Drawing.Printing;
using System.Threading;

namespace offlineOCR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, dynamic> Selected = new Dictionary<string, dynamic>() {
            {"Year",null },
            {"province",null },
            {"level",null },
            {"studentName",null},
            {"major",null}
        };

        private string FileName;
        public string ImagePath;
        private string TempFile;
        private string littleFileName;
        public Canvas printcanvas;
        private Logger _logger;
        private string storefile;
        private DealMySQL mysql;
        private SelectInfo SelectInfo=new SelectInfo();
        public MainWindow()
        {
            InitializeComponent();
            province.ItemsSource = Province.GetProvinceList();
            province.DisplayMemberPath = "Key";
            province.SelectedValuePath = "Value";
            Year.ItemsSource = year.GetYearList();
            Year.DisplayMemberPath = "Key";
            Year.SelectedValuePath = "Value";
            Level.ItemsSource = year.GetLevelList();
            Level.DisplayMemberPath = "Key";
            Level.SelectedValuePath = "Value";
            Year.SelectionChanged += SelectYear;
            province.SelectionChanged += SelectProvince;
            Level.SelectionChanged += SelectLevel;
            importBtn.Click += CheckBtn;
            StudentName.TextChanged += StudentNameInput;
            SearchBtn.Click += SearchBtn1;
            SaveBtn.Click += SavaImage;
            SaveBtn.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            SaveBtn.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            PrintBtn.Click += DealPrint;
            PrintBtn.IsEnabled = false;
            SaveBtn.IsEnabled = false;
            TempFile = Environment.CurrentDirectory+"/tempfile";
            storefile= Environment.CurrentDirectory+@"\save";
            if (!Directory.Exists(TempFile))
            {
                Directory.CreateDirectory(TempFile);
            }
            if (!Directory.Exists(storefile))
            {
                Directory.CreateDirectory(storefile);
            }
            _logger = new Logger(Log);
            mysql = new DealMySQL();
            
        }

        private void StudentNameInput(object sender, TextChangedEventArgs e)
        {
            SelectInfo.StudentName = StudentName.Text;
        }

        public void SelectYear(object s,SelectionChangedEventArgs e)
        {
            string year = Year.SelectedValue.ToString();
            SelectInfo.Year = year;

        }
        public void SelectProvince(object s, SelectionChangedEventArgs e) {
            string year = province.SelectedValue.ToString();
            SelectInfo.Province = year;
        }
        public void SelectLevel(object s, SelectionChangedEventArgs e)
        {
            string year = Level.SelectedValue.ToString();
            SelectInfo.Level = year;
        }
        public async void CheckBtn(object sender, RoutedEventArgs e)
        {
            // _logger.AddInfo(Environment.CurrentDirectory);
          
            ParmeterToSql par = new ParmeterToSql(SelectInfo);
           
            if (!ParmeterToSql.CheckPar(SelectInfo))
            {
                _logger.AddError("请输入要导入的年份，省份，本/专科");
                return;
            };
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "jpg图片文件|*.jpg|png图片文件|*.png";
            if (dialog.ShowDialog() == true)
            {
                var test = dialog.FileName;
                Cmd cmd = new Cmd();
                // Console.WriteLine(test);
                
                _logger.AddInfo("正在识别，请稍等...");
                string result=null;
               result= await cmd.RunCmd(test);
               result = FileUtil.ReadResultFromFile(test + ".txt");
               if ("".Equals(result))
               {
                   _logger.AddError("识别错误");
                   return;
               }
               // _logger.AddInfo(result);
               string newfilename = storefile+@"\"+System.Guid.NewGuid().ToString()+System.IO.Path.GetExtension(test);
               Console.WriteLine(newfilename);
               File.Copy(test,newfilename);
              
               newfilename = newfilename.Replace("\\", "\\\\");
                mysql.insertDate(
                    $"INSERT INTO `offline`(`year`, `province`, `level`, `imagelink`, `imagetext`) VALUES ('{SelectInfo.Year}','{SelectInfo.Province}','{SelectInfo.Level}','{newfilename}','{result}')");
                 this.Year.SelectedIndex=0;
                 this.province.SelectedIndex = 0;
                 this.Level.SelectedIndex = 0;
                 _logger.AddInfo("识别完成...");
            }
        }

        public void SearchBtn1(object sender, RoutedEventArgs e)
        {
            //将年份，省份，本科专科作为参数，填进sql语句
            //将学生姓名，专业作为参数，填进sql2
            //查出图片链接，渲染到image框
           // _logger.AddInfo(Environment.CurrentDirectory.Replace("\\","\\\\"));
           ParmeterToSql parmeterToSql=new ParmeterToSql(SelectInfo);
           // _logger.AddInfo(parmeterToSql.getFlag().ToString());
           // Console.WriteLine(parmeterToSql.GetSQLStr());
            var test=mysql.Command(parmeterToSql.GetSQLStr(SelectInfo));
            
            if (test["res"] != 100)
            {
                Console.WriteLine(test["mess"]);
                _logger.AddError(test["mess"]);
                return;
            }
            DataTable data = test["data"];
            ResultBox.ItemsSource = data.DefaultView;
            ResultBox.GridLinesVisibility=DataGridGridLinesVisibility.All;
            ResultBox.IsReadOnly = true;
            PrintBtn.Click += DealPrint;
            _logger.AddInfo("查询成功...");
        }

        public void DataGridTest(object sender, RoutedEventArgs e)
        {
            var b = (DataRowView) ResultBox.SelectedItem;
            if (b==null)
            {
                return;
            }
            var test = b["图片链接"].ToString();
            var ya = b["年份"].ToString();
            var provin=b["省份"].ToString();
            var le = b["本专科"].ToString();
            StringBuilder str = new StringBuilder();
            str.Append(ya).Append("-").Append(provin).Append("-").Append(le).Append("录取名单.png");
            littleFileName = str.ToString();
            this.FileName = TempFile+"/"+str.ToString();
            Console.WriteLine(test);
            this.ImagePath = test;
            
            littleImage.Source= GetImage(test);
            // PrintBtn.IsEnabled = true;
        }

        public void SavaImage(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "保存图片";
            dlg.FileName = littleFileName;
            Stream myStream;
            if (dlg.ShowDialog() == true)
            {
                Console.WriteLine(dlg.FileName);
            }

            try
            {
                File.Copy(FileName, dlg.FileName);
            }
            catch (IOException ex)
            {
                Console.WriteLine("此文件已存在");
                _logger.AddError("此文件已存在...");
            }
            
        }
        public static BitmapImage GetImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using (Stream ms = new MemoryStream(File.ReadAllBytes(imagePath)))
                {
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }
            }
            return bitmap;
        }

        public void DealPrint(object sender, RoutedEventArgs e)
        {
            // Console.WriteLine("打印吗？");
            // PrintManage printManage = new PrintManage(GetImage(ImagePath));
            // printManage.ShowDialog();
            // App.Current.MainWindow = test ;
            // PrintDialog pDialog = new PrintDialog();
            // pDialog.PageRangeSelection = PageRangeSelection.AllPages;
            // pDialog.UserPageRangeEnabled = true;
            //
            // // Display the dialog. This returns true if the user presses the Print button.
            // Nullable<Boolean> print = pDialog.ShowDialog();
            // if (print == true)
            // {
            //     Export(new Uri(@"C:\Users\HELLO_WORLD\Desktop\test.xps"),printcanvas);
            //     // pDialog.PrintVisual(printcanvas,"Printing Canvas");
            //     XpsDocument xpsDocument = new XpsDocument(@"C:\Users\HELLO_WORLD\Desktop\test.xps", FileAccess.ReadWrite);
            //     FixedDocumentSequence fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
            //     pDialog.PrintDocument(fixedDocSeq.DocumentPaginator, "Test print job");
            //
            //
            //
            // }
            PrintDirectClass printDirectClass = new PrintDirectClass();
            printDirectClass.Print(FileName);
        }
        public void Export(Uri path, Canvas surface)
        {
            if (path == null) return;
    
            Transform transform = surface.LayoutTransform;
            // surface.LayoutTransform = null;
            //
            // Size size = new Size(surface.Width, surface.Height);
            // surface.Measure(size);
            // surface.Arrange(new Rect(size));
   
            Package package = Package.Open(path.LocalPath, FileMode.Create);
            XpsDocument doc = new XpsDocument(package);
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(doc);
            writer.Write(surface);
            doc.Close();
            package.Close();
            // surface.LayoutTransform = transform;
        }
        public void ClickImage(object sender, MouseEventArgs e)
        {
            Console.WriteLine("图片点击事件");
            Image sf = sender as Image;
           
            PrintManage printManage=new PrintManage(GetImage(ImagePath),FileName);
            printManage.Owner = this;
            printManage.ShowDialog();
        }

        public void BigImageClick(object sender, MouseEventArgs e)
        {
            return;
            bigImage.Visibility = Visibility.Hidden;
            // bigImage.SetValue(Grid.ZIndexProperty,0);
            littleImage.Visibility = Visibility.Visible;
            
        }
    }
}
