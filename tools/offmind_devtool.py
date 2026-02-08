import argparse
import json
from dataclasses import asdict, dataclass, field
from datetime import datetime
from pathlib import Path
from typing import List


@dataclass
class SongStats:
    play_count: int = 0
    total_listened: float = 0.0
    created_at: str = field(default_factory=lambda: datetime.now().isoformat())


@dataclass
class Song:
    id: str
    title: str
    artist: str
    album: str
    file_path: str
    album_art_path: str
    stats: SongStats = field(default_factory=SongStats)


@dataclass
class Album:
    id: str
    name: str
    song_ids: List[str] = field(default_factory=list)


@dataclass
class Playlist:
    id: str
    name: str
    song_ids: List[str] = field(default_factory=list)


@dataclass
class Library:
    songs: List[Song] = field(default_factory=list)
    albums: List[Album] = field(default_factory=list)
    playlists: List[Playlist] = field(default_factory=list)


def load_library(path: Path) -> Library:
    if not path.exists():
        return Library()
    data = json.loads(path.read_text())
    return Library(
        songs=[Song(**{**song, "stats": SongStats(**song.get("stats", {}))}) for song in data.get("songs", [])],
        albums=[Album(**album) for album in data.get("albums", [])],
        playlists=[Playlist(**playlist) for playlist in data.get("playlists", [])],
    )


def save_library(path: Path, library: Library) -> None:
    payload = asdict(library)
    path.write_text(json.dumps(payload, indent=2, ensure_ascii=False))


def add_song(args: argparse.Namespace) -> None:
    path = Path(args.library).expanduser()
    library = load_library(path)
    song = Song(
        id=args.id,
        title=args.title,
        artist=args.artist,
        album=args.album,
        file_path=args.file_path,
        album_art_path=args.album_art_path,
    )
    library.songs.append(song)
    save_library(path, library)


def add_collection(args: argparse.Namespace, kind: str) -> None:
    path = Path(args.library).expanduser()
    library = load_library(path)
    if kind == "album":
        library.albums.append(Album(id=args.id, name=args.name))
    else:
        library.playlists.append(Playlist(id=args.id, name=args.name))
    save_library(path, library)


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Off-Mind developer helper tool")
    parser.add_argument("--library", required=True, help="Ruta al archivo library.json")
    sub = parser.add_subparsers(dest="command", required=True)

    song = sub.add_parser("add-song", help="Agregar canción a la biblioteca")
    song.add_argument("--id", required=True)
    song.add_argument("--title", required=True)
    song.add_argument("--artist", required=True)
    song.add_argument("--album", default="")
    song.add_argument("--file-path", required=True)
    song.add_argument("--album-art-path", default="")
    song.set_defaults(func=add_song)

    album = sub.add_parser("add-album", help="Agregar álbum")
    album.add_argument("--id", required=True)
    album.add_argument("--name", required=True)
    album.set_defaults(func=lambda args: add_collection(args, "album"))

    playlist = sub.add_parser("add-playlist", help="Agregar playlist")
    playlist.add_argument("--id", required=True)
    playlist.add_argument("--name", required=True)
    playlist.set_defaults(func=lambda args: add_collection(args, "playlist"))

    return parser


def main() -> None:
    parser = build_parser()
    args = parser.parse_args()
    args.func(args)


if __name__ == "__main__":
    main()
