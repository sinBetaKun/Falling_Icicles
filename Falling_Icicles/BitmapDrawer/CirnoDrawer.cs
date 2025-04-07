using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using Vortice.DXGI;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using Color = Vortice.Mathematics.Color;

namespace Falling_Icicles.BitmapDrawer
{
    public class CirnoDrawer : BitmapDrawerBase
    {
        static readonly int width = 32;
        static readonly int height = 32;
        static readonly int scale = 4;
        static readonly int goAroundDist = 160;
        static readonly int runDistYamada = 300;

        static readonly Color[] colors = [
            new(  0,   0,   0,   0),
            new(  0,   0,   0),
            new( 16,  99,  33),
            new( 24, 165,  66),
            new( 41, 115, 214),
            new(115, 206, 247),
            new(255, 255, 255),
            new(  8,  49, 123),
            new(181, 173, 222),
            new(  0,   0, 132),
            new(132, 132, 247),
            new(239, 132, 132),
            new(247, 181, 173),
            new(247, 231, 222),
            new(199,  99,  90),
            new(198,   0,   0),
            new( 49,  66, 247),
            new(206, 198, 247),
            new(165, 156, 206),
            new( 24,  24, 132),
            ];

        static private readonly byte[] labels = [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 3, 1, 2, 2, 3, 3, 3, 3, 3, 1, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 3, 3, 3, 3, 3, 3, 2, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 3, 3, 3, 2, 1, 1, 4, 4, 5, 5, 5, 5, 4, 5, 5, 5, 4, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 3, 3, 1, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 2, 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 2, 1, 4, 5, 5, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 4, 5, 1, 0, 0, 0, 0, 0,
            0, 0, 0, 1, 3, 2, 1, 5, 5, 7, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 7, 1, 0, 0, 0, 0, 0,
            0, 0, 1, 3, 2, 1, 5, 5, 7, 5, 6, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 5, 7, 1, 0, 0, 0, 0,
            0, 0, 0, 1, 2, 1, 5, 5, 7, 5, 5, 6, 5, 6, 6, 5, 6, 5, 6, 6, 6, 5, 6, 6, 4, 5, 5, 1, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 1, 5, 4, 7, 5, 5, 5, 4, 5, 5, 5, 5, 4, 5, 5, 5, 4, 5, 5, 5, 4, 5, 1, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 4, 4, 7, 5, 5, 4, 7, 5, 5, 5, 4, 7, 5, 5, 5, 7, 4, 5, 5, 5, 7, 5, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 4, 4, 4, 5, 7, 7,11, 5, 5, 7,12, 7, 4, 5, 5,12, 7, 4, 5, 5, 7, 1, 1, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 4, 7, 5, 7, 4, 7,12, 5, 7,12,13,13, 7, 4, 1, 1, 7, 1, 1, 5, 1, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 1, 4, 7, 7, 4, 4, 4, 1, 1, 1, 1,13,13,13, 7, 1, 8, 7, 1, 0, 1, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 4, 4, 4, 4, 7, 5, 1, 8, 1, 1,13,13,13,13,13, 9, 6, 4, 1, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 4, 4, 7, 4, 7, 5, 5, 6, 6, 9,13,13,13,13,13,10, 6, 5, 1, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 4, 1, 7, 4, 7, 5, 5, 6, 9,10,13,13,13,13,13, 9,13, 4, 5, 1, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 1, 0, 1, 4, 7, 5, 5,12, 9, 9,13,13,13,13,13,13,12, 4, 1, 1, 5, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 1, 5, 6, 1, 1, 1, 7, 5,11,12,13,13,13,13,13,13,12, 5, 1, 6, 1, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 4, 1, 6, 1, 7, 1, 5,14,11,12,12,12,12,11, 1, 1, 1, 4, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 6, 6,16,17, 6,15, 6,17, 1, 4, 5, 0, 4, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 5, 5, 5, 6, 6, 0, 0, 1, 6,17,16,16,16,15,15,18, 1, 5, 6, 6, 6, 5, 0, 0, 0, 0, 0,
            0, 0, 0, 5, 5, 5, 6, 6, 6, 5, 5, 1,13,12, 1,16,16,16,15,16,15,12, 1, 0, 4, 4, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 4, 4, 4, 5, 5, 4, 1,13,13, 1,19,19,16,16,16,16,16, 1,12, 1, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 1,12,13, 1,19,19,16,16,16,16,16,16,17, 1, 5, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 5, 6, 1, 1,18,19,16, 6,16,16, 6,16,16, 6, 6, 1, 5, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 5, 6, 4, 1,18,19,16,17, 6,16, 6, 6, 6,16, 6, 1, 4, 4, 5, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 5, 5, 4, 0, 0, 1,18, 6, 6, 6,17, 6, 6, 6,17, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,12,12, 1, 1, 1, 1, 1,12, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,13, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            ];

