using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace DataBinding
{
    public class AlternatingRowStyleSelector : StyleSelector
    {
        public Style DefaultStyle
        {
            get;
            set;
        }

        public Style AlternateStyle
        {
            get;
            set;
        }

        // Track the position.
        private int i = 0;
        public override Style SelectStyle(object item, DependencyObject container)
        {
            // Reset the counter if this is the first item.
            ItemsControl ctrl = ItemsControl.ItemsControlFromItemContainer(container);
            if (item == ctrl.Items[0])
            {
                i = 0;
            }
            i++;

            // Choose between the two styles based on the current position.
            if (i % 2 == 1)
            {
                return DefaultStyle;
            }
            else
            {
                return AlternateStyle;
            }
            
        }
    }
}
