using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro;
using System.ServiceModel.Syndication;
using Image = System.Drawing.Image;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using ListBox = System.Windows.Controls.ListBox;

namespace PS3_Game_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        string webUrl;
        DataTable dtpkg = new DataTable();
        DataTable dtpkg2 = new DataTable();
        DataTable dtiso = new DataTable();
        DataTable dtiso2 = new DataTable();
        DataTable dtgame = new DataTable();
        DataTable dtgame2 = new DataTable();

        DataTable dtud = new DataTable();
        DataTable dt2 = new DataTable();
        int i = 0;
        static string s = null;
        static string scid = null;
        string pkg;
        string pkg_directory;
        string iso_directory;
        string game_directory;
        string color;
        string ctheme = "BaseDark";

        FileInfo[] Files;
        FileInfo[] Files2;
        string[] directories;
        DirectoryInfo dinfo;

        string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;//System.Windows.Shapes.Path.GetDirectoryName(Application.ExecutablePath.);
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        
        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            ChangeAppStyle();
            
            //rss();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dtud.Columns.Add("Version");
            dtud.Columns.Add("Size");
            dtud.Columns.Add("SHA1");
            dtud.Columns.Add("FWVer");
            dtud.Columns.Add("Url");
            dtud.Columns.Add("Sub");
            dtud.Columns.Add("Name");

            dtpkg.Columns.Add("IsSelected");
            dtpkg.Columns.Add("Name");
            dtpkg.Columns.Add("CID");
            dtpkg.Columns.Add("type");
            dtpkg.Columns.Add("Size");

            dtpkg2.Columns.Add("Name");
            dtpkg2.Columns.Add("CID");
            dtpkg2.Columns.Add("type");
            dtpkg2.Columns.Add("Size");
            dtpkg2.Columns.Add("icon");
            dtpkg2.Columns.Add("tool");
            dtpkg2.Columns.Add("count");
            dtpkg2.Columns.Add("bl");
            dtpkg2.Columns.Add("tileh");
            dtpkg2.Columns.Add("tilew");
            dtpkg2.Columns.Add("column1w");
            dtpkg2.Columns.Add("column2w");
            dtpkg2.Columns.Add("roww");
            dtpkg2.Columns.Add("imags");
            dtpkg2.Columns.Add("text1s");
            dtpkg2.Columns.Add("text2s");

            dtiso.Columns.Add("IsSelected");
            dtiso.Columns.Add("Name");
            dtiso.Columns.Add("CID");
            dtiso.Columns.Add("type");
            dtiso.Columns.Add("Size");

            dtiso2.Columns.Add("Name");
            dtiso2.Columns.Add("CID");
            dtiso2.Columns.Add("type");
            dtiso2.Columns.Add("Size");
            dtiso2.Columns.Add("icon");
            dtiso2.Columns.Add("tool");
            dtiso2.Columns.Add("count");
            dtiso2.Columns.Add("bl");
            dtiso2.Columns.Add("tileh");
            dtiso2.Columns.Add("tilew");
            dtiso2.Columns.Add("column1w");
            dtiso2.Columns.Add("column2w");
            dtiso2.Columns.Add("roww");
            dtiso2.Columns.Add("imags");
            dtiso2.Columns.Add("text1s");
            dtiso2.Columns.Add("text2s");

            dtgame.Columns.Add("IsSelected");
            dtgame.Columns.Add("Name");
            dtgame.Columns.Add("CID");
            dtgame.Columns.Add("type");
            dtgame.Columns.Add("Size");

            dtgame2.Columns.Add("Name");
            dtgame2.Columns.Add("CID");
            dtgame2.Columns.Add("type");
            dtgame2.Columns.Add("Size");
            dtgame2.Columns.Add("icon");
            dtgame2.Columns.Add("tool");
            dtgame2.Columns.Add("count");
            dtgame2.Columns.Add("bl");
            dtgame2.Columns.Add("tileh");
            dtgame2.Columns.Add("tilew");
            dtgame2.Columns.Add("column1w");
            dtgame2.Columns.Add("column2w");
            dtgame2.Columns.Add("roww");
            dtgame2.Columns.Add("imags");
            dtgame2.Columns.Add("text1s");
            dtgame2.Columns.Add("text2s");

            textBox1.Text = pkg_directory;
            textBox2.Text = iso_directory;
            textBox.Text = game_directory;
            //rss();
            o_folder();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int c = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                c++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[c]);
        }

        public static async void ShowDialog(CustomDialog dialog)
        {
            var main = (MainWindow)System.Windows.Application.Current.MainWindow;
            await main.ShowMetroDialogAsync(dialog);
        }
        

        private void GenerateImage(string bkImage, string cname)
        {
            System.Drawing.Image backImage = (Bitmap)System.Drawing.Image.FromFile(bkImage);
            int targetHeight = 320; //height and width of the finished image
            int targetWidth = 320;

            //be sure to use a pixelformat that supports transparency
            using (var bitmap = new Bitmap(targetWidth, targetHeight,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    //this ensures that the backgroundcolor is transparent
                    canvas.Clear(System.Drawing.Color.Transparent);

                    //this selects the entire backimage and and paints
                    //it on the new image in the same size, so its not distorted.
                    int w = (320 - backImage.Width) / 2;
                    int h = (320 - backImage.Height) / 2;
                    canvas.DrawImage(backImage,
                              new System.Drawing.Rectangle(w, h, backImage.Width, backImage.Height),
                              new System.Drawing.Rectangle(0, 0, backImage.Width, backImage.Height),
                              GraphicsUnit.Pixel);

                    //this paints the frontimage with a offset at the given coordinates
                    //canvas.DrawImage(frontImage, 5, 25);

                    canvas.Save();
                    bitmap.Save(cname, ImageFormat.Png);
                    backImage.Dispose();

                }
            }
        }
        
        #region<<style>>

        public void ChangeAppStyledark()
        {
            // get the theme from the window
            // get the theme from the current application
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                        ThemeManager.GetAccent("Indigo"),
                                        ThemeManager.GetAppTheme("BaseDark"));
        }

        public void ChangeAppStyle()
        {

            if (ctheme == "BaseDark")
            {
                //button7.Content = "Light Theme";
                b7tb1.Text = "Light Theme";
                //ctheme = "BaseLight";
            }
            else
            {
                //button7.Content = "Dark Theme";
                b7tb1.Text = "Dark Theme";
                //ctheme = "BaseDark";
            }
            // get the theme from the window
            // get the theme from the current application
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);

            if(color == null)
            {
                color = "Crimson";
            }
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                        ThemeManager.GetAccent(color),

                                        ThemeManager.GetAppTheme(ctheme));

            Properties.Settings.Default.color = color;
            Properties.Settings.Default.Save();

        }

        public void ChangeAppStylelight()
        {
            // get the theme from the window
            var theme = ThemeManager.DetectAppStyle(System.Windows.Application.Current);

            // now set the Red accent and dark theme
            ThemeManager.ChangeAppStyle(System.Windows.Application.Current,
                                        ThemeManager.GetAccent("Steel"),
                                        ThemeManager.GetAppTheme("BaseLight"));
        }

        #endregion<<style>>

        #region<<settings>>
        private void LoadSettings()
        {
            pkg_directory = Properties.Settings.Default.pkg_folder;
            iso_directory = Properties.Settings.Default.iso_folder;
            game_directory = Properties.Settings.Default.game_folder;
            color = Properties.Settings.Default.color;
        }
        private void SaveSettings()
        {
            Properties.Settings.Default.pkg_folder = pkg_directory;
            Properties.Settings.Default.iso_folder = iso_directory;
            Properties.Settings.Default.game_folder = game_directory;
            Properties.Settings.Default.color = color;
            Properties.Settings.Default.Save();
        }
        
        #endregion<<settings>>

        #region<<files>>
        private void o_folder()
        {
            /*
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.progress1.Visibility = Visibility.Visible;
            }));*/
            //set_load(true);

            i = 0;
            //dataGrid1sign.ItemsSource.
            //dt.Rows.Clear();
            dt2.Rows.Clear();

            //VisitPlanItems.DataContext = dt2.DefaultView;
            //extractIcons();
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                //this.image1.Visibility = Visibility.Collapsed;
                //this.label6.Visibility = Visibility.Collapsed;
            }));

            appPath = appPath.Replace("PS3gbs.exe", "");
            pkg = appPath;
            //rename();

            /*xDPx fix
             PKG Folder check*/

            if(!Directory.Exists(appPath + "/PKG"))
            {
                Directory.CreateDirectory(appPath + "/PKG");
            }

            dinfo = new DirectoryInfo(appPath + "/PKG");
            //files = Directory.GetFiles(appPath, "*.pkg");
            Files = dinfo.GetFiles("*.pkg");
            //string[] tempsubdirectoryEntries = Directory.GetDirectories(appPath);
            int sr = 0;
            // subdirectoryEntries = new string[tempsubdirectoryEntries.Length - 1];
            foreach (string s in Directory.GetDirectories(appPath))
            {
                if (sr <= 19 && s.Remove(0, appPath.Length) != "bin")
                {
                    //subdirectoryEntries[sr] = s.Remove(0, appPath.Length);
                    sr++;
                }
            }
            //subdirectoryEntries = Directory.GetDirectories(appPath);
            // listBox2.ItemsSource = subdirectoryEntries;


            foreach (FileInfo file in Files)
            {
                FileStream pkgFilehead = File.Open(file.FullName, FileMode.Open);
                byte[] testmagic = new byte[0x06];
                //pkgFilehead.Seek(0x30, SeekOrigin.Begin);
                pkgFilehead.Read(testmagic, 0, 0x06);
                pkgFilehead.Close();
                byte[] magic1 = new byte[] { 0x7F, 0x50, 0x4B, 0x47, 0x00, 0x00 };
                byte[] magic2 = new byte[] { 0x7F, 0x50, 0x4B, 0x47, 0x80, 0x00 };
                string pkgtype;
                bool isdebug = testmagic.SequenceEqual(magic1);
                bool isretail = testmagic.SequenceEqual(magic2);
                if (isdebug == true)
                {
                    pkgtype = "Debug ";
                }

                else if (isretail == true)
                {
                    pkgtype = "Retail";
                }

                else
                {
                    pkgtype = "";

                }

                if (pkgtype == "Retail")
                {
                    byte[] data = new byte[0x80];
                    byte[] result;
                    byte[] testsha = new byte[0x08];


                    FileStream pkgFilesha = File.Open(file.FullName, FileMode.Open);
                    byte[] readsha = new byte[0x08];
                    //pkgFilehead.Seek(0x30, SeekOrigin.Begin);
                    pkgFilesha.Read(data, 0, 0x80);
                    pkgFilesha.Seek(0xb8, SeekOrigin.Begin);
                    pkgFilesha.Read(readsha, 0, 0x08);
                    pkgFilesha.Close();


                    SHA1 sha = new SHA1CryptoServiceProvider();
                    // This is one implementation of the abstract class SHA1.
                    result = sha.ComputeHash(data);
                    Buffer.BlockCopy(result, 12, testsha, 0, 8);

                    bool ishan = !readsha.SequenceEqual(testsha);
                    if (ishan == true)
                    {
                        pkgtype = "HAN   ";
                    }
                }

                if (pkgtype != "")
                {
                    s = file.ToString();
                    if (s != "Package_List.pkg")
                    {
                        s = s.Replace("&", "and");

                        FileStream pkgFile = File.Open(file.FullName, FileMode.Open);
                        byte[] cid = new byte[0x30];
                        pkgFile.Seek(0x30, SeekOrigin.Begin);
                        pkgFile.Read(cid, 0, 0x30);
                        pkgFile.Close();
                        scid = Encoding.ASCII.GetString(cid);
                        int index = scid.IndexOf("\0");
                        if (index > 0)
                            scid = scid.Substring(0, index);
                        string sz = SizeSuffix(file.Length);
                        s = file.ToString();

                        if (s != "")
                        {
                             DataRow dr1 = dtpkg.NewRow();
                             dr1["IsSelected"] = false;
                             dr1["Name"] = s;
                             dr1["CID"] = scid;
                             dr1["type"] = pkgtype;
                             dr1["Size"] = sz;
                             dtpkg.Rows.Add(dr1);
                        }
                        //dt.Rows.Add("false"+"    " + s, "      " + scid + "      ", "      " + pkgtype + "      ", "   " + sz);
                        //tabItem5.
                         //dataGrid1.DataContext = dtpkg.DefaultView;
                         //dataGrid1sign.DataContext = dtpkg.DefaultView;
                        //g1.Children.Add
                        //dataGrid2.DataContext = dtpkg.DefaultView;

                        string iconpath = s.Replace(".pkg", ".PNG");
                        iconpath = appPath + "tools/icons/" + iconpath;
                        if (!File.Exists(iconpath))
                        {
                            iconpath = appPath + "tools/icons/download.png";
                        }

                        DataRow dr = dtpkg2.NewRow();
                        dr["Name"] = s;
                        dr["CID"] = scid;
                        dr["type"] = pkgtype;
                        dr["size"] = sz ;
                        dr["icon"] = iconpath;
                        dr["tool"] = "  " + s + "  " + scid + "  " + sz;
                        dr["count"] = i;
                        dr["bl"] = "0.0";
                        dr["tileh"] = "50";
                        dr["tilew"] = "800";
                        dr["column1w"] = "150";
                        dr["column2w"] = "650";
                        dr["roww"] = "25";
                        dr["imags"] = "50";
                        dr["text1s"] = "16";
                        dr["text2s"] = "12";
                        dtpkg2.Rows.Add(dr);


                        //dt.Rows.Add("    " + s, "      " + scid + "      ", "      " + pkgtype + "      ", "   " + sz + "      ","icon.png");
                        //tabItem5.
                        //VisitPlanItems.
                        //PKGList.DataContext = dtpkg2.DefaultView;

                        //Launchpad.DataContext = dtpkg2.DefaultView;
                        VisitPlanItems.DataContext = dtpkg2.DefaultView;
                        lbtest.DataContext = dtpkg2.DefaultView;
                        lvpkginfo.DataContext = dtpkg2.DefaultView;


                        //Thumbnails.Items.Add(new BitmapImage(new Uri(iconpath)));
                        //tile1[i] = new Tile();
                        i++;
                    }
                    
                }

            }

            //set_load(false);
            /*
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                this.progress1.Visibility = Visibility.Collapsed;
            }));*/
        }

        private void open_iso_folder()
        {
            appPath = appPath.Replace("PS3gbs.exe", "");

            dinfo = new DirectoryInfo(iso_directory);
            //files = Directory.GetFiles(appPath, "*.pkg");
            Files2 = dinfo.GetFiles("*.iso");

            foreach (FileInfo file in Files2)
            {
                
                // Use ProcessStartInfo class.
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = "bin/7z.exe";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "e " + '"' + file + "-o" + '"' + " bin/icons " + '"' + "PS3_GAME\\ICON0.png" + '"';

                string iname = file.Name.Replace("iso", "png");
                File.Move("bin/icons/ocon0.png", "bin/icons/" + iname);

            }

        }

        private void open_game_folder()
        {
            appPath = appPath.Replace("PS3gbs.exe", "");
            dinfo = new DirectoryInfo(appPath);
            //files = Directory.GetFiles(appPath, "*.pkg");
            //Files = dinfo.GetFiles("*.pkg");
            directories = Directory.GetDirectories(game_directory);

            foreach (string directoriy in directories)
            {
                if (File.Exists(directoriy + "\\PS3_GAME\\PARAM.SFO"))
                {
                    File.Copy(directoriy + "\\PS3_GAME\\PARAM.SFO", "bin/sfo/" + directoriy + ".sfo");
                    if (File.Exists(directoriy + "\\PS3_GAME\\ICON0.png"))
                    {
                        File.Copy(directoriy + "\\PS3_GAME\\ICON0.png", "bin/icons/" + directoriy + ".png");

                    }

                }
            }
        }

        #endregion<<files>>

        #region<<tiles>>

        private void tiles_GotFocus(object sender, RoutedEventArgs e)
        {
            Tile clickedButton = (Tile)sender;
            string g = clickedButton.Count;

            foreach (DataRow row in dt2.Rows)
            {
                string tname = row["count"].ToString();
                if (tname == g)
                {

                    row["bl"] = "8.0";
                    row["tileh"] = "100";
                    row["tilew"] = "800";
                    row["column1w"] = "150";
                    row["column2w"] = "650";
                    row["roww"] = "50";
                    row["imags"] = "100";
                    // VisitPlanItems.DataContext = dt2.DefaultView;
                }
                else
                {
                    row["bl"] = "0.0";
                    row["tileh"] = "60";
                    row["tilew"] = "800";
                    row["column1w"] = "150";
                    row["column2w"] = "650";
                    row["roww"] = "30";
                    row["imags"] = "50";
                    //VisitPlanItems.DataContext = dt2.DefaultView;
                }
                //dispatcherTimer2.Start();
                //VisitPlanItems.DataContext = dt2.DefaultView;
            }
        }

        private void tiles_LostFocus(object sender, RoutedEventArgs e)
        {
            Tile clickedButton = (Tile)sender;
            string g = clickedButton.Count;

            foreach (DataRow row in dt2.Rows)
            {
                string tname = row["count"].ToString();
                if (tname == g)
                {

                    row["bl"] = "0.0";
                    row["tileh"] = "60";
                    row["tilew"] = "800";
                    row["column1w"] = "150";
                    row["column2w"] = "650";
                    row["roww"] = "30";
                    row["imags"] = "50";
                    //dispatcherTimer2.Stop();
                    //VisitPlanItems.DataContext = dt2.DefaultView;
                }
            }
        }

        private void tiles_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            /*  Tile clickedButton = (Tile)sender;
              string g = clickedButton.Count;

              foreach (DataRow row in dt2.Rows)
              {
                  string tname = row["count"].ToString();
                  if (tname == g)
                  {

                      row["bl"] = "8.0";
                      row["tileh"] = "100";
                      row["tilew"] = "800";
                      row["column1w"] = "150";
                      row["column2w"] = "650";
                      row["roww"] = "50";
                      row["imags"] = "100";
                     // VisitPlanItems.DataContext = dt2.DefaultView;
                  }
                  else 
                  {
                      row["bl"] = "0.0";
                      row["tileh"] = "60";
                      row["tilew"] = "800";
                      row["column1w"] = "150";
                      row["column2w"] = "650";
                      row["roww"] = "30";
                      row["imags"] = "50";
                      //VisitPlanItems.DataContext = dt2.DefaultView;
                  }
                  //dispatcherTimer2.Start();
                  VisitPlanItems.DataContext = dt2.DefaultView;
              }*/
        }

        private void tiles_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /* Tile clickedButton = (Tile)sender;
             string g = clickedButton.Count;

             foreach (DataRow row in dt2.Rows)
             {
                 string tname = row["count"].ToString();
                 if (tname == g)
                 {

                     row["bl"] = "0.0";
                     row["tileh"] = "60";
                     row["tilew"] = "800";
                     row["column1w"] = "150";
                     row["column2w"] = "650";
                     row["roww"] = "30";
                     row["imags"] = "50";
                     //dispatcherTimer2.Stop();
                     VisitPlanItems.DataContext = dt2.DefaultView;
                 }
             }*/
        }

        private void tiles_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Tile clickedButton = (Tile)sender;
            string g = clickedButton.Count;
            foreach (DataRow row in dt2.Rows)
            {
                string tname = row["count"].ToString();
                if (tname == g)
                {
                    string tname2 = row["Name"].ToString();
                }
            }
        }
        
        #endregion<<tiles>>

        #region<<buttons>>

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                this.textBox1.Text = dialog.SelectedPath;

            pkg_directory = textBox1.Text;
            Properties.Settings.Default.pkg_folder = pkg_directory;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                this.textBox2.Text = dialog.SelectedPath;

            iso_directory = textBox2.Text;
            Properties.Settings.Default.iso_folder = iso_directory;
            Properties.Settings.Default.Save();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                this.textBox.Text = dialog.SelectedPath;

            game_directory = textBox.Text;
            Properties.Settings.Default.game_folder = game_directory;
            Properties.Settings.Default.Save();
        }


        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (ctheme == "BaseDark")
            {
                //button7.Content = "Dark Theme";
                ctheme = "BaseLight";
            }
            else
            {
                //button7.Content = "Light Theme";
                ctheme = "BaseDark";
            }
            ChangeAppStyle();
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {

            color = ((ListBoxItem)listBox1.SelectedValue).Content.ToString();
            ChangeAppStyle();
        }

        private void dataGrid1_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Process.Start(appPath + "\\");
        }



        #endregion<<buttons>>

        #region<<Drop>>

        private void StetDialog(CustomDialog Dialog)
        {
            Grid ng = new Grid();


            System.Windows.Controls.Label nlabel = new System.Windows.Controls.Label() { Content = "Directory Not Found" };
            nlabel.SetValue(Canvas.LeftProperty, 50d);
            nlabel.SetValue(Canvas.TopProperty, 40d);
            nlabel.FontSize = 20;

            /*TextBox ntextBox = new TextBox() { Text = "Directory Not Found" };
            ntextBox.SetValue(Canvas.LeftProperty, 100d);
            ntextBox.SetValue(Canvas.TopProperty, 80d);*/

            System.Windows.Controls.Button cancelBtn = new System.Windows.Controls.Button { Content = "Cancel", Margin = new Thickness(5) };
            cancelBtn.SetValue(Canvas.LeftProperty, 400d);
            cancelBtn.SetValue(Canvas.TopProperty, 40d);

            cancelBtn.Width = 70d;
            //cancelBtn.Height = 70d;
            cancelBtn.Click += (s, e) =>
        {
            DialogManager.HideMetroDialogAsync(this, Dialog);
        };

            Canvas canvas = new Canvas();
            canvas.Children.Add(nlabel);
            //canvas.Children.Add(ntextBox);
            canvas.Children.Add(cancelBtn);

            Dialog.Content = canvas;
        }


        CustomDialog nd = new CustomDialog();


        private void textBox1_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Handled = true;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        private void textBox1_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);


            string s2 = "";

            foreach (string File in FileList)
                s2 = s2 + " " + File;
            if (Directory.Exists(s2))
            {
                textBox1.Text = s2;

                pkg_directory = textBox1.Text;
                Properties.Settings.Default.pkg_folder = pkg_directory;
                Properties.Settings.Default.Save();
            }
            else
            {
                StetDialog(nd);
                ShowDialog(nd);
            }
        }

       

        private void textBox2_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Handled = true;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        private void textBox2_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);


            string s2 = "";

            foreach (string File in FileList)
                s2 = s2 + " " + File;
            if (Directory.Exists(s2))
            { 
            textBox2.Text = s2;

                iso_directory = textBox2.Text;
                Properties.Settings.Default.iso_folder = iso_directory;
                Properties.Settings.Default.Save();
            }
            else
            {
                StetDialog(nd);
                ShowDialog(nd);
            }
        }

        private void textBox_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Handled = true;
            else
                e.Effects = System.Windows.DragDropEffects.None;
        }

        private void textBox_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);


            string s2 = "";

            foreach (string File in FileList)
                s2 = s2 + " " + File;
            if (Directory.Exists(s2))
            {
                textBox.Text = s2;

                game_directory = textBox.Text;
                Properties.Settings.Default.game_folder = game_directory;
                Properties.Settings.Default.Save();
            }
            else
            {
                StetDialog(nd);
                ShowDialog(nd);
            }
        }




        #endregion<<Drop>>

        #region<<updater>>

        private void buttonud_Click(object sender, RoutedEventArgs e)
        {
            //  Set up the url of the xml file containing the game updates
            webUrl = "https://a0.ww.np.dl.playstation.net/tpl/np/" + textBoxud.Text + "/" + textBoxud.Text + "-ver.xml";
            //  Update status..
            //lbl_Url.Text = "fetching... " + webUrl;
            //  Needed to allow the certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //  Make a new webClient to get the xml file and get/parse it on another thread.
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += HttpsCompleted;
            wc.DownloadStringAsync(new Uri(webUrl));
        }

        private void dlbuttonud_Click(object sender, RoutedEventArgs e)
        {
            //  Set up the url of the xml file containing the game updates
            webUrl = "https://a0.ww.np.dl.playstation.net/tpl/np/" + textBoxud.Text + "/" + textBoxud.Text + "-ver.xml";
            //  Update status..
            //lbl_Url.Text = "fetching... " + webUrl;
            //  Needed to allow the certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //  Make a new webClient to get the xml file and get/parse it on another thread.
            WebClient wc = new WebClient();
            string lfile = textBoxud.Text + "_" + DateTime.Now.ToString("yyyyMMddHH") + "-ver.xml";
            wc.DownloadFile(webUrl, "XML/" + lfile);
            string[] array1 = Directory.GetFiles("XML/");

            foreach (string name in array1)
            {
                if (name.Contains(textBoxud.Text) && name.Contains("-ver.xml") && name != lfile)
                {
                    bool a = FileEquals(name, "XML/" + lfile);

                    if (a = true)
                    {


                    }
                }
            }
        }

       static bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void HttpsCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            dtud.Rows.Clear();
            textBox3.Text = "";
            //  Check if it downloaded the xml file ok
            if (e.Error == null)
            {
                //  Make an new XmlDocument
                XmlDocument xdoc = new XmlDocument();
                // Gotta love try :P
                try
                {
                    //  Load the xml file from e.Result into the XmlDoc
                    xdoc.LoadXml(e.Result);
                    //  Make nodeList to hold all the Package Elements
                    XmlNodeList elemList = xdoc.GetElementsByTagName("package");
                    //  Loop through the list and get each entry
                    
                    for (int i = 0; i < elemList.Count; i++)
                    {
                        //  Obvious?
                        string updateVersion = elemList[i].Attributes["version"].Value;
                        string updateSize = elemList[i].Attributes["size"].Value;
                        string updateSHA1 = elemList[i].Attributes["sha1sum"].Value.ToUpper();
                        string updateUrl = elemList[i].Attributes["url"].Value;
                        string updateFWVer = elemList[i].Attributes["ps3_system_ver"].Value;
                        string sub = elemList[i].Attributes["url"].Value;


                        updateFWVer = updateFWVer.Substring(0, 5);
                        if (updateFWVer.StartsWith("0"))
                        {
                          updateFWVer = updateFWVer.TrimStart('0') ;

                        }
                        

                        int indexof = sub.LastIndexOf('-');
                        sub = sub.Substring(0, indexof);

                        string name = sub.Substring(sub.LastIndexOf("/") + 2) + ".pkg";

                        sub = sub.Substring(sub.LastIndexOf("-")+2);
                        sub = sub.Substring(0, 2) + "." + sub.Substring(2);
                        if (sub.StartsWith("0"))
                        {
                            sub = sub.TrimStart('0');

                        }
                        //updateVersion = "   " + updateVersion ;
                        if (updateVersion.StartsWith("0"))
                        {
                            updateVersion = updateVersion.TrimStart('0');

                        }


                        //  Add it to the dataGridView
                       
                        DataRow dr = dtud.NewRow();
                        dr["Version"] = updateVersion;
                        dr["Size"] = "  Download  " + SizeSuffix(Convert.ToInt64(updateSize));
                        dr["SHA1"] = updateSHA1;
                        //;
                        dr["FWVer"] = updateFWVer;
                        dr["Url"] = updateUrl;
                        dr["Sub"] = sub;
                        dr["Name"] = name;
                        dtud.Rows.Add(dr);

                        dataGrid.DataContext = dtud.DefaultView;
                        //dataGrid. .Add(updateVersion, updateSize, updateSHA1, updateUrl, updateFWVer);
                    }
                    //  Update the Status labels
                    textBox3.Text = xdoc.SelectSingleNode("/titlepatch/tag/package/paramsfo/TITLE").InnerText;
                    //lbl_Url.Text = webUrl;
                }
                //  If error..
                catch (Exception a)
                {
                    //  Show error
                    //showError();
                }
            }
            else // If error..
            {
                //  Show error
                //showError();
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            object item = dataGrid.SelectedItem;
            string ID = (dataGrid.SelectedCells[4].Column.GetCellContent(item) as TextBlock).Text;
            // Some operations with this row
        }

        private async Task LoadingDialogAsync()
        {
            var controller = await this.ShowProgressAsync("Logging In...", "");
        }
        private void StetDialogp(CustomDialog Dialog)
        {
            Grid ng = new Grid();


            System.Windows.Controls.Label nlabel = new System.Windows.Controls.Label() { Content = "Directory Not Found" };
            nlabel.SetValue(Canvas.LeftProperty, 50d);
            nlabel.SetValue(Canvas.TopProperty, 40d);
            nlabel.FontSize = 20;

            /*TextBox ntextBox = new TextBox() { Text = "Directory Not Found" };
            ntextBox.SetValue(Canvas.LeftProperty, 100d);
            ntextBox.SetValue(Canvas.TopProperty, 80d);*/

            System.Windows.Controls.Button cancelBtn = new System.Windows.Controls.Button { Content = "Cancel", Margin = new Thickness(5) };
            cancelBtn.SetValue(Canvas.LeftProperty, 400d);
            cancelBtn.SetValue(Canvas.TopProperty, 40d);

            cancelBtn.Width = 70d;
            //cancelBtn.Height = 70d;
            cancelBtn.Click += (s, e) =>
            {
                DialogManager.HideMetroDialogAsync(this, Dialog);
              
            };

            Canvas canvas = new Canvas();
            canvas.Children.Add(nlabel);
            //canvas.Children.Add(ntextBox);
            canvas.Children.Add(cancelBtn);

            Dialog.Content = canvas;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object item = dataGrid.SelectedItem;
            int rid = dataGrid.SelectedIndex;
            string ID = (dataGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
            string name = (dataGrid.SelectedCells[7].Column.GetCellContent(item) as TextBlock).Text;

            downloader dl = new downloader(ID, "Updates/" + name);
           
            dl.Show();


            
        }




        #endregion<<updater>>

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.metroAnimatedTabControl.Visibility = Visibility.Visible;
            //this.gridInfo1.Visibility = Visibility.Hidden;
            this.gridInfo2.Visibility = Visibility.Hidden;
            this.gridInfo3.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //this.gridInfo1.Visibility = Visibility.Visible;
            this.metroAnimatedTabControl.Visibility = Visibility.Hidden;
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lbtest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView item1 = this.lbtest.SelectedItem as DataRowView;
            
            if (item1 != null)
            {
                Object[] items2 = item1.Row.ItemArray;
                string item = items2[4].ToString();
                item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                lvpkginfo.Items.Clear();
                int n = 0;
                while(n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    string t = items2[n].ToString();
                    lvpkginfo.Items.Add(t1[n] + t );
                    n++;
                }

                /*Please note you can always do this another methid just get the sfo somehow so we can work with it*/

                #region << pkg2sfo >>

                try
                {
                    //we need the item info 
                    //mainly we need to items location

                    /*This can be used to decrypt a pkg file*/
                    PS3gbs.pkg2sfo PKGSFO = new PS3gbs.pkg2sfo();
                    PKGSFO.DecryptPKGFile(appPath + "/Pkg/" + items2[0].ToString());//get pkg name

                    //this will actually decrypt the item so we will need to clean the folder after moving the sfo 
                    //this could be cleaner also we can actually instead of exacting the pkg we could always load sfo from the buffer in the exact code but this will work for now
                    if (Directory.Exists(appPath + @"\temp\pkg"))
                    {
                        if (!Directory.Exists("Work"))
                        {
                            //create working directory
                            Directory.CreateDirectory("Work");
                        }
                        //copy sfo to working folder
                        File.Copy(appPath + @"\temp\pkg\" + items2[0].ToString().Replace(".pkg", "") + @"\PARAM.SFO", appPath + @"\Work\PARAM.SFO", true);

                        //Directory.Delete(appPath + @"\temp\pkg");
                        DeleteDirectory(appPath + @"\temp\pkg");

                        
                    }
                }
                catch(Exception ex)
                {
                    //this will propably break the code might need to be tweaked a bit 
                }

                #region << PARAM.SFO >>

                //this code will always work ! 
                Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO(appPath + @"\Work\PARAM.SFO");
                foreach (var psfoitem in sfo.Tables)
                {
                    lvpkgsfo.Items.Add(psfoitem.Name + " : " + psfoitem.Value);
                }

                #endregion << PARAM.SFO >>

                #endregion << pkg2sfo >> 

            }
        }
        public static void DeleteDirectory(string target_dir)
        {
            try
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target_dir, false);
            }
            catch(Exception ex)
            {

            }
        }
    }
    
}
