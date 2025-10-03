using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TimerTest
{
    public class TestObject
    {
        private Ship ship;
        private Bomb bomb1;
        private Bomb bomb2;


        private DateTime lastUpdate;
        private double timeSinceLog = 0;
        public void Start()
        {
            lastUpdate = DateTime.Now;
            var gameTimer = new System.Timers.Timer(100);
            gameTimer.Elapsed += GameLoop;
            gameTimer.Start();
        }

        private void GameLoop(object? sender, ElapsedEventArgs e)
        {

            double deltaTime = (DateTime.Now - lastUpdate).TotalMilliseconds;
            lastUpdate = DateTime.Now;

            ship.Update(deltaTime);
            bomb1.Update(deltaTime);
            bomb2.Update(deltaTime);

            timeSinceLog += deltaTime;

            // Log positions every 1 second
            if (timeSinceLog >= 1000)
            {
                Console.WriteLine($"Bomb1 (MEDIUM) Position: {bomb1.pos}");
                Console.WriteLine($"Bomb2 (HEAVY) Position: {bomb2.pos}");

                Console.WriteLine();

                timeSinceLog = 0;
            }
        }

        public TestObject()
        {
            ship = new Ship();
            bomb1 = new Bomb(0, Weight.MEDIUM);
            bomb2 = new Bomb(0, Weight.HEAVY);
            Start();
        }
    }
}
