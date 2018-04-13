using System;

namespace SpyandPlaybackTestTool.Actions
{
    internal class ButtonAction : AbsAction
    {
        public override void DoExecute()
        {
            switch (PlaybackObject.action)
            {
                case "Click":
                    try
                    {
                        if (UiElement.AutomationElement.Current.IsEnabled)
                        {
                            UiElement.AsButton().Click();
                            Result = true;
                        }
                        else
                        {
                            Result = false;
                        }
                    }
                    catch (Exception)
                    {
                        Result = false;
                        //throw;
                    }
                    break;

                case "DoubleClick":
                    try
                    {
                        UiElement.AsButton().DoubleClick();
                        Result = true;
                    }
                    catch (Exception)
                    {
                        Result = false;
                        //throw;
                    }
                    break;

                case "IsEnabled":
                    try
                    {
                        if (UiElement.AsButton().IsEnabled)
                            Result = true;
                        else
                            Result = false;
                    }
                    catch (Exception)
                    {
                        Result = false;
                        //throw;
                    }
                    break;

                default:
                    Result = false;
                    //Thread.Sleep(2000);
                    break;
            }
        }
    }
}