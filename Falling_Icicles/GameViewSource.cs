using Falling_Icicles.BitmapDrawer;
using Falling_Icicles.Information;
using System.Numerics;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.DXGI;
using Vortice.Mathematics;
using Windows.Graphics.Imaging;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace Falling_Icicles
{
    public class GameViewSource : IShapeSource
    {
        private readonly IGraphicsDevicesAndContext devices;
        private readonly DisposeCollector disposer;
        public readonly ControllerParams Param;

        bool isFirst = true;

        public int Stage => Param.StageNum;

        private readonly CirnoDrawer cirno;
        private readonly GreaterFairyBitmapDrawer yamada;
        private readonly DotString dotString;
        private readonly IciclesDrawer icicles;

        public readonly static Int2 Edge1 = new(-860, -400);
        public readonly static Int2 Edge2 = new(860, 400);
        readonly static int scroleDis = 50;

        public int FPS;
        public int Frame;
        public int Length;


        ID2D1CommandList? commandList;

        public ID2D1Image Output => commandList ?? throw new Exception($"{nameof(commandList)}がnullです。事前にUpdateを呼び出す必要があります。");

        public GameViewSource(IGraphicsDevicesAndContext devices, ControllerParams param)
        {
            this.devices = devices;
            this.Param = param;

            disposer = new();

            yamada = new(devices, this);
            disposer.Collect(yamada);

            cirno = new(devices, yamada, this);
            disposer.Collect(cirno);

            dotString = new(devices, yamada, this);
            disposer.Collect(dotString);

            icicles = new(devices, yamada, cirno, this);
            disposer.Collect(icicles);
        }

        public void Update(TimelineItemSourceDescription timelineItemSourceDescription)
        {
            FPS = timelineItemSourceDescription.FPS;
            Frame = 0;
            Length = timelineItemSourceDescription.ItemDuration.Frame;
            int endFrame = timelineItemSourceDescription.ItemPosition.Frame;

            int stage = Param.StageNum;

            yamada.ResetPos();
            cirno.ResetPos();
            icicles.ResetPos();
            dotString.ResetPos();

            if (Stage <= Param.LogStep.AllowedStage)
            {
                if (stage == 0)
                {
                    dotString.SetToGoToCirno();
                    dotString.UpdatePlace();
                }

                for (Frame = 0; Frame < endFrame; Frame++)
                {
                    // 次のフレームにおける大妖精の位置を更新
                    yamada.UpdatePlace();
                    cirno.UpdatePlace();
                    icicles.UpdatePlace();

                    if (yamada.IsNearTo(icicles, 20))
                    {
                        dotString.SetToGameOver();
                        dotString.UpdatePlace();
                        break;
                    }

                    if (Stage > 1)
                    {
                        if (yamada.X < Edge1.X || yamada.X > Edge2.X
                            || yamada.Y < Edge1.Y || yamada.Y > Edge2.Y)
                        {
                            dotString.SetToGameOver();
                            dotString.UpdatePlace();
                            break;
                        }

                        int dif_cx =
                            cirno.X < Edge1.X + scroleDis
                            ? (Edge1.X + scroleDis - cirno.X)
                            : cirno.X > Edge2.X - scroleDis
                                ? (Edge2.X - scroleDis - cirno.X)
                                :0;
                        int dif_cy =
                            cirno.Y < Edge1.Y + scroleDis
                            ? (Edge1.Y + scroleDis - cirno.Y)
                            : cirno.Y > Edge2.Y - scroleDis
                                ? (Edge2.Y - scroleDis - cirno.Y)
                                :0;

                        if (dif_cx != 0 || dif_cy != 0)
                        {
                            yamada.Shift(dif_cx, dif_cy);
                            cirno.Shift(dif_cx, dif_cy);
                            icicles.Shift(dif_cx, dif_cy);
                        }
                    }

                    if (yamada.IsNearTo(cirno, 30))
                    {
                        // チルノに接触した時
                        if (Param.LogStep.StageClear(stage))
                        {
                            FallingIciclesDialog.ShowNotification("STAGE CLEAR!", $"<|ステージ {stage}|> クリア\n\n会話の続きが解禁されました。");
                        }
                        break;
                    }
                }
            }

            bool viewIsOld = false;

            viewIsOld |= yamada.UpdateImage();
            viewIsOld |= cirno.UpdateImage();
            viewIsOld |= icicles.UpdateImage();
            viewIsOld |= dotString.UpdateImage();

            if (isFirst || viewIsOld)
            {
                var dc = devices.DeviceContext;

                commandList?.Dispose();//新規作成前に、前回のCommandListを必ず破棄する
                commandList = dc.CreateCommandList();

                dc.Target = commandList;
                dc.BeginDraw();
                dc.Clear(null);
                dc.DrawImage(yamada.Output);
                dc.DrawImage(cirno.Output);
                dc.DrawImage(icicles.Output);
                dc.DrawImage(dotString.Output);
                dc.EndDraw();
                dc.Target = null;//Targetは必ずnullに戻す。
                commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある

                isFirst = false;
            }
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージド状態を破棄します (マネージド オブジェクト)
                    commandList?.Dispose();
                    disposer.Dispose();
                }

                // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~SampleShapeSource()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}