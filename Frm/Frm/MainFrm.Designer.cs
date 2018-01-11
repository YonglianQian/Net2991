﻿namespace Frm
{
    partial class MainFrm
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
            DevExpress.XtraEditors.TileItemElement tileItemElement1 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement2 = new DevExpress.XtraEditors.TileItemElement();
            DevExpress.XtraEditors.TileItemElement tileItemElement3 = new DevExpress.XtraEditors.TileItemElement();
            this.tileControl1 = new DevExpress.XtraEditors.TileControl();
            this.tileGroup1 = new DevExpress.XtraEditors.TileGroup();
            this.tileItem1 = new DevExpress.XtraEditors.TileItem();
            this.tileGroup2 = new DevExpress.XtraEditors.TileGroup();
            this.tileItem6 = new DevExpress.XtraEditors.TileItem();
            this.tileGroup3 = new DevExpress.XtraEditors.TileGroup();
            this.tileItem5 = new DevExpress.XtraEditors.TileItem();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // tileControl1
            // 
            this.tileControl1.AllowDragTilesBetweenGroups = false;
            this.tileControl1.AllowItemHover = true;
            this.tileControl1.AppearanceGroupHighlighting.HoveredMaskColor = System.Drawing.Color.DarkSalmon;
            this.tileControl1.AppearanceGroupHighlighting.MaskColor = System.Drawing.Color.MediumTurquoise;
            this.tileControl1.AppearanceItem.Hovered.BackColor = System.Drawing.Color.Khaki;
            this.tileControl1.AppearanceItem.Hovered.Options.UseBackColor = true;
            this.tileControl1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tileControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
            this.tileControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.tileControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tileControl1.DragSize = new System.Drawing.Size(0, 0);
            this.tileControl1.Groups.Add(this.tileGroup1);
            this.tileControl1.Groups.Add(this.tileGroup2);
            this.tileControl1.Groups.Add(this.tileGroup3);
            this.tileControl1.IndentBetweenGroups = 60;
            this.tileControl1.ItemBorderVisibility = DevExpress.XtraEditors.TileItemBorderVisibility.Always;
            this.tileControl1.ItemContentAnimation = DevExpress.XtraEditors.TileItemContentAnimationType.ScrollTop;
            this.tileControl1.Location = new System.Drawing.Point(0, 0);
            this.tileControl1.MaxId = 6;
            this.tileControl1.Name = "tileControl1";
            this.tileControl1.SelectionColor = System.Drawing.Color.Empty;
            this.tileControl1.Size = new System.Drawing.Size(784, 562);
            this.tileControl1.TabIndex = 0;
            this.tileControl1.Text = "插箱测试系统";
            // 
            // tileGroup1
            // 
            this.tileGroup1.Items.Add(this.tileItem1);
            this.tileGroup1.Name = "tileGroup1";
            this.tileGroup1.Text = "测试";
            // 
            // tileItem1
            // 
            this.tileItem1.AppearanceItem.Normal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.tileItem1.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16F);
            this.tileItem1.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileItem1.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement1.Text = "测试管理";
            this.tileItem1.Elements.Add(tileItemElement1);
            this.tileItem1.Id = 0;
            this.tileItem1.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tileItem1.Name = "tileItem1";
            this.tileItem1.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem1_ItemClick);
            // 
            // tileGroup2
            // 
            this.tileGroup2.Items.Add(this.tileItem6);
            this.tileGroup2.Name = "tileGroup2";
            this.tileGroup2.Text = "数据管理";
            // 
            // tileItem6
            // 
            this.tileItem6.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16F);
            this.tileItem6.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement2.Text = "数据管理";
            this.tileItem6.Elements.Add(tileItemElement2);
            this.tileItem6.Id = 5;
            this.tileItem6.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tileItem6.Name = "tileItem6";
            this.tileItem6.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem6_ItemClick);
            // 
            // tileGroup3
            // 
            this.tileGroup3.Items.Add(this.tileItem5);
            this.tileGroup3.Name = "tileGroup3";
            this.tileGroup3.Text = "系统配置";
            // 
            // tileItem5
            // 
            this.tileItem5.AppearanceItem.Normal.BackColor = System.Drawing.Color.CornflowerBlue;
            this.tileItem5.AppearanceItem.Normal.Font = new System.Drawing.Font("Tahoma", 16F);
            this.tileItem5.AppearanceItem.Normal.Options.UseBackColor = true;
            this.tileItem5.AppearanceItem.Normal.Options.UseFont = true;
            tileItemElement3.Text = "系统管理";
            this.tileItem5.Elements.Add(tileItemElement3);
            this.tileItem5.Id = 4;
            this.tileItem5.ItemSize = DevExpress.XtraEditors.TileItemSize.Medium;
            this.tileItem5.Name = "tileItem5";
            this.tileItem5.ItemClick += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tileItem5_ItemClick);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.BackColor = System.Drawing.Color.LightSteelBlue;
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Appearance.Image = global::Frm.Properties.Resources.系统1;
            this.labelControl1.Appearance.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelControl1.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
            this.labelControl1.Location = new System.Drawing.Point(154, 12);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(457, 36);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "实时信号采集测量与仿真系统平台";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.tileControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainFrm";
            this.Text = "导航";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TileControl tileControl1;
        private DevExpress.XtraEditors.TileGroup tileGroup1;
        private DevExpress.XtraEditors.TileItem tileItem1;
        private DevExpress.XtraEditors.TileGroup tileGroup2;
        private DevExpress.XtraEditors.TileGroup tileGroup3;
        private DevExpress.XtraEditors.TileItem tileItem5;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TileItem tileItem6;
    }
}