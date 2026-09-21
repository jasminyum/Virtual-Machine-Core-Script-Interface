namespace modbus_portal
{
    partial class FormDegisken
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDegisken));
            txtVarName = new TextBox();
            txtAddress = new TextBox();
            btnEkle = new Button();
            button1 = new Button();
            lstVariables = new ListBox();
            label1 = new Label();
            label2 = new Label();
            cmbValueType = new ComboBox();
            txtValue = new TextBox();
            label3 = new Label();
            label4 = new Label();
            btnImportCSV = new Button();
            SuspendLayout();
            // 
            // txtVarName
            // 
            txtVarName.Location = new Point(38, 111);
            txtVarName.Name = "txtVarName";
            txtVarName.Size = new Size(125, 23);
            txtVarName.TabIndex = 0;
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(289, 112);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(125, 23);
            txtAddress.TabIndex = 1;
            // 
            // btnEkle
            // 
            btnEkle.Location = new Point(537, 108);
            btnEkle.Name = "btnEkle";
            btnEkle.Size = new Size(94, 29);
            btnEkle.TabIndex = 2;
            btnEkle.Text = "Ekle";
            btnEkle.UseVisualStyleBackColor = true;
            btnEkle.Click += btnEkle_Click;
            // 
            // button1
            // 
            button1.Location = new Point(637, 108);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 3;
            button1.Text = "Sil";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnSil_Click;
            // 
            // lstVariables
            // 
            lstVariables.FormattingEnabled = true;
            lstVariables.ItemHeight = 15;
            lstVariables.Location = new Point(38, 164);
            lstVariables.Name = "lstVariables";
            lstVariables.Size = new Size(693, 259);
            lstVariables.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(289, 88);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 5;
            label1.Text = "Adres:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 88);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 6;
            label2.Text = "Değer Adı:";
            // 
            // cmbValueType
            // 
            cmbValueType.FormattingEnabled = true;
            cmbValueType.Location = new Point(169, 111);
            cmbValueType.Name = "cmbValueType";
            cmbValueType.Size = new Size(114, 23);
            cmbValueType.TabIndex = 7;
            // 
            // txtValue
            // 
            txtValue.Location = new Point(420, 112);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(111, 23);
            txtValue.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(169, 88);
            label3.Name = "label3";
            label3.Size = new Size(28, 15);
            label3.TabIndex = 9;
            label3.Text = "Tür:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(420, 88);
            label4.Name = "label4";
            label4.Size = new Size(83, 15);
            label4.TabIndex = 10;
            label4.Text = "Değer (1.0 vb):";
            // 
            // btnImportCSV
            // 
            btnImportCSV.Location = new Point(38, 35);
            btnImportCSV.Name = "btnImportCSV";
            btnImportCSV.Size = new Size(159, 26);
            btnImportCSV.TabIndex = 11;
            btnImportCSV.Text = "CSV İçe Aktar";
            btnImportCSV.UseVisualStyleBackColor = true;
            btnImportCSV.Click += btnImportCSV_Click;
            // 
            // FormDegisken
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(767, 453);
            Controls.Add(btnImportCSV);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtValue);
            Controls.Add(cmbValueType);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lstVariables);
            Controls.Add(button1);
            Controls.Add(btnEkle);
            Controls.Add(txtAddress);
            Controls.Add(txtVarName);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormDegisken";
            Text = "Değişken Ekle";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtVarName;
        private TextBox txtAddress;
        private Button btnEkle;
        private Button button1;
        private ListBox lstVariables;
        private Label label1;
        private Label label2;
        private ComboBox cmbValueType;
        private TextBox txtValue;
        private Label label3;
        private Label label4;
        private Button btnImportCSV;
    }
}