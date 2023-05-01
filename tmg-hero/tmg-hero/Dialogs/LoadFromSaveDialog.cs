namespace tmg_hero.Dialogs;

internal static class LoadFromSaveDialog
{
    public static Task ShowLoadFromSaveDialog(Func<string, Task> injectSaveFunction)
    {
        // Create a new form that will serve as the dialog window
        var inputForm = new Form
        {
            Width = 500,
            Height = 200,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = "Load Save Game Data",
            StartPosition = FormStartPosition.CenterScreen
        };

        var label = new Label
        {
            Left = 20,
            Top = 20,
            Width = 400,
            Text = "Please paste your save game data:"
        };
        inputForm.Controls.Add(label);

        var textBox = new TextBox
        {
            Left = 20,
            Top = 50,
            Width = 440,
            Multiline = true
        };
        inputForm.Controls.Add(textBox);

        var okButton = new Button
        {
            Text = "OK",
            Left = 320,
            Width = 100,
            Top = 110,
            DialogResult = DialogResult.OK
        };

        var tcs = new TaskCompletionSource<bool>();

        okButton.Click += async (s, a) =>
        {
            await injectSaveFunction(textBox.Text);
            tcs.SetResult(true);
            inputForm.Close();
        };
        inputForm.Controls.Add(okButton);

        var cancelButton = new Button
        {
            Text = "Cancel",
            Left = 200,
            Width = 100,
            Top = 110,
            DialogResult = DialogResult.Cancel
        };
        cancelButton.Click += (s, a) => inputForm.Close();
        inputForm.Controls.Add(cancelButton);

        inputForm.AcceptButton = okButton;
        inputForm.CancelButton = cancelButton;
        inputForm.ShowDialog();
        return tcs.Task;
    }
}
