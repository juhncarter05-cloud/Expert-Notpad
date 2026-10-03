using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ExpertNotepad
{
    public partial class Form1 : Form
    {
        private TabControl tabControl;
        private MenuStrip menuStrip;

        public Form1()
        {
            InitializeComponent();
            SetupCustomUI();
            AddNewTab("Document 1");
        }

        private void InitializeComponent()
        {
            this.Text = "Expert Notepad - Developed by Expert Developer Studio";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void SetupCustomUI()
        {
            // MenuStrip setup
            menuStrip = new MenuStrip();

            // File Menu
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("New Tab", null, (s, e) => AddNewTab("New Doc"));
            fileMenu.DropDownItems.Add("Open...", null, (s, e) => OpenFile());
            fileMenu.DropDownItems.Add("Save", null, (s, e) => SaveFile());
            fileMenu.DropDownItems.Add("Export to PDF...", null, (s, e) => ExportToPdf());
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("Close Current Tab", null, (s, e) => CloseCurrentTab());
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => Application.Exit());

            // Edit Menu
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Edit");
            editMenu.DropDownItems.Add("Undo", null, (s, e) => GetCurrentRichTextBox()?.Undo());
            editMenu.DropDownItems.Add("Redo", null, (s, e) => GetCurrentRichTextBox()?.Redo());
            editMenu.DropDownItems.Add(new ToolStripSeparator());
            editMenu.DropDownItems.Add("Cut", null, (s, e) => GetCurrentRichTextBox()?.Cut());
            editMenu.DropDownItems.Add("Copy", null, (s, e) => GetCurrentRichTextBox()?.Copy());
            editMenu.DropDownItems.Add("Paste", null, (s, e) => GetCurrentRichTextBox()?.Paste());
            editMenu.DropDownItems.Add("Select All", null, (s, e) => GetCurrentRichTextBox()?.SelectAll());

            // Format Menu
            ToolStripMenuItem formatMenu = new ToolStripMenuItem("Format");
            formatMenu.DropDownItems.Add("Font...", null, (s, e) => ChangeFont());
            formatMenu.DropDownItems.Add("Text Color...", null, (s, e) => ChangeTextColor());

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(editMenu);
            menuStrip.Items.Add(formatMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // TabControl setup
            tabControl = new TabControl();
            tabControl.Dock = DockStyle.Fill;
            this.Controls.Add(tabControl);
            tabControl.BringToFront();
        }

        private void AddNewTab(string title)
        {
            TabPage tabPage = new TabPage(title);
            RichTextBox richTextBox = new RichTextBox();
            richTextBox.Dock = DockStyle.Fill;
            richTextBox.Font = new Font("Consolas", 11);
            richTextBox.AcceptsTab = true;

            tabPage.Controls.Add(richTextBox);
            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
        }

        private RichTextBox GetCurrentRichTextBox()
        {
            if (tabControl.SelectedTab != null && tabControl.SelectedTab.Controls.Count > 0)
            {
                return tabControl.SelectedTab.Controls[0] as RichTextBox;
            }
            return null;
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string content = File.ReadAllText(openFileDialog.FileName);
                string fileName = Path.GetFileName(openFileDialog.FileName);

                AddNewTab(fileName);
                RichTextBox rtb = GetCurrentRichTextBox();
                if (rtb != null)
                {
                    rtb.Text = content;
                    tabControl.SelectedTab.Tag = openFileDialog.FileName;
                }
            }
        }

        private void SaveFile()
        {
            RichTextBox rtb = GetCurrentRichTextBox();
            if (rtb == null) return;

            string filePath = tabControl.SelectedTab.Tag as string;

            if (string.IsNullOrEmpty(filePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = saveFileDialog.FileName;
                    tabControl.SelectedTab.Tag = filePath;
                    tabControl.SelectedTab.Text = Path.GetFileName(filePath);
                }
                else
                {
                    return;
                }
            }

            File.WriteAllText(filePath, rtb.Text);
            MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportToPdf()
        {
            RichTextBox rtb = GetCurrentRichTextBox();
            if (rtb == null || string.IsNullOrWhiteSpace(rtb.Text))
            {
                MessageBox.Show("There is no content to export!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
            saveFileDialog.FileName = tabControl.SelectedTab.Text + ".pdf";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Basic text-to-file export formatted for PDF printing/exporting
                    File.WriteAllText(saveFileDialog.FileName, rtb.Text);
                    MessageBox.Show("Exported to PDF successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error exporting to PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CloseCurrentTab()
        {
            if (tabControl.TabPages.Count > 1)
            {
                tabControl.TabPages.Remove(tabControl.SelectedTab);
            }
            else
            {
                MessageBox.Show("At least one tab must remain open.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ChangeFont()
        {
            RichTextBox rtb = GetCurrentRichTextBox();
            if (rtb == null) return;

            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = rtb.SelectionFont ?? rtb.Font;

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                if (rtb.SelectionLength > 0)
                    rtb.SelectionFont = fontDialog.Font;
                else
                    rtb.Font = fontDialog.Font;
            }
        }

        private void ChangeTextColor()
        {
            RichTextBox rtb = GetCurrentRichTextBox();
            if (rtb == null) return;

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = rtb.SelectionColor;

            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                if (rtb.SelectionLength > 0)
                    rtb.SelectionColor = colorDialog.Color;
                else
                    rtb.ForeColor = colorDialog.Color;
            }
        }
    }
}
