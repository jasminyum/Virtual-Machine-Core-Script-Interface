namespace modbus_portal
{
    partial class FormOzellik
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOzellik));
            txtSearch = new TextBox();
            btnSearch = new Button();
            rtbHelp = new RichTextBox();
            SuspendLayout();
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(33, 49);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(450, 27);
            txtSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(489, 47);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(94, 29);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Ara";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // rtbHelp
            // 
            rtbHelp.Location = new Point(33, 82);
            rtbHelp.Name = "rtbHelp";
            rtbHelp.Size = new Size(550, 316);
            rtbHelp.TabIndex = 2;
            rtbHelp.Text = "";
            // 
            // FormOzellik
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(618, 450);
            Controls.Add(rtbHelp);
            Controls.Add(btnSearch);
            Controls.Add(txtSearch);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormOzellik";
            Text = "Özellik Ara / Yardım";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSearch;
        private Button btnSearch;
        private RichTextBox rtbHelp;
    }
}