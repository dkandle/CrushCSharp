namespace Crush
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
            openAudioFileDialog = new OpenFileDialog();
            openButton = new Button();
            selectedWaveFile = new Label();
            statusLabel = new Label();
            runButton = new Button();
            clipCountLabel = new Label();
            doneButton = new Button();
            SuspendLayout();
            // 
            // openButton
            // 
            openButton.Location = new Point(88, 82);
            openButton.Name = "openButton";
            openButton.Size = new Size(187, 23);
            openButton.TabIndex = 0;
            openButton.Text = "Select Wave File";
            openButton.UseVisualStyleBackColor = true;
            openButton.Click += openButton_Click;
            // 
            // selectedWaveFile
            // 
            selectedWaveFile.AutoSize = true;
            selectedWaveFile.Location = new Point(373, 82);
            selectedWaveFile.Name = "selectedWaveFile";
            selectedWaveFile.Size = new Size(104, 15);
            selectedWaveFile.TabIndex = 1;
            selectedWaveFile.Text = "Select an input file";
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(84, 193);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(131, 15);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Status Will Appear Here";
            // 
            // runButton
            // 
            runButton.Location = new Point(89, 134);
            runButton.Name = "runButton";
            runButton.Size = new Size(138, 23);
            runButton.TabIndex = 3;
            runButton.Text = "Process File";
            runButton.UseVisualStyleBackColor = true;
            runButton.Click += runButton_Click;
            // 
            // clipCountLabel
            // 
            clipCountLabel.AutoSize = true;
            clipCountLabel.Location = new Point(85, 240);
            clipCountLabel.Name = "clipCountLabel";
            clipCountLabel.Size = new Size(150, 15);
            clipCountLabel.TabIndex = 4;
            clipCountLabel.Text = "Clip Count will appear here";
            clipCountLabel.Visible = false;
            // 
            // doneButton
            // 
            doneButton.FlatAppearance.BorderColor = Color.Black;
            doneButton.FlatAppearance.BorderSize = 3;
            doneButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            doneButton.Location = new Point(89, 295);
            doneButton.Name = "doneButton";
            doneButton.Size = new Size(109, 50);
            doneButton.TabIndex = 6;
            doneButton.Text = "Quit";
            doneButton.UseVisualStyleBackColor = true;
            doneButton.Click += doneButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(doneButton);
            Controls.Add(clipCountLabel);
            Controls.Add(runButton);
            Controls.Add(statusLabel);
            Controls.Add(selectedWaveFile);
            Controls.Add(openButton);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog openAudioFileDialog;
        private Button openButton;
        private Label selectedWaveFile;
        private Label statusLabel;
        private Button runButton;
        private Label clipCountLabel;
        private Button doneButton;
    }
}
