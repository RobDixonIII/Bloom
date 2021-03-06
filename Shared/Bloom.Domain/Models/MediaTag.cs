﻿namespace Bloom.Domain.Models
{
    /// <summary>
    /// Represents the metadata tag of a media file or stream.
    /// </summary>
    public class MediaTag
    {
        /// <summary>
        /// Creates a new media tag instance.
        /// </summary>
        /// <param name="track">An album track.</param>
        /// <param name="album">An album.</param>
        public static MediaTag Create(AlbumTrack track, Album album)
        {
            if (track == null)
                return null;

            var trackCount = album == null ? 0 : album.GetTrackCount(track.DiscNumber);
            return new MediaTag
            {
                Title = track.Song?.Name,
                ArtistName = track.Song?.Artist?.Name,
                AlbumName = album?.Name,
                AlbumArtist = album?.Artist?.Name,
                MixedArtistAlbum = album?.IsMixedArtist,
                GenreName = track.Song?.Genre?.Name,
                Year = album?.FirstReleasedOn?.Year,
                DiscNumber = track.DiscNumber,
                DiscCount = album?.DiscCount ?? 1,
                TrackNumber = track.TrackNumber,
                TrackCount = trackCount == 0 ? null : trackCount,
                Bpm = track.Song?.Bpm,
                Rating = track.Song?.Rating,
                PlayCount = track.Song?.PlayCount
            };
        }

        /// <summary>
        /// Creates the specified song.
        /// </summary>
        /// <param name="song">The song.</param>
        public static MediaTag Create(Song song)
        {
            if (song == null)
                return null;

            return new MediaTag
            {
                Title = song.Name,
                ArtistName = song.Artist?.Name,
                GenreName = song.Genre?.Name,
                Bpm = song.Bpm,
                Rating = song.Rating,
                PlayCount = song.PlayCount
            };
        }

        /// <summary>
        /// Gets or sets the media title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the artist.
        /// </summary>
        public string ArtistName { get; set; }

        /// <summary>
        /// Gets or sets the name of the album.
        /// </summary>
        public string AlbumName { get; set; }

        /// <summary>
        /// Gets or sets the album artist.
        /// </summary>
        public string AlbumArtist { get; set; }

        /// <summary>
        /// Gets or sets whether this media's album is a mixed artist album.
        /// </summary>
        public bool? MixedArtistAlbum { get; set; }

        /// <summary>
        /// Gets or sets a list of composer names.
        /// </summary>
        public string Composers { get; set; }

        /// <summary>
        /// Gets or sets the grouping.
        /// </summary>
        public string Grouping { get; set; }

        /// <summary>
        /// Gets or sets the name of the genre.
        /// </summary>
        public string GenreName { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Gets or sets the track number.
        /// </summary>
        public int? TrackNumber { get; set; }

        /// <summary>
        /// Gets or sets the track count.
        /// </summary>
        public int? TrackCount { get; set; }

        /// <summary>
        /// Gets or sets the disc number.
        /// </summary>
        public int? DiscNumber { get; set; }

        /// <summary>
        /// Gets or sets the disc count.
        /// </summary>
        public int? DiscCount { get; set; }

        /// <summary>
        /// Gets or sets the beats per minute.
        /// </summary>
        public double? Bpm { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        public int? Rating { get; set; }

        /// <summary>
        /// Gets or sets the play count.
        /// </summary>
        public int? PlayCount { get; set; }
    }
}
