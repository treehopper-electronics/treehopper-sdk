namespace FirmwareManager
{
    partial class FirmwareManagerForm
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
            this.rebootBtn = new System.Windows.Forms.Button();
            this.firmwareBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.updateSerialBtn = new System.Windows.Forms.Button();
            this.serialLabel = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.generateSerialBtn = new System.Windows.Forms.Button();
            this.updateNameBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rebootBtn
            // 
            this.rebootBtn.Location = new System.Drawing.Point(48, 98);
            this.rebootBtn.Name = "rebootBtn";
            this.rebootBtn.Size = new System.Drawing.Size(161, 23);
            this.rebootBtn.TabIndex = 0;
            this.rebootBtn.Text = "Reboot";
            this.rebootBtn.UseVisualStyleBackColor = true;
            this.rebootBtn.Click += new System.EventHandler(this.rebootBtn_Click);
            // 
            // firmwareBtn
            // 
            this.firmwareBtn.Location = new System.Drawing.Point(48, 127);
            this.firmwareBtn.Name = "firmwareBtn";
            this.firmwareBtn.Size = new System.Drawing.Size(161, 23);
            this.firmwareBtn.TabIndex = 1;
            this.firmwareBtn.Text = "Boot Into Bootloader";
            this.firmwareBtn.UseVisualStyleBackColor = true;
            this.firmwareBtn.Click += new System.EventHandler(this.firmwareBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(48, 169);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(161, 20);
            this.textBox1.TabIndex = 2;
            // 
            // updateSerialBtn
            // 
            this.updateSerialBtn.Location = new System.Drawing.Point(48, 195);
            this.updateSerialBtn.Name = "updateSerialBtn";
            this.updateSerialBtn.Size = new System.Drawing.Size(161, 23);
            this.updateSerialBtn.TabIndex = 3;
            this.updateSerialBtn.Text = "Update Serial Number";
            this.updateSerialBtn.UseVisualStyleBackColor = true;
            this.updateSerialBtn.Click += new System.EventHandler(this.updateSerialBtn_Click);
            // 
            // serialLabel
            // 
            this.serialLabel.AutoSize = true;
            this.serialLabel.Location = new System.Drawing.Point(48, 22);
            this.serialLabel.Name = "serialLabel";
            this.serialLabel.Size = new System.Drawing.Size(113, 13);
            this.serialLabel.TabIndex = 4;
            this.serialLabel.Text = "No boards connected.";
            // 
            // nameBox
            // 
            this.nameBox.Location = new System.Drawing.Point(51, 254);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(158, 20);
            this.nameBox.TabIndex = 5;
            // 
            // generateSerialBtn
            // 
            this.generateSerialBtn.Location = new System.Drawing.Point(48, 50);
            this.generateSerialBtn.Name = "generateSerialBtn";
            this.generateSerialBtn.Size = new System.Drawing.Size(161, 23);
            this.generateSerialBtn.TabIndex = 6;
            this.generateSerialBtn.Text = "Factory Program";
            this.generateSerialBtn.UseVisualStyleBackColor = true;
            this.generateSerialBtn.Click += new System.EventHandler(this.generateSerialBtn_Click);
            // 
            // updateNameBtn
            // 
            this.updateNameBtn.Location = new System.Drawing.Point(51, 280);
            this.updateNameBtn.Name = "updateNameBtn";
            this.updateNameBtn.Size = new System.Drawing.Size(158, 23);
            this.updateNameBtn.TabIndex = 7;
            this.updateNameBtn.Text = "Update Name";
            this.updateNameBtn.UseVisualStyleBackColor = true;
            this.updateNameBtn.Click += new System.EventHandler(this.updateNameBtn_Click);
            // 
            // FirmwareManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 335);
            this.Controls.Add(this.updateNameBtn);
            this.Controls.Add(this.generateSerialBtn);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.serialLabel);
            this.Controls.Add(this.updateSerialBtn);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.firmwareBtn);
            this.Controls.Add(this.rebootBtn);
            this.Name = "FirmwareManagerForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirmwareManagerForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button rebootBtn;
        private System.Windows.Forms.Button firmwareBtn;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button updateSerialBtn;
        private System.Windows.Forms.Label serialLabel;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Button generateSerialBtn;
        private System.Windows.Forms.Button updateNameBtn;

    }
}

