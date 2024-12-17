
using CommunityToolkit.Maui.Views;

namespace WordleApp;

public partial class MainPage : ContentPage
{
    public List<Button> keys;
    private List<Label> addedLabels = new List<Label>();
    private List<Frame> addedFrames = new List<Frame>();





    private int letterCounter = 0, guessCounter = 0;
    private bool gridDrawn = false;
    private string guess;
    public MainPage()
    {
        InitializeComponent();
        DrawGrid();

        //populate list of keys
        keys = new List<Button>
        {
            aKey, bKey, cKey, dKey, eKey, fKey, gKey, hKey, iKey, jKey,
            kKey, lKey, mKey, nKey, oKey, pKey, qKey, rKey, sKey, tKey,
            uKey, vKey, wKey, xKey, yKey, zKey, back_btn
        };
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
        }//if grid hasnt been drawn - first iteration

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

    private async void GoToStats(object sender, EventArgs e)
    {

    }

    private void qKey_Clicked(object sender, EventArgs e)
    {

    }
}



