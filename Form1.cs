namespace Crush
{
    public partial class Form1 : Form
    {
        public static Form1 TheForm;
        public Form1()
        {
            InitializeComponent();
            TheForm = this;
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openAudioFileDialog.InitialDirectory = path;
            openAudioFileDialog.Filter = "*.wav|*.wav";
            var r = openAudioFileDialog.ShowDialog();
            if (r == DialogResult.OK)
            {
                var f = openAudioFileDialog.FileName;
                selectedWaveFile.Text = f;


            }
            else
            {
                // don't do anything
            }
        }

        public void UpdateStatus(string m)
        {
            statusLabel.Text = m;

            Application.DoEvents();

        }

        private void runButton_Click(object sender, EventArgs e)
        {
            MainLoop myMainLoop = new MainLoop(selectedWaveFile.Text);
            myMainLoop.DoProcessing();
            doneButton.Text = "Done";
            
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
