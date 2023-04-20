using Microsoft.Playwright;

namespace tmg_hero
{
    //On stop save the current state of the game to file. Also there should be a button to get the save.
    //On start the player will need to manually load / or we could prompt for them to copy a save file and import to clipboard or supply a file path

    public partial class Control : Form
    {
        private const string Url = "https://www.theresmoregame.com/play/";
        private bool _isPlaying;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _browserContext;
        private IPage _page;
        private System.Windows.Forms.Timer _gameTimer;

        public Control()
        {
            InitializeComponent();
            _isPlaying = false;
        }

        private async void PlayStop_Click(object sender, EventArgs e)
        {
            if (_isPlaying)
            {
                _isPlaying = false;
            }
            else
            {
                _isPlaying = true;
                await PlayGameAsync();
            }
        }

        private async Task PlayGameAsync()
        {
            _gameTimer = new System.Windows.Forms.Timer();
            _gameTimer.Interval = 1000; // Adjust this value to set the interval between interactions
            _gameTimer.Tick += async (s, e) =>
            {
                if (_isPlaying)
                {
                    await _page.ClickAsync("text=Artisan Workshop");
                }
                else
                {
                    _gameTimer.Stop();
                }
            };
            _gameTimer.Start();
        }

        private void LoadSave_Click(object sender, EventArgs e)
        {
            // Create a new form that will serve as the dialog window
            Form inputForm = new Form
            {
                Width = 500,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Load Save Game Data",
                StartPosition = FormStartPosition.CenterScreen
            };

            // Add a label to instruct the user
            Label label = new Label
            {
                Left = 20,
                Top = 20,
                Width = 400,
                Text = "Please paste your save game data:"
            };
            inputForm.Controls.Add(label);

            // Add a text box for the user to input their save game data
            TextBox textBox = new TextBox
            {
                Left = 20,
                Top = 50,
                Width = 440,
                Multiline = true
            };
            inputForm.Controls.Add(textBox);

            // Add an "OK" button and set up its event handler
            Button okButton = new Button
            {
                Text = "OK",
                Left = 320,
                Width = 100,
                Top = 110,
                DialogResult = DialogResult.OK
            };
            okButton.Click += async (s, a) =>
            {
                // Call your InjectSaveGameData method with the user's input
                await InjectSaveGameData(textBox.Text);
                inputForm.Close();
            };
            inputForm.Controls.Add(okButton);

            // Add a "Cancel" button and set up its event handler
            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 200,
                Width = 100,
                Top = 110,
                DialogResult = DialogResult.Cancel
            };
            cancelButton.Click += (s, a) => { inputForm.Close(); };
            inputForm.Controls.Add(cancelButton);

            // Show the dialog window
            inputForm.AcceptButton = okButton;
            inputForm.CancelButton = cancelButton;
            inputForm.ShowDialog();
        }

        private async Task InjectSaveGameData(string saveGameData)
        {
            // Initialize a new Playwright instance if not already initialized
            if (_browser == null)
            {
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            }

            // Create a new context and page if not already created
            if (_browserContext == null)
            {
                _browserContext = await _browser.NewContextAsync();
            }
            if (_page == null)
            {
                _page = await _browserContext.NewPageAsync();
            }

            // Navigate to the game URL
            await _page.GotoAsync(Url);
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // Click on the "Import" button
            await _page.GetByRole(AriaRole.Banner).GetByRole(AriaRole.Button).Nth(3).ClickAsync();

            // Click on the "Import from clipboard" button
            await _page.ClickAsync("text=Import from clipboard");

            // Click on the "Click here to paste a save" text
            await _page.ClickAsync("text=Click here to paste a save");

            // Set the value of the text area to the save game data and trigger the "input" event to update the game state
            //await _page.EvaluateAsync(@$"document.querySelector('textarea').value = '{saveGameData.Replace("'", @"\'")}'");
            //await _page.EvaluateAsync("document.querySelector('textarea').dispatchEvent(new Event('input'))");
            await _page.Keyboard.DownAsync("Control");
            await _page.Keyboard.PressAsync("KeyV");
            await _page.Keyboard.UpAsync("Control");
        }
    }
}
