using System;
using System.Windows.Forms;

namespace HexMineSweeper
{
    public partial class HexSweeperMenu : Form
    {
        public HexSweeperMenu() => InitializeComponent();

        private void button_easy_Click(object sender, EventArgs e) => StartGame(7);
        private void button_medium_Click(object sender, EventArgs e) => StartGame(12);
        private void button_hard_Click(object sender, EventArgs e) => StartGame(20);

        private void StartGame(int size)
        {
            Hide();

            HexSweeperGame formGame = new HexSweeperGame(size);

            formGame.FormClosing += new FormClosingEventHandler(OnCloseGameForm);

            formGame.Show();
        }

        private void OnCloseGameForm(object sender, FormClosingEventArgs e) => Show();
    }
}
