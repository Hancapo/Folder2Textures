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
        public static bool isChinese;
        public static bool isEnglish;
        public static bool isSpanish;
        public static bool isFrench;

        public Form1()
        {
            InitializeComponent();
            isEmpty = true;
            cbTrasp.SelectedIndex = 2;
            cbFormat.SelectedIndex = 0;
            CheckForIllegalCrossThreadCalls = false;
            isEnglish = true;
            
            
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
                     if (isEnglish)
                     {
                         Infobox.AppendText("\nReading folder " + Path.GetFileName(carpeta) + "...");

                     }

                     if (isChinese)
                     {
                         Infobox.AppendText("\nR阅读文件夹 " + Path.GetFileName(carpeta) + "...");

                     }

                     if (isSpanish)
                     {
                         Infobox.AppendText("\nLeyendo carpeta " + Path.GetFileName(carpeta) + "...");

                     }

                     if (isFrench)
                     {
                         Infobox.AppendText("\nDossier de lecture " + Path.GetFileName(carpeta) + "...");

                     }

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
                         if (isEnglish)
                         {
                             Infobox.AppendText("\n0 compatible textures, skipping...");

                         }
                         if (isSpanish)
                         {
                             Infobox.AppendText("\n0 texturas compatibles, omitiendo...");

                         }
                         if (isChinese)
                         {
                             Infobox.AppendText("\n0个兼容的纹理，跳过...");

                         }
                         if (isFrench)
                         {
                             Infobox.AppendText("\n0 textures compatibles, en sautant...");

                         }

                     }
                     else
                     {
                         if (isEnglish)
                         {
                             Infobox.AppendText("\n" + imgcount + " compatible textures.");

                         }
                         if (isFrench)
                         {
                             Infobox.AppendText("\n" + imgcount + " textures compatibles.");

                         }
                         if (isSpanish)
                         {
                             Infobox.AppendText("\n" + imgcount + " texturas compatibles.");

                         }
                         if (isChinese)
                         {
                             Infobox.AppendText("\n" + imgcount + " 兼容的纹理");

                         }

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

                             if (isChinese)
                             {
                                 Infobox.AppendText("\n处理中 " + filename + "...");

                             }
                             if (isEnglish)
                             {
                                 Infobox.AppendText("\nProcessing " + filename + "...");

                             }
                             if (isFrench)
                             {
                                 Infobox.AppendText("\nTraitement " + filename + "...");

                             }
                             if (isSpanish)
                             {
                                 Infobox.AppendText("\nProcesando " + filename + "...");

                             }

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
                if (isEnglish)
                {
                    MessageBox.Show("No folders selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                if (isFrench)
                {
                    MessageBox.Show("Aucun dossier n'a été sélectionné.", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                if (isSpanish)
                {
                    MessageBox.Show("No se ha seleccionado ninguna carpeta.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                if (isChinese)
                {
                    MessageBox.Show("未选择文件夹", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
            }
            else
            {
                DisableControls();
                await OpenIVOTD(bfb.SelectedFolders).ConfigureAwait(false);
                if (isChinese)
                {
                    MessageBox.Show("完成", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                if (isSpanish)
                {
                    MessageBox.Show("Completado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                if (isFrench)
                {
                    MessageBox.Show("Fait", "Informations", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                if (isEnglish)
                {
                    MessageBox.Show("Done", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
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

        private void spanishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isChinese = false;
            isEnglish = false;
            isSpanish = true;
            isFrench = false;
            btnConvert.Text = "Convertir";
            btnClear.Text = "Limpiar";
            labelOutput.Text = "Formato de salida";
            labelTransp.Text = "Método de detección de transparencia";
            FileMenu.Text = "Archivo";
            selectFolder.Text = "Seleccionar carpetas";
            languageToolStripMenuItem.Text = "Idioma";
            checkRecursive.Text = "Búsqueda recursiva de carpetas";
            spanishToolStripMenuItem.Text = "Español";
            englishToolStripMenuItem.Text = "Inglés";
            chineseToolStripMenuItem.Text = "Chino";
            frenchToolStripMenuItem.Text = "Francés";
            richTextBox1.Text = "Soporte para JPG, BMP, DDS y PNG";
            cbTrasp.Items[0] = "Por Flags (solo PNG)";
            cbTrasp.Items[1] = "Por pixel (LENTO!)";
            cbTrasp.Items[2] = "Apagado";
            cbFormat.Items[0] = "OpenIV (.OTD)";


        }

        private void chineseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isChinese = true;
            isEnglish = false;
            isSpanish = false;
            isFrench = false;
            btnConvert.Text = "兑换";
            btnClear.Text = "全部清除";
            labelOutput.Text = "输出格式";
            labelTransp.Text = "检测透明度";
            FileMenu.Text = "文件";
            selectFolder.Text = "选择文件夹";
            languageToolStripMenuItem.Text = "语言";
            checkRecursive.Text = "递归文件夹搜索";
            spanishToolStripMenuItem.Text = "西班牙文";
            englishToolStripMenuItem.Text = "英语";
            chineseToolStripMenuItem.Text = "中文";
            frenchToolStripMenuItem.Text = "法兰西斯";
            richTextBox1.Text = "JPG，BMP，DDS和PNG的文件支持";
            cbTrasp.Items[0] = "按标志（仅限PNG）";
            cbTrasp.Items[1] = "按像素（慢！）";
            cbTrasp.Items[2] = "关";
            cbFormat.Items[0] = "OpenIV（格式.OTD）";
        }

        private void frenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isChinese = false;
            isEnglish = false;
            isSpanish = false;
            isFrench = true;
            btnConvert.Text = "Convertir";
            btnClear.Text = "Clair";
            labelOutput.Text = "Format de sortie";
            labelTransp.Text = "Détecter la méthode de transparence";
            FileMenu.Text = "Dossier";
            selectFolder.Text = "Sélectionner les dossiers";
            languageToolStripMenuItem.Text = "Langue";
            checkRecursive.Text = "Recherche récursive de dossiers";
            spanishToolStripMenuItem.Text = "Espagnol";
            englishToolStripMenuItem.Text = "Anglais";
            chineseToolStripMenuItem.Text = "Chinois";
            frenchToolStripMenuItem.Text = "Français";
            richTextBox1.Text = "Support de fichiers pour JPG, BMP, DDS et PNG";
            cbTrasp.Items[0] = "Par les drapeaux (PNG uniquement)";
            cbTrasp.Items[1] = "Par pixels (LENT!)";
            cbTrasp.Items[2] = "Off";
            cbFormat.Items[0] = "OpenIV (.OTD)";

        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isChinese = false;
            isEnglish = true;
            isSpanish = false;
            isFrench = false;
            btnConvert.Text = "Convert";
            btnClear.Text = "Clear";
            labelOutput.Text = "Output format";
            labelTransp.Text = "Detect transparency method";
            FileMenu.Text = "File";
            selectFolder.Text = "Select folders";
            languageToolStripMenuItem.Text = "Language";
            checkRecursive.Text = "Recursive folder search";
            spanishToolStripMenuItem.Text = "Spanish";
            englishToolStripMenuItem.Text = "English";
            chineseToolStripMenuItem.Text = "Chinese";
            frenchToolStripMenuItem.Text = "French";
            richTextBox1.Text = "File support for JPG, BMP, DDS and PNG";
            cbTrasp.Items[0] = "By flags (PNG only)";
            cbTrasp.Items[1] = "By pixels (SLOW!)";
            cbTrasp.Items[2] = "Off";
            cbFormat.Items[0] = "OpenIV (.OTD)";
        }
    }
}
