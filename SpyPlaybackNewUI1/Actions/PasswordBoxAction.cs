using System;

namespace SpyandPlaybackTestTool.Actions
{
    internal class PasswordBoxAction : AbsAction
    {
        public override void DoExecute()
        {
            switch (PlaybackObject.action)
            {
                case "SetText":
                    try
                    {
                        UiElement.AsTextBox().Enter(PlaybackObject.text);
                        Result = false;
                    }
                    catch (Exception)
                    {
                        Result = true;
                    }

                    break;

                default:

                    break;
            }
        }
    }
}