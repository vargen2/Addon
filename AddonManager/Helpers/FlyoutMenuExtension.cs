using AddonManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AddonManager.Helpers
{
    //public static class FlyoutMenuExtension
    //{
    //    public static List<MenuFlyoutItemBase> GetMyItems(DependencyObject obj)
    //    {
    //        return (List<MenuFlyoutItemBase>)obj.GetValue(MyItemsProperty);
    //    }
    //    public static void SetMyItems(DependencyObject obj, List<MenuFlyoutItemBase> value)
    //    {
    //        obj.SetValue(MyItemsProperty, value);
    //    }

    //    public static readonly DependencyProperty MyItemsProperty =
    //        DependencyProperty.Register("MyItems",
    //            typeof(List<MenuFlyoutItemBase>),
    //            typeof(FlyoutMenuExtension),
    //            new PropertyMetadata(new List<MenuFlyoutItemBase>(), (sender, e) =>
    //            {
    //                var menu = sender as MenuFlyoutSubItem;
    //                menu.Items.Clear();
    //                if (e != null && e.NewValue != null)
    //                {
    //                    Debug.WriteLine("hit " + e.ToString() + " " + e.NewValue.ToString());
    //                    var downloads = e.NewValue as List<Download>;
    //                    foreach (var item in downloads)
    //                    {
    //                        menu.Items.Add(new MenuFlyoutItem() { Text = item.ToString() });
    //                        Debug.WriteLine(item.Version);
    //                    }
    //                }

    //            }));
    //}
}
