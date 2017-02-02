using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    //gamecanvas
    class GameSurface
    {
        //various metrics
        public static int gridWidth, gridHeight, squareSize;
        
        public static Rectangle playArea;
        public static Random r = new Random();

        public static Vector2 foodPos { get; private set; }
        public static Point food;

        public static int cellsLeft = 0;

        public static void Build(int widthPixel, int heightPixel) //size of drawing area
        {
            Point cell = new Point(widthPixel / gridWidth, heightPixel / gridHeight);

            //smaller unit determines square size
            if (cell.Y < cell.X)
            { //height pieces are smaller, therefor add letterbox to the right and left
                squareSize = cell.Y;

                int canvasWidth = squareSize * gridWidth; 
                int letterboxWidth = widthPixel - canvasWidth; //difference between canvas and whole drawing area
                int letterboxOffset = (letterboxWidth / 2 / squareSize) * squareSize; //make sure offset is within grid

                playArea = new Rectangle(letterboxOffset, 0, canvasWidth, heightPixel);
            }
            else
            { //almost the same for height
                squareSize = cell.X;

                int canvasHeight = squareSize * gridHeight;
                int letterboxHeight = heightPixel - canvasHeight;
                int letterboxOffset = (letterboxHeight / 2 / squareSize) * squareSize;

                playArea = new Rectangle(0, letterboxOffset, widthPixel, canvasHeight);
            }

            cellsLeft = gridWidth * gridHeight;
        }

        public static void PlaceFood(int x = -1, int y = -1)
        {
            do
            {
                if (x < 0)
                    x = r.Next(0, gridWidth);
                if (y < 0)
                    y = r.Next(0, gridHeight);
            }
            while (Player.Segment.collisionMap[x, y]);

            food = new Point(x, y);
            foodPos = ToScreenCoords(x, y);

        }

        public static Vector2 ToScreenCoords(int gridX, int gridY)
        {//convert to pixel
            int x = playArea.X + squareSize * gridX;
            int y = playArea.Y + squareSize * gridY;

            return new Vector2(x, y);
        }
    }

}
