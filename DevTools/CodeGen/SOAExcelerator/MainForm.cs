using ByDesign.Excelerator.Classes;
using ByDesign.Excelerator.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ByDesign.Excelerator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

            InitializeComponent();
            txtWorkingPath.Text = @"C:\CodeTemp\";
            txtSRDPath.Text = @"Z:\Clients\ByDesign\New Work\";
            //_ds = null;
            txtProjectFolder.Text = @"C:\Code\bdgit\SOA\";
            openFileDialog1.DefaultExt = ".xlsx";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "Choose Excel SRD";

            openFileDialog1.InitialDirectory = txtSRDPath.Text;
            if (DesignTimeHelper.IsInDesignMode)
                openFileDialog1.InitialDirectory = @"C:\CodeDev\SOAGenNew\";

            //openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Filter = "Excel Files (*.xlsx)|*.xlsx";
            openFileDialog1.FileName = "*soa*.xlsx";

            folderBrowserDialog1.Description = "Set working folder";
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyDocuments;
            //folderBrowserDialog1.in = txtWorkingPath.Text;


            var nextStepsFile = Path.Combine(Environment.CurrentDirectory, "Templates", "NextSteps.rtf");
            if (File.Exists(nextStepsFile) == false)
                richTextBox1.Text = "File is missing: " + nextStepsFile;
            else
                richTextBox1.LoadFile(nextStepsFile);

        }

        private void FirstChanceHandler(object sender, FirstChanceExceptionEventArgs e)
        {
            string msg = string.Format("FirstChanceException event raised in {0}: {1}",
                AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
            MessageBox.Show(msg);
        }



        private void btnChooseSrd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(openFileDialog1.InitialDirectory) == false && Directory.Exists(openFileDialog1.InitialDirectory) == false)
                openFileDialog1.InitialDirectory = @"Z:\Clients\ByDesign\New Work";

            if (openFileDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            string fileName = openFileDialog1.FileName;
            if (string.IsNullOrEmpty(fileName) || File.Exists(fileName) == false)
                return;
            txtSRDPath.Text = fileName;
            loadFile();
        }

        private void loadFile()
        {
            string fileName = txtSRDPath.Text.Trim();
            if (string.IsNullOrEmpty(fileName) || File.Exists(fileName) == false)
                return;

            var er = new ExcelReader();
            var ds = er.GetDataSet(fileName);

            if (ds == null)
            {
                txtSRDPath.SelectAll();
                btnChooseSrd.Focus();
                return;

            }

            var srd = Srd.Parse(ds);

            bindingSource1.DataSource = srd;
            propertiesBindingSource.DataSource = srd.Model.Properties;
            //codeFilesBindingSource .List .RaiseListChangedEvents = true;
            codeFilesBindingSource.DataSource = srd.CodeFiles;

            MessageBox.Show("Load File Complete");

        }

        private void chkMiscTable_CheckedChanged(object sender, EventArgs e)
        {
            this.miscTableIDNumericUpDown.Enabled = chkMiscTable.Checked;
            this.miscTableIDNumericUpDown.Enabled = chkMiscTable.Checked;

        }

        private void btnGenerateFile_Click(object sender, EventArgs e)
        {
            var srd = bindingSource1.DataSource as Srd;
            if (srd == null)
                return;
            string templateFolder = null;
            if (rbCrudinatorUI.Checked)
                templateFolder = "CrudinatorUI";
            else if (rbManualUI.Checked)
                templateFolder = "ManualCrudUI";
            else if(rbReadOnlyService.Checked)
                templateFolder = "ReadOnlyService";
            else if (rbSkeletonService.Checked)
                templateFolder = "SkeletonService";

            srd.GenerateAllFiles(templateFolder);
            codeFilesBindingSource.DataSource = srd.CodeFiles;
            //codeFilesDataGridView.DataSource = srd.CodeFiles;
            string rootDir = txtWorkingPath.Text;

            AppDomain.CurrentDomain.FirstChanceException -= FirstChanceHandler;
            srd.ClearFolder(rootDir);
            AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

            srd.WriteFiles(rootDir);

            MessageBox.Show(this, "Files written to " + rootDir, "Complete");
        }

        private void btnCopyAndAddToProject_Click(object sender, EventArgs e)
        {
            var srd = bindingSource1.DataSource as Srd;
            if (srd == null)
                return;

            string destinationPath = txtProjectFolder.Text.Trim();
            if (Directory.Exists(destinationPath) == false)
            {
                MessageBox.Show("Destination path doesnt exist");
                return;
            }

            foreach (var sourceFilePath in srd.CodeFiles.Where(p => p.CopyToProject == true).Select(p => p.Path))
            {
                //TODO This is wrong and I need to find csproj files.
                string destFileName = destinationPath + sourceFilePath;

                var parts = srd.FilePath.Split(new char[] { '/' });
                string fileName = destFileName.Split('\\').Last();
                fileName = fileName.Split('/').Last();

                if (destFileName.Contains("Registration"))
                    destinationPath = txtProjectFolder.Text + "Applications\\Web\\BackOffice2\\Areas\\" + parts[0] + "\\";
                else if (destFileName.Contains("\\Applications\\Web\\BackOffice2\\Areas") && destFileName.Contains("Controller"))
                    destinationPath = txtProjectFolder.Text + "Applications\\Web\\BackOffice2\\Areas\\" + parts[0] + "\\Controllers\\";
                else if (destFileName.Contains("ByDesign.SOA.Applications.WebAPI") )
                    destinationPath = txtProjectFolder.Text + "Applications\\Web\\ByDesign.SOA.Applications.WebAPI\\Areas\\" + parts[0] + "\\Controllers\\";
                else if (destFileName.Contains("ByDesign.Business.Interfaces"))
                    destinationPath = txtProjectFolder.Text + "Business\\ByDesign.Business.Interfaces\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.Business.Services"))
                    destinationPath = txtProjectFolder.Text + "Business\\ByDesign.Business.Services\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.Common.IModels"))
                    destinationPath = txtProjectFolder.Text + "Common\\ByDesign.Common.IModels\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.Common.Models"))
                    destinationPath = txtProjectFolder.Text + "Common\\ByDesign.Common.Models\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.Data.Interfaces"))
                    destinationPath = txtProjectFolder.Text + "Data\\ByDesign.Data.Interfaces\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.Data.Repositories"))
                    destinationPath = txtProjectFolder.Text + "Data\\ByDesign.Data.Repositories\\" + srd.FilePath + "\\";
                else if(destFileName.Contains("ByDesign.SOA.Testing.Applications.WebAPI"))
                    destinationPath = txtProjectFolder.Text + "Testing\\Applications\\Web\\ByDesign.SOA.Testing.Applications.WebAPI\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.Soa.Testing.Applications.Backoffice2"))
                    destinationPath = txtProjectFolder.Text + "Testing\\Applications\\Web\\ByDesign.Soa.Testing.Applications.Backoffice2\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("ByDesign.SOA.Testing.Services"))
                    destinationPath = txtProjectFolder.Text + "Testing\\Services\\ByDesign.SOA.Testing.Services\\" + srd.FilePath + "\\";
                else if (destFileName.Contains("Procs"))
                    destinationPath = txtProjectFolder.Text + "SQL\\Procs\\";
                else if (destFileName.Contains("Rollout Scripts"))
                    destinationPath = txtProjectFolder.Text + "SQL\\Rollout Scripts\\";

                if(!Directory.Exists(destinationPath))
                    Directory.CreateDirectory(destinationPath);

                string sourceFileName = txtWorkingPath.Text.Trim() + sourceFilePath;

                destFileName = destinationPath + fileName;
                destinationPath = txtProjectFolder.Text.Trim();
                if (destFileName.Contains("Registration"))
                {
                    if (!File.Exists(destFileName))
                    {
                        File.Copy(sourceFileName, destFileName, true);
                        addFileToProj(destFileName);
                    }
                }
                else
                {
                    File.Copy(sourceFileName, destFileName, true);
                    addFileToProj(destFileName);
                }
            }

            MessageBox.Show("Copy Complete and add to project complete.");
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            loadFile();
        }

        private void addFileToProj(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            string fileName = fi.Name;
            DirectoryInfo directory = fi.Directory;

            FileInfo projFile = directory.GetFiles("*.csproj").FirstOrDefault();
            string relativePath = "";
            //walk up directories until find *.csproj
            //keep track of path
            while (directory != null && projFile == null)
            {
                directory = directory.Parent;
                if (directory != null)
                    projFile = directory.GetFiles("*.csproj").FirstOrDefault();
            }
            if (projFile == null)
                return;

            int pos = fi.Directory.FullName.Length - directory.FullName.Length;

            if (pos == 0)
                relativePath = fileName;
            else
            {
                relativePath = fi.Directory.FullName.Substring(directory.FullName.Length) + "\\" + fileName;
                if (relativePath.StartsWith("\\"))
                    relativePath = relativePath.Substring(1);
            }

            string contents = File.ReadAllText(projFile.FullName);
            if (contents.ToLower().Contains(fileName.ToLower()))
            {
                //file is already in csproj
                return;
            }

            //find second </ItemGroup>
            pos = contents.IndexOf("</ItemGroup>", StringComparison.CurrentCultureIgnoreCase);
            if (pos < 0)
                return;

            pos = contents.IndexOf("</ItemGroup>", pos + 1, StringComparison.CurrentCultureIgnoreCase);
            if (pos < 0)
                return;

            //relativePath
            //add file ref
            contents = contents.Substring(0, pos) + "  <Compile Include=\"" + relativePath + "\" />" + Environment.NewLine + "  " + contents.Substring(pos);
            File.WriteAllText(projFile.FullName, contents);
        }
    }
}
