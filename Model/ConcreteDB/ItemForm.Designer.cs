namespace Autocad_ConcerteList
{
    partial class ItemForm
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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.seriesCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label10 = new System.Windows.Forms.Label();
            this.formworkmirrorTextBox = new System.Windows.Forms.TextBox();
            this.formworkTextBox = new System.Windows.Forms.TextBox();
            this.thicknessTextBox = new System.Windows.Forms.TextBox();
            this.heigthTextBox = new System.Windows.Forms.TextBox();
            this.lenghtTextBox = new System.Windows.Forms.TextBox();
            this.volumeTextBox = new System.Windows.Forms.TextBox();
            this.weigthTextBox = new System.Windows.Forms.TextBox();
            this.electricsIdxTextBox2 = new System.Windows.Forms.TextBox();
            this.electricsIdxTextBox1 = new System.Windows.Forms.TextBox();
            this.balconycutComboBox = new System.Windows.Forms.ComboBox();
            this.balconyDoorComboBox = new System.Windows.Forms.ComboBox();
            this.markLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.itemGroupComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.errorValueProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorValueProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.seriesCheckedListBox);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.formworkmirrorTextBox);
            this.panel1.Controls.Add(this.formworkTextBox);
            this.panel1.Controls.Add(this.thicknessTextBox);
            this.panel1.Controls.Add(this.heigthTextBox);
            this.panel1.Controls.Add(this.lenghtTextBox);
            this.panel1.Controls.Add(this.volumeTextBox);
            this.panel1.Controls.Add(this.weigthTextBox);
            this.panel1.Controls.Add(this.electricsIdxTextBox2);
            this.panel1.Controls.Add(this.electricsIdxTextBox1);
            this.panel1.Controls.Add(this.balconycutComboBox);
            this.panel1.Controls.Add(this.balconyDoorComboBox);
            this.panel1.Controls.Add(this.markLabel);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.itemGroupComboBox);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(281, 573);
            this.panel1.TabIndex = 0;
            // 
            // seriesCheckedListBox
            // 
            this.seriesCheckedListBox.FormattingEnabled = true;
            this.seriesCheckedListBox.Location = new System.Drawing.Point(9, 22);
            this.seriesCheckedListBox.Name = "seriesCheckedListBox";
            this.seriesCheckedListBox.Size = new System.Drawing.Size(259, 64);
            this.seriesCheckedListBox.TabIndex = 59;
            this.seriesCheckedListBox.Tag = "dbo.I_S_Series.SeriesId";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 6);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(38, 13);
            this.label10.TabIndex = 58;
            this.label10.Text = "Серия";
            // 
            // formworkmirrorTextBox
            // 
            this.formworkmirrorTextBox.Location = new System.Drawing.Point(9, 380);
            this.formworkmirrorTextBox.MaxLength = 2;
            this.formworkmirrorTextBox.Name = "formworkmirrorTextBox";
            this.formworkmirrorTextBox.Size = new System.Drawing.Size(259, 20);
            this.formworkmirrorTextBox.TabIndex = 9;
            this.formworkmirrorTextBox.Tag = "dbo.I_R_Item.FormworkMirror";
            this.formworkmirrorTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.formworkmirrorTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.formworkmirrorTextBox_KeyDown);
            // 
            // formworkTextBox
            // 
            this.formworkTextBox.Location = new System.Drawing.Point(9, 261);
            this.formworkTextBox.MaxLength = 2;
            this.formworkTextBox.Name = "formworkTextBox";
            this.formworkTextBox.Size = new System.Drawing.Size(259, 20);
            this.formworkTextBox.TabIndex = 6;
            this.formworkTextBox.Tag = "dbo.I_R_Item.Formwork";
            this.formworkTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.formworkTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.formworkTextBox_KeyDown);
            // 
            // thicknessTextBox
            // 
            this.thicknessTextBox.Location = new System.Drawing.Point(9, 222);
            this.thicknessTextBox.MaxLength = 4;
            this.thicknessTextBox.Name = "thicknessTextBox";
            this.thicknessTextBox.Size = new System.Drawing.Size(259, 20);
            this.thicknessTextBox.TabIndex = 5;
            this.thicknessTextBox.Tag = "dbo.I_R_Item.Thickness";
            this.thicknessTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.thicknessTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.thicknessTextBox_KeyDown);
            // 
            // heigthTextBox
            // 
            this.heigthTextBox.Location = new System.Drawing.Point(9, 183);
            this.heigthTextBox.MaxLength = 5;
            this.heigthTextBox.Name = "heigthTextBox";
            this.heigthTextBox.Size = new System.Drawing.Size(259, 20);
            this.heigthTextBox.TabIndex = 4;
            this.heigthTextBox.Tag = "dbo.I_R_Item.Height";
            this.heigthTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.heigthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.heigthTextBox_KeyDown);
            // 
            // lenghtTextBox
            // 
            this.lenghtTextBox.Location = new System.Drawing.Point(9, 144);
            this.lenghtTextBox.MaxLength = 5;
            this.lenghtTextBox.Name = "lenghtTextBox";
            this.lenghtTextBox.Size = new System.Drawing.Size(259, 20);
            this.lenghtTextBox.TabIndex = 3;
            this.lenghtTextBox.Tag = "dbo.I_R_Item.Lenght";
            this.lenghtTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.lenghtTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lenghtTextBox_KeyDown);
            // 
            // volumeTextBox
            // 
            this.volumeTextBox.Location = new System.Drawing.Point(9, 497);
            this.volumeTextBox.Name = "volumeTextBox";
            this.volumeTextBox.Size = new System.Drawing.Size(259, 20);
            this.volumeTextBox.TabIndex = 13;
            this.volumeTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.volumeTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.volumeTextBox_KeyDown);
            // 
            // weigthTextBox
            // 
            this.weigthTextBox.Location = new System.Drawing.Point(9, 458);
            this.weigthTextBox.Name = "weigthTextBox";
            this.weigthTextBox.Size = new System.Drawing.Size(259, 20);
            this.weigthTextBox.TabIndex = 12;
            this.weigthTextBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.weigthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.weigthTextBox_KeyDown);
            // 
            // electricsIdxTextBox2
            // 
            this.electricsIdxTextBox2.Location = new System.Drawing.Point(239, 419);
            this.electricsIdxTextBox2.Name = "electricsIdxTextBox2";
            this.electricsIdxTextBox2.Size = new System.Drawing.Size(29, 20);
            this.electricsIdxTextBox2.TabIndex = 11;
            this.electricsIdxTextBox2.Text = "э";
            // 
            // electricsIdxTextBox1
            // 
            this.electricsIdxTextBox1.Location = new System.Drawing.Point(9, 419);
            this.electricsIdxTextBox1.MaxLength = 2;
            this.electricsIdxTextBox1.Name = "electricsIdxTextBox1";
            this.electricsIdxTextBox1.Size = new System.Drawing.Size(224, 20);
            this.electricsIdxTextBox1.TabIndex = 10;
            this.electricsIdxTextBox1.TextChanged += new System.EventHandler(this.updateItemEntry);
            this.electricsIdxTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.electricsIdxTextBox1_KeyDown);
            // 
            // balconycutComboBox
            // 
            this.balconycutComboBox.DisplayMember = "BalconyCut";
            this.balconycutComboBox.FormattingEnabled = true;
            this.balconycutComboBox.Location = new System.Drawing.Point(9, 340);
            this.balconycutComboBox.Name = "balconycutComboBox";
            this.balconycutComboBox.Size = new System.Drawing.Size(259, 21);
            this.balconycutComboBox.TabIndex = 8;
            this.balconycutComboBox.Tag = "dbo.I_S_BalconyDoor.BalconyCut";
            this.balconycutComboBox.ValueMember = "BalconyCut";
            this.balconycutComboBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            // 
            // balconyDoorComboBox
            // 
            this.balconyDoorComboBox.DisplayMember = "BalconyDoor";
            this.balconyDoorComboBox.FormattingEnabled = true;
            this.balconyDoorComboBox.Location = new System.Drawing.Point(9, 300);
            this.balconyDoorComboBox.Name = "balconyDoorComboBox";
            this.balconyDoorComboBox.Size = new System.Drawing.Size(259, 21);
            this.balconyDoorComboBox.TabIndex = 7;
            this.balconyDoorComboBox.Tag = "dbo.I_S_BalconyDoor.BalconyDoor";
            this.balconyDoorComboBox.ValueMember = "BalconyDoor";
            this.balconyDoorComboBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            // 
            // markLabel
            // 
            this.markLabel.AutoSize = true;
            this.markLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.markLabel.Location = new System.Drawing.Point(6, 531);
            this.markLabel.Name = "markLabel";
            this.markLabel.Size = new System.Drawing.Size(49, 13);
            this.markLabel.TabIndex = 56;
            this.markLabel.Text = "Марка:";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(208, 546);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(60, 24);
            this.okButton.TabIndex = 55;
            this.okButton.Text = "Создать";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // itemGroupComboBox
            // 
            this.itemGroupComboBox.FormattingEnabled = true;
            this.itemGroupComboBox.Location = new System.Drawing.Point(9, 104);
            this.itemGroupComboBox.Name = "itemGroupComboBox";
            this.itemGroupComboBox.Size = new System.Drawing.Size(259, 21);
            this.itemGroupComboBox.TabIndex = 2;
            this.itemGroupComboBox.Tag = "dbo.I_S_ItemGroup.ItemGroup";
            this.itemGroupComboBox.TextChanged += new System.EventHandler(this.updateItemEntry);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 481);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 13);
            this.label13.TabIndex = 50;
            this.label13.Text = "Объем, м3";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 442);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 13);
            this.label12.TabIndex = 48;
            this.label12.Text = "Вес, кг";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 403);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 46;
            this.label9.Text = "Электрика";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 364);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 44;
            this.label8.Text = "Зеркальность";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 324);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 13);
            this.label7.TabIndex = 42;
            this.label7.Text = "Подрезка под балкон";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 284);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(145, 13);
            this.label6.TabIndex = 40;
            this.label6.Text = "Балконный проем (в свету)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 245);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 38;
            this.label5.Text = "Опалубка";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 206);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 36;
            this.label4.Text = "Толщина, мм";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Высота, мм";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Длина, мм";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "Обозначение";
            // 
            // errorValueProvider
            // 
            this.errorValueProvider.BlinkRate = 0;
            this.errorValueProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorValueProvider.ContainerControl = this;
            // 
            // ItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 573);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ItemForm";
            this.Text = "Создание ЖБИ";
            this.Load += new System.EventHandler(this.ItemForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorValueProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button okButton;
        public System.Windows.Forms.ComboBox itemGroupComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox balconyDoorComboBox;
        public System.Windows.Forms.ComboBox balconycutComboBox;
        public System.Windows.Forms.TextBox electricsIdxTextBox2;
        public System.Windows.Forms.TextBox electricsIdxTextBox1;
        public System.Windows.Forms.TextBox volumeTextBox;
        public System.Windows.Forms.TextBox weigthTextBox;
        public System.Windows.Forms.ErrorProvider errorValueProvider;
        public System.Windows.Forms.TextBox formworkTextBox;
        public System.Windows.Forms.TextBox thicknessTextBox;
        public System.Windows.Forms.TextBox heigthTextBox;
        public System.Windows.Forms.TextBox lenghtTextBox;
        public System.Windows.Forms.TextBox formworkmirrorTextBox;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.CheckedListBox seriesCheckedListBox;
        public System.Windows.Forms.Label markLabel;
    }
}