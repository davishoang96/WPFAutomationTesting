using System;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool.Actions
{
    internal class PressEnter : AbsAction
    {
        public override void DoExecute()
        {
            switch (PlaybackObject.action)
            {
                case "PressEnter":
                    try
                    {
                        SendKeys.SendWait("{ENTER}");
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    break;

                default: //Thread.Sleep(2000);
                    break;
            }
        }
    }
}