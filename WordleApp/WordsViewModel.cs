﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordleApp.ViewModels
{
    public class WordsViewModel : INotifyPropertyChanged
    {
        // URL to fetch the words file
        private const string WordsFileUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        private string FilePath => Path.Combine(FileSystem.Current.AppDataDirectory, "words.txt");

        // Event to notify property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private List<string> _listOfWords = new();
        public List<string> Words => _listOfWords;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }
        // Indicates whether the ViewModel is not busy
        public bool IsNotBusy => !IsBusy;

        // HTTP client used to fetch the words
        private readonly HttpClient _httpClient;

        public WordsViewModel(HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();

            if (File.Exists(FilePath))
                LoadWordsFromFile();
        }

        // Loads words from file into the list
        private void LoadWordsFromFile()
        {
            _listOfWords = File.ReadAllLines(FilePath).ToList();
        }

        // Saves words to a file
        private void SaveWordsToFile(string data)
        {
            File.WriteAllText(FilePath, data);
        }

        // Fetches words from a remote URL
        private async Task FetchWordsFromRemote()
        {
            var response = await _httpClient.GetStringAsync(WordsFileUrl);
            _listOfWords = response.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            SaveWordsToFile(response);
        }

        // Initializes the word list
        public async Task InitializeWordList()
        {
            if (IsBusy) return;

            IsBusy = true;

            if (File.Exists(FilePath))
            {
                LoadWordsFromFile();
            }
            else
            {
                await FetchWordsFromRemote();
            }

            IsBusy = false;
        }

        // Ensure the word list is initialized properly
        public async Task EnsureWordsInitializedAsync()
        {
            if (_listOfWords.Count == 0)
            {
                if (File.Exists(FilePath)) 
                {
                    LoadWordsFromFile();
                }
                else 
                {
                    await FetchWordsFromRemote();
                }
            }
        }

        //Notify the UI of property changes
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
