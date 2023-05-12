using Microsoft.Playwright;
using tmg_hero.Engine;
using tmg_hero.winforms.Dialogs;

namespace tmg_hero.winforms;

//On stop save the current state of the game to file. Also there should be a button to get the save.
//On start the player will need to manually load / or we could prompt for them to copy a save file and import to clipboard or supply a file path

public partial class Control : Form
{
    private readonly GameController _gameController;
    private readonly SaveGameManager _saveGameManager;
    private IPage? _page;
    private CancellationTokenSource? _cancellationTokenSource;

    public Control()
    {
        InitializeComponent();
        _gameController = new GameController();
        _saveGameManager = new SaveGameManager();
    }

    private async void PlayStop_Click(object sender, EventArgs e)
    {
        if (_page is null)
        {
            _page = await _saveGameManager.OpenGameAsync();
            //Display a dialog telling to import save before playing, then return
            await LoadFromSaveDialog.ShowLoadFromSaveDialog(x => SaveGameManager.LoadSaveGame(x, _page));
        }
        if (_cancellationTokenSource?.IsCancellationRequested == false)
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

    private async void LoadSave_Click(object sender, EventArgs e)
    {
        _page ??= await _saveGameManager.OpenGameAsync();
        await LoadFromSaveDialog.ShowLoadFromSaveDialog(x => SaveGameManager.LoadSaveGame(x, _page));
    }
}
