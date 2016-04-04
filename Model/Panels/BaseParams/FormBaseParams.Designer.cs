namespace Autocad_ConcerteList.Model.Panels.BaseParams
{
    partial class FormBaseParams
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
            this.dataGridViewBalconyDoor = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewBalconyCut = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBalconyDoor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBalconyCut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewBalconyDoor
            // 
            this.dataGridViewBalconyDoor.AllowUserToAddRows = false;
            this.dataGridViewBalconyDoor.AllowUserToDeleteRows = false;
            this.dataGridViewBalconyDoor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBalconyDoor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewBalconyDoor.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBalconyDoor.MultiSelect = false;
            this.dataGridViewBalconyDoor.Name = "dataGridViewBalconyDoor";
            this.dataGridViewBalconyDoor.Size = new System.Drawing.Size(407, 160);
            this.dataGridViewBalconyDoor.TabIndex = 0;
            this.dataGridViewBalconyDoor.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewBalconyDoor_CellContentDoubleClick);
            this.dataGridViewBalconyDoor.DoubleClick += new System.EventHandler(this.dataGridViewBalconyDoor_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Балконы";
            // 
            // dataGridViewBalconyCut
            // 
            this.dataGridViewBalconyCut.AllowUserToAddRows = false;
            this.dataGridViewBalconyCut.AllowUserToDeleteRows = false;
            this.dataGridViewBalconyCut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBalconyCut.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewBalconyCut.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBalconyCut.MultiSelect = false;
            this.dataGridViewBalconyCut.Name = "dataGridViewBalconyCut";
            this.dataGridViewBalconyCut.Size = new System.Drawing.Size(407, 157);
            this.dataGridViewBalconyCut.TabIndex = 0;
            this.dataGridViewBalconyCut.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewBalconyCut_CellContentDoubleClick);
            this.dataGridViewBalconyCut.DoubleClick += new System.EventHandler(this.dataGridViewBalconyCut_DoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Подрезки";
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
            this.splitContainer1.Panel1.Controls.Add(this.dataGridViewBalconyDoor);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridViewBalconyCut);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(407, 321);
            this.splitContainer1.SplitterDistance = 160;
            this.splitContainer1.TabIndex = 2;
            // 
            // FormBaseParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 395);
            this.Controls.Add(this.splitContainer1);
            this.Name = "FormBaseParams";
            this.Text = "FormBaseParams";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBalconyDoor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBalconyCut)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewBalconyDoor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridViewBalconyCut;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}