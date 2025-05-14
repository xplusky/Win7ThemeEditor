using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Win7ThemeEditor.ColorPicker
{
    public class BindOnEnterTextBox : TextBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                var bindingExpression = BindingOperations.GetBindingExpression(this, TextProperty);
                if (bindingExpression != null)
                    bindingExpression.UpdateSource();
            }
        }
    }
}
