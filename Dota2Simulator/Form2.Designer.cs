
namespace Dota2Simulator
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.tb_name = new System.Windows.Forms.TextBox();
            this.tb_丢装备 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_x = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_y = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_delay = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tb_状态抗性 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_攻速 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 171);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // tb_name
            // 
            this.tb_name.Location = new System.Drawing.Point(77, -4);
            this.tb_name.Margin = new System.Windows.Forms.Padding(4);
            this.tb_name.Name = "tb_name";
            this.tb_name.Size = new System.Drawing.Size(87, 23);
            this.tb_name.TabIndex = 4;
            this.tb_name.Text = "黑鸟";
            this.tb_name.TextChanged += new System.EventHandler(this.Tb_name_TextChanged);
            // 
            // tb_丢装备
            // 
            this.tb_丢装备.Location = new System.Drawing.Point(77, 19);
            this.tb_丢装备.Margin = new System.Windows.Forms.Padding(4);
            this.tb_丢装备.Name = "tb_丢装备";
            this.tb_丢装备.Size = new System.Drawing.Size(87, 23);
            this.tb_丢装备.TabIndex = 5;
            this.tb_丢装备.Text = "1,2,3";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(8, 199);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(149, 62);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "英雄名称";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "要丢的装备";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 97);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "偏移X";
            // 
            // tb_x
            // 
            this.tb_x.Location = new System.Drawing.Point(77, 93);
            this.tb_x.Margin = new System.Windows.Forms.Padding(4);
            this.tb_x.Name = "tb_x";
            this.tb_x.Size = new System.Drawing.Size(87, 23);
            this.tb_x.TabIndex = 8;
            this.tb_x.Text = "250";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 120);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "偏移Y";
            // 
            // tb_y
            // 
            this.tb_y.Location = new System.Drawing.Point(77, 116);
            this.tb_y.Margin = new System.Windows.Forms.Padding(4);
            this.tb_y.Name = "tb_y";
            this.tb_y.Size = new System.Drawing.Size(87, 23);
            this.tb_y.TabIndex = 10;
            this.tb_y.Text = "250";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 143);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 13;
            this.label6.Text = "测试延迟";
            // 
            // tb_delay
            // 
            this.tb_delay.Location = new System.Drawing.Point(77, 139);
            this.tb_delay.Margin = new System.Windows.Forms.Padding(4);
            this.tb_delay.Name = "tb_delay";
            this.tb_delay.Size = new System.Drawing.Size(87, 23);
            this.tb_delay.TabIndex = 12;
            this.tb_delay.Text = "30";
            this.tb_delay.TextChanged += new System.EventHandler(this.tb_delay_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 46);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 17);
            this.label7.TabIndex = 16;
            this.label7.Text = "状态抗性";
            // 
            // tb_状态抗性
            // 
            this.tb_状态抗性.Location = new System.Drawing.Point(77, 42);
            this.tb_状态抗性.Margin = new System.Windows.Forms.Padding(4);
            this.tb_状态抗性.Name = "tb_状态抗性";
            this.tb_状态抗性.Size = new System.Drawing.Size(87, 23);
            this.tb_状态抗性.TabIndex = 15;
            this.tb_状态抗性.Text = "0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(77, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 72);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 17);
            this.label8.TabIndex = 19;
            this.label8.Text = "攻击速度";
            // 
            // tb_攻速
            // 
            this.tb_攻速.Location = new System.Drawing.Point(77, 68);
            this.tb_攻速.Margin = new System.Windows.Forms.Padding(4);
            this.tb_攻速.Name = "tb_攻速";
            this.tb_攻速.Size = new System.Drawing.Size(87, 23);
            this.tb_攻速.TabIndex = 18;
            this.tb_攻速.Text = "0";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(173, 277);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tb_攻速);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tb_状态抗性);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tb_delay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tb_y);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_x);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_丢装备);
            this.Controls.Add(this.tb_name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form2";
            this.Text = "Form2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_name;
        private System.Windows.Forms.TextBox tb_丢装备;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_x;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_y;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_delay;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tb_状态抗性;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_攻速;
    }
}