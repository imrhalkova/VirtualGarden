namespace VirtualGarden
{
    public partial class Form1 : Form
    {
        private Garden _garden;
        public Form1()
        {
            InitializeComponent();
            _garden = new Garden(4, 4, new Player(20, 20));
            UpdateUI();
        }

        private void UpdateUI()
        {

        }

        private void NewDayButton_Click(object sender, EventArgs e)
        {
            _garden.NewDay();
            UpdateUI();
        }
    }
}
