using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;

namespace Falling_Icicles.BitmapDrawer
{
    public class IciclesDrawer : BitmapDrawerBase
    {
        static readonly int width = 32;
        static readonly int height = 32;
        static readonly int scale = 4;

        static readonly byte[] darkAreas = [
            19, 13, 22, 14,
            15, 14, 23, 15,
            12, 15, 24, 16,
             9, 16, 24, 17,
            ];

        static readonly int countOf_DarkAreas = 4;

        static readonly byte[] lightAreas = [
            16, 16, 23, 17,
            13, 17, 23, 18,
            17, 18, 23, 19,
            20, 19, 22, 20,
            ];

        static readonly int countOf_LightAreas = 4;

        readonly GreaterFairyBitmapDrawer Yamada;
        readonly CirnoDrawer Cirno;
        private readonly Xoshiro256StarStar xoshiro;
        private ulong xoshiroNext;
        private ulong seed = 0;

        enum HomingStatus
        {
            No,
            IsFirst,
            NotFirst,
        }

        int tick = 0;

        List<(float x, float y, int mx, int my, bool f, HomingStatus h)> icicles = [];

        int counter_1 = 0;

        public IciclesDrawer(IGraphicsDevicesAndContext devices, GreaterFairyBitmapDrawer Yamada, CirnoDrawer Cirno, GameViewSource gameViewSource) : base(devices, gameViewSource)
        {
            this.Yamada = Yamada;
            this.Cirno = Cirno;
            
            xoshiro = new(seed);
            xoshiroNext = xoshiro.Next();

            AddNewBitmap(width * scale, height * scale);

            ID2D1SolidColorBrush light = devices.DeviceContext.CreateSolidColorBrush(new(132, 222, 247));
            ID2D1SolidColorBrush dark = devices.DeviceContext.CreateSolidColorBrush(new(66, 132, 247));

            var dc = devices.DeviceContext;

            dc.Target = bitmaps[0];
            dc.BeginDraw();
            dc.Clear(null);

            for (int i = 0; i < countOf_DarkAreas; i++)
            {
                dc.FillRectangle(
                    new Vortice.RawRectF(
                        darkAreas[i * 4 + 0] * scale,
                        darkAreas[i * 4 + 1] * scale,
                        darkAreas[i * 4 + 2] * scale,
                        darkAreas[i * 4 + 3] * scale
                        ),
                    dark
                    );
            }

            for (int i = 0; i < countOf_LightAreas; i++)
            {
                dc.FillRectangle(
                    new Vortice.RawRectF(
                        lightAreas[i * 4 + 0] * scale,
                        lightAreas[i * 4 + 1] * scale,
                        lightAreas[i * 4 + 2] * scale,
                        lightAreas[i * 4 + 3] * scale
                        ),
                    light
                    );
            }

            dc.EndDraw();
            dc.Target = null;   //Targetは必ずnullに戻す。


            light.Dispose();
            dark.Dispose();
        }

        public override void UpdatePlace()
        {
            tick++;
            switch (gameViewSource.Stage)
            {
                case 2:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            AddCross(false, false, false);
                        }
                        else
                        {
                            AddPlus(false, false, false);
                        }
                    }

