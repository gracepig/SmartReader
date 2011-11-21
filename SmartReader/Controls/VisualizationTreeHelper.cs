using System.Windows;
using System.Windows.Media;

namespace SmartReader.Controls
{

    public static class VisualizationTreeHelper
    {
        public static T FindParent<T>(UIElement control) where T : UIElement
        {
            UIElement p = VisualTreeHelper.GetParent(control) as UIElement;
            if (p != null)
            {
                if (p is T)
                    return p as T;
                else
                    return VisualizationTreeHelper.FindParent<T>(p);
            }
            return null;
        }

        public static T FindChildControl<T>(UIElement control) where T : UIElement
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                UIElement child = VisualTreeHelper.GetChild(control, i) as UIElement;
                if (child != null && child is T)
                    return child as T;
                else
                    FindChildControl<T>(child);
            }
            return null;
        }
    }


}
