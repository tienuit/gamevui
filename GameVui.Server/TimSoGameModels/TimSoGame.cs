using System;
namespace GameVui.Server.TimSoGameModels
{
    public class TimSoGame
    {
        public bool IsGameOver { get; private set; }

        public Client Player1 { get; set; }

        public Client Player2 { get; set; }

        
        private int totalNumber;
        private int[] field;
        private int[] target;
        private int currentPos = 0;
        private DateTime createdTime;
        public TimSoGame(int _totalNumber = 100)
        {
            totalNumber = _totalNumber;
            var now = DateTime.Now;
            createdTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            field = new int[totalNumber];
            target = new int[totalNumber];
            Random rd = new Random(DateTime.Now.Millisecond);
            // Reset game
            for (var i = 0; i < totalNumber; i++)
            {
                field[i] = i;
                target[i] = i;
            }
            for (var i = 0; i < totalNumber; i++)
            {
                var rdNumber = rd.Next(totalNumber);
                var temp = field[rdNumber];
                field[rdNumber] = field[i];
                field[i] = temp;

                rdNumber = rd.Next(rdNumber);
                temp = target[rdNumber];
                target[rdNumber] = target[i];
                target[i] = temp;
            }

        }
        public int TotalNumber { get { return totalNumber; } }
        public DateTime CreatedTime { get { return createdTime; } }
        public int[] Field
        {
            get { return field;}
        }
        public int[] Target
        {
            get { return target; }
        }

        public int CurrentNumber { get { return target[currentPos]; } }
     
        public bool Play(int position, out bool endGame, out int currentNumber)
        {
            if (IsGameOver)
            {
                currentNumber = -1;
                endGame = true;
                return false;
            }
            return PlaceMarker( position, out endGame, out currentNumber);

        }

    
        public int CheckWinner()
        {
            if (Player1.Count > Player2.Count)
                return 1;
            else if (Player1.Count < Player2.Count)
                return 2;
            else 
                return 0;
        }

        private bool PlaceMarker(int position, out bool endGame, out int cNumber)
        {
            endGame = false;
            cNumber = -1;
            if (position > field.Length)
                return false;
            if (field[position] == target[currentPos])
            {
                currentPos += 1;
                if (currentPos >= totalNumber)
                {
                    IsGameOver = true;
                    endGame = true;
                    return true;
                }
                cNumber = target[currentPos];
                return true;
            }
            return false;
        }
    }
}