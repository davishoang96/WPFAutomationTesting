using Gu.Wpf.UiAutomation;

using SpyandPlaybackTestTool.SpyPlaybackObjects;

namespace SpyandPlaybackTestTool.Actions
{
    internal abstract class AbsAction
    {
        public PlaybackObject PlaybackObject { get; set; }

        public SpyObject SpyObject { get; set; }
        public UiElement UiElement;
        public bool Result;
        public bool IsExist;
        public bool IsNotExist;

        public abstract void DoExecute();
    }
}