        private readonly GreaterFairyBitmapDrawer Yamada;

        private readonly Xoshiro256StarStar xoshiro;
        private ulong xoshiroNext;
        private ulong seed = 0;

        public int X => (int)xList[0];
        public int Y => (int)yList[0];

        public CirnoDrawer(IGraphicsDevicesAndContext devices, GreaterFairyBitmapDrawer Yamada, GameViewSource gameViewSource) : base(devices, gameViewSource)
        {
            this.Yamada = Yamada;

            xoshiro = new(seed);
            xoshiroNext = xoshiro.Next();

            initialXList.Add(400);
            initialYList.Add(0);
            initialRotateList.Add(0);
            initialBmIndexListList.Add(0);

            AddNewBitmap(width * scale, height * scale);

            var dc = devices.DeviceContext;

            ID2D1SolidColorBrush[] brushs = [.. colors.Select(i => devices.DeviceContext.CreateSolidColorBrush(i))];

            dc.Target = bitmaps[0];
            dc.BeginDraw();
            dc.Clear(null);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (labels[i * width + j] > 0)
                        dc.FillRectangle(new Vortice.RawRectF(j * scale, i * scale, (j + 1) * scale, (i + 1) * scale), brushs[labels[i * width + j]]);

            dc.EndDraw();
            dc.Target = null;//Targetは必ずnullに戻す。
            foreach (var item in brushs)
                item?.Dispose();
        }

        public override void UpdatePlace()
        {
            switch (gameViewSource.Stage)
            {
                case 0:
                    return;

                case 1:
                    TakeDistanceFromYamada();
                    MoveLimit();
                    return;

                case 2:
                    TakeDistanceFromYamada();
                    return;

                default:
                    AwayFromYamada();
                    return;
            }
        }

        private void AwayFromYamada()
        {
            (int horizontal, int vertical) = CalcDirect(Yamada, 0, 0, goAroundDist);

            (int move_x, int move_y) = GoAround(horizontal, vertical);

            CalcNextPos(move_x, move_y);
        }

        private void TakeDistanceFromYamada()
        {
            if (IsNearTo(Yamada, runDistYamada))
            {
                (int horizontal, int vertical) = CalcDirect(Yamada, 0, 0, goAroundDist);

                (int move_x, int move_y) = GoAround(horizontal, vertical);

                CalcNextPos(move_x, move_y);
            }
        }

        private void CalcNextPos(int move_x, int move_y)
        {
            if (move_x == 0 || move_y == 0)
            {
                xList[0] += 200f * move_x / gameViewSource.FPS;
                yList[0] += 200f * move_y / gameViewSource.FPS;
            }
            else
            {
                xList[0] += 141f * move_x / gameViewSource.FPS;
                yList[0] += 141f * move_y / gameViewSource.FPS;
            }
        }

        private (int move_x, int move_y) GoAround(int horizontal, int vertical)
        {
            float left = xList[0] - GameViewSource.Edge1.X;
            float right = GameViewSource.Edge2.X - xList[0];
            float top = yList[0] - GameViewSource.Edge1.Y;
            float bottom = GameViewSource.Edge2.Y - yList[0];

            int sign1 =
                left < goAroundDist
                ? 1
                : right < goAroundDist
                    ? -1
                    : 0;
            
            int sign2 =
                top < goAroundDist
                ? 1
                : bottom < goAroundDist
                    ? -1
                    : 0;

            if (seed != gameViewSource.Param.SeedValue)
            {
                seed = gameViewSource.Param.SeedValue;
                xoshiro.Seed(seed);
                xoshiroNext = xoshiro.Next();
            }

            return
                sign2 == 0
                ? sign1 == 0
                    ? (horizontal == 0 ? (xoshiroNext & 2) > 0 ? -1 : 1 : -horizontal, -vertical)
                    : (sign1, vertical == 0 ? ((xoshiroNext & 1) > 0 ? -1 : 1) : -vertical)
                : sign1 == 0
                    ? (-horizontal, sign2)
                    : (horizontal > 0 && vertical > 0)
                        ? ((sign1 > 0 ? left : right) < (sign2 > 0 ? top : bottom)
                            ? (0, sign2)
                            : (sign1, 0))
                        : (-horizontal, -vertical);
        }

        private void MoveLimit()
        {
            // 端からはみ出た時(X)
            if (xList[0] < GameViewSource.Edge1.X)
            {
                xList[0] = GameViewSource.Edge1.X;
            }
            else if (xList[0] > GameViewSource.Edge2.X)
            {
                xList[0] = GameViewSource.Edge2.X;
            }

            // 端からはみ出た時(Y)
            if (yList[0] < GameViewSource.Edge1.Y)
            {
                yList[0] = GameViewSource.Edge1.Y;
            }
            else if (yList[0] > GameViewSource.Edge2.Y)
            {
                yList[0] = GameViewSource.Edge2.Y;
            }
        }
    }
}
