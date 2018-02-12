using System;
using System.Collections.Generic;

namespace TicTacToe
{
    public enum GameMode
    {
        PlayerVsPlayer,
        PlayerVsPC,
        PCVsPC
    };

    public enum PlayerMarker
    {
        X,
        O,
        Empty
    }


    internal class Settings
    {
        public static int ButtonWidth { get; set; }
        public static int ButtonHeight { get; set; }
        public static int BoardSize { get; set; }

        public static PlayerMarker CurrentPlayer { get; set; }
        public static GameMode CurrentGameMode { get; set; }

        public static bool GameOver { get; set; }
        public static PlayerMarker WonPlayer { get; set; }

        // ustawienia domyślne w konstruktorze
        public Settings()
        {
            ButtonWidth = 64;
            ButtonHeight = 64;
            BoardSize = 3;

            CurrentPlayer = PlayerMarker.X;
            CurrentGameMode = GameMode.PlayerVsPlayer;

            GameOver = false;
            WonPlayer = PlayerMarker.Empty;
        }
        
    }
}
