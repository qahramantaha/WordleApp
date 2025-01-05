
using CommunityToolkit.Maui.Views;
using WordleApp.ViewModels;

namespace WordleApp;

public partial class MainPage : ContentPage
{
    private WordsViewModel _wordsViewModel = new WordsViewModel();

    public List<Button> keys;
    private List<Label> addedLabels = new List<Label>();
    private List<Frame> addedFrames = new List<Frame>();
    private List<string> words = new List<string>();
    private List<string> guesses = new List<string>();
    private List<char> correctLettersGuessed = new List<char>();
    private List<char> yellowLettersGuessed = new List<char>();
    private List<char> wrongLettersGuessed = new List<char>();

    private Random random = new Random();
    private string SaveFilePath => Path.Combine(FileSystem.Current.AppDataDirectory, "stats.txt");
    private string WordsFilePath => Path.Combine(FileSystem.Current.AppDataDirectory, "words.txt");



    private int letterCounter = 0, guessCounter = 0;
    private int numWins, gamesPlayed, percentWon, streak;
    private bool gameRunning = false;
    private bool validInput;
    private bool won = false;
    private string _playerFilePath;
    private CancellationTokenSource timerCancellationTokenSource;
    private bool gridDrawn = false;
    private string guess;
    private string _playerName;

    private string chosenWord { get; set; }


    private bool isHardMode;


    public MainPage(string playerName)
    {
        InitializeComponent();
        BindingContext = _wordsViewModel;
        _playerName = playerName;
        LoadPlayerStats();
        PlayerNameLabel.Text = $"Welcome, {_playerName}!";
        InitializeGameAsync();

        timerLabel = new Label
        {
            Text = "Time Remaining: 15:00",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            IsVisible = false, // Initially hidden
            TextColor = Colors.Red // Default color for light mode
        };


        MainLayout.Children.Add(timerLabel);
         
        //populate list of keys
        keys = new List<Button>
        {
            aKey, bKey, cKey, dKey, eKey, fKey, gKey, hKey, iKey, jKey,
            kKey, lKey, mKey, nKey, oKey, pKey, qKey, rKey, sKey, tKey,
            uKey, vKey, wKey, xKey, yKey, zKey, back_btn
        };

        if ((bool)Application.Current.Resources["IsDarkMode"])
        {
            stats_image.Source = "stats_dark.png";
            how_image.Source = "how_dark.png";
            


        }
        else
        {
            stats_image.Source = "stats.png";
            how_image.Source = "how.png";

        }

        isHardMode = (bool)Application.Current.Resources["IsHardMode"];
        PlayGame();

    }

 //determines whether app is dark mode and assigns images 
    public void PlayGame()
    {
        
        if ((bool)Application.Current.Resources["IsDarkMode"])
        {
            stats_image.Source = "stats_dark.png";
            how_image.Source = "how_dark.png";
        }
        else
        {
            stats_image.Source = "stats.png";
            how_image.Source = "how.png";
        }

        LoadPlayerStats();
        RestartGame();
        GetWord();

        if (isHardMode)
        {
            timerLabel.IsVisible = true; 
            StartTimer();
        }
        else
        {
            timerLabel.IsVisible = false; 
        }

    }

    // Handles the click event for the "Play Again" button and starts a new game
    private void playAgain_btn_Clicked(object sender, EventArgs e)
    {
        //plays game when clicked
        PlayGame();
    }