                    UpdateIcicles(false);
                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 3:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            AddPlus(false, false, false);
                            AddCross(true, false, false);
                        }
                        else
                        {
                            AddPlus(true, false, false);
                            AddCross(false, false, false);
                        }
                    }

                    UpdateIcicles(false);
                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 4:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            AddPlus(false, false, false);
                            AddCross(true, true, false);
                        }
                        else
                        {
                            AddPlus(true, true, false);
                            AddCross(false, false, false);
                        }

                        UpdateIcicles(true);
                    }
                    else
                    {
                        UpdateIcicles(false);
                    }

                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 5:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        seed = gameViewSource.Param.SeedValue + (ulong)(tick / gameViewSource.FPS);
                        xoshiro.Seed(seed);
                        xoshiroNext = xoshiro.Next();

                        if (xoshiroNext % 2 == 0)
                        {
                            AddPlus(false, true, false);
                            AddCross(true, false, false);
                        }
                        else
                        {
                            AddPlus(true, false, false);
                            AddCross(false, true, false);
                        }

                        UpdateIcicles(true);
                    }
                    else
                    {
                        UpdateIcicles(false);
                    }

                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 6:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        seed = gameViewSource.Param.SeedValue + (ulong)(tick / gameViewSource.FPS);
                        xoshiro.Seed(seed);
                        xoshiroNext = xoshiro.Next();

                        if (xoshiroNext % 2 == 0)
                        {
                            AddPlus(false, true, false);
                            AddCross(true, false, false);
                        }
                        else
                        {
                            AddPlus(true, false, false);
                            AddCross(false, true, false);
                        }

                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            AddAsterisk(0, 0, true, false);
                        }

                        UpdateIcicles(true);
                    }
                    else
                    {
                        UpdateIcicles(false);
                    }

                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 7:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        seed = gameViewSource.Param.SeedValue + (ulong)(tick / gameViewSource.FPS);
                        xoshiro.Seed(seed);
                        xoshiroNext = xoshiro.Next();

                        if (xoshiroNext % 2 == 0)
                        {
                            AddPlus(false, true, false);
                            AddPlus(true, false, false);
                            AddCross(true, false, false);
                        }
                        else
                        {
                            AddPlus(true, false, false);
                            AddCross(true, false, false);
                            AddCross(false, true, false);
                        }

                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            if (tick / gameViewSource.FPS % 4 == 0)
                            {
                                AddPlus(true, false, true);
                            }
                            else
                            {
                                AddCross(true, false, true);
                            }
                        }
                        else
                        {
                            AddAsterisk(0, 0, true, false);
                        }

                        UpdateIcicles(true);
                    }
                    else
                    {
                        UpdateIcicles(false);
                    }

                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 8:
                    if (tick % gameViewSource.FPS == 0)
                    {
                        seed = gameViewSource.Param.SeedValue + (ulong)(tick / gameViewSource.FPS);
                        xoshiro.Seed(seed);
                        xoshiroNext = xoshiro.Next();

                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            if ((xoshiroNext & 1) > 0)
                            {
                                AddPlus(true, true, false);
                                AddCross(true, false, false);
                            }
                            else
                            {
                                AddPlus(true, false, false);
                                AddCross(true, true, false);
                            }

                            AddAsterisk(0, 0, true, false);
                        }
                        else
                        {
                            switch(xoshiroNext % 4)
                            {
                                case 0:
                                    AddPlus(true, true, false);
                                    AddPlus(false, false, false);
                                    AddCross(true, false, false);
                                    AddCross(false, false, false);
                                    break;

                                case 1:
                                    AddPlus(true, false, false);
                                    AddPlus(false, true, false);
                                    AddCross(true, false, false);
                                    AddCross(false, false, false);
                                    break;

                                case 2:
                                    AddPlus(true, false, false);
                                    AddPlus(false, false, false);
                                    AddCross(true, true, false);
                                    AddCross(false, false, false);
                                    break;

                                case 3:
                                    AddPlus(true, false, false);
                                    AddPlus(false, false, false);
                                    AddCross(true, false, false);
                                    AddCross(false, true, false);
                                    break;

                            }

                            switch ((xoshiroNext >> 2) & 0b11)
                            {
                                case 0:
                                    AddPlus(true, false, true);
                                    break;

                                case 1:
                                    AddPlus(false, false, true);
                                    break;

                                case 2:
                                    AddCross(true, false, true);
                                    break;

                                case 3:
                                    AddCross(false, true, true);
                                    break;

                            }
                        }

                        UpdateIcicles(true);
                    }
                    else
                    {
                        UpdateIcicles(false);
                    }

                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                case 9:
                    if (tick / gameViewSource.FPS % 5 == 0)
                    {
                        seed = gameViewSource.Param.SeedValue + (ulong)(tick / gameViewSource.FPS);
                        xoshiro.Seed(seed);
                        xoshiroNext = xoshiro.Next();

                    }
                    if (tick % gameViewSource.FPS == 0)
                    {
                        seed = gameViewSource.Param.SeedValue + (ulong)(tick / gameViewSource.FPS);
                        xoshiro.Seed(seed);
                        xoshiroNext = xoshiro.Next();
                        switch ((xoshiroNext >> 2) & 0b11)
                        {
                            case 0:
                                AddPlus(true, true, false);
                                AddPlus(false, false, false);
                                AddCross(true, false, false);
                                AddCross(false, false, false);
                                break;

                            case 1:
                                AddPlus(true, false, false);
                                AddPlus(false, true, false);
                                AddCross(true, false, false);
                                AddCross(false, false, false);
                                break;

                            case 2:
                                AddPlus(true, false, false);
                                AddPlus(false, false, false);
                                AddCross(true, true, false);
                                AddCross(false, false, false);
                                break;

                            case 3:
                                AddPlus(true, false, false);
                                AddPlus(false, false, false);
                                AddCross(true, false, false);
                                AddCross(false, true, false);
                                break;

                        }

                        if ((xoshiroNext & 1) > 0)
                        {
                            AddAsterisk(1, 0, true, false);
                            AddAsterisk(-1, 0, true, false);
                            AddAsterisk(0, 1, true, false);
                            AddAsterisk(0, -1, true, false);
                        }
                        else
                        {
                            AddAsterisk(1, 1, true, false);
                            AddAsterisk(1, -1, true, false);
                            AddAsterisk(-1, 1, true, false);
                            AddAsterisk(-1, -1, true, false);
                        }

                        if (tick / gameViewSource.FPS % 2 == 0)
                        {
                            AddBox();   
                            AddPlus(true, false, true);
                            AddPlus(false, false, true);
                        }
                        else
                        {
                            switch ((xoshiroNext >> 2) & 0b11)
                            {
                                case 0:
                                    AddPlus(true, true, true);
                                    AddPlus(false, false, true);
                                    break;

                                case 1:
                                    AddPlus(false, true, true);
                                    AddCross(true, false, true);
                                    break;

                                case 2:
                                    AddPlus(false, false, true);
                                    AddCross(true, true, true);
                                    break;

                                case 3:
                                    AddPlus(true, false, true);
                                    AddCross(false, true, true);
                                    break;

                            }
                        }

                        UpdateIcicles(true);
                    }
                    else
                    {
                        UpdateIcicles(false);
                    }

                    RemoveOutofLimit();
                    ConvertStatus2Pos();
                    return;

                default:
                    return;
            }
        }

        private void AddBox()
        {
            icicles.Add((Yamada.X, Yamada.Y - 100, 1, 0, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y - 100, 1, 0, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y - 100, -1, 0, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y - 100, -1, 0, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y + 100, 1, 0, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y + 100, 1, 0, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y + 100, -1, 0, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X, Yamada.Y + 100, -1, 0, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X + 100, Yamada.Y, 0, -1, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X + 100, Yamada.Y, 0, -1, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X + 100, Yamada.Y, 0, 1, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X + 100, Yamada.Y, 0, 1, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X - 100, Yamada.Y, 0, -1, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X - 100, Yamada.Y, 0, -1, false, HomingStatus.IsFirst));
            icicles.Add((Yamada.X - 100, Yamada.Y, 0, 1, true, HomingStatus.IsFirst));
            icicles.Add((Yamada.X - 100, Yamada.Y, 0, 1, false, HomingStatus.IsFirst));
        }

        private void AddAsterisk(int horizontal, int vertical, bool isFast, bool h)
        {
            if (horizontal == 0 && vertical == 0)
            {
                (horizontal, vertical) = Cirno.CalcDirect(Yamada, 0, 0, 128);
            }

            HomingStatus hs = h ? HomingStatus.IsFirst : HomingStatus.No;
            icicles.Add((Cirno.X, Cirno.Y, horizontal, vertical, isFast, hs));
            icicles.Add((Cirno.X - 37, Cirno.Y - 20, horizontal, vertical, isFast, hs));
            icicles.Add((Cirno.X - 37, Cirno.Y + 20, horizontal, vertical, isFast, hs));
            icicles.Add((Cirno.X + 37, Cirno.Y - 20, horizontal, vertical, isFast, hs));
            icicles.Add((Cirno.X + 37, Cirno.Y + 20, horizontal, vertical, isFast, hs));
            icicles.Add((Cirno.X, Cirno.Y - 40, horizontal, vertical, isFast, hs));
            icicles.Add((Cirno.X, Cirno.Y + 40, horizontal, vertical, isFast, hs));
        }

        private void AddCross(bool isFast, bool h, bool surround)
        {
            HomingStatus hs = h ? HomingStatus.IsFirst : HomingStatus.No;
            if (surround)
            {
                icicles.Add((Yamada.X - 100, Yamada.Y - 100, 1, 1, isFast, hs));
                icicles.Add((Yamada.X - 100, Yamada.Y + 100, 1, -1, isFast, hs));
                icicles.Add((Yamada.X + 100, Yamada.Y + 100, -1, -1, isFast, hs));
                icicles.Add((Yamada.X + 100, Yamada.Y - 100, -1, 1, isFast, hs));
            }
            else
            {
                icicles.Add((Cirno.X, Cirno.Y, 1, 1, isFast, hs));
                icicles.Add((Cirno.X, Cirno.Y, 1, -1, isFast, hs));
                icicles.Add((Cirno.X, Cirno.Y, -1, -1, isFast, hs));
                icicles.Add((Cirno.X, Cirno.Y, -1, 1, isFast, hs));
            }
        }

        private void AddPlus(bool isFast, bool h, bool surround)
        {
            HomingStatus hs = h ? HomingStatus.IsFirst : HomingStatus.No;
            if (surround)
            {
                icicles.Add((Yamada.X - 141, Yamada.Y, 1, 0, isFast, hs));
                icicles.Add((Yamada.X + 141, Yamada.Y, -1, 0, isFast, hs));
                icicles.Add((Yamada.X, Yamada.Y + 141, 0, -1, isFast, hs));
                icicles.Add((Yamada.X, Yamada.Y - 141, 0, 1, isFast, hs));
            }
            else
            {
                icicles.Add((Cirno.X, Cirno.Y, 1, 0, isFast, hs));
                icicles.Add((Cirno.X, Cirno.Y, -1, 0, isFast, hs));
                icicles.Add((Cirno.X, Cirno.Y, 0, -1, isFast, hs));
                icicles.Add((Cirno.X, Cirno.Y, 0, 1, isFast, hs));
            }
        }

        public override void Shift(int x, int y)
        {
            for (int i = 0; i < countOfCharacters; i++)
            {
                icicles[i] = (icicles[i].x + x, icicles[i].y + y, icicles[i].mx, icicles[i].my, icicles[i].f, icicles[i].h);
            }
        }

        private void UpdateIcicles(bool fhUpd)
        {
            for (int i = 0; i < icicles.Count; i++)
            {
                var icicle = icicles[i];
                int move_x = icicle.mx;
                int move_y = icicle.my;

                // ホーミング弾の角度の更新
                if (icicle.h == HomingStatus.NotFirst && fhUpd)
                {
                    var (horizontal, vertical) = CalcDirect(Yamada, 0, i, 128);
                    move_x = horizontal;
                    move_y = vertical;
                }

                // 速度調整はこ↑こ↓
                float f = ((move_x == 0 || move_y == 0) ? 300 : 212) / (icicle.f ? 1 : 2);

                icicles[i] = (
                                icicle.x + f * move_x / gameViewSource.FPS,
                                icicle.y + f * move_y / gameViewSource.FPS,
                                move_x,
                                move_y,
                                icicle.f,
                                icicle.h == HomingStatus.IsFirst
                                ? HomingStatus.NotFirst
                                : icicle.h
                                );
            }
        }

        private void ConvertStatus2Pos()
        {
            if (icicles.Count < countOfCharacters)
            {
                while (icicles.Count < countOfCharacters)
                {
                    xList.RemoveAt(0);
                    yList.RemoveAt(0);
                    rotateList.RemoveAt(0);
                    bmIndexList.RemoveAt(0);
                    countOfCharacters--;
                }
            }
            else if (icicles.Count > countOfCharacters)
            {
                while (icicles.Count > countOfCharacters)
                {
                    xList.Add(0);
                    yList.Add(0);
                    rotateList.Add(0);
                    bmIndexList.Add(0);
                    countOfCharacters++;
                }
            }

            for (int i = 0; i < icicles.Count; i++)
            {
                var icicle = icicles[i];
                xList[i] = icicle.x;
                yList[i] = icicle.y;
                if (icicle.mx > 0)
                {
                    if (icicle.my < 0)
                    {
                        rotateList[i] = 3 * MathF.PI / 4;
                    }
                    else if (icicle.my > 0)
                    {
                        rotateList[i] = -3 * MathF.PI / 4;
                    }
                    else
                    {
                        rotateList[i] = MathF.PI;
                    }
                }
                else if (icicle.mx < 0)
                {
                    if (icicle.my < 0)
                    {
                        rotateList[i] = MathF.PI / 4;
                    }
                    else if (icicle.my > 0)
                    {
                        rotateList[i] = -MathF.PI / 4;
                    }
                    else
                    {
                        rotateList[i] = 0;
                    }
                }
                else
                {
                    if (icicle.my < 0)
                    {
                        rotateList[i] = MathF.PI / 2;
                    }
                    else if (icicle.my > 0)
                    {
                        rotateList[i] = -MathF.PI / 2;
                    }
                }
            }
        }

        private void RemoveOutofLimit()
        {
            for (int i = 0; i < icicles.Count; i++)
            {
                if (icicles[i].x < GameViewSource.Edge1.X
                    || icicles[i].x > GameViewSource.Edge2.X
                    || icicles[i].y < GameViewSource.Edge1.Y
                    || icicles[i].y > GameViewSource.Edge2.Y)
                {
                    icicles.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void ResetPos()
        {
            base.ResetPos();
            tick = 0;
            icicles.Clear();
        }
    }
}
