using System;
using System.Windows.Automation;

namespace SpyandPlaybackTestTool.Actions
{
    internal class ComboBoxAction : AbsAction
    {
        public override void DoExecute()
        {
            AutomationElement a = UiElement.AutomationElement;

            ExpandCollapsePattern expandCollapsePattern = a.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
            expandCollapsePattern.Expand();
            var components = a.FindAll(TreeScope.Subtree, Condition.TrueCondition);

            var comboBoxEditItemCondition = new PropertyCondition(AutomationElement.ClassNameProperty, "ListBoxItem");
            var listItems = a.FindAll(TreeScope.Subtree, comboBoxEditItemCondition);//It can only get one item in the list (the first one).

            foreach(AutomationElement a5 in listItems)
            {
                (a5.GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern).Select();
            }

            switch (PlaybackObject.action)
            {
                case "SetText":
                    try
                    {
                        ((ValuePattern)a.GetCurrentPattern(ValuePattern.Pattern)).SetValue(PlaybackObject.text);

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
                        (listItems[PlaybackObject.itemIndex].GetCurrentPattern(SelectionItemPattern.Pattern) as SelectionItemPattern).Select();
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

            expandCollapsePattern.Collapse();
        }
    }
}