using Gu.Wpf.UiAutomation;
using System;

namespace SpyandPlaybackTestTool.Actions
{
    internal class CheckBoxAction : AbsAction
    {
        public override void DoExecute()
        {
            switch (PlaybackObject.action)
            {
                case "Select":
                    try
                    {
                        if (UiElement.AsCheckBox().IsChecked == false)
                        {
                            UiElement.AsCheckBox().Click();
                            Result = true;
                        }
                        else
                            Result = false;
                    }
                    catch (Exception)
                    {
                        Result = false;
                        //throw;
                    }
                    break;

                case "Unselect":
                    try
                    {
                        if (UiElement.AsCheckBox().IsChecked == true)
                        {
                            UiElement.AsCheckBox().Click();
                            Result = true;
                        }
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
                    break;
            }
        }
    }
}