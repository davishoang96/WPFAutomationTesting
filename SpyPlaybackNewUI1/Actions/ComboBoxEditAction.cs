using Gu.Wpf.UiAutomation;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

namespace SpyandPlaybackTestTool.Actions
{
    internal class ComboBoxEditAction : AbsAction
    {
        public override void DoExecute()
        {
            AutomationElement a = UiElement.AutomationElement;
            ExpandCollapsePattern expandCollapsePattern = a.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            expandCollapsePattern.Expand();
            var cbxEditItems = UiElement.FindAll(TreeScope.Subtree, new System.Windows.Automation.PropertyCondition(AutomationElement.ClassNameProperty, "ComboBoxEditItem"));
            switch (PlaybackObject.action)
            {
                case "SetText":
                    try
                    {
                        var Editor = a.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.AutomationIdProperty, "PART_Editor"));
                        ((ValuePattern)Editor.GetCurrentPattern(ValuePattern.Pattern)).SetValue(PlaybackObject.text);
                        expandCollapsePattern.Collapse();
                        Result = true;
                    }
                    catch (Exception)
                    {
                        Result = false;
                        //throw;
                    }
                    break;

                case "Select":
                    try
                    {
                        if (cbxEditItems[PlaybackObject.itemIndex].AutomationElement.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "CheckEdit")) != null)
                        {
                            var checkBox = cbxEditItems[PlaybackObject.itemIndex].FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ClassNameProperty, "CheckEdit"));
                            checkBox.AsCheckBox().Click();
                            SendKeys.SendWait("{ENTER}");
                        }
                        else
                        {
                            cbxEditItems[PlaybackObject.itemIndex].AutomationElement.SelectionItemPattern().Select();
                            expandCollapsePattern.Collapse();
                        }
                        Result = true;
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

            //expandCollapsePattern.Collapse();
        }
    }
}