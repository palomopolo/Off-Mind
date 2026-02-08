using System;
using System.Collections.Generic;

namespace OffMindApp;

public sealed class Song
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string AlbumArtPath { get; set; } = string.Empty;
    public SongStats Stats { get; set; } = new();
}

public sealed class SongStats
{
    public int PlayCount { get; set; }
    public TimeSpan TotalListened { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public sealed class Album
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Guid> SongIds { get; set; } = new();
}

public sealed class Playlist
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Guid> SongIds { get; set; } = new();
}

public sealed class Library
{
    public List<Song> Songs { get; set; } = new();
    public List<Album> Albums { get; set; } = new();
    public List<Playlist> Playlists { get; set; } = new();
}
