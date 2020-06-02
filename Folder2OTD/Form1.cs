using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;

namespace Folder2OTD
{
    public partial class Form1 : Form
    {
        public static int des;
        public static int des2;
        public static double operations;
        public static double operations2;
        public static bool IsTransp;
        public static string command;
        public static string folderblank;
        public static string formatdds;
        public static string[] directorio;
        public static bool isEmpty;
        public static string log;
        public static BetterFolderBrowser bfb = new BetterFolderBrowser();
        public static IEnumerable<String> imgfolders;

        public Form1()
        {
            InitializeComponent();
            isEmpty = true;
            cbTrasp.SelectedIndex = 2;
            cbFormat.SelectedIndex = 0;
            CheckForIllegalCrossThreadCalls = false;
            
            
        }

        private void selectFolder_Click(object sender, EventArgs e)
        {
            bfb.Multiselect = true;
            
            bfb.ShowDialog();
            ltbFolders.DataSource = bfb.SelectedFolders;
            directorio = bfb.SelectedFolders;

            if (bfb.SelectedFolder.Length >= 1)
            {
                isEmpty = false;
            }
            else
            {
                isEmpty = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();

        }

        public bool IsTransparent(Bitmap image)
        {
            if (cbTrasp.SelectedIndex == 0)
            {
                //if (image.ToString().Contains(".tga"))
                //{

                //}
                if ((image.Flags & 0x2) != 0)
                {
                    return IsTransp = true;
                }
                else
                {
                    return IsTransp = false;
                }
            }
            if (cbTrasp.SelectedIndex == 1)
            {
                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        if (image.GetPixel(x, y).A < 255)
                        {
                            return IsTransp = true;
                        }
                    }
                }
                return IsTransp = false;
            }

            if (cbTrasp.SelectedIndex == 2)
            {
                return IsTransp = false;
            }
            
            
            return IsTransp;

        }

        public string ReplaceName(string pngfile, bool Do)
        {

            string pngfole = Path.GetFileName(pngfile);
            string cur = Path.GetDirectoryName(pngfile);

            if (Do == true)
            {
                if (pngfole.Contains(" "))
                {
                    pngfole = pngfole.Replace(" ", "_");
                    return cur + "\\" + pngfole;
                }
                return cur + "\\" + pngfole;
            }
            else
            {
                return cur + "\\" + pngfole;
            }



        }

        public int CheckResolution(Bitmap image)
        {

            int mip = 0;
            if (image.Width < image.Height)
            {

                for (double i = 1; i < 32; i++)
                {
                    operations = image.Width / Math.Pow(2, i);

                    if (operations <= 2)
                    {
                        mip = Convert.ToInt32(i);
                        break;
                    }

                }


            }
            else
            {
                for (double c = 1; c < 32; c++)
                {
                    operations = image.Height / Math.Pow(2, c);

                    if (operations <= 2)
                    {
                        mip = Convert.ToInt32(c);
                        break;
                    }

                }
            }

            return mip;

        }

        public async Task OpenIVOTD(string[] documento)
        {
            string DXTformat = "";
            int mipmapotd = 0;
            int indexcount = 0;
            await Task.Run(() =>
             {
                 foreach (var carpeta in documento)
                 {
                     
                     StringBuilder otdbuilder = new StringBuilder();
                     otdbuilder.AppendLine("Version 13 30");
                     otdbuilder.AppendLine("{");
                     Infobox.AppendText("\nReading folder " + Path.GetFileName(carpeta) + "...");


                     if (checkRecursive.Checked)
                     {
                         imgfolders = Directory.EnumerateFiles(carpeta, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".jpg") || s.EndsWith(".dds"));
                     }
                     else
                     {
                         imgfolders = Directory.EnumerateFiles(carpeta, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".png") || s.EndsWith(".bmp") || s.EndsWith(".jpg") || s.EndsWith(".dds"));
                     }

                     int imgcount = imgfolders.Count();

                     if (imgcount == 0)
                     {
                         Infobox.AppendText("\n0 compatible textures, skipping...");

                     }
                     else
                     {
                         Infobox.AppendText("\n" + imgcount + " compatible textures.");

                     }

                     ltbFolders.SelectedIndex = indexcount++;
                     foreach (var imgfiles in imgfolders)
                     {

                         if (imgfolders?.Any() != true)
                         {


                         }
                         else
                         {
                             string filename = Path.GetFileName(imgfiles);
                             string filename2 = Path.GetFileNameWithoutExtension(imgfiles);
                             

                             Infobox.AppendText("\nProcessing " + filename + "...");

                             string foldername = Path.GetDirectoryName(imgfiles);

                             if (imgfiles.Contains(".dds"))
                             {
                                 DXTformat = "DXT1";
                                 mipmapotd = 1;
                             }
                             else
                             {
                                 Bitmap bp = new Bitmap(imgfiles);

                                 if (IsTransparent(bp))
                                 {
                                     DXTformat = "DXT5";
                                 }
                                 else
                                 {
                                     DXTformat = "DXT1";

                                 }

                                 mipmapotd = CheckResolution(bp);



                             }



                             StringBuilder otxbuild = new StringBuilder();
                             otxbuild.AppendLine("Version 13 30");
                             otxbuild.AppendLine("{");
                             otxbuild.AppendLine("	Image " + filename);
                             otxbuild.AppendLine("	Type Regular");
                             otxbuild.AppendLine("    PixelFormat " + DXTformat);
                             otxbuild.AppendLine("	Levels " + mipmapotd);
                             otxbuild.AppendLine("	Usage UNKNOWN");
                             otxbuild.AppendLine("	UsageFlags -");
                             otxbuild.AppendLine("	ExtraFlags 0");
                             otxbuild.AppendLine("}");

                             File.WriteAllText(carpeta + "//" + filename2 + ".otx", otxbuild.ToString());

                             otdbuilder.AppendLine("	" + Path.GetFileName(foldername).Replace("/", "") + "\\" + filename2 + ".otx");
                         }



                     }

                     otdbuilder.AppendLine("}");
                     File.WriteAllText(carpeta + ".otd", otdbuilder.ToString());
                 }
             }


                ).ConfigureAwait(false);

        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {

            Infobox.Text = string.Empty;

            if (isEmpty)
            {
                MessageBox.Show("No folders selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                DisableControls();
                await OpenIVOTD(bfb.SelectedFolders).ConfigureAwait(false);
                MessageBox.Show("Done", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EnableControls();
            }
        }


        private void Infobox_TextChanged(object sender, EventArgs e)
        {
            Infobox.SelectionStart = Infobox.Text.Length;
            Infobox.ScrollToCaret();
        }

        private void Clear()
        {
            ltbFolders.DataSource = null;
            isEmpty = true;
            Infobox.Text = string.Empty;
        }

        private void DisableControls()
        {
            btnClear.Enabled = false;
            btnConvert.Enabled = false;
            cbFormat.Enabled = false;
            cbTrasp.Enabled = false;
            checkRecursive.Enabled = false;
        }

        private void EnableControls()
        {
            btnClear.Enabled = true;
            btnConvert.Enabled = true;
            cbFormat.Enabled = true;
            cbTrasp.Enabled = true;
            checkRecursive.Enabled = true;
        }
    }
}
