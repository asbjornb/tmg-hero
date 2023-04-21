using Microsoft.Playwright;
using System.Threading;
using tmg_hero.Dialogs;
using tmg_hero.Engine;

namespace tmg_hero;

//On stop save the current state of the game to file. Also there should be a button to get the save.
//On start the player will need to manually load / or we could prompt for them to copy a save file and import to clipboard or supply a file path

public partial class Control : Form
{
    private const string Url = "https://www.theresmoregame.com/play/";
    private GameController _gameController;
    private CancellationTokenSource? _cancellationTokenSource;

    public Control()
    {
        InitializeComponent();
        _gameController = new GameController();
    }

    private async void PlayStop_Click(object sender, EventArgs e)
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
            _gameController.StopPlaying();
        }
        else
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await _gameController.PlayGameAsync(_cancellationTokenSource.Token);
        }
    }

    private void LoadSave_Click(object sender, EventArgs e)
    {
        LoadFromSaveDialog.ShowLoadFromSaveDialog(_gameController.InjectSaveGameData);
    }
}
