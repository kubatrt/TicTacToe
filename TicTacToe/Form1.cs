using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        // Jedyna struktura danych na której opiera się gra, lista przycisków
        private List<Button> buttons = new List<Button>();

        public Form1()
        {
            InitializeComponent();
            // dodaj zdarzenie dla gra PCvsPC
            timer1.Tick += new EventHandler(PCGame);
            // dymślnie rozpocznij grę w trybie PvP
            startGame(GameMode.PlayerVsPlayer);
        }

        void startGame(GameMode mode)
        {
            new Settings(); // zresetuj ustawienia do domyślnych

            // wczytaj opcje ustawione w menu
            Settings.CurrentGameMode = mode;
            if (x3ToolStripMenuItem.Checked)
                Settings.BoardSize = 3;
            else if (x4ToolStripMenuItem.Checked)
                Settings.BoardSize = 4;
            else if (x5ToolStripMenuItem.Checked)
                Settings.BoardSize = 5;

            // losowanie pierwszego gracza
            Random rand = new Random();
            int whoStart = rand.Next(0, 2); // losuje 0 albo 1...

            if (whoStart == 0)
                Settings.CurrentPlayer = PlayerMarker.X;    // to człowiek
            else
                Settings.CurrentPlayer = PlayerMarker.O;    // to komputer

            
            PrepareBoard();
            setCaption();

            // pierwszy ruch komputera
            if (Settings.CurrentGameMode == GameMode.PlayerVsPC
                && Settings.CurrentPlayer == PlayerMarker.O)
            {
                PerformPCMove(Settings.CurrentPlayer);
                NextPlayer();
            }
            else if(Settings.CurrentGameMode == GameMode.PCVsPC)
            {
                timer1.Start();
            }
        }

        // Zdarzenie dodane do timera odpowiedzialne za grę 2 komputerów
        void PCGame(object sender, EventArgs e)
        {
            if(!Settings.GameOver)
            {
                PerformPCMove(Settings.CurrentPlayer);
                CheckWinConditionsForCurrentPlayer();
                NextPlayer();
            }
            else
            {
                timer1.Stop();
                WhichPlayerWon();
            }
        }

        // Ustawia belke tytułową dla okna, czy tura
        void setCaption()
        {
            string caption = "KiK";

            if (Settings.GameOver)
                this.Text = caption;
            else
                this.Text = caption + " - tura: " + Settings.CurrentPlayer;
        }

        // sprwadza który gracz wygrał i wyświetla odpowiedni komunikat
        void WhichPlayerWon()
        {
            if (Settings.WonPlayer == PlayerMarker.Empty)
            {
                MessageBox.Show("REMIS!");
            }
            else if (Settings.WonPlayer == PlayerMarker.X)
            {
                MessageBox.Show("WYGRYWA GRACZ: X!");
            }
            else if (Settings.WonPlayer == PlayerMarker.O)
            {
                MessageBox.Show("WYGRYWA GRACZ: O!");
            }
        }

        // Zdarzenie na kliknięcie pola
        void ButtonClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            // przy którymś z warunków, wyjdź i nic nie rób
            if (button.Text != string.Empty || Settings.GameOver || Settings.CurrentGameMode == GameMode.PCVsPC)
                return;

            // przypisz pole i sprawdź czy wygrana dla gracza
            button.Text = Settings.CurrentPlayer.ToString();
            CheckWinConditionsForCurrentPlayer();

            if (GetEmptyFieldsLeft() > 0 && !Settings.GameOver)
            {
                if (Settings.CurrentGameMode == GameMode.PlayerVsPlayer)
                {
                    // następny gracz
                    NextPlayer();
                }
                else if (Settings.CurrentGameMode == GameMode.PlayerVsPC)
                {
                    // następny gracz
                    NextPlayer();

                    // i wykonaj ruch komputera
                    PerformPCMove(Settings.CurrentPlayer);
                    CheckWinConditionsForCurrentPlayer();
                    
                    if(Settings.GameOver || GetEmptyFieldsLeft() == 0)
                        WhichPlayerWon();
                    else
                        NextPlayer();
                }
            }
            else
            {
                WhichPlayerWon();
            }
        }

        // sprawdź w rzędzie poziomo, konkretnie sprawdź pola na wsp. x dla zadanego y
        bool CheckWinningPatternHorizontal(int startY, PlayerMarker player)
        {
            for (int x = 0; x < Settings.BoardSize; ++x)
            {
                int index = (startY * Settings.BoardSize) + x;  // policz który to indeks w liście przycisków
                // jeżeli którykolwiek będzie inny od oczekiwanego możesz przerwać i zwrócić false
                if (buttons[index].Text != player.ToString())   
                    return false;
            }
            return true;
        }

        // sprawdź w rzędzie pionowo, konkretnie sprawdz pola na wsp. y dla zadanego x
        bool CheckWinningPatternVertical(int startX, PlayerMarker player)
        {
            for (int y = 0; y < Settings.BoardSize; ++y)
            {
                int index = (y * Settings.BoardSize) + startX;
                if (buttons[index].Text != player.ToString())
                    return false;
            }

            return true;
        }

        // sprwadź na skoks od lewej do prawej
        bool CheckWinningPatternDiagonallyRight(PlayerMarker player)
        {
            bool win = true;
            for (int y = 0; y < Settings.BoardSize; ++y)
            {
                for (int x = 0; x < Settings.BoardSize; ++x)
                {
                    int index = (y * Settings.BoardSize) + x;
                    if (x == y)
                    {
                        if (buttons[index].Text != player.ToString())
                            win = false;
                    }
                }
            }
            return win;
        }

        // sprawdź na skosk do prawej do lewej
        bool CheckWinningPatternDiagonallyLeft(PlayerMarker player)
        {
            bool win = true;
            for (int y = Settings.BoardSize; y > 0; --y)
            {
                for (int x = Settings.BoardSize; x > 0; --x)
                {
                    int index = (y * Settings.BoardSize) - x;
                    if (x == y)
                    {
                        if (buttons[index].Text != player.ToString())
                            win = false;
                    }
                }
            }
            return win;
        }

        // sprawdzenie wszystkich wariantów wygranej dla aktualnego gracza
        void CheckWinConditionsForCurrentPlayer()
        {
            /* przejście przez elementy w tablicy 2D używając tablicy 1D
            for (int y = 0; y < Settings.BoardSize; ++y)
            {
                for (int x = 0; x < Settings.BoardSize; ++x)
                {
                    int index = (y * Settings.BoardSize) + x;
                }

            }*/
            bool win = false;

            if(CheckWinningPatternDiagonallyLeft(Settings.CurrentPlayer))
                win = true;
            else if(CheckWinningPatternDiagonallyRight(Settings.CurrentPlayer))
                win = true;

            // sprawdzaj dalej
            if (!win)
            {
                // sprawdź kolejne wiersze
                for (int y = 0; y < Settings.BoardSize; ++y)
                {
                    if (CheckWinningPatternHorizontal(y, Settings.CurrentPlayer))
                    {
                        win = true;
                        break;
                    }
                }
            }

            if (!win)
            {
                // sprawdź kolejne kolumny
                for (int x = 0; x < Settings.BoardSize; ++x)
                {
                    if (CheckWinningPatternVertical(x, Settings.CurrentPlayer))
                    {
                        win = true;
                        break;
                    }
                }
            }

            // na koniec jeżeli któreś sprawdzenie dało true
            if (win == true)
            {
                Settings.WonPlayer = Settings.CurrentPlayer;
                Settings.GameOver = true;
            }
        }

        // ile zostało wolnych pól na planszy
        int GetEmptyFieldsLeft()
        {
            int count = 0;
            foreach (Button button in buttons)
            {
                if (button.Text == String.Empty)
                    count++;
            }
            return count;
        }

        void PerformPCMove(PlayerMarker player)
        {
            // Ruch komputera, można zaimplementować coś z...
            https://en.wikipedia.org/wiki/Tic-tac-toe

            // wybierz losowe wolne pole
            System.Random rand = new Random();
            List<int> emptyIndexes = new List<int>();
            for(int i=0; i < buttons.Count(); ++i)
            {
                if (buttons[i].Text == String.Empty)    // sprawdź które przyciski są puste
                    emptyIndexes.Add(i);    //  dodaj indeks to listy
            }

            int selectedIndex = rand.Next(0, emptyIndexes.Count()); // wylosuj indeks wolnego pola
            buttons[emptyIndexes[selectedIndex]].Text = player.ToString();  // użyj go aby wykonać ruch
        }


        void NextPlayer()
        {
            if (Settings.GameOver)
                return;

            if (Settings.CurrentPlayer == PlayerMarker.X)
                Settings.CurrentPlayer = PlayerMarker.O;
            else
                Settings.CurrentPlayer = PlayerMarker.X;

            setCaption();
        }

        void PrepareBoard()
        {
            // wyczyść przyciski na panelu
            panelBoard.Controls.Clear();
            buttons.Clear();

            for (int y = 0; y < Settings.BoardSize; y++)
            {
                for (int x = 0; x < Settings.BoardSize; x++)
                {
                    // stwórz przycisk i ustaw odpowiednie parametry
                    Button button = new Button();
                    button.Size = new Size(Settings.ButtonWidth, Settings.ButtonHeight);
                    button.Location = new Point(x * Settings.ButtonWidth, y * Settings.ButtonHeight);
                    button.Text = String.Empty;
                    button.Visible = true;
                    button.Click += new EventHandler(ButtonClicked);
                    // dodaj przycisk do listy, aby mieć do nich dostęp, a nie zczytywać za każdym razem z panelBoard.Controls[]...
                    buttons.Add(button);
                }
            }

            panelBoard.Controls.AddRange(buttons.ToArray());
            ResizeForm();
        }

        // Ustaw rozmiar głównego okna (this.Size) adekwatny do rozmiaru planszy
        void ResizeForm()
        {
            // trzeba dodać trochę pikseli żeby przyciski się mieściły, trzeba choćby uwzględnić menu
            int widthAligment = Settings.ButtonWidth / 4;
            int heightAligment = Settings.ButtonHeight;

            switch (Settings.BoardSize)
            {
                case 3:
                    this.Size = new Size(Settings.BoardSize * Settings.ButtonWidth + widthAligment, 
                        Settings.BoardSize * Settings.ButtonHeight + heightAligment);
                    break;
                case 4:
                    this.Size = new Size(Settings.BoardSize * Settings.ButtonWidth + widthAligment,
                        Settings.BoardSize * Settings.ButtonHeight + heightAligment);
                    break;
                case 5:
                    this.Size = new Size(Settings.BoardSize * Settings.ButtonWidth + widthAligment,
                        Settings.BoardSize * Settings.ButtonHeight + heightAligment);
                    break;
            }

        }


        private void planszaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void wyjścieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void oProgramieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Gra w kółko i krzyżyk. 2018");
        }

        private void graczVsGraczToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(GameMode.PlayerVsPlayer);
        }

        private void graczVsKomputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(GameMode.PlayerVsPC);
        }

        private void x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x3ToolStripMenuItem.Checked = true;
            x4ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = false;
        }

        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x3ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = true;
            x5ToolStripMenuItem.Checked = false;
        }

        private void x5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x3ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = false;
            x5ToolStripMenuItem.Checked = true;
        }

        private void komputerVsKomputerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(GameMode.PCVsPC);
        }
    }
}
