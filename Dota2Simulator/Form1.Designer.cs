using System.Drawing;
using System.Windows.Forms;

namespace Dota2Simulator
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
            btnReplace = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            btnDelete = new Button();
            btnModify = new Button();
            label5 = new Label();
            label6 = new Label();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            btnDeleteGroup = new Button();
            comboBox3 = new ComboBox();
            label7 = new Label();
            SuspendLayout();
            // 
            // btnReplace
            // 
            btnReplace.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnReplace.Font = new Font("微软雅黑", 11F);
            btnReplace.Location = new Point(671, 548);
            btnReplace.Margin = new Padding(2, 2, 2, 2);
            btnReplace.Name = "btnReplace";
            btnReplace.Size = new Size(87, 32);
            btnReplace.TabIndex = 0;
            btnReplace.Text = "正则替换";
            btnReplace.UseVisualStyleBackColor = true;
            btnReplace.Click += btnReplace_Click;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("微软雅黑", 9F);
            textBox1.Location = new Point(16, 55);
            textBox1.Margin = new Padding(2, 2, 2, 2);
            textBox1.MaxLength = 3276700;
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(380, 242);
            textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox2.Font = new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 134);
            textBox2.Location = new Point(410, 55);
            textBox2.Margin = new Padding(2, 2, 2, 2);
            textBox2.MaxLength = 3276700;
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(350, 242);
            textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox3.Font = new Font("微软雅黑", 9F);
            textBox3.Location = new Point(410, 339);
            textBox3.Margin = new Padding(2, 2, 2, 2);
            textBox3.MaxLength = 3276700;
            textBox3.Multiline = true;
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(350, 196);
            textBox3.TabIndex = 3;
            // 
            // textBox4
            // 
            textBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            textBox4.Font = new Font("微软雅黑", 9F);
            textBox4.Location = new Point(16, 339);
            textBox4.Margin = new Padding(2, 2, 2, 2);
            textBox4.MaxLength = 3276700;
            textBox4.Multiline = true;
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(380, 241);
            textBox4.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("微软雅黑", 16F);
            label1.Location = new Point(16, 18);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(189, 30);
            label1.TabIndex = 5;
            label1.Text = "输入要替换的文字";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("微软雅黑", 14F);
            label2.Location = new Point(410, 18);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(88, 25);
            label2.TabIndex = 6;
            label2.Text = "匹配正则";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("微软雅黑", 16F);
            label3.Location = new Point(16, 302);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(145, 30);
            label3.TabIndex = 7;
            label3.Text = "替换后的文字";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Font = new Font("微软雅黑", 14F);
            label4.Location = new Point(410, 304);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(88, 25);
            label4.TabIndex = 8;
            label4.Text = "替换正则";
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDelete.Font = new Font("微软雅黑", 11F);
            btnDelete.Location = new Point(585, 548);
            btnDelete.Margin = new Padding(2, 2, 2, 2);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(72, 32);
            btnDelete.TabIndex = 9;
            btnDelete.Text = "删除明细";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnModify
            // 
            btnModify.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnModify.Font = new Font("微软雅黑", 11F);
            btnModify.Location = new Point(410, 548);
            btnModify.Margin = new Padding(2, 2, 2, 2);
            btnModify.Name = "btnModify";
            btnModify.Size = new Size(87, 32);
            btnModify.TabIndex = 10;
            btnModify.Text = "新增/修改";
            btnModify.UseVisualStyleBackColor = true;
            btnModify.Click += btnModify_Click;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Font = new Font("微软雅黑", 14F);
            label5.Location = new Point(522, 18);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(69, 25);
            label5.TabIndex = 12;
            label5.Text = "规则组";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Font = new Font("微软雅黑", 14F);
            label6.Location = new Point(522, 304);
            label6.Margin = new Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new Size(69, 25);
            label6.TabIndex = 13;
            label6.Text = "组明细";
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox1.Font = new Font("微软雅黑", 11F);
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(600, 18);
            comboBox1.Margin = new Padding(2, 2, 2, 2);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(160, 28);
            comboBox1.TabIndex = 15;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // comboBox2
            // 
            comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox2.Font = new Font("微软雅黑", 11F);
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(599, 303);
            comboBox2.Margin = new Padding(2, 2, 2, 2);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(160, 28);
            comboBox2.TabIndex = 16;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // btnDeleteGroup
            // 
            btnDeleteGroup.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDeleteGroup.Font = new Font("微软雅黑", 11F);
            btnDeleteGroup.Location = new Point(511, 548);
            btnDeleteGroup.Margin = new Padding(2, 2, 2, 2);
            btnDeleteGroup.Name = "btnDeleteGroup";
            btnDeleteGroup.Size = new Size(60, 32);
            btnDeleteGroup.TabIndex = 17;
            btnDeleteGroup.Text = "删除组";
            btnDeleteGroup.UseVisualStyleBackColor = true;
            btnDeleteGroup.Click += btnDeleteGroup_Click;
            // 
            // comboBox3
            // 
            comboBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox3.Font = new Font("微软雅黑", 11F);
            comboBox3.FormattingEnabled = true;
            comboBox3.Location = new Point(243, 303);
            comboBox3.Margin = new Padding(2, 2, 2, 2);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(153, 28);
            comboBox3.TabIndex = 18;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("微软雅黑", 16F);
            label7.Location = new Point(160, 302);
            label7.Margin = new Padding(2, 0, 2, 0);
            label7.Name = "label7";
            label7.Size = new Size(79, 30);
            label7.TabIndex = 19;
            label7.Text = "自动化";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(780, 601);
            Controls.Add(label7);
            Controls.Add(comboBox3);
            Controls.Add(btnDeleteGroup);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(btnModify);
            Controls.Add(btnDelete);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(btnReplace);
            Margin = new Padding(2, 2, 2, 2);
            MinimumSize = new Size(796, 640);
            Name = "Form1";
            Text = "正则替换 .net9";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private Button btnDeleteGroup;
        private ComboBox comboBox3;
        private Label label7;
    }
}