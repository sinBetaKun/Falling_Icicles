using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DXGI;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;

namespace Falling_Icicles.BitmapDrawer
{
    public class DotString : BitmapDrawerBase
    {
        static readonly int goToCirno_width = 40;
        static readonly int goToCirno_height = 10;
        static readonly int gameOver_width = 40;
        static readonly int gameOver_height = 9;
        static readonly int scale = 4;

        static readonly byte[] goToCirno_BlackAreas = [
             2, 4,  5, 9,
             6, 4,  9, 7,
            13, 2, 14, 7,
            12, 3, 15, 4,
            16, 4, 19, 7,
            22, 2, 25, 7,
            26, 2, 27, 3,
            26, 4, 27, 7,
            28, 4, 30, 5,
            28, 5, 29, 7,
            31, 4, 34, 7,
            35, 4, 38, 7,
            ];

        static readonly int countOf_GoToCirno_BlackAreas = 12;

        static readonly byte[] goToCirno_WhiteAreas = [
             3, 5,  4, 6,
             2, 7,  4, 8,
             7, 5,  8, 6,
            17, 5, 18, 6,
            23, 3, 25, 6,
            32, 5, 33, 7,
            36, 5, 37, 6,
            ];

        static readonly int countOf_GoToCirno_WhiteAreas = 7;

        static readonly byte[] gameOver_BlackAreas = [
             2, 2,  6, 7,
             7, 2, 10, 7,
            11, 2, 12, 7,
            17, 2, 20, 3,
            23, 2, 26, 7,
            27, 2, 28, 6,
            31, 2, 34, 3,
            35, 2, 38, 7,
            12, 3, 13, 4,
            17, 3, 18, 7,
            28, 6, 29, 7,
            31, 3, 32, 7,
            13, 4, 14, 5,
            18, 4, 19, 5,
            29, 2, 30, 6,
            32, 4, 33, 5,
            14, 3, 15, 4,
            18, 6, 20, 7,
            32, 6, 34, 7,
            15, 2, 16, 7,
            ];

        static readonly int countOf_GameOver_BlackAreas = 20;

        static readonly byte[] gameOver_WhiteAreas = [
             3, 3,  6, 4,
             8, 3,  9, 4,
            24, 3, 25, 6,
            37, 2, 38, 3,
             3, 4,  4, 6,
             8, 5,  9, 7,
            36, 3, 37, 4,
             4, 5,  5, 6,
            37, 4, 38, 5,
            36, 5, 37, 7,
            ];

        static readonly int countOf_GameOver_WhiteAreas = 10;

        GreaterFairyBitmapDrawer Yamada;

        enum Status
        {
            None,
            GoToCirno,
            GameOver,
        }

        private Status status = Status.None;

        public DotString(IGraphicsDevicesAndContext devices, GreaterFairyBitmapDrawer Yamada, GameViewSource gameViewSource) : base(devices, gameViewSource)
        {
            this.Yamada = Yamada;

            AddNewBitmap(goToCirno_width * scale, goToCirno_height * scale);
            AddNewBitmap(gameOver_width * scale, gameOver_height * scale);

            ID2D1SolidColorBrush white = devices.DeviceContext.CreateSolidColorBrush(new Color4(1, 1, 1, 1));
            ID2D1SolidColorBrush black = devices.DeviceContext.CreateSolidColorBrush(new Color4(0, 0, 0, 1));

            var dc = devices.DeviceContext;

            dc.Target = bitmaps[0];
            dc.BeginDraw();
            dc.Clear(null);
            dc.FillRectangle(new Vortice.RawRectF(0, 0, goToCirno_width * scale, goToCirno_height * scale), white);

            for (int i = 0; i < countOf_GoToCirno_BlackAreas; i++)
            {
                dc.FillRectangle(
                    new Vortice.RawRectF(
                        goToCirno_BlackAreas[i * 4 + 0] * scale,
                        goToCirno_BlackAreas[i * 4 + 1] * scale,
                        goToCirno_BlackAreas[i * 4 + 2] * scale,
                        goToCirno_BlackAreas[i * 4 + 3] * scale
                        ),
                    black
                    );
            }

            for (int i = 0; i < countOf_GoToCirno_WhiteAreas; i++)
            {
                dc.FillRectangle(
                    new Vortice.RawRectF(
                        goToCirno_WhiteAreas[i * 4 + 0] * scale,
                        goToCirno_WhiteAreas[i * 4 + 1] * scale,
                        goToCirno_WhiteAreas[i * 4 + 2] * scale,
                        goToCirno_WhiteAreas[i * 4 + 3] * scale
                        ),
                    white
                    );
            }

            dc.EndDraw();
            dc.Target = null;   //Targetは必ずnullに戻す。

            dc.Target = bitmaps[1];
            dc.BeginDraw();
            dc.Clear(null);
            dc.FillRectangle(new Vortice.RawRectF(0, 0, gameOver_width * scale, gameOver_height * scale), white);

            for (int i = 0; i < countOf_GameOver_BlackAreas; i++)
            {
                dc.FillRectangle(
                    new Vortice.RawRectF(
                        gameOver_BlackAreas[i * 4 + 0] * scale,
                        gameOver_BlackAreas[i * 4 + 1] * scale,
                        gameOver_BlackAreas[i * 4 + 2] * scale,
                        gameOver_BlackAreas[i * 4 + 3] * scale
                        ),
                    black
                    );
            }

            for (int i = 0; i < countOf_GameOver_WhiteAreas; i++)
            {
                dc.FillRectangle(
                    new Vortice.RawRectF(
                        gameOver_WhiteAreas[i * 4 + 0] * scale,
                        gameOver_WhiteAreas[i * 4 + 1] * scale,
                        gameOver_WhiteAreas[i * 4 + 2] * scale,
                        gameOver_WhiteAreas[i * 4 + 3] * scale
                        ),
                    white
                    );
            }

            dc.EndDraw();
            dc.Target = null;   //Targetは必ずnullに戻す。


            white.Dispose();
            black.Dispose();
        }

        public void SetToEmpty()
        {
            status = Status.None;
        }

        public void SetToGoToCirno()
        {
            status = Status.GoToCirno;
        }

        public void SetToGameOver()
        {
            status = Status.GameOver;
        }

        public override void UpdatePlace()
        {
            ResetPos();
            switch (status)
            {
                case Status.None:
                    return;
                case Status.GoToCirno:
                    xList.Add(400);
                    yList.Add(-50);
                    rotateList.Add(0);
                    bmIndexList.Add(0);
                    countOfCharacters = 1;
                    return;
                case Status.GameOver:
                    xList.Add(Yamada.X);
                    yList.Add(Yamada.Y - 50);
                    rotateList.Add(0);
                    bmIndexList.Add(1);
                    countOfCharacters = 1;
                    return;
                default:
                    
                    return;
            }
        }
    }
}
