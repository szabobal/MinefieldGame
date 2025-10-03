using System;

namespace TimerTest
{
    public class Ship
    {
        public int Position { get; private set; }

        private double timeSinceMove = 0;
        private double timeSinceBomb = 0;
        private double bombInterval = 950; // Start at 500ms

        public void Update(double deltaTime)
        {
            timeSinceMove += deltaTime;
            timeSinceBomb += deltaTime;

            // Move every 1 second
            if (timeSinceMove >= 1000)
            {
                Position++;
                timeSinceMove = 0;
            }

            // Drop bombs at decreasing intervals (500ms -> 300ms)
            if (timeSinceBomb >= bombInterval)
            {
                DropBomb();
                timeSinceBomb = 0;

                // Decrease interval by 10ms, minimum 300ms
                if (bombInterval > 700)
                {
                    bombInterval = Math.Max(700, bombInterval - 50);
                }
            }
        }

        private void DropBomb()
        {
            Console.WriteLine($"Bomb dropped at position {Position}!");
            // Add your bomb creation logic here
        }

        public Ship()
        {
            Position = 0;
        }
    }
}