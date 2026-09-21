namespace modbus_portal
{
    partial class Form1
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnDisconnect = new Button();
            txtWriteReal1 = new TextBox();
            txtWriteReal2 = new TextBox();
            txtWriteInt1 = new TextBox();
            txtWriteInt2 = new TextBox();
            btnWrite = new Button();
            txtLog = new RichTextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            cmbDeviceIndex = new ComboBox();
            txtScanTime = new TextBox();
            txtTargetDeviceIp = new TextBox();
            txtTargetDevicePort = new TextBox();
            txtTargetSlaveId = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            writeScript = new Button();
            panel1 = new Panel();
            label15 = new Label();
            btnValue = new Button();
            searchFeature = new Button();
            cmbScriptTuru = new ComboBox();
            scriptCode = new RichTextBox();
            panel2 = new Panel();
            pnlStatusLed = new Panel();
            label14 = new Label();
            btnStopAllScans = new Button();
            btnLoadConfigLocal = new Button();
            btnSaveConfigLocal = new Button();
            btnApplyInterval = new Button();
            label13 = new Label();
            txtUiInterval = new TextBox();
            label12 = new Label();
            addUpdate = new Button();
            panel3 = new Panel();
            txtIp = new TextBox();
            btnConnect = new Button();
            lblRead = new Label();
            txtPort = new TextBox();
            panel5 = new Panel();
            panel6 = new Panel();
            label16 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // btnDisconnect
            // 
            btnDisconnect.BackColor = Color.NavajoWhite;
            btnDisconnect.FlatStyle = FlatStyle.Flat;
            btnDisconnect.Font = new Font("Segoe UI", 12F);
            btnDisconnect.Location = new Point(15, 231);
            btnDisconnect.Name = "btnDisconnect";
            btnDisconnect.Size = new Size(245, 40);
            btnDisconnect.TabIndex = 3;
            btnDisconnect.Text = "Bağlantıyı Kes";
            btnDisconnect.UseVisualStyleBackColor = false;
            btnDisconnect.Click += btnDisconnect_Click;
            // 
            // txtWriteReal1
            // 
            txtWriteReal1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            txtWriteReal1.Location = new Point(73, 81);
            txtWriteReal1.Name = "txtWriteReal1";
            txtWriteReal1.Size = new Size(102, 29);
            txtWriteReal1.TabIndex = 5;
            txtWriteReal1.Text = "2.0";
            // 
            // txtWriteReal2
            // 
            txtWriteReal2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            txtWriteReal2.Location = new Point(73, 131);
            txtWriteReal2.Name = "txtWriteReal2";
            txtWriteReal2.Size = new Size(102, 29);
            txtWriteReal2.TabIndex = 6;
            txtWriteReal2.Text = "7.0";
            // 
            // txtWriteInt1
            // 
            txtWriteInt1.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            txtWriteInt1.Location = new Point(73, 182);
            txtWriteInt1.Name = "txtWriteInt1";
            txtWriteInt1.Size = new Size(102, 29);
            txtWriteInt1.TabIndex = 7;
            txtWriteInt1.Text = "20";
            // 
            // txtWriteInt2
            // 
            txtWriteInt2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            txtWriteInt2.Location = new Point(73, 235);
            txtWriteInt2.Name = "txtWriteInt2";
            txtWriteInt2.Size = new Size(102, 29);
            txtWriteInt2.TabIndex = 8;
            txtWriteInt2.Text = "50";
            // 
            // btnWrite
            // 
            btnWrite.BackColor = Color.PaleGreen;
            btnWrite.FlatStyle = FlatStyle.Flat;
            btnWrite.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            btnWrite.Location = new Point(14, 296);
            btnWrite.MinimumSize = new Size(161, 40);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(161, 40);
            btnWrite.TabIndex = 9;
            btnWrite.Text = "Verileri Yaz";
            btnWrite.UseVisualStyleBackColor = false;
            btnWrite.Click += btnWrite_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(494, 213);
            txtLog.MinimumSize = new Size(411, 275);
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(802, 519);
            txtLog.TabIndex = 10;
            txtLog.Text = "";
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // cmbDeviceIndex
            // 
            cmbDeviceIndex.Font = new Font("Segoe UI", 12F);
            cmbDeviceIndex.FormattingEnabled = true;
            cmbDeviceIndex.Location = new Point(16, 64);
            cmbDeviceIndex.Name = "cmbDeviceIndex";
            cmbDeviceIndex.Size = new Size(244, 29);
            cmbDeviceIndex.TabIndex = 11;
            // 
            // txtScanTime
            // 
            txtScanTime.Font = new Font("Segoe UI", 12F);
            txtScanTime.Location = new Point(923, 64);
            txtScanTime.Name = "txtScanTime";
            txtScanTime.Size = new Size(184, 29);
            txtScanTime.TabIndex = 12;
            txtScanTime.Text = "2";
            // 
            // txtTargetDeviceIp
            // 
            txtTargetDeviceIp.Font = new Font("Segoe UI", 12F);
            txtTargetDeviceIp.Location = new Point(291, 64);
            txtTargetDeviceIp.Name = "txtTargetDeviceIp";
            txtTargetDeviceIp.Size = new Size(183, 29);
            txtTargetDeviceIp.TabIndex = 13;
            txtTargetDeviceIp.Text = "50.10.8.110";
            // 
            // txtTargetDevicePort
            // 
            txtTargetDevicePort.Font = new Font("Segoe UI", 12F);
            txtTargetDevicePort.Location = new Point(503, 64);
            txtTargetDevicePort.Name = "txtTargetDevicePort";
            txtTargetDevicePort.Size = new Size(182, 29);
            txtTargetDevicePort.TabIndex = 14;
            txtTargetDevicePort.Text = "502";
            // 
            // txtTargetSlaveId
            // 
            txtTargetSlaveId.Font = new Font("Segoe UI", 12F);
            txtTargetSlaveId.Location = new Point(714, 64);
            txtTargetSlaveId.Name = "txtTargetSlaveId";
            txtTargetSlaveId.Size = new Size(184, 29);
            txtTargetSlaveId.TabIndex = 15;
            txtTargetSlaveId.Text = "1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label1.Location = new Point(16, 26);
            label1.Name = "label1";
            label1.Size = new Size(150, 32);
            label1.TabIndex = 16;
            label1.Text = "Cihaz Seçimi";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label2.ImeMode = ImeMode.NoControl;
            label2.Location = new Point(291, 40);
            label2.Name = "label2";
            label2.Size = new Size(70, 21);
            label2.TabIndex = 17;
            label2.Text = "Cihaz IP:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label3.ImeMode = ImeMode.NoControl;
            label3.Location = new Point(503, 40);
            label3.Name = "label3";
            label3.Size = new Size(44, 21);
            label3.TabIndex = 18;
            label3.Text = "Port:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label4.ImeMode = ImeMode.NoControl;
            label4.Location = new Point(714, 40);
            label4.Name = "label4";
            label4.Size = new Size(122, 21);
            label4.TabIndex = 19;
            label4.Text = "Target Slave ID:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label5.ImeMode = ImeMode.NoControl;
            label5.Location = new Point(923, 40);
            label5.Name = "label5";
            label5.Size = new Size(109, 21);
            label5.TabIndex = 20;
            label5.Text = "Scan Time (s):";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label6.ImeMode = ImeMode.NoControl;
            label6.Location = new Point(15, 18);
            label6.Name = "label6";
            label6.Size = new Size(242, 32);
            label6.TabIndex = 21;
            label6.Text = "ROBUSTEL Bağlantısı";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label7.ImeMode = ImeMode.NoControl;
            label7.Location = new Point(14, 83);
            label7.Name = "label7";
            label7.Size = new Size(55, 21);
            label7.TabIndex = 22;
            label7.Text = "Real 1:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label8.ImeMode = ImeMode.NoControl;
            label8.Location = new Point(14, 133);
            label8.Name = "label8";
            label8.Size = new Size(58, 21);
            label8.TabIndex = 23;
            label8.Text = "Real 2:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label9.ImeMode = ImeMode.NoControl;
            label9.Location = new Point(14, 237);
            label9.Name = "label9";
            label9.Size = new Size(47, 21);
            label9.TabIndex = 25;
            label9.Text = "Int 2:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            label10.ImeMode = ImeMode.NoControl;
            label10.Location = new Point(14, 184);
            label10.Name = "label10";
            label10.Size = new Size(44, 21);
            label10.TabIndex = 24;
            label10.Text = "Int 1:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label11.ImeMode = ImeMode.NoControl;
            label11.Location = new Point(201, 18);
            label11.Name = "label11";
            label11.Size = new Size(65, 25);
            label11.TabIndex = 26;
            label11.Text = "Loglar";
            // 
            // writeScript
            // 
            writeScript.BackColor = SystemColors.ControlLight;
            writeScript.FlatStyle = FlatStyle.Flat;
            writeScript.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            writeScript.Location = new Point(19, 879);
            writeScript.Name = "writeScript";
            writeScript.Size = new Size(532, 43);
            writeScript.TabIndex = 27;
            writeScript.Text = "Script Çalıştır";
            writeScript.UseVisualStyleBackColor = false;
            writeScript.Click += writeScript_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel1.BackColor = SystemColors.InactiveCaption;
            panel1.Controls.Add(label15);
            panel1.Controls.Add(btnValue);
            panel1.Controls.Add(searchFeature);
            panel1.Controls.Add(cmbScriptTuru);
            panel1.Controls.Add(scriptCode);
            panel1.Controls.Add(writeScript);
            panel1.Location = new Point(1318, 27);
            panel1.Name = "panel1";
            panel1.Size = new Size(572, 941);
            panel1.TabIndex = 28;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label15.ImeMode = ImeMode.NoControl;
            label15.Location = new Point(19, 26);
            label15.Name = "label15";
            label15.Size = new Size(93, 21);
            label15.TabIndex = 33;
            label15.Text = "Script Türü:";
            // 
            // btnValue
            // 
            btnValue.BackColor = Color.Aquamarine;
            btnValue.FlatStyle = FlatStyle.Flat;
            btnValue.Font = new Font("Segoe UI", 12F);
            btnValue.ImeMode = ImeMode.NoControl;
            btnValue.Location = new Point(402, 26);
            btnValue.Name = "btnValue";
            btnValue.Size = new Size(149, 59);
            btnValue.TabIndex = 34;
            btnValue.Text = "Değişken Ata";
            btnValue.UseVisualStyleBackColor = false;
            btnValue.Click += btnValue_Click;
            // 
            // searchFeature
            // 
            searchFeature.BackColor = Color.BlanchedAlmond;
            searchFeature.FlatStyle = FlatStyle.Flat;
            searchFeature.Font = new Font("Segoe UI", 12F);
            searchFeature.Location = new Point(224, 26);
            searchFeature.Name = "searchFeature";
            searchFeature.Size = new Size(163, 59);
            searchFeature.TabIndex = 33;
            searchFeature.Text = "Özellik Ara /Yardım";
            searchFeature.UseVisualStyleBackColor = false;
            searchFeature.Click += searchFeature_Click;
            // 
            // cmbScriptTuru
            // 
            cmbScriptTuru.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            cmbScriptTuru.FormattingEnabled = true;
            cmbScriptTuru.Location = new Point(19, 56);
            cmbScriptTuru.Name = "cmbScriptTuru";
            cmbScriptTuru.Size = new Size(186, 29);
            cmbScriptTuru.TabIndex = 33;
            // 
            // scriptCode
            // 
            scriptCode.Location = new Point(19, 101);
            scriptCode.Name = "scriptCode";
            scriptCode.Size = new Size(532, 757);
            scriptCode.TabIndex = 29;
            scriptCode.Text = "";
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel2.BackColor = SystemColors.GradientActiveCaption;
            panel2.Controls.Add(pnlStatusLed);
            panel2.Controls.Add(label14);
            panel2.Controls.Add(btnStopAllScans);
            panel2.Controls.Add(btnLoadConfigLocal);
            panel2.Controls.Add(btnSaveConfigLocal);
            panel2.Controls.Add(btnApplyInterval);
            panel2.Controls.Add(label13);
            panel2.Controls.Add(txtUiInterval);
            panel2.Controls.Add(label12);
            panel2.Location = new Point(14, 605);
            panel2.Name = "panel2";
            panel2.Size = new Size(989, 211);
            panel2.TabIndex = 30;
            // 
            // pnlStatusLed
            // 
            pnlStatusLed.BackColor = Color.MediumSpringGreen;
            pnlStatusLed.Location = new Point(705, 170);
            pnlStatusLed.Name = "pnlStatusLed";
            pnlStatusLed.Size = new Size(258, 22);
            pnlStatusLed.TabIndex = 32;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Segoe UI", 12F);
            label14.Location = new Point(705, 147);
            label14.Name = "label14";
            label14.Size = new Size(131, 21);
            label14.TabIndex = 32;
            label14.Text = "Bağlantı Durumu:";
            // 
            // btnStopAllScans
            // 
            btnStopAllScans.BackColor = Color.PeachPuff;
            btnStopAllScans.FlatStyle = FlatStyle.Flat;
            btnStopAllScans.Font = new Font("Segoe UI", 12F);
            btnStopAllScans.ImeMode = ImeMode.NoControl;
            btnStopAllScans.Location = new Point(705, 53);
            btnStopAllScans.Name = "btnStopAllScans";
            btnStopAllScans.Size = new Size(258, 72);
            btnStopAllScans.TabIndex = 36;
            btnStopAllScans.Text = "Tüm Taramaları Acil Durdur";
            btnStopAllScans.UseVisualStyleBackColor = false;
            btnStopAllScans.Click += btnStopAllScans_Click;
            // 
            // btnLoadConfigLocal
            // 
            btnLoadConfigLocal.BackColor = Color.Thistle;
            btnLoadConfigLocal.FlatStyle = FlatStyle.Flat;
            btnLoadConfigLocal.Font = new Font("Segoe UI", 12F);
            btnLoadConfigLocal.ImeMode = ImeMode.NoControl;
            btnLoadConfigLocal.Location = new Point(388, 147);
            btnLoadConfigLocal.Name = "btnLoadConfigLocal";
            btnLoadConfigLocal.Size = new Size(274, 45);
            btnLoadConfigLocal.TabIndex = 35;
            btnLoadConfigLocal.Text = "Yedekten Yükle";
            btnLoadConfigLocal.UseVisualStyleBackColor = false;
            btnLoadConfigLocal.Click += btnLoadConfigLocal_Click;
            // 
            // btnSaveConfigLocal
            // 
            btnSaveConfigLocal.BackColor = Color.FromArgb(255, 255, 192);
            btnSaveConfigLocal.FlatStyle = FlatStyle.Flat;
            btnSaveConfigLocal.Font = new Font("Segoe UI", 12F);
            btnSaveConfigLocal.ImeMode = ImeMode.NoControl;
            btnSaveConfigLocal.Location = new Point(388, 53);
            btnSaveConfigLocal.Name = "btnSaveConfigLocal";
            btnSaveConfigLocal.Size = new Size(274, 72);
            btnSaveConfigLocal.TabIndex = 34;
            btnSaveConfigLocal.Text = "Ayarları Yedekle (PC'ye Kaydet)";
            btnSaveConfigLocal.UseVisualStyleBackColor = false;
            btnSaveConfigLocal.Click += btnSaveConfigLocal_Click;
            // 
            // btnApplyInterval
            // 
            btnApplyInterval.BackColor = Color.MistyRose;
            btnApplyInterval.FlatStyle = FlatStyle.Flat;
            btnApplyInterval.Font = new Font("Segoe UI", 12F);
            btnApplyInterval.Location = new Point(21, 147);
            btnApplyInterval.Name = "btnApplyInterval";
            btnApplyInterval.Size = new Size(323, 45);
            btnApplyInterval.TabIndex = 33;
            btnApplyInterval.Text = "Uygula";
            btnApplyInterval.UseVisualStyleBackColor = false;
            btnApplyInterval.Click += btnApplyInterval_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new Font("Segoe UI", 12F);
            label13.ImeMode = ImeMode.NoControl;
            label13.Location = new Point(21, 63);
            label13.Name = "label13";
            label13.Size = new Size(209, 21);
            label13.TabIndex = 32;
            label13.Text = "Arayüz Yenileme Sıklığı (ms):";
            // 
            // txtUiInterval
            // 
            txtUiInterval.Font = new Font("Segoe UI", 12F);
            txtUiInterval.Location = new Point(21, 96);
            txtUiInterval.Name = "txtUiInterval";
            txtUiInterval.Size = new Size(323, 29);
            txtUiInterval.TabIndex = 32;
            txtUiInterval.Text = "500";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label12.ImeMode = ImeMode.NoControl;
            label12.Location = new Point(21, 21);
            label12.Name = "label12";
            label12.Size = new Size(74, 25);
            label12.TabIndex = 31;
            label12.Text = "Ayarlar";
            // 
            // addUpdate
            // 
            addUpdate.BackColor = Color.PaleTurquoise;
            addUpdate.FlatStyle = FlatStyle.Flat;
            addUpdate.Font = new Font("Segoe UI", 12F);
            addUpdate.ImeMode = ImeMode.NoControl;
            addUpdate.Location = new Point(1127, 46);
            addUpdate.Name = "addUpdate";
            addUpdate.Size = new Size(157, 47);
            addUpdate.TabIndex = 31;
            addUpdate.Text = "Ekle/Güncelle";
            addUpdate.UseVisualStyleBackColor = false;
            addUpdate.Click += addUpdate_Click;
            // 
            // panel3
            // 
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel3.Controls.Add(txtIp);
            panel3.Controls.Add(btnConnect);
            panel3.Controls.Add(lblRead);
            panel3.Controls.Add(txtPort);
            panel3.Controls.Add(btnDisconnect);
            panel3.Controls.Add(label6);
            panel3.Location = new Point(12, 152);
            panel3.Name = "panel3";
            panel3.Size = new Size(275, 816);
            panel3.TabIndex = 33;
            // 
            // txtIp
            // 
            txtIp.Font = new Font("Segoe UI", 12F);
            txtIp.Location = new Point(16, 75);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(244, 29);
            txtIp.TabIndex = 38;
            txtIp.Text = "50.10.8.121";
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.PowderBlue;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Font = new Font("Segoe UI", 12F);
            btnConnect.ImeMode = ImeMode.NoControl;
            btnConnect.Location = new Point(15, 175);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(245, 40);
            btnConnect.TabIndex = 37;
            btnConnect.Text = "Bağlan";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += btnConnect_Click;
            // 
            // lblRead
            // 
            lblRead.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lblRead.BackColor = Color.LightBlue;
            lblRead.Font = new Font("Segoe UI", 12F);
            lblRead.ImeMode = ImeMode.NoControl;
            lblRead.Location = new Point(15, 296);
            lblRead.MinimumSize = new Size(245, 311);
            lblRead.Name = "lblRead";
            lblRead.Size = new Size(245, 520);
            lblRead.TabIndex = 4;
            lblRead.Text = "Veriler";
            // 
            // txtPort
            // 
            txtPort.Font = new Font("Segoe UI", 12F);
            txtPort.Location = new Point(15, 125);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(244, 29);
            txtPort.TabIndex = 36;
            txtPort.Text = "5020";
            // 
            // panel5
            // 
            panel5.Controls.Add(addUpdate);
            panel5.Controls.Add(label1);
            panel5.Controls.Add(cmbDeviceIndex);
            panel5.Controls.Add(txtTargetDeviceIp);
            panel5.Controls.Add(label2);
            panel5.Controls.Add(label3);
            panel5.Controls.Add(txtTargetDevicePort);
            panel5.Controls.Add(txtTargetSlaveId);
            panel5.Controls.Add(label4);
            panel5.Controls.Add(txtScanTime);
            panel5.Controls.Add(label5);
            panel5.Location = new Point(12, 27);
            panel5.Name = "panel5";
            panel5.Size = new Size(1300, 116);
            panel5.TabIndex = 34;
            // 
            // panel6
            // 
            panel6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel6.Controls.Add(panel2);
            panel6.Controls.Add(label16);
            panel6.Controls.Add(txtWriteReal1);
            panel6.Controls.Add(label11);
            panel6.Controls.Add(label7);
            panel6.Controls.Add(txtWriteReal2);
            panel6.Controls.Add(label9);
            panel6.Controls.Add(label8);
            panel6.Controls.Add(btnWrite);
            panel6.Controls.Add(label10);
            panel6.Controls.Add(txtWriteInt1);
            panel6.Controls.Add(txtWriteInt2);
            panel6.Location = new Point(293, 152);
            panel6.Name = "panel6";
            panel6.Size = new Size(1019, 816);
            panel6.TabIndex = 34;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label16.ImeMode = ImeMode.NoControl;
            label16.Location = new Point(10, 21);
            label16.Name = "label16";
            label16.Size = new Size(114, 25);
            label16.TabIndex = 35;
            label16.Text = "Writing Test";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.GradientInactiveCaption;
            ClientSize = new Size(1902, 991);
            Controls.Add(panel1);
            Controls.Add(txtLog);
            Controls.Add(panel5);
            Controls.Add(panel6);
            Controls.Add(panel3);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 162);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimumSize = new Size(1918, 1030);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Robustel Arayüzü";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel6.ResumeLayout(false);
            panel6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btnDisconnect;
        private TextBox txtWriteReal1;
        private TextBox txtWriteReal2;
        private TextBox txtWriteInt1;
        private TextBox txtWriteInt2;
        private Button btnWrite;
        private RichTextBox txtLog;
        private System.Windows.Forms.Timer timer1;
        private ComboBox cmbDeviceIndex;
        private TextBox txtScanTime;
        private TextBox txtTargetDeviceIp;
        private TextBox txtTargetDevicePort;
        private TextBox txtTargetSlaveId;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Button writeScript;
        private Panel panel1;
        private RichTextBox scriptCode;
        private Panel panel2;
        private Label label12;
        private Button addUpdate;
        private Button btnApplyInterval;
        private Label label13;
        private TextBox txtUiInterval;
        private Button btnSaveConfigLocal;
        private Button btnLoadConfigLocal;
        private Button btnStopAllScans;
        private Panel pnlStatusLed;
        private Label label14;
        private ComboBox cmbScriptTuru;
        private Button btnValue;
        private Button searchFeature;
        private Label label15;
        private Panel panel3;
        private Panel panel5;
        private Panel panel6;
        private Label label16;
        private Label lblRead;
        private Button btnConnect;
        private TextBox txtPort;
        private TextBox txtIp;
    }
}