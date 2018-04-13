using System;

namespace SpyandPlaybackTestTool.Actions
{
    internal class RadioButtonAction : AbsAction
    {
        public override void DoExecute()
        {
            switch (PlaybackObject.action)
            {
                case "Click":
                    try
                    {
                        //if (UiElement.AsRadioButton().IsChecked == false)
                        //{
                        //}

                        UiElement.AsRadioButton().Click();
                        Result = true;
                    }
                    catch (Exception)
                    {
                        Result = false;
                        //throw;
                    }
                    break;

                case "IsChecked":
                    try
                    {
                        if (UiElement.AsRadioButton().IsChecked == true)
                        {
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
                    }
                    break;

                case "IsUnChecked":
                    try
                    {
                        if (UiElement.AsRadioButton().IsChecked == false)
                        {
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