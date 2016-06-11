namespace Autocad_ConcerteList.Src.RegystryPanel.IncorrectMark
{
    partial class FormPanels
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
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewPanels = new System.Windows.Forms.ListView();
            this.Mark = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonShow = new System.Windows.Forms.Button();
            this.comboBoxSer = new System.Windows.Forms.ComboBox();
            this.labelSer = new System.Windows.Forms.Label();
            this.checkBoxGroup = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxInfo.Location = new System.Drawing.Point(0, 0);
            this.textBoxInfo.Multiline = true;
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.ReadOnly = true;
            this.textBoxInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxInfo.Size = new System.Drawing.Size(537, 231);
            this.textBoxInfo.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(474, 637);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Прервать";
            this.toolTip1.SetToolTip(this.buttonCancel, "Прервать регистрацию панелей.");
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(393, 637);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "Исправить";
            this.toolTip1.SetToolTip(this.buttonOk, "Исправление атрибутов марок на чертеже. И продолжение регистрации панелей.");
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewPanels);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBoxInfo);
            this.splitContainer1.Size = new System.Drawing.Size(537, 617);
            this.splitContainer1.SplitterDistance = 382;
            this.splitContainer1.TabIndex = 3;
            // 
            // listViewPanels
            // 
            this.listViewPanels.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Mark,
            this.Status});
            this.listViewPanels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPanels.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listViewPanels.LabelWrap = false;
            this.listViewPanels.Location = new System.Drawing.Point(0, 0);
            this.listViewPanels.Name = "listViewPanels";
            this.listViewPanels.ShowGroups = false;
            this.listViewPanels.Size = new System.Drawing.Size(537, 382);
            this.listViewPanels.TabIndex = 0;
            this.listViewPanels.UseCompatibleStateImageBehavior = false;
            this.listViewPanels.View = System.Windows.Forms.View.Details;
            this.listViewPanels.DoubleClick += new System.EventHandler(this.buttonShow_Click);
            this.listViewPanels.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewPanels_KeyUp);
            // 
            // Mark
            // 
            this.Mark.Text = "Марка";
            this.Mark.Width = 280;
            // 
            // Status
            // 
            this.Status.Text = "Статус";
            this.Status.Width = 307;
            // 
            // buttonShow
            // 
            this.buttonShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonShow.Location = new System.Drawing.Point(5, 637);
            this.buttonShow.Name = "buttonShow";
            this.buttonShow.Size = new System.Drawing.Size(75, 23);
            this.buttonShow.TabIndex = 2;
            this.buttonShow.Text = "Показать";
            this.buttonShow.UseVisualStyleBackColor = true;
            this.buttonShow.Click += new System.EventHandler(this.buttonShow_Click);
            // 
            // comboBoxSer
            // 
            this.comboBoxSer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxSer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSer.FormattingEnabled = true;
            this.comboBoxSer.Location = new System.Drawing.Point(210, 639);
            this.comboBoxSer.Name = "comboBoxSer";
            this.comboBoxSer.Size = new System.Drawing.Size(137, 21);
            this.comboBoxSer.TabIndex = 4;
            this.comboBoxSer.Visible = false;
            // 
            // labelSer
            // 
            this.labelSer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSer.AutoSize = true;
            this.labelSer.Location = new System.Drawing.Point(166, 642);
            this.labelSer.Name = "labelSer";
            this.labelSer.Size = new System.Drawing.Size(38, 13);
            this.labelSer.TabIndex = 5;
            this.labelSer.Text = "Серия";
            this.labelSer.Visible = false;
            // 
            // checkBoxGroup
            // 
            this.checkBoxGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxGroup.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxGroup.BackgroundImage = global::Autocad_ConcerteList.Properties.Resources.group;
            this.checkBoxGroup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.checkBoxGroup.Location = new System.Drawing.Point(113, 637);
            this.checkBoxGroup.Name = "checkBoxGroup";
            this.checkBoxGroup.Size = new System.Drawing.Size(24, 23);
            this.checkBoxGroup.TabIndex = 7;
            this.toolTip1.SetToolTip(this.checkBoxGroup, "Группировка панелей с одной маркой");
            this.checkBoxGroup.UseVisualStyleBackColor = true;
            this.checkBoxGroup.Visible = false;
            this.checkBoxGroup.CheckedChanged += new System.EventHandler(this.checkBoxGroup_CheckedChanged);
            // 
            // FormPanels
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(561, 669);
            this.Controls.Add(this.checkBoxGroup);
            this.Controls.Add(this.labelSer);
            this.Controls.Add(this.comboBoxSer);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonShow);
            this.Controls.Add(this.buttonCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "FormPanels";
            this.Text = "Панели с несоответствующими марками";
            this.Load += new System.EventHandler(this.FormPanels_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.Button buttonOk;
        public System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ColumnHeader Mark;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.Button buttonShow;
        public System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelSer;
        public System.Windows.Forms.ComboBox comboBoxSer;
        public System.Windows.Forms.ListView listViewPanels;
        private System.Windows.Forms.CheckBox checkBoxGroup;
    }
}