    // Draws grid on first iteration, then only adds the frame for every iteration after.
    private void DrawGrid()
    {
                
        if (!gridDrawn)
        {
            for (int i = 0; i < 6; i++)
                GuessGrid.AddRowDefinition(new RowDefinition());

            for (int i = 0; i < 5; i++)
                GuessGrid.AddColumnDefinition(new ColumnDefinition());
        }

        
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 6; col++)
            {
               
                    Frame styledFrame = new Frame
                    {
                        BackgroundColor = (Color)Application.Current.Resources["BackGroundColor"],
                        CornerRadius = 0,
                        HasShadow = false,
                        Padding = new Thickness(5),
                        BorderColor = (Color)Application.Current.Resources["TextColor"]
                    };

                    GuessGrid.Add(styledFrame, row, col);

                    addedFrames.Add(styledFrame);
                
               
            }
        }
        gridDrawn = true;
    }
   


    //  Retrieves list of words from view model, calls method to pick a word
    private async void GetWord()
    {
        words = _wordsViewModel.Words;
        chosenWord = PickWord();
    }

    //Picks a word from the list based on a random number & returns it.
    public string PickWord()
    {
       
        if (words.Count > 0)
        {
            int randomNumber = random.Next(0, words.Count);
            return words[randomNumber];
        }
        else
        {
            return null;
        }
    }

    // Determines which button has been clicked, adds a label to the grid with the corresponding value.               
    private async void Button_Clicked(object sender, EventArgs e)
    {
             
        if (guessCounter < 6 && letterCounter < 5)
        {
            if (sender is Button button)
            {
                string text = button.Text;

                var label = new Label
                {
                    Text = text,
                    FontSize = 30,
                    TextColor = (Color)Application.Current.Resources["TextColor"],
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center

                };

                GuessGrid.Add(label, letterCounter, guessCounter);
                addedLabels.Add(label);
                letterCounter++;

                //animation - increase size of letter briefly on input
                await label.ScaleTo(1.2, 200);
                await Task.Delay(100);
                await label.ScaleTo(1, 200);

                if (letterCounter == 5)
                    enter_btn.IsEnabled = true;
                else
                    enter_btn.IsEnabled = false;
            }
        }

    }

    //Handles the event when the "Enter" button is clicked, validates the word, and processes the guess
    private void Enter_Clicked(object sender, EventArgs e)
    {
       
        if (guessCounter < 6)
        {
            guess = "";
            int rowIndex = guessCounter;
            bool isRowFull = true;

            foreach (var child in GuessGrid.Children)
            {
                if (GuessGrid.GetRow(child) == rowIndex && child is Label label)
                {
                    if (string.IsNullOrEmpty(label.Text))
                    {
                        isRowFull = false;
                        break;
                    }
                    else
                    {
                        guess += label.Text;
                    }
                }
            }
            guess = guess.ToLower();
            ValidWord();
            if (!validInput)
            {
                DisplayAlert("Invalid Input", "Word not in word list", "Ok");
            }
            else
            {
                if (isRowFull)
                {
                    guesses.Add(guess);
                    checkWord();
                    guessCounter++;
                    letterCounter = 0;
                }
            }
        }
    
    }

  //  Removes last label added to grid.
    private void Backspace_Clicked(object sender, EventArgs e)
    {
       
        if (letterCounter > 0)
        {
            var lastLabel = addedLabels[addedLabels.Count - 1];
            GuessGrid.Children.Remove(lastLabel);
            addedLabels.Remove(lastLabel);
            letterCounter--;
            if (letterCounter < 5)
            {
                enter_btn.IsEnabled = false;
            }
        }
        if (letterCounter < 0)
        {
            letterCounter = GuessGrid.ColumnDefinitions.Count - 1;
        }
    }

  //  Compares each letter in guess to each letter in chosen word,
  //  changes frame colour to green, yellow or grey.Calls Win() if game is won,
  //   or Lose() if you run out of guesses.
    private async void checkWord()
    {
        
        enter_btn.IsEnabled = false;
        int row = guessCounter;

        List<char> chosenLetters = chosenWord.ToList();
        List<int> greenLetters = new List<int>();
        List<int> yellowLetters = new List<int>();

        for (int col = 0; col < 5; col++)
        {
            //go through word looking for correct spots
            if (guess[col] == chosenWord[col])
            {
                //record green indexes
                greenLetters.Add(col);
                correctLettersGuessed.Add(guess[col]);
                chosenLetters.Remove(guess[col]);
            }
        }

        for (int col = 0; col < 5; col++)
        {
            if (chosenLetters.Contains(guess[col]))
            {
                yellowLetters.Add(col);
                chosenLetters.Remove(guess[col]);
                yellowLettersGuessed.Add(guess[col]);
            }
        }

        for (int column = 0; column < 5; column++)
        {
            Frame currentFrame = null;

            foreach (var child in GuessGrid.Children)
            {
                if (child is Frame && GuessGrid.GetRow(child) == row && GuessGrid.GetColumn(child) == column)
                {
                    currentFrame = (Frame)child;
                    break;
                }
            }
            if (currentFrame != null)
            {
                //frame animation
                await currentFrame.ScaleTo(1.2, 100);
                await Task.Delay(100);
                await currentFrame.ScaleTo(1, 100);

                if (greenLetters.Contains(column))
                {
                    //turn square purple
                    currentFrame.BackgroundColor = Colors.Purple;
                    ChangeKeyPurple(guess[column]);
                }
                else if (yellowLetters.Contains(column))
                {
                    //else turn square orange
                    currentFrame.BackgroundColor = Colors.Orange;
                    if (!correctLettersGuessed.Contains(guess[column]))
                    {
                        ChangeKeyOrange(guess[column]);
                    }
                }
                else
                {
                    //turn square darker grey
                    currentFrame.BackgroundColor = Colors.Gray;
                    if (!correctLettersGuessed.Contains(guess[column]))
                    {
                        wrongLettersGuessed.Add(guess[column]);
                        if (!yellowLettersGuessed.Contains(guess[column]))
                            ChangeKeyGrey(guess[column]);
                    }
                }
            }
        }
        //if all letters are green
        if (greenLetters.Count == 5)
        {
            won = true;
            Win();
        }
        chosenLetters.Clear();
        greenLetters.Clear();
        yellowLetters.Clear();
        if (guessCounter == 6 && !won)
            Lose();
    }

    //Compares guess to word list to check if valid.
    private void ValidWord()
    {
        for (int i = 0; i < words.Count; i++)
        {
            if (guess.Equals(words[i]))
            {
                validInput = true;
                return;
            }
            else
                validInput = false;
        }
    }

    // Informs player that they won, increments appropriate variables, calls to save details to file, then shows Stats pop up page.
    private async void Win()
    {

        timerCancellationTokenSource?.Cancel();

        gamesPlayed++;
        playAgain_btn.IsVisible = true;
        numWins++;
        streak++;
        SavePlayerStats();

        await DisplayAlert("Win!", "You Won!", "Ok");
        DisableKeyboard();
        gameRunning = false;


        Stats statsPage = new Stats(_playerName);
        statsPage.UpdateStatistics();
        await this.ShowPopupAsync(statsPage);
    }

   // Displays correct word to player, saves details, shows Stats pop up page.
    private async void Lose()
    {

        timerCancellationTokenSource?.Cancel();

        
       
        gamesPlayed++;
        playAgain_btn.IsVisible = true;
        streak = 0;
        SavePlayerStats();

        DisplayAnswer(chosenWord);
        DisableKeyboard();
        gameRunning = false;

        Stats statsPage = new Stats(_playerName);
        statsPage.UpdateStatistics();
        await this.ShowPopupAsync(statsPage);
    }

    // Saves the player's statistics to a file.
    public void SavePlayerStats()
    {
        // Determine the file path for the player's stats
        _playerFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{_playerName}_stats.txt");

        // Calculate win percentage
        percentWon = gamesPlayed > 0 ? (numWins * 100) / gamesPlayed : 0;


        using (StreamWriter sw = new StreamWriter(_playerFilePath, false))
        {
            // Write the stats to the file
            sw.WriteLine(numWins);  // Number of wins
            sw.WriteLine(streak);   // Streak
            sw.WriteLine(gamesPlayed); // Games played
            sw.WriteLine(percentWon);  // Win percentage
        }


    }


    //Loads the player's statistics from a file or initializes default stats if the file does not exist
    public void LoadPlayerStats()
    {
        _playerFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, $"{_playerName}_stats.txt");

        if (File.Exists(_playerFilePath))
        {

            // If the file exists, load the player's stats
            using (StreamReader sr = new StreamReader(_playerFilePath))
            {
                numWins = int.Parse(sr.ReadLine() ?? "0");
                streak = int.Parse(sr.ReadLine() ?? "0");
                gamesPlayed = int.Parse(sr.ReadLine() ?? "0");
                percentWon = int.Parse(sr.ReadLine() ?? "0");
            }


        }
        else
        {
            // If the file does not exist, do the following
            ResetStats();
            SavePlayerStats();
        }
    }


    // Method to reset stats to their default values
    private void ResetStats()
    {
        numWins = 0;
        streak = 0;
        gamesPlayed = 0;
        percentWon = 0;
    }

    //Preps the page to play again, checks if game is in hard mode
    public void RestartGame()
    {
    
        //clear grid
        ClearGrid();

        //redraw grid
        DrawGrid();

        //enable keyboard
        EnableKeyboard();
        ResetKeyColor();
        gameRunning = true;

        playAgain_btn.IsVisible = false;
        isHardMode = (bool)Application.Current.Resources["IsHardMode"];
    }


    //Removes existing labels and frames from the grid, also resets variables.
    private void ClearGrid()
    {
       
        foreach (var label in addedLabels)
        {
            GuessGrid.Children.Remove(label);
        }

        foreach (var frame in addedFrames)
        {
            GuessGrid.Children.Remove(frame);
        }

        //reset variables / clear lists
        addedLabels.Clear();
        addedFrames.Clear();
        guesses.Clear();
        correctLettersGuessed.Clear();
        yellowLettersGuessed.Clear();
        wrongLettersGuessed.Clear();
        guess = "";
        letterCounter = 0;
        guessCounter = 0;
        won = false;
    }

    //Displays correct word
    private async void DisplayAnswer(string answer)
    {
        
        await DisplayAlert("Answer", $"The correct word was: {answer}", "OK");
    }

    // Disables all keys in keyboard.
    private void DisableKeyboard()
    {
      
        foreach (var button in keys)
        {
            button.IsEnabled = false;
        }
    }

    //Enables all keys in keyboard.
    private void EnableKeyboard()
    {
        
        foreach (var button in keys)
        {
            button.IsEnabled = true;
        }
    }

    //Turns corresponding key green.

    private void ChangeKeyPurple(char key)
    {
        char lowerKey = char.ToLower(key);

        foreach (var button in keys)
        {
            if (button.Text != null && char.ToLower(button.Text[0]) == lowerKey)
            {
                button.BackgroundColor = (Color)Application.Current.Resources["Purple"];
                break; 
            }
        }
    }

    //Turns corresponding key yellow
    private void ChangeKeyOrange(char key)
    {
       
        char lowerKey = char.ToLower(key);

        foreach (var button in keys)
        {
            if (button.Text != null && char.ToLower(button.Text[0]) == lowerKey)
            {
                button.BackgroundColor = Colors.Orange;
                break;
            }
        }
    }

    //Turns corresponding key grey, if game is in hard mode, those keys are also disabled.
    private void ChangeKeyGrey(char key)
    {
            
        
        char lowerKey = char.ToLower(key);

        foreach (var button in keys)
        {
            if (button.Text != null && char.ToLower(button.Text[0]) == lowerKey)
            {
                button.BackgroundColor = Colors.Gray;
                break;
            }
        }

        if (isHardMode)
        {
            foreach (var button in keys)
            {
                if (button.Text != null && char.ToLower(button.Text[0]) == lowerKey)
                {
                    button.IsEnabled = false;
                    break;
                }
            }
        }
    }

    // Resets all keys back to original colour.
    private void ResetKeyColor()
    {
       
        foreach (var button in keys)
        {
            button.BackgroundColor = Colors.LightGrey;
        }
    }


    //Updates player statistics from the saved file
    public void UpdateStatistics()
    {
        if (File.Exists(SaveFilePath))
        {
            using (StreamReader sr = new StreamReader(SaveFilePath))
            {
                numWins = int.Parse(sr.ReadLine());
                streak = int.Parse(sr.ReadLine());
                gamesPlayed = int.Parse(sr.ReadLine());

                percentWon = gamesPlayed > 0 ? (numWins * 100) / gamesPlayed : 0;
            }
        }
        else
        {
            numWins = 0;
            streak = 0;
            gamesPlayed = 0;
            percentWon = 0;
        }
    }

    private Label timerLabel; // Add this as a member variable to display the timer.

    private async void StartTimer()
    {
        if (!isHardMode) return; // Only start the timer in hard mode.

        timerCancellationTokenSource?.Cancel(); // Cancel any existing timer.
        timerCancellationTokenSource = new CancellationTokenSource();
        int totalSeconds = 900; 

        while (totalSeconds > 0)
        {
            if (timerCancellationTokenSource.Token.IsCancellationRequested)
                return;

            // Calculate minutes and seconds
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            // Update the timerLabel using Dispatcher.Dispatch
            Dispatcher.Dispatch(() =>
            {
                timerLabel.Text = $"Time Remaining: {minutes:D2}:{seconds:D2}";
            });

            // Wait for 1 second
            await Task.Delay(1000);
            totalSeconds--;
        }

        // If time runs out, trigger Lose
        if (totalSeconds <= 0)
        {
            Dispatcher.Dispatch(() =>
            {
                Lose();
            });
        }
    }

    // Async method to initialize the game
    private async void InitializeGameAsync()
    {
        await _wordsViewModel.EnsureWordsInitializedAsync();

        if (_wordsViewModel.Words.Count == 0)
        {
            await DisplayAlert("Error", "Failed to load word list. Please check your internet connection.", "OK");
            return;
        }

        LoadPlayerStats();
        PlayerNameLabel.Text = $"Welcome, {_playerName}!";
        PlayGame();
    }

    //Shows HowToPlay pop up page.
    private async void GoToHow(object sender, EventArgs e)
    {
       
        HowToPlay howToPlayPage = new HowToPlay();
        await this.ShowPopupAsync(howToPlayPage);

    }

   // Shows Settings pop up page.
    private async void GoToSettings(object sender, EventArgs e)
    {
    

        Settings settingsPage = new Settings();
        await this.ShowPopupAsync(settingsPage);

    }

    // Shows Stats pop up page
    private async void GoToStats(object sender, EventArgs e)
    {
             
        Stats statsPage = new Stats(_playerName);
        statsPage.UpdateStatistics();
        await this.ShowPopupAsync(statsPage);
    }

  
}



