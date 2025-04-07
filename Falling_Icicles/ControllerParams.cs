using Falling_Icicles.Information;
using Falling_Icicles.LogControl;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace Falling_Icicles
{
    public class ControllerParams(SharedDataStore? sharedData) : ShapeParameterBase(sharedData)
    {
        [Display(Name = "ステージ", Description = "")]
        [TextBoxSlider("F0", "", 0, 9)]
        [DefaultValue(0)]
        [Range(0, 9)]
        public int StageNum { get => stageNum; set => Set(ref stageNum, value); }
        int stageNum = 0;

        [Display(Name = "シード値", Description = "")]
        [TextBoxSlider("F0", "", 0, 8)]
        [DefaultValue(0)]
        [Range(0, ulong.MaxValue)]
        public ulong SeedValue { get => seedValue; set => Set(ref seedValue, value); }
        ulong seedValue = 0;

        [Display(Name = "水平方向", Description = "１⇒右\n－１⇒左")]
        [AnimationSlider("F0", "", -1, 1)]
        public Animation Horizontal { get; set; } = new(0, -1, 1);

        [Display(Name = "垂直方向", Description  = "１⇒下\n－１⇒上")]
        [AnimationSlider("F0", "", -1, 1)]
        public Animation Vertice { get; set; } = new(0, -1, 1);

        [Display(Name = "Log", Description = "")]
        [LogController(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public LogStep LogStep { get => logStep; set => Set(ref logStep, value); }
        LogStep logStep = new();

        public ControllerParams() : this(null)
        {
        }

        public override IEnumerable<string> CreateMaskExoFilter(int keyFrameIndex, ExoOutputDescription desc, ShapeMaskExoOutputDescription shapeMaskParameters)
        {
            return [];
        }

        public override IEnumerable<string> CreateShapeItemExoFilter(int keyFrameIndex, ExoOutputDescription desc)
        {
            return [];
        }

        public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
        {
            return new GameViewSource(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Vertice, Horizontal];

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (!PluginInfo.Agree)
            {
                MessageBox.Show(
                    "あなた、" +
                    "\n『注意書き』ちゃんと読んでなかったでしょ…" +
                    "\n正しく従わないとこのゲームは遊べないようになっているの。" +
                    "\n次は最後までしっかり読んでね！",
                    $"{PluginInfo.Title} 「注意書きはしっかり読もうね…」",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                throw new Exception("あなたは注意書きの指示に従わなかった。");
            }
            var data = store.Load<SharedData>();
            if (data is null)
                return;
            data.CopyTo(this);
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SharedData(this));
        }

        public class SharedData
        {
            public int StageNum { get; set; } = 0;
            public ulong SeedValue { get; set; } = 0;
            public Animation Vertice { get; set; } = new(0, -1, 1);
            public Animation Horizontal { get; set; } = new(0, -1, 1);
            public LogStep LogStep { get; set; } = new();

            public SharedData(ControllerParams parameter)
            {
                StageNum = parameter.StageNum;
                SeedValue = parameter.SeedValue;
                Vertice.CopyFrom(parameter.Vertice);
                Horizontal.CopyFrom(parameter.Horizontal);
                LogStep = new(parameter.LogStep);
            }

            public void CopyTo(ControllerParams parameter)
            {
                parameter.StageNum = StageNum;
                parameter.SeedValue = SeedValue;
                parameter.Vertice.CopyFrom(Vertice);
                parameter.Horizontal.CopyFrom(Horizontal);
                parameter.LogStep = new(LogStep);
            }
        }
    }
}