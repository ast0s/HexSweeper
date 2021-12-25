using HexMineSweeper.Controllers;
using System;
using System.Windows.Forms;

namespace HexMineSweeper
{
    public partial class HexSweeperGame : Form
    {
        MapController mapController = new MapController();
        public HexSweeperGame(int size)
        {
            InitializeComponent();
            mapController.Init(this, size);
        }

        public void TimerTick(object sender, EventArgs e)
        { mapController.timerLabel.Text = $"Time: ({mapController.GetTimeFromTimer()})"; }
    }
}
