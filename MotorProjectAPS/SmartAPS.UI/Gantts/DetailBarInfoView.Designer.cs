namespace SmartAPS.UI.Gantts
{
    partial class DetailBarInfoView
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnDispBar = new DevExpress.XtraEditors.SimpleButton();
            this.btnDispEqp = new DevExpress.XtraEditors.SimpleButton();
            this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.layoutControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(601, 461);
            this.panelControl1.TabIndex = 0;
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnDispBar);
            this.layoutControl1.Controls.Add(this.btnDispEqp);
            this.layoutControl1.Controls.Add(this.memoEdit1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(2, 2);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(597, 457);
            this.layoutControl1.TabIndex = 5;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnDispBar
            // 
            this.btnDispBar.Location = new System.Drawing.Point(300, 345);
            this.btnDispBar.Name = "btnDispBar";
            this.btnDispBar.Size = new System.Drawing.Size(285, 100);
            this.btnDispBar.StyleController = this.layoutControl1;
            this.btnDispBar.TabIndex = 9;
            this.btnDispBar.Text = "DispatchLog(Bar)";
            this.btnDispBar.Click += new System.EventHandler(this.btnDispBar_Click);
            // 
            // btnDispEqp
            // 
            this.btnDispEqp.Location = new System.Drawing.Point(12, 345);
            this.btnDispEqp.Name = "btnDispEqp";
            this.btnDispEqp.Size = new System.Drawing.Size(284, 100);
            this.btnDispEqp.StyleController = this.layoutControl1;
            this.btnDispEqp.TabIndex = 8;
            this.btnDispEqp.Text = "DispatchLog(Eqp)";
            this.btnDispEqp.Click += new System.EventHandler(this.btnDispEqp_Click);
            // 
            // memoEdit1
            // 
            this.memoEdit1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.memoEdit1.EditValue = "";
            this.memoEdit1.Location = new System.Drawing.Point(12, 12);
            this.memoEdit1.Name = "memoEdit1";
            this.memoEdit1.Properties.AllowFocused = false;
            this.memoEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.memoEdit1.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.memoEdit1.Properties.Appearance.Options.UseBackColor = true;
            this.memoEdit1.Properties.Appearance.Options.UseForeColor = true;
            this.memoEdit1.Properties.ReadOnly = true;
            this.memoEdit1.Properties.WordWrap = false;
            this.memoEdit1.Size = new System.Drawing.Size(573, 329);
            this.memoEdit1.StyleController = this.layoutControl1;
            this.memoEdit1.TabIndex = 5;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5});
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(597, 457);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.memoEdit1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(577, 333);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnDispEqp;
            this.layoutControlItem4.Location = new System.Drawing.Point(0, 333);
            this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 104);
            this.layoutControlItem4.MinSize = new System.Drawing.Size(91, 26);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(288, 104);
            this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.btnDispBar;
            this.layoutControlItem5.Location = new System.Drawing.Point(288, 333);
            this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 104);
            this.layoutControlItem5.MinSize = new System.Drawing.Size(91, 26);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(289, 104);
            this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // DetailBarInfoView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelControl1);
            this.Name = "DetailBarInfoView";
            this.Size = new System.Drawing.Size(601, 461);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnDispBar;
        private DevExpress.XtraEditors.SimpleButton btnDispEqp;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraEditors.MemoEdit memoEdit1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
    }
}
