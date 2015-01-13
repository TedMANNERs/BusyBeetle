using System.Collections.Generic;
using System.Drawing;

namespace BusyBeetle.Core
{
    public class GameOfLifeWorld : World
    {
        public GameOfLifeWorld(int width, int height)
            : base(width, height)
        {
        }

        public override GameType GameType
        {
            get { return GameType.GameOfLife; }
        }

        public override List<PixelData> Tick()
        {
            List<PixelData> modifiedPixels = new List<PixelData>();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    List<PixelData> aliveNeighbours = GetNeighbours(i, j);

                    if (GetAt(i, j).ToArgb() == Color.Black.ToArgb())
                    {
                        if (aliveNeighbours.Count < 2)
                        {
                            modifiedPixels.Add(new PixelData(i, j, Color.White));
                        }
                        else if (aliveNeighbours.Count < 4)
                        {
                            modifiedPixels.Add(new PixelData(i, j, Color.Black));
                        }
                        else
                        {
                            modifiedPixels.Add(new PixelData(i, j, Color.White));
                        }
                    }
                    else if (aliveNeighbours.Count == 3)
                    {
                        modifiedPixels.Add(new PixelData(i, j, Color.Black));
                    }
                }
            }

            foreach (PixelData modifiedPixel in modifiedPixels)
                SetAt(modifiedPixel.PositionX, modifiedPixel.PositionY, modifiedPixel.Color);

            return modifiedPixels;
        }

        private List<PixelData> GetNeighbours(int x, int y)
        {
            List<PixelData> neighbours = new List<PixelData>();
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y)
                        continue;

                    Color color = GetAt(Clamp(i), Clamp(j));
                    if (color.ToArgb() == Color.Black.ToArgb())
                    {
                        neighbours.Add(new PixelData(Clamp(i), Clamp(j), color));
                    }
                }
            }
            return neighbours;
        }

        private int Clamp(int i)
        {
            if (i < 0)
                return Width - 1;
            if (i >= Width)
                return 0;
            return i;
        }
    }
}