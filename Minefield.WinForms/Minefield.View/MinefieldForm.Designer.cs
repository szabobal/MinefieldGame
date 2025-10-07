namespace Minefield.View
{
    partial class MinefieldForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            menuFileNewGame = new ToolStripMenuItem();
            menuFileSaveGame = new ToolStripMenuItem();
            menuFileLoadGame = new ToolStripMenuItem();
            menuFileExitGame = new ToolStripMenuItem();
            tableLayoutPanel = new TableLayoutPanel();
            openFileDialog = new OpenFileDialog();
            saveFileDialog = new SaveFileDialog();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { menuFile });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(684, 24);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { menuFileNewGame, menuFileSaveGame, menuFileLoadGame, menuFileExitGame });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(37, 20);
            menuFile.Text = "File";
            // 
            // menuFileNewGame
            // 
            menuFileNewGame.Name = "menuFileNewGame";
            menuFileNewGame.Size = new Size(180, 22);
            menuFileNewGame.Text = "New game";
            menuFileNewGame.Click += MenuFileNewGame_Click;
            // 
            // menuFileSaveGame
            // 
            menuFileSaveGame.Name = "menuFileSaveGame";
            menuFileSaveGame.Size = new Size(180, 22);
            menuFileSaveGame.Text = "Save game";
            menuFileSaveGame.Click += MenuFileSaveGame_Click;
            // 
            // menuFileLoadGame
            // 
            menuFileLoadGame.Name = "menuFileLoadGame";
            menuFileLoadGame.Size = new Size(180, 22);
            menuFileLoadGame.Text = "Load game";
            menuFileLoadGame.Click += MenuFileLoadGame_ClickAsync;
            // 
            // menuFileExitGame
            // 
            menuFileExitGame.Name = "menuFileExitGame";
            menuFileExitGame.Size = new Size(180, 22);
            menuFileExitGame.Text = "Exit game";
            menuFileExitGame.Click += MenuFileExitGame_Click;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 24);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Size = new Size(684, 637);
            tableLayoutPanel.TabIndex = 1;
            // 
            // openFileDialog
            // 
            openFileDialog.FileName = "openFileDialog1";
            // 
            // MinefieldForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 661);
            Controls.Add(tableLayoutPanel);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(700, 700);
            Name = "MinefieldForm";
            Text = "Minefield";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private TableLayoutPanel tableLayoutPanel;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem menuFileNewGame;
        private ToolStripMenuItem menuFileSaveGame;
        private ToolStripMenuItem menuFileLoadGame;
        private ToolStripMenuItem menuFileExitGame;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
    }
}
