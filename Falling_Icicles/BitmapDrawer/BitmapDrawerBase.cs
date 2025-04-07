using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using Vortice.DXGI;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;

namespace Falling_Icicles.BitmapDrawer
{
    public abstract class BitmapDrawerBase : IDisposable
    {
        readonly IGraphicsDevicesAndContext devices;

        protected readonly GameViewSource gameViewSource;

        public ID2D1Image Output => countOfCharacters > 0 ? commandList : empty;

        readonly ID2D1Bitmap1 empty;

        bool isEmpty = true;

        protected readonly List<ID2D1Bitmap1> bitmaps = [];

        private readonly List<(int width, int height)> sizes = [];

        ID2D1CommandList? commandList;

        protected readonly DisposeCollector disposer;

        readonly List<AffineTransform2D> transforms = [];

        protected int countOfCharacters = 0;

        protected readonly List<float> xList = [];

        protected readonly List<float> yList = [];

        protected readonly List<float> rotateList = [];

        protected readonly List<int> bmIndexList = [];

        protected readonly List<float> initialXList = [];

        protected readonly List<float> initialYList = [];

        protected readonly List<float> initialRotateList = [];

        protected readonly List<int> initialBmIndexListList = [];

        public BitmapDrawerBase(IGraphicsDevicesAndContext devices, GameViewSource gameViewSource)
        {
            this.devices = devices;
            this.gameViewSource = gameViewSource;

            disposer = new();

            var dc = devices.DeviceContext;

            empty = dc.CreateEmptyBitmap();
            disposer.Collect(empty);
        }

        public virtual void Shift(int x, int y)
        {
            for (int i = 0; i < countOfCharacters; i++)
            {
                xList[i] += x;
                yList[i] += y;
            }
        }

        protected int AddNewBitmap(int width, int height)
        {
            int index = bitmaps.Count;
            var dc = devices.DeviceContext;

            ID2D1Bitmap1 bitmap = dc.CreateBitmap(
                new SizeI(width, height),
                new BitmapProperties1(
                    new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied),
                    96, 96, BitmapOptions.Target)
                );

            bitmaps.Add(bitmap);

            sizes.Add(((int)bitmap.Size.Width, (int)bitmap.Size.Height));

            return index;
        }

        public bool IsNearTo(BitmapDrawerBase target, int distance)
        {
            for (int i = 0; i < target.countOfCharacters; i++)
            {
                for (int j = 0; j < countOfCharacters; j++)
                {
                    if (Math.Abs(target.xList[i] - xList[j]) < distance && Math.Abs(target.yList[i] - yList[j]) < distance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public (int horizontal, int vertical) CalcDirect(BitmapDrawerBase target, int iTarg, int iThis, int distance)
        {
            int horizontal = 0;
            int vertical = 0;

            if (Math.Abs(target.xList[iTarg] - xList[iThis]) > distance)
            {
                horizontal = target.xList[iTarg] < xList[iThis] ? -1 : 1;
            }
            if (Math.Abs(target.yList[iTarg] - yList[iThis]) > distance)
            {
                vertical = target.yList[iTarg] < yList[iThis] ? -1 : 1;
            }
            if(horizontal == 0 && vertical == 0)
            {
                horizontal = target.xList[iTarg] < xList[iThis] ? -1 : target.xList[iTarg] > xList[iThis] ? 1 : 0;
                vertical = target.yList[iTarg] < yList[iThis] ? -1 : target.yList[iTarg] > yList[iThis] ? 1 : 0;
            }

            return (horizontal, vertical);
        }

        public bool UpdateImage()
        {
            bool isOld = true;
            if (transforms.Count < countOfCharacters)
            {
                while (transforms.Count < countOfCharacters)
                {
                    AffineTransform2D trans = new(devices.DeviceContext);
                    trans.SetInput(0, empty, true);
                    transforms.Add(trans);
                }
            }
            else if (transforms.Count > countOfCharacters)
            {
                while (transforms.Count > countOfCharacters)
                {
                    AffineTransform2D trans = transforms.Last();
                    trans.SetInput(0, null, true);
                    trans.Dispose();
                    transforms.Remove(trans);
                }
            }
            else
            {
                isOld = false;
            }

            if (isOld || commandList is null)
            {
                var dc = devices.DeviceContext;
                commandList?.Dispose();
                commandList = dc.CreateCommandList();
                dc.Target = commandList;
                dc.BeginDraw();
                dc.Clear(null);
                transforms.ForEach(t => dc.DrawImage(t.Output));
                dc.EndDraw();
                dc.Target = null;//Targetは必ずnullに戻す。
                commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある

                if (isEmpty != (countOfCharacters == 0))
                {
                    isEmpty = countOfCharacters == 0;
                }
            }

            for (int i = 0; i < countOfCharacters; i++)
            {
                if (transforms[i].GetInput(0) != bitmaps[bmIndexList[i]])
                {
                    transforms[i].SetInput(0, bitmaps[bmIndexList[i]], true);
                    isOld = true;
                }

                int width = sizes[bmIndexList[i]].width;
                int height = sizes[bmIndexList[i]].height;

                transforms[i].TransformMatrix =
                    Matrix3x2.CreateTranslation(-width / 2, -height / 2)
                    * Matrix3x2.CreateRotation(rotateList[i])
                    * Matrix3x2.CreateTranslation(xList[i], yList[i]);
            }

            return isOld;
        }

        public virtual void ResetPos()
        {
            xList.Clear();
            yList.Clear();
            rotateList.Clear();
            bmIndexList.Clear();

            xList.AddRange(initialXList);
            yList.AddRange(initialYList);
            rotateList.AddRange(initialRotateList);
            bmIndexList.AddRange(initialBmIndexListList);
            countOfCharacters = initialXList.Count;
        }

        public abstract void UpdatePlace();


        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージド状態を破棄します (マネージド オブジェクト)
                    transforms.ForEach(trans =>
                    {
                        trans.SetInput(0, null, true);
                        trans.Dispose();
                    });

                    bitmaps.ForEach(bitmap => bitmap.Dispose());
                    commandList?.Dispose();
                    disposer.Dispose();
                }

                // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
