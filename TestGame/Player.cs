using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    internal class Player
    {
        //public List<Vector2> segments { get; private set; } = new List<Vector2>();
        public List<Segment> segs { get; private set; } = new List<Segment>();

        public static int lastSegmentIndex = 0;

        //public bool AddSegment { set; get; } = false;
        public int X, Y;
        public State state = State.NONE;
        public int collected = 0;


        public float Speed { get; set; } = 8f; //squares per second
        public float partStep = 0.0f;

        public Player(int gridX, int gridY, int count = 1)
        {
            X = gridX;
            Y = gridY;
            for (int i = 0; i < count; i++)
            {
                segs.Add(new Segment(gridX - i, gridY));
                //collisionMap[x, y] = true;
                //segments.Add(GameSurface.ToScreenCoords(x, y));
            }

            lastSegmentIndex = segs.Count - 1;

            GameSurface.PlaceFood(20, 20);
        }

        public class Segment
        {
            //2d collision map, todo: flatten array for better performance
            public static bool[,] collisionMap = new bool[GameSurface.gridWidth, GameSurface.gridHeight];
            
            public Vector2 screenPosition;
            public int X = 0, Y = 0;

            public Segment(int x, int y)
            {
                SetCoords(x, y);
            }

            public void SetCoords(int _x, int _y)
            {
                collisionMap[X, Y] = false; //update collisionmap
                collisionMap[_x, _y] = true;
                X = _x;
                Y = _y;
                screenPosition = GameSurface.ToScreenCoords(_x, _y);
            }
        }

        public void Update() //remove last element and add a new one in front
        {
            if (partStep >= 1.0f)
            {
                int wholeSteps = (int)partStep;
                partStep = 0.0f;

                while (wholeSteps > 0) //update until all steps are processed; might not be the best solution if the game is running faster
                {
                    switch (state)
                    {
                        case State.UP:
                            Y -= 1;
                            break;
                        case State.DOWN:
                            Y += 1;
                            break;
                        case State.LEFT:
                            X -= 1;
                            break;
                        case State.RIGHT:
                            X += 1;
                            break;
                    }

                    //Collision detection
                    int w = GameSurface.gridWidth;
                    int h = GameSurface.gridHeight;

                    if ((X < 0 || Y < 0 || X >= w || Y >= h) 
                        || Segment.collisionMap[X, Y])
                    {
                        state = State.CRASHED;
                        return;
                    }
                        

                    var i = segs[lastSegmentIndex]; //get last segment...

                    if (GameSurface.food.X == X && GameSurface.food.Y == Y) //food collision
                    {
                        segs.Insert(lastSegmentIndex, new Segment(i.X, i.Y));
                        collected++;
                        GameSurface.PlaceFood();
                        //AddSegment = false;
                    }
                    else
                    {
                        if (lastSegmentIndex > 0)
                            lastSegmentIndex--;
                        else
                            lastSegmentIndex = segs.Count - 1;
                    }
                    i.SetCoords(X, Y); //...so we can move it to the front

                    

                    
                    //var i = segments[segments.Count - 1];
                    //segments.RemoveAt(segments.Count - 1);
                    //segments.Insert(0, GameSurface.ToScreenCoords(X, Y));

                    wholeSteps--;
                }
            }
        }

        internal enum State : byte
        {
            NONE = 0x00,

            MOVING = 0x10,
            UP = 0x11,
            DOWN = 0x12,
            LEFT = 0x13,
            RIGHT = 0x14,

            CRASHED = 0x20,
            CLEARED = 0x30,

            MOVING_MASK = 0xF0
        }

    }
}
