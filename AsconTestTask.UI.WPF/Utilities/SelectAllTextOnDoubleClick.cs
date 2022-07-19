using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AsconTestTask.UI.WPF.Utilities;

public class SelectAllTextOnDoubleClick : DependencyObject
{
	public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
        "Active",
        typeof(bool),
        typeof(SelectAllTextOnDoubleClick),
        new PropertyMetadata(false, ActivePropertyChanged));

    private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;
        if ((e.NewValue as bool?).GetValueOrDefault(false))
        {
            textBox.MouseDoubleClick += OnMouseDoubleClick;
        }
        else
        {
            textBox.MouseDoubleClick -= OnMouseDoubleClick;
        }
    }

    private static void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        DependencyObject dependencyObject = GetParentFromVisualTree(e.OriginalSource);

        if (dependencyObject == null)
        {
            return;
        }

        var textBox = (TextBox)dependencyObject;
        if (!textBox.IsEnabled) return;
        textBox.Focus();
        textBox.SelectAll();
    }

    private static DependencyObject GetParentFromVisualTree(object source)
    {
        DependencyObject parent = source as UIElement;
        while (parent != null && !(parent is TextBox))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent;
    }

    [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
    [AttachedPropertyBrowsableForType(typeof(TextBox))]
    public static bool GetActive(DependencyObject @object)
    {
        return (bool) @object.GetValue(ActiveProperty);
    }

    public static void SetActive(DependencyObject @object, bool value)
    {
        @object.SetValue(ActiveProperty, value);
    }
}
