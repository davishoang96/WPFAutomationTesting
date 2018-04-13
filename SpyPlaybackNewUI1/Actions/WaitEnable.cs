using System;
using System.Threading;

namespace SpyandPlaybackTestTool.Actions
{
    internal class WaitEnable : AbsAction
    {
        public override void DoExecute()
        {
            try
            {
                if (PlaybackObject.text == null)
                {
                    Thread.Sleep(0);
                }
                else
                    Thread.Sleep(Convert.ToInt16(PlaybackObject.text));
                //Thread.Sleep(3000);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}