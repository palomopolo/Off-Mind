using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;

namespace OffMindApp;

public partial class MainWindow : Window
{
    private readonly StorageService _storage = new();
    private Library _library = new();
    private readonly System.Windows.Media.MediaPlayer _player = new();
    private readonly DispatcherTimer _timer = new();
    private DateTime _playStartedAt;
    private Song? _currentSong;

    public MainWindow()
    {
        InitializeComponent();
        LoadLibrary();

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += OnTick;
    }

    private void LoadLibrary()
    {
        _library = _storage.LoadLibrary();
        RefreshLists();
        DeveloperPanel.Visibility = _library.Songs.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void RefreshLists()
    {
        SongsList.Items.Clear();
        foreach (var song in _library.Songs)
        {
            SongsList.Items.Add($"{song.Title} · {song.Artist} ({song.Stats.PlayCount} plays)");
        }

        AlbumsList.Items.Clear();
        foreach (var album in _library.Albums)
        {
            AlbumsList.Items.Add($"{album.Name} · {album.SongIds.Count} canciones");
        }

        PlaylistsList.Items.Clear();
        foreach (var playlist in _library.Playlists)
        {
            PlaylistsList.Items.Add($"{playlist.Name} · {playlist.SongIds.Count} canciones");
        }
    }

    private void OnAddSong(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleInput.Text) || string.IsNullOrWhiteSpace(ArtistInput.Text))
        {
            MessageBox.Show("Agrega título y artista para continuar.");
            return;
        }

        var song = new Song
        {
            Title = TitleInput.Text.Trim(),
            Artist = ArtistInput.Text.Trim(),
            Album = AlbumInput.Text.Trim(),
            FilePath = FileInput.Text.Trim(),
            AlbumArtPath = AlbumArtInput.Text.Trim()
        };

        _library.Songs.Add(song);
        _storage.SaveLibrary(_library);
        RefreshLists();
        DeveloperPanel.Visibility = Visibility.Collapsed;
        ClearSongInputs();
    }

    private void ClearSongInputs()
    {
        TitleInput.Text = string.Empty;
        ArtistInput.Text = string.Empty;
        AlbumInput.Text = string.Empty;
        FileInput.Text = string.Empty;
        AlbumArtInput.Text = string.Empty;
    }

    private void OnAddAlbum(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AlbumNameInput.Text))
        {
            MessageBox.Show("Ingresa un nombre de álbum.");
            return;
        }

        _library.Albums.Add(new Album { Name = AlbumNameInput.Text.Trim() });
        AlbumNameInput.Text = string.Empty;
        _storage.SaveLibrary(_library);
        RefreshLists();
    }

    private void OnAddPlaylist(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(PlaylistNameInput.Text))
        {
            MessageBox.Show("Ingresa un nombre de playlist.");
            return;
        }

        _library.Playlists.Add(new Playlist { Name = PlaylistNameInput.Text.Trim() });
        PlaylistNameInput.Text = string.Empty;
        _storage.SaveLibrary(_library);
        RefreshLists();
    }

    private void OnBrowseMp3(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "MP3 files (*.mp3)|*.mp3" };
        if (dialog.ShowDialog() == true)
        {
            FileInput.Text = dialog.FileName;
        }
    }

    private void OnBrowseArt(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg" };
        if (dialog.ShowDialog() == true)
        {
            AlbumArtInput.Text = dialog.FileName;
        }
    }

    private void OnPlayClick(object sender, RoutedEventArgs e)
    {
        if (SongsList.SelectedIndex < 0)
        {
            MessageBox.Show("Selecciona una canción para reproducir.");
            return;
        }

        var song = _library.Songs[SongsList.SelectedIndex];
        if (string.IsNullOrWhiteSpace(song.FilePath))
        {
            MessageBox.Show("Esta canción no tiene archivo MP3 asociado.");
            return;
        }

        _currentSong = song;
        _player.Open(new Uri(song.FilePath, UriKind.Absolute));
        _player.Play();
        _playStartedAt = DateTime.Now;
        song.Stats.PlayCount += 1;
        _timer.Start();
        _storage.SaveLibrary(_library);
        RefreshLists();
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        _player.Stop();
        _timer.Stop();
        UpdateListenTime();
        _currentSong = null;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        UpdateListenTime();
        _playStartedAt = DateTime.Now;
    }

    private void UpdateListenTime()
    {
        if (_currentSong == null)
        {
            return;
        }

        var elapsed = DateTime.Now - _playStartedAt;
        if (elapsed.TotalSeconds < 1)
        {
            return;
        }

        _currentSong.Stats.TotalListened += elapsed;
        _storage.SaveLibrary(_library);
    }
}
