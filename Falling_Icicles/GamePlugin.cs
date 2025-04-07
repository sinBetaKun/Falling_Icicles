using Falling_Icicles.Information;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace Falling_Icicles
{
    public class GamePlugin : IShapePlugin
    {
        static GamePlugin()
        {
            FallingIciclesDialog.ShowIntro();
        }

        public string Name => $"<| {PluginInfo.Title} |>";

        /// <summary>
        /// 俺ちゃんの名前
        /// </summary>
        public PluginDetailsAttribute Details => new() { AuthorName = "sinβ" };

        public bool IsExoShapeSupported => false;

        public bool IsExoMaskSupported => false;

        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new ControllerParams(sharedData);
        }
    }
}
