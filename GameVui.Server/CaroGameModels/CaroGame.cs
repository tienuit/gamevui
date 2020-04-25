using System;
namespace GameVui.Server.CaroGameModels
{
    public class CaroGame
    {
        const int COLUMN = 20;
        const int ROW = 20;

        public bool IsGameOver { get; private set; }

        public bool IsDraw { get; private set; }

        public Client Player1 { get; set; }

        public Client Player2 { get; set; }

        public DateTime CreatedTime { get; set; }

        private int[,] chess_board = new int[COLUMN, ROW];

        public int turn;
        public DateTime StartTime;
        public CaroGame()
        {
            turn = 1;
            var now = DateTime.Now;
            CreatedTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            // Reset game
            for (var i = 0; i < COLUMN; i++)
            {
                for (var j = 0; j < ROW; j++)
                {
                    chess_board[i, j] = -1;
                }
            }
        }

        /// <summary>
        /// Insert a marker at a given position for a given player
        /// </summary>
        /// <param name="player">The player number should be 0 or 1</param>
        /// <param name="position">The position where to place the marker, should be between 0 and 9</param>
        /// <returns>True if a winner was found</returns>
        public bool Play(int player, int col, int row)
        {
            if (IsGameOver)
                return false;

            if (turn == player)
            {
                PlaceMarker(player, col, row);
                turn = 3 - turn;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks each different combination of marker placements and looks for a winner
        /// Each position is marked with an initial -1 which means no marker has yet been placed
        /// </summary>
        /// <returns>True if there is a winner</returns>
        public bool CheckWinner(int pCol, int pRow, int player)
        {
            //count nearby pawn
            int countBlock = 0;
            int count;
            int col, row;
            //////////////////////////////////////////////////////////
            count = 0;
            row = pRow;
            col = pCol - 1;
            while (col >= 0 && chess_board[col, row] == player)
            {
                count++;
                col--;
            }
            if (col >= 0 && chess_board[col, row] == 3 - player)
                countBlock++;
            col = pCol + 1;
            while (col < COLUMN && chess_board[col, row] == player)
            {
                count++;
                col++;
            }
            if (col < COLUMN && chess_board[col, row] == 3 - player)
                countBlock++;
            if (count == 4 && countBlock < 2)
            {
                return true;
            }
            ///////////////////////////////////////////////////////////////
            count = 0;
            countBlock = 0;
            col = pCol;
            row = pRow - 1;
            while (row >= 0 && chess_board[col, row] == player)
            {
                count++;
                row--;
            }
            if (row >= 0 && chess_board[col, row] == 3 - player)
                countBlock++;
            row = pRow + 1;
            while (row < ROW && chess_board[col, row] == player)
            {
                count++;
                row++;
            }
            if (row < ROW && chess_board[col, row] == 3 - player)
                countBlock++;
            if (count == 4 && countBlock < 2)
            {
                return true;
            }
            ///////////////////////
            count = 0;
            countBlock = 0;
            row = pRow - 1;
            col = pCol - 1;
            while (row >= 0 && col >= 0 && chess_board[col, row] == player)
            {
                count++;
                row--;
                col--;
            }
            if ((row >= 0 && col >= 0 && chess_board[col, row] == 3 - player))
                countBlock++;
            row = pRow + 1;
            col = pCol + 1;
            while (row < ROW && col < COLUMN && chess_board[col, row] == player)
            {
                count++;
                col++;
                row++;
            }
            if ((row < ROW && col < COLUMN && chess_board[col, row] == 3 - player))
                countBlock++;
            if (count == 4 && countBlock < 2)
            {
                return true;
            }
            ////////////////////////////////////
            count = 0;
            countBlock = 0;
            row = pRow + 1;
            col = pCol - 1;
            while (row < ROW && col >= 0 && chess_board[col, row] == player)
            {
                count++;
                row++;
                col--;
            }
            if (row < ROW && col >= 0 && chess_board[col, row] == 3 - player)
                countBlock++;

            row = pRow - 1;
            col = pCol + 1;
            while (row >= 0 && col < COLUMN && chess_board[col, row] == player)
            {
                count++;
                col++;
                row--;
            }
            if (row >= 0 && col < COLUMN && chess_board[col, row] == 3 - player)
                countBlock++;
            if (count == 4 && countBlock < 2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Places a marker at the given position for the given player as long as the position is marked as -1
        /// </summary>
        /// <param name="player">The player number should be 0 or 1</param>
        /// <param name="position">The position where to place the marker, should be between 0 and 9</param>
        /// <returns>True if the marker position was not already taken</returns>
        private void PlaceMarker(int player, int col, int row)
        {
            chess_board[col, row] = player;
        }
    }
}