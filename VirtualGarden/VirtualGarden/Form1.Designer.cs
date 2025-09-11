namespace VirtualGarden
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            NewDayButton = new Button();
            SuspendLayout();
            // 
            // NewDayButton
            // 
            NewDayButton.BackColor = Color.Gold;
            NewDayButton.FlatStyle = FlatStyle.Flat;
            NewDayButton.ForeColor = SystemColors.ControlText;
            NewDayButton.Location = new Point(289, 12);
            NewDayButton.Name = "NewDayButton";
            NewDayButton.Size = new Size(177, 40);
            NewDayButton.TabIndex = 0;
            NewDayButton.Text = "New Day";
            NewDayButton.UseVisualStyleBackColor = false;
            NewDayButton.Click += NewDayButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(NewDayButton);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button NewDayButton;
    }
}
