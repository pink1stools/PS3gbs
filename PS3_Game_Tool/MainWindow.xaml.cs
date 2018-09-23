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
using System.Windows.Automation.Peers;

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

            //set the app path correctly from initilizastion 
            appPath = appPath.Replace("PS3gbs.exe", "");

            //Create Folders and Directories Used By the app
            CreateBaseDirecotries();

            //rss();

            //i would also reccomend running this in a seperate thread so we can boot the app and maybe even report progress
            /*new Thread(delegate () 
             {
                 pkg_folder();
             }).Start();*/
            pkg_folder();
            open_iso_folder();
            open_game_folder();
            //SetupFolderWatchers
            SetupWatchers();
            StartWeb();
            //pkgwb.Navigate("http://www.psx-place.com");
            //trellowb.Navigate("https://trello.com/b/2MJwFHNs/sony-stuff");
        }

        private void SetupWatchers()
        {
            #region << PKG Watcher >>

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = appPath + pkg_directory;//iso path //pkg path //sfo path in working
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName; //look for last time change
            watcher.Filter = "*.*"; //filter extentions
            watcher.Changed += delegate
            {
                pkg_folder();
            }; //fire up the reload method
            watcher.EnableRaisingEvents = true; //bviously xD

            #endregion << PKG Watcher >>

            #region << ISO Watcher >>

            FileSystemWatcher iso_watcher = new FileSystemWatcher();
            iso_watcher.Path = appPath + pkg_directory;//iso path //pkg path //sfo path in working
            iso_watcher.NotifyFilter = NotifyFilters.LastWrite; //look for last time change
            iso_watcher.Filter = "."; //filter extentions
            iso_watcher.Changed += delegate
            {
                open_iso_folder();
            }; //fire up the reload method
            iso_watcher.EnableRaisingEvents = true; //Obviously xD

            #endregion << ISO Watcher >>


            #region << Game Watcher >>

            FileSystemWatcher Game_watcher = new FileSystemWatcher();
            Game_watcher.Path = appPath + pkg_directory;//iso path //pkg path //sfo path in working
            Game_watcher.NotifyFilter = NotifyFilters.LastWrite; //look for last time change
            Game_watcher.Filter = "."; //filter extentions
            Game_watcher.Changed += delegate
            {
                open_game_folder();
            }; //fire up the reload method
            Game_watcher.EnableRaisingEvents = true; //Obviously xD

            #endregion << Game Watcher >>


        }

        private void CreateBaseDirecotries()
        {
            if (!Directory.Exists(appPath + pkg_directory) && pkg_directory == "/PKG")
            {
                Directory.CreateDirectory(appPath + pkg_directory);
            }
            if (!Directory.Exists(appPath + iso_directory) && iso_directory == "/ISO")
            {
                Directory.CreateDirectory(appPath + iso_directory);
            }
            if (!Directory.Exists(appPath + "/SFO"))
            {
                Directory.CreateDirectory(appPath + "/SFO");
            }
            if (!Directory.Exists(appPath + "/icons"))
            {
                Directory.CreateDirectory(appPath + "/icons");
            }

            //add misc folders here 
            if (!Directory.Exists(appPath + game_directory) && pkg_directory == "/Game Files")
            {
                Directory.CreateDirectory(appPath + game_directory);
            }
        }

        private void StartWeb()
        {
            if(WebRequestTest() == true)
            {
                pkgwb.Navigate("http://www.psx-place.com");
                //trellowb.Navigate("https://trello.com/b/2MJwFHNs/sony-stuff");
                
            }

        }

        public static bool WebRequestTest()
        {
            string url = "http://www.google.com";
            try
            {
                System.Net.WebRequest myRequest = System.Net.WebRequest.Create(url);
                System.Net.WebResponse myResponse = myRequest.GetResponse();
            }
            catch (System.Net.WebException)
            {
                return false;
            }
            return true;
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

            if (color == null)
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
        private void pkg_folder()
        {
            try
            {


                i = 0;

                dt2.Rows.Clear();
                //should be wise to do this
                dtpkg2.Clear();
                dtpkg.Clear();
                /*xDPx*/
                //clean data context each time
                System.Windows.Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
    (ThreadStart)delegate
    {
        lbtest.DataContext = null;

        /*https://stackoverflow.com/a/22528015/3578728*/

    });





                appPath = appPath.Replace("PS3gbs.exe", "");
                pkg = appPath;
                //rename();

                /*xDPx fix
                 PKG Folder check*/

                if (!Directory.Exists(appPath + "/PKG"))
                {
                    Directory.CreateDirectory(appPath + "/PKG");
                }

                dinfo = new DirectoryInfo(appPath + "/PKG");
                Files = dinfo.GetFiles("*.pkg");



                foreach (FileInfo file in Files)
                {
                    try
                    {

                        string tname = file.Name.Replace(".pkg", "");
                        if (!File.Exists("SFO/" + tname + ".SFO") || !File.Exists("icons/" + tname + ".PNG"))
                        {



                            pkg PKGICO = new pkg();
                            PKGICO.read_header(file.FullName, "ICON0.PNG");

                            pkg PKGSFO = new pkg();
                            PKGSFO.read_header(file.FullName, "PARAM.SFO");//get pkg name

                            if (File.Exists("PARAM.SFO"))
                            {

                                if (File.Exists("SFO/" + tname + ".SFO"))
                                {

                                    File.Delete("SFO/" + tname + ".SFO");
                                }

                                File.Move("PARAM.SFO", "SFO/" + tname + ".SFO"); // Try to move

                            }
                            if (File.Exists("ICON0.PNG"))
                            {

                                if (File.Exists("icons/" + tname + ".PNG"))
                                {
                                    File.Delete("icons/" + tname + ".PNG");
                                }
                                File.Move("ICON0.PNG", "icons/" + tname + ".PNG"); // Try to move

                            }

                        }


                        FileStream pkgFilehead = File.Open(file.FullName, FileMode.Open);
                        byte[] testmagic = new byte[0x05];
                        pkgFilehead.Read(testmagic, 0, 0x05);
                        pkgFilehead.Close();
                        byte[] magic1 = new byte[] { 0x7F, 0x50, 0x4B, 0x47, 0x00 };
                        byte[] magic2 = new byte[] { 0x7F, 0x50, 0x4B, 0x47, 0x80 };
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
                                //dataGrid2.DataContext = dtpkg.DefaultView;

                                string iconpath = s.Replace(".pkg", ".PNG");
                                iconpath = appPath + "icons/" + iconpath;
                                if (!File.Exists(iconpath))
                                {
                                    iconpath = appPath + "tools/icons/download.png";
                                }
                                
                                string tid = scid.Remove(0, 7);
                                tid = tid.Remove(9, 20);
                                cupdates(tid, i);


                                DataRow dr = dtpkg2.NewRow();
                                dr["Name"] = s;
                                dr["CID"] = scid;
                                dr["type"] = pkgtype;
                                dr["size"] = sz;
                                dr["icon"] = iconpath;
                                dr["tool"] = "  " + s + "  " + scid + "  " + sz;
                                dr["count"] = i;
                                dr["bl"] = tid;
                                dr["tileh"] = "50";
                                dr["tilew"] = "800";
                                dr["column1w"] = "150";
                                dr["column2w"] = "650";
                                dr["roww"] = "25";
                                dr["imags"] = "50";
                                //dr["text1s"] = upc;
                                dr["text2s"] = "true";


                                
                                //dr["text1s"] = upc;
                                dtpkg2.Rows.Add(dr);

                                i++;
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        //invalid pkg or invalid item 
                    }


                }

                System.Windows.Application.Current.Dispatcher.Invoke(
                               DispatcherPriority.Normal,
                               (ThreadStart)delegate
                               {
                                   try
                                   {
                                       //i think its a safer bet to remove data context before adding into to it
                                       VisitPlanItems.DataContext = null;
                                       lbtest.DataContext = null;
                                       lvpkginfo.DataContext = null;

                                       VisitPlanItems.DataContext = dtpkg2.DefaultView;
                                       lbtest.DataContext = dtpkg2.DefaultView;
                                       lvpkginfo.DataContext = dtpkg2.DefaultView;

                                       /*Add a refresh item 
                                         Since we are running another thread call the refresh method
                                        */
                                       VisitPlanItems.Items.Refresh();
                                       lbtest.Items.Refresh();
                                       lvpkginfo.Items.Refresh();
                                   }
                                   catch (Exception ex)
                                   {

                                   }
                               });



            }
            catch (Exception ex)
            {
                /*added a try catch for this enitire method as well as something is causing it to fall over on some of the pkg's i tested */
            }
        }

        private void open_iso_folder()
        {
            int i2 = 0;
            appPath = appPath.Replace("PS3gbs.exe", "");
            if(iso_directory == "ISO")
            {
                iso_directory = appPath + iso_directory;
            }
            dinfo = new DirectoryInfo(iso_directory);
            Files2 = dinfo.GetFiles("*.iso");

            foreach (FileInfo file in Files2)
            {
                string iname = file.Name.Replace("iso", "png");
                string iname1 = file.Name.Replace("iso", "SFO");
                /*Process p = new Process();
                 ProcessStartInfo psi = new ProcessStartInfo();
                 psi.CreateNoWindow = false;
                 psi.UseShellExecute = false;
                 psi.FileName = "CMD.EXE";
                 psi.Arguments = "/k 7z.exe e " + '"' + iso_directory + "/" + file +  '"' + " -o" + "icons " + '"' + "PS3_GAME\\ICON0.png" + '"';
                 Process exeProcess = Process.Start(psi);
                 p.WaitForExit(); */

                
                if (!File.Exists(appPath + "icons/" + iname) || !File.Exists(appPath + "SFO/" + iname1))
                {
                    
                    if (File.Exists(appPath + "SFO/PARAM.SFO"))
                    {

                        File.Delete(appPath + "SFO/PARAM.SFO");
                    }
                    if (File.Exists(appPath + "icons/ICON0.png"))
                    {

                        File.Delete(appPath + "icons/ICON0.png");
                    }
                    
                    ProcessStartInfo startInfo1 = new ProcessStartInfo();
                    startInfo1.CreateNoWindow = true;
                    startInfo1.UseShellExecute = false;
                    startInfo1.FileName = "7z.exe";
                    startInfo1.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo1.Arguments = "e " + '"' + iso_directory + "/" + file + '"' + " -o" + "SFO " + '"' + "PS3_GAME\\PARAM.SFO" + '"';
                    Process exeProcess = Process.Start(startInfo1);
                    exeProcess.WaitForExit();


                    if (File.Exists(appPath + "SFO/" + iname1))
                    {

                        File.Delete(appPath + "SFO/" + iname1);

                    }
                    File.Move("SFO/PARAM.SFO", "SFO/" + iname1);


                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = "7z.exe";
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = "e " + '"' + iso_directory + "/" + file + '"' + " -o" + "icons " + '"' + "PS3_GAME\\ICON0.png" + '"';
                    Process exeProcess2 = Process.Start(startInfo);
                    exeProcess2.WaitForExit();
                    
                    if (File.Exists(appPath + "icons/" + iname))
                    {

                        File.Delete(appPath + "icons/" + iname);

                    }
                    File.Move("icons/icon0.png", "icons/" + iname);


                    
                }

                string sfoname = file.Name.Replace("iso", "SFO");

                if (File.Exists(appPath + "SFO/" + sfoname))
                {

                    
                    //Add to list
                    #region << PARAM.SFO >>
                    string path = appPath + "SFO/" + sfoname;
                    //this code will always work ! 
                    Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO(path);
                    //string cid = sfo.Tables[0];

                    lvisosfo.Items.Clear();
                    
                    #endregion << PARAM.SFO >>
                    string isos = file.Name;
                    string isoscid = sfo.TitleID;
                    string isosz = SizeSuffix(file.Length);

                    DataRow dr1 = dtiso.NewRow();
                        dr1["IsSelected"] = false;
                        dr1["Name"] = isos;
                        dr1["CID"] = isoscid;
                        dr1["type"] = null;
                        dr1["Size"] = isosz;
                        dtiso.Rows.Add(dr1);

                    //dataGrid2.DataContext = dtpkg.DefaultView;

                    string iconpath = appPath + "icons/" + iname;
                    
                    if (!File.Exists(iconpath))
                    {
                        iconpath = appPath + "icons/download.png";
                    }



                    cisoupdates(isoscid, i2);
                    DataRow dr = dtiso2.NewRow();
                    dr["Name"] = isos;
                    dr["CID"] = isoscid;
                    dr["type"] = 0;
                    dr["size"] = isosz;
                    dr["icon"] = iconpath;
                    dr["tool"] = "  " + isos + "  " + isoscid + "  "  ;
                    dr["count"] = i2;
                    dr["bl"] = "0.0";
                    dr["tileh"] = "50";
                    dr["tilew"] = "800";
                    dr["column1w"] = "150";
                    dr["column2w"] = "650";
                    dr["roww"] = "25";
                    dr["imags"] = "50";
                    //dr["text1s"] = upc;
                    dr["text2s"] = "true";



                    //dr["text1s"] = upc;
                    dtiso2.Rows.Add(dr);
                    

                    //VisitPlanItems.DataContext = dtpkg2.DefaultView;
                    lbisotest.DataContext = dtiso2.DefaultView;
                    lvisoinfo.DataContext = dtiso2.DefaultView;

                    
                    i2++;
                }

            }


       
    }

        private void open_game_folder()
        {
            int i3 = 0;
            appPath = appPath.Replace("PS3gbs.exe", "");
            if (game_directory == "Game Files")
            {
                game_directory = appPath + game_directory;
            }
            dinfo = new DirectoryInfo(game_directory);

            directories = Directory.GetDirectories(game_directory);

            foreach (string directoriy in directories)
            {
                if (File.Exists(directoriy + "\\PS3_GAME\\PARAM.SFO") && File.Exists(directoriy + "\\PS3_GAME\\ICON0.png"))
                {
                    string dname = directoriy.Replace(game_directory, "");
                    File.Copy(directoriy + "\\PS3_GAME\\PARAM.SFO", appPath + "SFO/" + dname + ".SFO", true);
                    
                    File.Copy(directoriy + "\\PS3_GAME\\ICON0.png", appPath + "icons/" + dname + ".PNG", true);

                    

                    if (File.Exists(appPath + "SFO/" + dname + ".SFO"))
                    {


                        //Add to list
                        #region << PARAM.SFO >>
                        string path = appPath + "SFO/" + dname + ".SFO";
                        //this code will always work ! 
                        Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO(path);
                        //string cid = sfo.Tables[0];

                        lvisosfo.Items.Clear();

                        #endregion << PARAM.SFO >>
                        string games = dname.Trim('\\');
                        string gamescid = sfo.TitleID;
                        string gamesz = SizeSuffix(GetDirectorySize( directoriy));

                        DataRow dr1 = dtgame.NewRow();
                        dr1["IsSelected"] = false;
                        dr1["Name"] = games;
                        dr1["CID"] = gamescid;
                        dr1["type"] = null;
                        dr1["Size"] = gamesz;
                        dtgame.Rows.Add(dr1);

                        //dataGrid2.DataContext = dtpkg.DefaultView;

                        string iconpath = appPath + "icons/" + dname + ".PNG";

                        if (!File.Exists(iconpath))
                        {
                            iconpath = appPath + "icons/download.png";
                        }



                        cgameupdates(gamescid, i3);
                        DataRow dr = dtgame2.NewRow();
                        dr["Name"] = games;
                        dr["CID"] = gamescid;
                        dr["type"] = 0;
                        dr["size"] = gamesz;
                        dr["icon"] = iconpath;
                        dr["tool"] = "  " + games + "  " + gamescid + "  ";
                        dr["count"] = i3;
                        dr["bl"] = "0.0";
                        dr["tileh"] = "50";
                        dr["tilew"] = "800";
                        dr["column1w"] = "150";
                        dr["column2w"] = "650";
                        dr["roww"] = "25";
                        dr["imags"] = "50";
                        //dr["text1s"] = upc;
                        dr["text2s"] = "true";



                        //dr["text1s"] = upc;
                        dtgame2.Rows.Add(dr);


                        //VisitPlanItems.DataContext = dtpkg2.DefaultView;
                        lbgametest.DataContext = dtgame2.DefaultView;
                        lvgameinfo.DataContext = dtgame2.DefaultView;


                        i3++;
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
                            updateFWVer = updateFWVer.TrimStart('0');

                        }


                        int indexof = sub.LastIndexOf('-');
                        sub = sub.Substring(0, indexof);

                        string name = sub.Substring(sub.LastIndexOf("/") + 2) + ".pkg";

                        sub = sub.Substring(sub.LastIndexOf("-") + 2);
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

        private void cupdates(string cid, int count)
        {
            //  Set up the url of the xml file containing the game updates
            webUrl = "https://a0.ww.np.dl.playstation.net/tpl/np/" + cid + "/" + cid + "-ver.xml";
            //  Update status..
            //lbl_Url.Text = "fetching... " + webUrl;
            //  Needed to allow the certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //  Make a new webClient to get the xml file and get/parse it on another thread.
            WebClient wc = new WebClient();
            // wc.DownloadStringCompleted += cupHttpsCompleted;

            // wc.DownloadStringAsync(new Uri(webUrl));

            Nullable<int> upc = null;
            wc.DownloadStringCompleted += (sender1, e1) =>
            {


                if (e1.Error == null)
                {
                    //  Make an new XmlDocument
                    XmlDocument xdoc = new XmlDocument();
                    // Gotta love try :P
                    //  Load the xml file from e.Result into the XmlDoc

                    try
                    {
                        xdoc.LoadXml(e1.Result);
                        //  Make nodeList to hold all the Package Elements
                        XmlNodeList elemList = xdoc.GetElementsByTagName("package");
                        //  Loop through the list and get each entry

                        if (elemList.Count != 0) // if id==2
                        {
                            upc = elemList.Count;                   //break; break or not depending on you
                        }
                        else
                        {
                            upc = null;
                        }


                        
                        foreach (DataRow dr in dtpkg2.Rows) // search whole table
                        {
                            if (Convert.ToInt32(dr["count"]) == count) // if id==2
                            {
                                dr["text1s"] = upc; //change the name
                                                               //break; break or not depending on you
                            }

                        }
                        if(elemList.Count == 0)
                        {
                           

                        }
                    }

                    catch (Exception a)
                    {

                        //  Show error
                        //showError();
                    }

                }

            };

            wc.DownloadStringAsync(new Uri(webUrl));

        }



        private void cisoupdates(string cid, int count)
        {
            //  Set up the url of the xml file containing the game updates
            webUrl = "https://a0.ww.np.dl.playstation.net/tpl/np/" + cid + "/" + cid + "-ver.xml";
            //  Update status..
            //lbl_Url.Text = "fetching... " + webUrl;
            //  Needed to allow the certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //  Make a new webClient to get the xml file and get/parse it on another thread.
            WebClient wc = new WebClient();
            // wc.DownloadStringCompleted += cupHttpsCompleted;

            // wc.DownloadStringAsync(new Uri(webUrl));

            Nullable<int> upc = null;
            wc.DownloadStringCompleted += (sender1, e1) =>
            {


                if (e1.Error == null)
                {
                    //  Make an new XmlDocument
                    XmlDocument xdoc = new XmlDocument();
                    // Gotta love try :P
                    //  Load the xml file from e.Result into the XmlDoc

                    try
                    {
                        xdoc.LoadXml(e1.Result);
                        //  Make nodeList to hold all the Package Elements
                        XmlNodeList elemList = xdoc.GetElementsByTagName("package");
                        //  Loop through the list and get each entry

                        if (elemList.Count != 0) // if id==2
                        {
                            upc = elemList.Count;                   //break; break or not depending on you
                        }
                        else
                        {
                            upc = null;
                        }



                        foreach (DataRow dr in dtiso2.Rows) // search whole table
                        {
                            if (Convert.ToInt32(dr["count"]) == count) // if id==2
                            {
                                dr["text1s"] = upc; //change the name
                                                    //break; break or not depending on you
                            }

                        }
                        if (elemList.Count == 0)
                        {


                        }
                    }

                    catch (Exception a)
                    {

                        //  Show error
                        //showError();
                    }

                }

            };

            wc.DownloadStringAsync(new Uri(webUrl));

        }


        private void cgameupdates(string cid, int count)
        {
            //  Set up the url of the xml file containing the game updates
            webUrl = "https://a0.ww.np.dl.playstation.net/tpl/np/" + cid + "/" + cid + "-ver.xml";
            //  Update status..
            //lbl_Url.Text = "fetching... " + webUrl;
            //  Needed to allow the certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //  Make a new webClient to get the xml file and get/parse it on another thread.
            WebClient wc = new WebClient();
            // wc.DownloadStringCompleted += cupHttpsCompleted;

            // wc.DownloadStringAsync(new Uri(webUrl));

            Nullable<int> upc = null;
            wc.DownloadStringCompleted += (sender1, e1) =>
            {


                if (e1.Error == null)
                {
                    //  Make an new XmlDocument
                    XmlDocument xdoc = new XmlDocument();
                    // Gotta love try :P
                    //  Load the xml file from e.Result into the XmlDoc

                    try
                    {
                        xdoc.LoadXml(e1.Result);
                        //  Make nodeList to hold all the Package Elements
                        XmlNodeList elemList = xdoc.GetElementsByTagName("package");
                        //  Loop through the list and get each entry

                        if (elemList.Count != 0) // if id==2
                        {
                            upc = elemList.Count;                   //break; break or not depending on you
                        }
                        else
                        {
                            upc = null;
                        }



                        foreach (DataRow dr in dtgame2.Rows) // search whole table
                        {
                            if (Convert.ToInt32(dr["count"]) == count) // if id==2
                            {
                                dr["text1s"] = upc; //change the name
                                                    //break; break or not depending on you
                            }

                        }
                        if (elemList.Count == 0)
                        {


                        }
                    }

                    catch (Exception a)
                    {

                        //  Show error
                        //showError();
                    }

                }

            };

            wc.DownloadStringAsync(new Uri(webUrl));

        }


        

        #endregion<<updater>>

        
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
            catch (Exception ex)
            {

            }
        }

        #region<<pkg_buttons>>

        private void lbtest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView item1 = this.lbtest.SelectedItem as DataRowView;

            if (item1 != null)
            {
                pkgwb.Visibility = Visibility.Hidden;
                pkgwb.Dispose();
                Object[] items2 = item1.Row.ItemArray;
                string item = items2[4].ToString();
                item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                lvpkginfo.Items.Clear();
                int n = 0;
                while (n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    string t = items2[n].ToString();
                    lvpkginfo.Items.Add(t1[n] + t);
                    n++;
                }

                /*Please note you can always do this another methid just get the sfo somehow so we can work with it*/

                #region << pkg2sfo >>
                /*
                try
                {
                    //we need the item info 
                    //mainly we need to items location

                    //This can be used to decrypt a pkg file
                    PS3gbs.pkg2sfo PKGSFO = new PS3gbs.pkg2sfo();
                    PKGSFO.DecryptPKGFile(appPath + "/Pkg/" + items2[0].ToString(), "PARAM.SFO");//get pkg name

                    

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
                }*/

                #region << PARAM.SFO >>
                string path = appPath + @"\SFO\" + items2[0].ToString().Replace(".pkg", ".SFO");
                //this code will always work ! 
                Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO(path);
                lvpkgsfo.Items.Clear();
                foreach (var psfoitem in sfo.Tables)
                {
                    lvpkgsfo.Items.Add(psfoitem.Name + " : " + psfoitem.Value);
                }

                #endregion << PARAM.SFO >>

                #endregion << pkg2sfo >> 

            }
        }

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


        private void button_Copy3_Click(object sender, RoutedEventArgs e)
        {
            DataRowView item1 = this.lbtest.SelectedItem as DataRowView;

            if (item1 != null)
            {

                Object[] items2 = item1.Row.ItemArray;
                // string item = items2[4].ToString();
                //item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                //lvpkginfo.Items.Clear();
                int n = 0;
                while (n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    string t = items2[n].ToString();

                    //lvpkginfo.Items.Add(t1[n] + t);
                    if (n == 1)
                    {
                        string tid = t.Remove(0, 7);
                        tid = tid.Remove(9, 20);
                        textBoxud.Text = tid;
                        buttonud.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        metroAnimatedTabControl.SelectedValue = updtab;
                    }
                    n++;
                }
            }
        }


        #endregion<<pkg_buttons>>

        #region<<iso_buttons>>

        private void lbisotest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView item1 = this.lbisotest.SelectedItem as DataRowView;

            if (item1 != null)
            {
               
                Object[] items2 = item1.Row.ItemArray;
                string item = items2[4].ToString();
                item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                lvisoinfo.Items.Clear();
                int n = 0;
                while (n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    string t = items2[n].ToString();
                    lvisoinfo.Items.Add(t1[n] + t);
                    n++;
                }

                /*Please note you can always do this another methid just get the sfo somehow so we can work with it*/

                

                #region << PARAM.SFO >>
                string path = appPath + @"\SFO\" + items2[0].ToString();
                path = path.Substring(0, path.Length - 3);
                path = path + "SFO";
                //this code will always work ! 
                Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO(path);
                lvisosfo.Items.Clear();
                foreach (var psfoitem in sfo.Tables)
                {
                    lvisosfo.Items.Add(psfoitem.Name + " : " + psfoitem.Value);
                }

                #endregion << PARAM.SFO >>
 

            }
        }


        #endregion<<iso_buttons>>

        #region<<game_buttons>>


        private void lbgametest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataRowView item1 = this.lbgametest.SelectedItem as DataRowView;

            if (item1 != null)
            {

                Object[] items2 = item1.Row.ItemArray;
                string item = items2[4].ToString();
                item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                lvgameinfo.Items.Clear();
                int n = 0;
                while (n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    string t = items2[n].ToString();
                    lvgameinfo.Items.Add(t1[n] + t);
                    n++;
                }

                /*Please note you can always do this another methid just get the sfo somehow so we can work with it*/



                #region << PARAM.SFO >>
                string path = appPath + @"\SFO\" + items2[0].ToString();
               // path = path.Substring(0, path.Length - 3);
                path = path + ".SFO";
                //this code will always work ! 
                Param_SFO.PARAM_SFO sfo = new Param_SFO.PARAM_SFO(path);
                lvgamesfo.Items.Clear();
                foreach (var psfoitem in sfo.Tables)
                {
                    lvgamesfo.Items.Add(psfoitem.Name + " : " + psfoitem.Value);
                }

                #endregion << PARAM.SFO >>


            }
        }


        #endregion<<game_buttons>>


        private void plist_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*
            DataRowView item1 = this.lbtest.SelectedItem as DataRowView;

            if (item1 != null)
            {
                Object[] items2 = item1.Row.ItemArray;
                string item = items2[4].ToString();
                item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                lvpkginfo.Items.Clear();
                string t = "";
                int n = 0;
                while (n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    t = items2[n].ToString();
                    lvpkginfo.Items.Add(t1[n] + t);
                    n++;
                }


                foreach (DataRow row in dtpkg2.Rows)
                {
                    int tname = Convert.ToInt32(row["count"]);
                    if (tname == n)
                    {

                        row["bl"] = "8.0";
                        row["tileh"] = "150";
                        row["tilew"] = "150";
                        row["column1w"] = "150";
                        row["column2w"] = "650";
                        row["roww"] = "50";
                        row["imags"] = "100";
                        VisitPlanItems.DataContext = dtpkg2.DefaultView;
                        lbtest.DataContext = dtpkg2.DefaultView;
                        lvpkginfo.DataContext = dtpkg2.DefaultView;
                    }
                    else
                    {
                        row["bl"] = "0.0";
                        row["tileh"] = "100";
                        row["tilew"] = "100";
                        row["column1w"] = "150";
                        row["column2w"] = "650";
                        row["roww"] = "30";
                        row["imags"] = "50";
                        VisitPlanItems.DataContext = dtpkg2.DefaultView;
                        lbtest.DataContext = dtpkg2.DefaultView;
                        lvpkginfo.DataContext = dtpkg2.DefaultView;
                    }
                    VisitPlanItems.DataContext = dtpkg2.DefaultView;
                    lbtest.DataContext = dtpkg2.DefaultView;
                    lvpkginfo.DataContext = dtpkg2.DefaultView;


                    //dispatcherTimer2.Start();
                    //VisitPlanItems.DataContext = dt2.DefaultView;
                }


            }


            lbtest.SelectedItem = (sender as Border).DataContext;*/
            //string k = lbtest.SelectedItem
            //if (!lbtest.IsFocused)
            // lbtest.Focus();
        }

        private void plist_MouseExit(object sender, System.Windows.Input.MouseEventArgs e)
        {
            /*
            DataRowView item1 = this.lbtest.SelectedItem as DataRowView;

            if (item1 != null)
            {
                Object[] items2 = item1.Row.ItemArray;
                string item = items2[4].ToString();
                item = item.Replace(".PNG", ".PKG");
                //this.label4.Content = items2[0].ToString();
                lvpkginfo.Items.Clear();
                string t = "";
                int n = 0;
                while (n < 4)
                {
                    string[] t1 = new string[] { "Name: ", "CID:    ", "Type:  ", "Size:   " };
                    //Object nob = items2[n];
                    t = items2[n].ToString();
                    lvpkginfo.Items.Add(t1[n] + t);
                    n++;
                }


                foreach (DataRow row in dtpkg2.Rows)
                {
                    int tname = Convert.ToInt32(row["count"]);
                    if (tname == n)
                    {

                        row["bl"] = "8.0";
                        row["tileh"] = "150";
                        row["tilew"] = "150";
                        row["column1w"] = "150";
                        row["column2w"] = "650";
                        row["roww"] = "50";
                        row["imags"] = "100";
                        // VisitPlanItems.DataContext = dtpkg2.DefaultView;
                        // lbtest.DataContext = dtpkg2.DefaultView;
                        //lvpkginfo.DataContext = dtpkg2.DefaultView;
                    }
                    else
                    {
                        row["bl"] = "0.0";
                        row["tileh"] = "100";
                        row["tilew"] = "100";
                        row["column1w"] = "150";
                        row["column2w"] = "650";
                        row["roww"] = "30";
                        row["imags"] = "50";
                        //VisitPlanItems.DataContext = dtpkg2.DefaultView;
                        //lbtest.DataContext = dtpkg2.DefaultView;
                        //lvpkginfo.DataContext = dtpkg2.DefaultView;
                    }


                    //dispatcherTimer2.Start();
                    //VisitPlanItems.DataContext = dt2.DefaultView;
                }


            }

            VisitPlanItems.DataContext = dtpkg2.DefaultView;
            lbtest.DataContext = dtpkg2.DefaultView;
            lvpkginfo.DataContext = dtpkg2.DefaultView;

            lbtest.SelectedItem = (sender as Border).DataContext;*/
            //string k = lbtest.SelectedItem
            //if (!lbtest.IsFocused)
            // lbtest.Focus();
        }


        private static long GetDirectorySize(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            return di.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
        }



    }

}
