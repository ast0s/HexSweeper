using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using static HexMineSweeper.Images;

namespace HexMineSweeper.Controllers
{
    class MapController
    {
        private int mapInitialSize;
        private int cellInitialSize;

        private int mapHeight;
        private int mapWidth;
        private int cellHeight;
        private int cellWidth;

        private int totalBombs;
        private int leftCells;

        private int[,] map;
        private HexButton[,] buttons;   

        private bool isFirstStep;
        private int firstCoordY;
        private int firstCoordX;

        public HexSweeperGame form;
        public Label timerLabel;

        public void Init(HexSweeperGame current, int size)
        {
            timer.Reset();
            form = current;
            mapInitialSize = size;
            cellInitialSize = form.Width / 8 * 7 / (mapInitialSize + 1);
            isFirstStep = true;

            mapHeight = size * 2;
            mapWidth = (int)Math.Round(size * 0.7);
            leftCells = mapHeight * mapWidth;
            cellHeight = (int)Math.Round(cellInitialSize / 2 * Math.Sqrt(3));
            cellWidth = cellInitialSize;
            
            InitMap();
            InitTimerLabel();
            InitButtons();
        }
        private void InitMap()
        {
            map = new int[mapHeight, mapWidth];

            for (int i = 0; i < mapHeight; i++)
                for (int j = 0; j < mapWidth; j++)
                    map[i, j] = 0;
        }
        private void InitButtons()
        {
            buttons = new HexButton[mapHeight, mapWidth];

            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    HexButton hb = new HexButton
                    {
                        Size = new Size(cellWidth, cellHeight),
                        Cursor = Cursors.Hand,
                        BackColor = Color.Gray
                    };

                    hb.MouseUp += new MouseEventHandler(OnButtonPressedMouse);

                    if (i % 2 == 0)
                        hb.Location = new Point(j * (cellWidth + cellWidth / 2) + form.Width / 16, form.Height / 6 + i / 2 * cellHeight);
                    else
                        hb.Location = new Point(cellWidth * 3 / 4 + j * (cellWidth + cellWidth / 2) + form.Width / 16, form.Height / 6 + cellHeight / 2 + i / 2 * cellHeight);
                    
                    form.Controls.Add(hb);

                    hb.iButton = i;
                    hb.jButton = j;
                    buttons[i, j] = hb;
                }
            }
        }
        private void InitTimerLabel()
        {
            timerLabel = new Label
            {
                Location = new Point(form.Width / 2 - 70, 40),
                Size = new Size(140, 25),
                BackColor = Color.LightGray,
                Font = new Font("Comic Sans MS", 12)
            };

            form.Controls.Add(timerLabel);
        }
        
        private Image SetImage(Image img) => new Bitmap(img, cellWidth, cellHeight + cellHeight / 18);

        private void OnButtonPressedMouse(object sender, MouseEventArgs e)
        {
            HexButton pb = sender as HexButton;
            switch (e.Button.ToString())
            {
                case "Right":
                    OnRightButtonPressed(pb);
                    break;
                case "Left":
                    OnLeftButtonPressed(pb);
                    break;
            }
        }
        private void OnRightButtonPressed(HexButton pb)
        {
            pb.rightButtonState++;
            pb.rightButtonState %= 3;

            switch (pb.rightButtonState)
            {
                case 0:
                    pb.Image = SetImage(nothing);
                    break;
                case 1:
                    pb.Image = SetImage(flaged);
                    break;
                case 2:
                    pb.Image = SetImage(informed);
                    break;
            }
        }
        private void OnLeftButtonPressed(HexButton pb)
        {
            if (isFirstStep)
                FirstStep(pb);

            OpenEmptyCells(pb.iButton, pb.jButton);
            CheckForFailAndWin(pb);
        }

        private void FirstStep(HexButton pb)
        {
            firstCoordY = pb.iButton;
            firstCoordX = pb.jButton;
            isFirstStep = false;

            SeedMap();
            AddCountersAroundBombs();
            TimerStart();
        }
        private void SeedMap()
        {
            Random r = new Random();
            totalBombs = mapInitialSize / 3 * mapInitialSize;

            for (int i = 0; i < totalBombs; i++)
            {
                int positionY = r.Next(0, mapHeight);
                int positionX = r.Next(0, mapWidth);

                while (map[positionY, positionX] == -1
                    || (Math.Abs(positionY - firstCoordY) <= 1 && Math.Abs(positionX - firstCoordX) <= 1)
                    || (Math.Abs(positionY - firstCoordY) <= 2 && positionX == firstCoordX))
                {
                    positionY = r.Next(0, mapHeight);
                    positionX = r.Next(0, mapWidth);
                }
                map[positionY, positionX] = -1;
            }
        }
        private void AddCountersAroundBombs()
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (map[i, j] == - 1)
                    {
                        if (i % 2 == 0)
                        {
                            if (IsInBorder(i - 2, j) && map[i - 2, j] != -1)
                                map[i - 2, j] = map[i - 2, j] + 1;
                            if (IsInBorder(i + 2, j) && map[i + 2, j] != -1)
                                map[i + 2, j] = map[i + 2, j] + 1;

                            for (int k = i - 1; k < i + 2; k += 2)
                                for (int l = j - 1; l < j + 1; l++)
                                    if (IsInBorder(k, l) && map[k, l] != -1)
                                        map[k, l] = map[k, l] + 1;
                        }
                        if (i % 2 != 0)
                        {
                            if (IsInBorder(i - 2, j) && map[i - 2, j] != -1)
                                map[i - 2, j] = map[i - 2, j] + 1;
                            if (IsInBorder(i + 2, j) && map[i + 2, j] != -1)
                                map[i + 2, j] = map[i + 2, j] + 1;

                            for (int k = i - 1; k < i + 2; k += 2)
                                for (int l = j; l < j + 2; l++)
                                    if (IsInBorder(k, l) && map[k, l] != -1)
                                        map[k, l] = map[k, l] + 1;
                        }
                    }
                }
            }
        }
        private void OpenCell(int i, int j)
        {
            buttons[i, j].Enabled = false;
            leftCells--;

            switch (map[i, j])
            {
                case -1:
                    buttons[i, j].Image = SetImage(bomb);
                    break;
                case 0:
                    buttons[i, j].Image = SetImage(_0);
                    break;
                case 1:
                    buttons[i, j].Image = SetImage(_1);
                    break;
                case 2:
                    buttons[i, j].Image = SetImage(_2);
                    break;
                case 3:
                    buttons[i, j].Image = SetImage(_3);
                    break;
                case 4:
                    buttons[i, j].Image = SetImage(_4);
                    break;
                case 5:
                    buttons[i, j].Image = SetImage(_5);
                    break;
                case 6:
                    buttons[i, j].Image = SetImage(_6);
                    break;
            }
        }
        private void OpenEmptyCells(int i, int j)
        {
            OpenCell(i, j);

            if (map[i, j] > 0 || map[i,j] == -1)
                return;

            if (i % 2 == 0)
            {
                for (int k = i - 1; k < i + 2; k += 2)
                {
                    for (int l = j - 1; l < j + 1; l++)
                    {
                        if (!IsInBorder(k, l) || !buttons[k, l].Enabled)
                            continue;
                        if (map[k, l] == 0)
                            OpenEmptyCells(k, l);
                        else if (map[k, l] > 0)
                            OpenCell(k, l);
                    }
                }

                if (IsInBorder(i - 2, j) && buttons[i - 2, j].Enabled)
                {
                    if (map[i - 2, j] == 0)
                        OpenEmptyCells(i - 2, j);
                    else if (map[i - 2, j] > 0)
                        OpenCell(i - 2, j);
                }

                if (IsInBorder(i + 2, j) && buttons[i + 2, j].Enabled)
                {
                    if (map[i + 2, j] == 0)
                        OpenEmptyCells(i + 2, j);
                    else if (map[i + 2, j] > 0)
                        OpenCell(i + 2, j);
                }
            }
            if (i % 2 != 0)
            {
                for (int k = i - 1; k < i + 2; k += 2)
                {
                    for (int l = j; l < j + 2; l++)
                    {
                        if (!IsInBorder(k, l) || !buttons[k, l].Enabled)
                            continue;
                        if (map[k, l] == 0)
                            OpenEmptyCells(k, l);
                        else if (map[k, l] > 0)
                            OpenCell(k, l);
                    }
                }

                if (IsInBorder(i - 2, j) && buttons[i - 2, j].Enabled)
                {
                    if (map[i - 2, j] == 0)
                        OpenEmptyCells(i - 2, j);
                    else if (map[i - 2, j] > 0)
                        OpenCell(i - 2, j);
                }

                if (IsInBorder(i + 2, j) && buttons[i + 2, j].Enabled)
                {
                    if (map[i + 2, j] == 0)
                        OpenEmptyCells(i + 2, j);
                    else if (map[i + 2, j] > 0)
                        OpenCell(i + 2, j);
                }
            }
        }
        private bool IsInBorder(int i, int j)
        {
            if (i < 0 || j < 0 || i > mapHeight - 1 || j > mapWidth - 1)
                return false;
            else
                return true;
        }

        private void CheckForFailAndWin(HexButton pb)
        {
            if (map[pb.iButton, pb.jButton] == -1)
            {
                TimerStop();
                ExposeAllBombs("Fail");
                MessageBox.Show($"You lost! Your time is { GetTimeFromTimer() }");
                form.Close();
            }
            if (totalBombs == leftCells)
            {
                TimerStop();
                ExposeAllBombs("Win");
                MessageBox.Show($"You won! Your time is { GetTimeFromTimer() }");
                form.Close();
            }
        }
        private void ExposeAllBombs(string state)
        {
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    if (map[i, j] == -1)
                    {
                        switch (state)
                        {
                            case "Win": buttons[i, j].Image = SetImage(nobomb);
                                break;
                            case "Fail": buttons[i, j].Image = SetImage(bombed);
                                break;
                        }
                    }
                }
            }
        }

        Stopwatch timer = new Stopwatch();
        public string GetTimeFromTimer()
        {
            var milliseconds = timer.ElapsedMilliseconds;
            var seconds = milliseconds / 1000;

            return string.Format("{0:00}:{1:00}:{2:00}", seconds / 60, seconds % 60, milliseconds % 100);
        }
        public void TimerStart()
        {
            timer.Start();
            form.timer1.Start();
        }
        public void TimerStop()
        {
            timerLabel.Text = $"Time: ({GetTimeFromTimer()})";
            timer.Stop();
            form.timer1.Stop();
        }
    }
}
