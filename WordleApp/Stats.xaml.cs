using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordleApp;

public partial class Stats : Popup, INotifyPropertyChanged
{
    private int _numWins;
    private int _gamesPlayed;
    private int _percentWon;
    private int _streak;
    private string _playerName;
    private string SaveFilePath => Path.Combine(FileSystem.Current.AppDataDirectory, $"{_playerName}_stats.txt");

    public int NumWins
    {
        get => _numWins;
        set
        {
            if (_numWins != value)
            {
                _numWins = value;
                OnPropertyChanged();
            }
        }
    }

    public int GamesPlayed
    {
        get => _gamesPlayed;
        set
        {
            if (_gamesPlayed != value)
            {
                _gamesPlayed = value;
                OnPropertyChanged();
            }
        }
    }

    public int PercentWon
    {
        get => _percentWon;
        set
        {
            if (_percentWon != value)
            {
                _percentWon = value;
                OnPropertyChanged();
            }
        }
    }

    public int Streak
    {
        get => _streak;
        set
        {
            if (_streak != value)
            {
                _streak = value;
                OnPropertyChanged();
            }
        }
    }


    public Stats(string playerName)
    {
        InitializeComponent();
        _playerName = playerName;
        BindingContext = this;
        LoadStatistics();

    }

    // Change the protection level to public to make it accessible
    public void LoadStatistics()
    {
        // Check if the stats file exists before reading
        if (File.Exists(SaveFilePath))
        {

            // Read and parse statistics data
            var lines = File.ReadAllLines(SaveFilePath);

            if (lines.Length >= 4)
            {
                NumWins = int.Parse(lines[0]);
                Streak = int.Parse(lines[1]);
                GamesPlayed = int.Parse(lines[2]);
                PercentWon = int.Parse(lines[3]);
            }
            else
            {
                // If the file is incomplete, reset stats and save them
                ResetStats();
                SaveDefaultStats();
            }


        }
        else
        {
            // If no file exists, initialize statistics to zero
            ResetStats();
            SaveDefaultStats(); // Create a new file for the new user
        }
    }

    // Save default stats for new players
    private void SaveDefaultStats()
    {

        using (StreamWriter sw = new StreamWriter(SaveFilePath, false))
        {
            sw.WriteLine(0); // NumWins
            sw.WriteLine(0); // Streak
            sw.WriteLine(0); // GamesPlayed
            sw.WriteLine(0); // PercentWon
        }

    }

    // Reset stats to default values
    private void ResetStats()
    {
        NumWins = 0;
        Streak = 0;
        GamesPlayed = 0;
        PercentWon = 0;
    }

    public void UpdateStatistics()
    {
        // Reload statistics if needed

        LoadStatistics();

    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
