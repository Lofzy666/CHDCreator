using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ChdCreator
{
    public partial class MainForm : Form
    {
        private bool useCompressedFiles = true;

        // Tool path constants
        private const string TOOL_7Z = "7z.exe";
        private const string TOOL_CHDMAN = "chdman.exe";

        // Control name constants
        private const string CTRL_TXTFOLDER = "txtFolder";
        private const string CTRL_LISTARCHIVES = "listArchives";
        private const string CTRL_CHKRECURSE = "chkRecurse";
        private const string CTRL_CHKKEEPARCHIVES = "chkKeepArchives";
        private const string CTRL_CHKLOGGING = "chkLogging";
        private const string CTRL_CHKDIRECTFILES = "chkDirectFiles";
        private const string CTRL_PROGRESSBAR = "progressBar";
        private const string CTRL_TXTLOG = "txtLog";

        public MainForm()
        {
            ExtractEmbeddedTools();
            InitializeUI();
            ValidateRequiredTools();
        }

        private void ExtractEmbeddedTools()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            ExtractResource("ChdCreator.Properties.Resources._7z", Path.Combine(appDir, TOOL_7Z));
            ExtractResource("ChdCreator.Properties.Resources.chdman", Path.Combine(appDir, TOOL_CHDMAN));
        }

        private void ExtractResource(string resourceName, string outputPath)
        {
            if (File.Exists(outputPath)) return; // Already extracted

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        using (var file = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                file.Write(buffer, 0, bytesRead);
                            }
                        }
                        Log(string.Format("Extracted {0}", Path.GetFileName(outputPath)));
                    }
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("Failed to extract tool {0}: {1}", Path.GetFileName(outputPath), ex.Message));
            }
        }

        private void ValidateRequiredTools()
        {
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] requiredTools = { TOOL_7Z, TOOL_CHDMAN };
            foreach (var tool in requiredTools)
            {
                string toolPath = Path.Combine(appDir, tool);
                if (!File.Exists(toolPath) && !IsInPath(tool))
                {
                    Log(string.Format("Warning: {0} not found in app directory or PATH.", tool));
                }
            }
        }

        private bool IsInPath(string fileName)
        {
            string pathVar = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(pathVar)) return false;
            foreach (string dir in pathVar.Split(Path.PathSeparator))
            {
                if (File.Exists(Path.Combine(dir, fileName))) return true;
            }
            return false;
        }

        private void InitializeUI()
        {
            // Set form properties
            this.Text = "CHD Creator";
            this.Width = 700;
            this.Height = 700;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Folder Selection Label
            Label lblFolder = new Label();
            lblFolder.Text = "Select Folder:";
            lblFolder.Location = new System.Drawing.Point(20, 20);
            lblFolder.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblFolder);

            // Folder TextBox
            TextBox txtFolder = new TextBox();
            txtFolder.Name = "txtFolder";
            txtFolder.Location = new System.Drawing.Point(120, 20);
            txtFolder.Size = new System.Drawing.Size(450, 25);
            txtFolder.ReadOnly = true;
            this.Controls.Add(txtFolder);

            // Browse Button
            Button btnSelectFolder = new Button();
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Text = "Browse...";
            btnSelectFolder.Location = new System.Drawing.Point(580, 20);
            btnSelectFolder.Size = new System.Drawing.Size(90, 25);
            btnSelectFolder.Click += BtnSelectFolder_Click;
            this.Controls.Add(btnSelectFolder);

            // Archive List Label
            Label lblArchives = new Label();
            lblArchives.Text = "Files Found:";
            lblArchives.Location = new System.Drawing.Point(20, 60);
            lblArchives.Size = new System.Drawing.Size(150, 20);
            this.Controls.Add(lblArchives);

            // Archive ListBox
            ListBox listArchives = new ListBox();
            listArchives.Name = "listArchives";
            listArchives.Location = new System.Drawing.Point(20, 85);
            listArchives.Size = new System.Drawing.Size(650, 160);
            listArchives.SelectionMode = SelectionMode.MultiExtended;
            this.Controls.Add(listArchives);

            // Recursive Search Checkbox
            CheckBox chkRecurse = new CheckBox();
            chkRecurse.Name = "chkRecurse";
            chkRecurse.Text = "Search Recursively";
            chkRecurse.Location = new System.Drawing.Point(20, 255);
            chkRecurse.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(chkRecurse);

            // Keep Archives Checkbox
            CheckBox chkKeepArchives = new CheckBox();
            chkKeepArchives.Name = "chkKeepArchives";
            chkKeepArchives.Text = "Keep Original Archives";
            chkKeepArchives.Location = new System.Drawing.Point(220, 255);
            chkKeepArchives.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(chkKeepArchives);

            // Logging Checkbox
            CheckBox chkLogging = new CheckBox();
            chkLogging.Name = "chkLogging";
            chkLogging.Text = "Enable Logging";
            chkLogging.Location = new System.Drawing.Point(420, 255);
            chkLogging.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(chkLogging);

            // Compressed Files Checkbox
            CheckBox chkCompressed = new CheckBox();
            chkCompressed.Name = "chkCompressed";
            chkCompressed.Text = "Process Compressed Archives";
            chkCompressed.Location = new System.Drawing.Point(20, 280);
            chkCompressed.Size = new System.Drawing.Size(280, 20);
            chkCompressed.Checked = true;
            chkCompressed.CheckedChanged += ChkCompressed_CheckedChanged;
            this.Controls.Add(chkCompressed);

            // Direct Files Checkbox
            CheckBox chkDirectFiles = new CheckBox();
            chkDirectFiles.Name = "chkDirectFiles";
            chkDirectFiles.Text = "Process .cue .bin .iso Files";
            chkDirectFiles.Location = new System.Drawing.Point(300, 280);
            chkDirectFiles.Size = new System.Drawing.Size(280, 20);
            chkDirectFiles.Checked = false;
            this.Controls.Add(chkDirectFiles);

            // Progress Bar
            ProgressBar progressBar = new ProgressBar();
            progressBar.Name = "progressBar";
            progressBar.Location = new System.Drawing.Point(20, 310);
            progressBar.Size = new System.Drawing.Size(650, 25);
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            this.Controls.Add(progressBar);

            // Start Button
            Button btnStart = new Button();
            btnStart.Name = "btnStart";
            btnStart.Text = "Start Conversion";
            btnStart.Location = new System.Drawing.Point(20, 345);
            btnStart.Size = new System.Drawing.Size(150, 30);
            btnStart.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            btnStart.Click += BtnStart_Click;
            this.Controls.Add(btnStart);

            // Log TextBox Label
            Label lblLog = new Label();
            lblLog.Text = "Log:";
            lblLog.Location = new System.Drawing.Point(20, 380);
            lblLog.Size = new System.Drawing.Size(100, 20);
            this.Controls.Add(lblLog);

            // Log TextBox
            TextBox txtLog = new TextBox();
            txtLog.Name = "txtLog";
            txtLog.Location = new System.Drawing.Point(20, 405);
            txtLog.Size = new System.Drawing.Size(650, 250);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            this.Controls.Add(txtLog);
        }

        private void ChkCompressed_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                useCompressedFiles = chk.Checked;
            }
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    TextBox txtFolder = this.Controls["txtFolder"] as TextBox;
                    if (txtFolder != null)
                    {
                        txtFolder.Text = fbd.SelectedPath;
                        LoadArchives();
                    }
                }
            }
        }

        private void LoadArchives()
        {
            ListBox listArchives = this.Controls[CTRL_LISTARCHIVES] as ListBox;
            TextBox txtFolder = this.Controls[CTRL_TXTFOLDER] as TextBox;
            CheckBox chkRecurse = this.Controls[CTRL_CHKRECURSE] as CheckBox;
            CheckBox chkDirectFiles = this.Controls[CTRL_CHKDIRECTFILES] as CheckBox;

            if (listArchives == null || txtFolder == null || string.IsNullOrEmpty(txtFolder.Text)) return;
            listArchives.Items.Clear();
            var option = chkRecurse != null && chkRecurse.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            
            if (useCompressedFiles)
            {
                var files7z = Directory.GetFiles(txtFolder.Text, "*.7z", option);
                var filesZip = Directory.GetFiles(txtFolder.Text, "*.zip", option);
                listArchives.Items.AddRange(files7z);
                listArchives.Items.AddRange(filesZip);
                Log(string.Format("Found {0} compressed archives.", files7z.Length + filesZip.Length));
            }

            if (chkDirectFiles != null && chkDirectFiles.Checked)
            {
                var cueFiles = Directory.GetFiles(txtFolder.Text, "*.cue", option);
                var isoFiles = Directory.GetFiles(txtFolder.Text, "*.iso", option);
                listArchives.Items.AddRange(cueFiles);
                listArchives.Items.AddRange(isoFiles);
                Log(string.Format("Found {0} .cue files and {1} .iso files.", cueFiles.Length, isoFiles.Length));
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            ListBox listArchives = this.Controls[CTRL_LISTARCHIVES] as ListBox;
            
            if (listArchives == null || listArchives.Items.Count == 0)
            {
                MessageBox.Show("No archives or files found. Please select a folder first.", "No Archives", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (string archive in listArchives.Items)
            {
                ProcessArchive(archive);
            }
            Log("All archives processed.");
            MessageBox.Show("Conversion complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProcessArchive(string archive)
        {
            CheckBox chkKeepArchives = this.Controls[CTRL_CHKKEEPARCHIVES] as CheckBox;
            string fileExt = Path.GetExtension(archive).ToLower();
            bool keepArchives = chkKeepArchives != null && chkKeepArchives.Checked;
            string archiveDir = Path.GetDirectoryName(archive);

            if (fileExt == ".7z" || fileExt == ".zip")
            {
                Log(string.Format("Extracting {0}...", Path.GetFileName(archive)));
                RunProcess(TOOL_7Z, string.Format("x \"{0}\" -bsp1", archive), archiveDir);
            }

            var cues = Directory.GetFiles(archiveDir, "*.cue", SearchOption.TopDirectoryOnly);
            var isos = Directory.GetFiles(archiveDir, "*.iso", SearchOption.TopDirectoryOnly);

            foreach (var cue in cues)
            {
                ConvertToChd(cue, "createcd");
                CleanupFiles(archive, archiveDir, cue, null, keepArchives, fileExt);
            }

            foreach (var iso in isos)
            {
                ConvertToChd(iso, "createcd");
                CleanupFiles(archive, archiveDir, null, iso, keepArchives, fileExt);
            }
        }

        private void CleanupFiles(string archive, string archiveDir, string cueFile, string isoFile, bool keepArchives, string fileExt)
        {
            if (!keepArchives && (fileExt == ".7z" || fileExt == ".zip"))
                SafeDelete(archive);
            
            if (!string.IsNullOrEmpty(cueFile) && !keepArchives)
                SafeDelete(cueFile);
            
            if (!string.IsNullOrEmpty(isoFile) && !keepArchives)
                SafeDelete(isoFile);
            
            foreach (var bin in Directory.GetFiles(archiveDir, "*.bin"))
                SafeDelete(bin);
        }

        private void SafeDelete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                Log(string.Format("Failed to delete {0}: {1}", Path.GetFileName(filePath), ex.Message));
            }
        }

        private void ConvertToChd(string sourceFile, string chdmanMode)
        {
            string chdOut = Path.ChangeExtension(sourceFile, ".chd");
            Log(string.Format("Converting {0} → {1}...", Path.GetFileName(sourceFile), Path.GetFileName(chdOut)));
            RunProcess(TOOL_CHDMAN, string.Format("{0} -i \"{1}\" -o \"{2}\"", chdmanMode, sourceFile, chdOut), Path.GetDirectoryName(sourceFile));
        }

        private void RunProcess(string exe, string args, string workingDir)
        {
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string exePath = Path.Combine(appDir, exe);
                if (!File.Exists(exePath))
                    exePath = exe; // Fall back to PATH if not in app directory

                var psi = new ProcessStartInfo(exePath, args)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir
                };

                using (var p = Process.Start(psi))
                {
                    while (!p.StandardOutput.EndOfStream)
                    {
                        string line = p.StandardOutput.ReadLine();
                        Log(line);

                        if (line != null && line.Contains("%"))
                        {
                            string percentStr = new string(line.Where(char.IsDigit).ToArray());
                            int percent;
                            if (int.TryParse(percentStr, out percent))
                            {
                                ProgressBar progressBar = this.Controls[CTRL_PROGRESSBAR] as ProgressBar;
                                if (progressBar != null)
                                    progressBar.Value = Math.Min(100, percent);
                            }
                        }
                    }
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("Error: {0}", ex.Message));
                MessageBox.Show(string.Format("Error running {0}: {1}", exe, ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Log(string message)
        {
            TextBox txtLog = this.Controls[CTRL_TXTLOG] as TextBox;
            CheckBox chkLogging = this.Controls[CTRL_CHKLOGGING] as CheckBox;
            TextBox txtFolder = this.Controls[CTRL_TXTFOLDER] as TextBox;

            if (!string.IsNullOrEmpty(message) && txtLog != null)
            {
                txtLog.AppendText(string.Format("[{0:HH:mm:ss}] {1}{2}", DateTime.Now, message, Environment.NewLine));
                
                if (chkLogging != null && chkLogging.Checked && txtFolder != null && !string.IsNullOrEmpty(txtFolder.Text))
                {
                    try
                    {
                        File.AppendAllText(Path.Combine(txtFolder.Text, "conversion.log"),
                            string.Format("[{0:yyyy-MM-dd HH:mm:ss}] {1}{2}", DateTime.Now, message, Environment.NewLine));
                    }
                    catch
                    {
                        // Silent fail on log file write to avoid cascading errors
                    }
                }
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
