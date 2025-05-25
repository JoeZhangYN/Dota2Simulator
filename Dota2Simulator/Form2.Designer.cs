
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            label1 = new System.Windows.Forms.Label();
            tb_name = new System.Windows.Forms.TextBox();
            tb_阵营 = new System.Windows.Forms.TextBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            lb_name = new System.Windows.Forms.Label();
            lb_阵营 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            tb_x = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            tb_y = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            tb_delay = new System.Windows.Forms.TextBox();
            lb_状态抗性 = new System.Windows.Forms.Label();
            tb_状态抗性 = new System.Windows.Forms.TextBox();
            button1 = new System.Windows.Forms.Button();
            lb_攻速 = new System.Windows.Forms.Label();
            tb_攻速 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // tb_name
            // 
            resources.ApplyResources(tb_name, "tb_name");
            tb_name.Name = "tb_name";
            tb_name.TextChanged += Tb_name_TextChanged;
            // 
            // tb_阵营
            // 
            resources.ApplyResources(tb_阵营, "tb_阵营");
            tb_阵营.Name = "tb_阵营";
            tb_阵营.TextChanged += tb_阵营_TextChanged;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // lb_name
            // 
            resources.ApplyResources(lb_name, "lb_name");
            lb_name.Name = "lb_name";
            // 
            // lb_阵营
            // 
            resources.ApplyResources(lb_阵营, "lb_阵营");
            lb_阵营.Name = "lb_阵营";
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // tb_x
            // 
            resources.ApplyResources(tb_x, "tb_x");
            tb_x.Name = "tb_x";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // tb_y
            // 
            resources.ApplyResources(tb_y, "tb_y");
            tb_y.Name = "tb_y";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // tb_delay
            // 
            resources.ApplyResources(tb_delay, "tb_delay");
            tb_delay.Name = "tb_delay";
            tb_delay.TextChanged += Tb_delay_TextChanged;
            // 
            // lb_状态抗性
            // 
            resources.ApplyResources(lb_状态抗性, "lb_状态抗性");
            lb_状态抗性.Name = "lb_状态抗性";
            // 
            // tb_状态抗性
            // 
            resources.ApplyResources(tb_状态抗性, "tb_状态抗性");
            tb_状态抗性.Name = "tb_状态抗性";
            tb_状态抗性.TextChanged += tb_状态抗性_TextChanged;
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // lb_攻速
            // 
            resources.ApplyResources(lb_攻速, "lb_攻速");
            lb_攻速.Name = "lb_攻速";
            // 
            // tb_攻速
            // 
            resources.ApplyResources(tb_攻速, "tb_攻速");
            tb_攻速.Name = "tb_攻速";
            tb_攻速.TextChanged += Tb_攻速_TextChanged;
            // 
            // Form2
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(lb_攻速);
            Controls.Add(tb_攻速);
            Controls.Add(button1);
            Controls.Add(lb_状态抗性);
            Controls.Add(tb_状态抗性);
            Controls.Add(label6);
            Controls.Add(tb_delay);
            Controls.Add(label5);
            Controls.Add(tb_y);
            Controls.Add(label4);
            Controls.Add(tb_x);
            Controls.Add(lb_阵营);
            Controls.Add(lb_name);
            Controls.Add(tb_阵营);
            Controls.Add(tb_name);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Name = "Form2";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_name;
        private System.Windows.Forms.Label lb_阵营;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lb_状态抗性;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lb_攻速;
        public System.Windows.Forms.TextBox tb_阵营;
        public System.Windows.Forms.TextBox tb_name;
        public System.Windows.Forms.TextBox tb_x;
        public System.Windows.Forms.TextBox tb_y;
        public System.Windows.Forms.TextBox tb_delay;
        public System.Windows.Forms.TextBox tb_状态抗性;
        public System.Windows.Forms.TextBox tb_攻速;
    }
}