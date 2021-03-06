﻿using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using Prism.Mvvm;

namespace Bloom.Domain.Models
{
    /// <summary>
    /// Represents a musical artist.
    /// </summary>
    /// <seealso cref="BindableBase" />
    [Table(Name = "artist")]
    public class Artist : BindableBase
    {
        /// <summary>
        /// Creates a new artist instance.
        /// </summary>
        /// <param name="name">The artist name.</param>
        public static Artist Create(string name)
        {
            return new Artist
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }
        
        /// <summary>
        /// Gets or sets the artist identifier.
        /// </summary>
        [Column(Name = "id", IsPrimaryKey = true)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the artist name.
        /// </summary>
        [Column(Name = "name")]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _name;

        /// <summary>
        /// Gets or sets the date this artist started their career.
        /// </summary>
        [Column(Name = "started_on")]
        public DateTime? StartedOn
        {
            get { return _startedOn; }
            set { SetProperty(ref _startedOn, value); }
        }
        private DateTime? _startedOn;

        /// <summary>
        /// Gets or sets the date this artist ended their career.
        /// </summary>
        [Column(Name = "ended_on")]
        public DateTime? EndedOn
        {
            get { return _endedOn; }
            set { SetProperty(ref _endedOn, value); }
        }
        private DateTime? _endedOn;

        /// <summary>
        /// Gets or sets the identifier of the city this artist formed in.
        /// </summary>
        [Column(Name = "formed_city_id")]
        public Guid? FormedCityId { get; set; }

        /// <summary>
        /// Gets or sets the artist's bio.
        /// </summary>
        [Column(Name = "bio")]
        public string Bio
        {
            get { return _bio; }
            set { SetProperty(ref _bio, value); }
        }
        private string _bio;

        /// <summary>
        /// Gets or sets the artist rating.
        /// </summary>
        [Column(Name = "rating")]
        public int? Rating { get; set; }

        /// <summary>
        /// Gets or sets the notes on this artist.
        /// </summary>
        [Column(Name = "notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the artist's Twitter username.
        /// </summary>
        [Column(Name = "twitter")]
        public string Twitter
        {
            get { return _twitter; }
            set { SetProperty(ref _twitter, value); }
        }
        private string _twitter;

        /// <summary>
        /// Gets or sets a whether this is a solo artist.
        /// </summary>
        [Column(Name = "is_solo")]
        public bool IsSolo
        {
            get { return _isSolo; }
            set { SetProperty(ref _isSolo, value); }
        }
        private bool _isSolo;

        /// <summary>
        /// Gets or sets the artist play count.
        /// </summary>
        [Column(Name = "play_count")]
        public int PlayCount { get; set; }

        /// <summary>
        /// Gets or sets the artist skip count.
        /// </summary>
        [Column(Name = "skip_count")]
        public int SkipCount { get; set; }

        /// <summary>
        /// Gets or sets the artist's remove count.
        /// </summary>
        [Column(Name = "remove_count")]
        public int RemoveCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this library follows this artist.
        /// </summary>
        [Column(Name = "follow")]
        public bool Follow { get; set; }

        /// <summary>
        /// Gets or sets the artist's photos.
        /// </summary>
        public List<Photo> Photos { get; set; }

        /// <summary>
        /// Gets or sets the artist members.
        /// </summary>
        public List<ArtistMember> Members { get; set; }

        /// <summary>
        /// Gets the next artist member priority.
        /// </summary>
        public int GetNextMemberPriority()
        {
            var highestPriority = 0;
            if (Members != null && Members.Any())
                highestPriority = Members.OrderByDescending(member => member.Priority).First().Priority;

            return highestPriority + 1;
        }

        /// <summary>
        /// Gets the profile image.
        /// </summary>
        public Photo ProfileImage
        {
            get
            {
                if (Photos == null || Photos.Count == 0)
                    return null;

                return Photos[0];
            }
            set
            {
                if (value == null || value.Id == Guid.Empty)
                    return;

                if (Photos == null)
                    Photos = new List<Photo> { value };
                else if (Photos.Count == 0)
                    Photos.Add(value);
                else
                    Photos[0] = value;
            }
        }

        /// <summary>
        /// Sets the profile image path.
        /// </summary>
        /// <param name="profileImagePath">The profile image path.</param>
        public void SetProfileImagePath(string profileImagePath)
        {
            if (Photos == null)
                Photos = new List<Photo> { Photo.Create(profileImagePath) };
            else if (Photos.Count == 0)
                Photos.Add(Photo.Create(profileImagePath));
            else
                Photos[0].FilePath = profileImagePath;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}
