﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Bloom.Data.Interfaces;
using Bloom.Domain.Models;

namespace Bloom.Data.Repositories
{
    /// <summary>
    /// Access methods for artist data.
    /// </summary>
    public class ArtistRepository : IArtistRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistRepository" /> class.
        /// </summary>
        /// <param name="roleRepository">The role repository.</param>
        /// <param name="photoRespository">The photo respository.</param>
        /// <param name="personRepository">The person repository.</param>
        public ArtistRepository(IRoleRepository roleRepository, IPhotoRespository photoRespository, IPersonRepository personRepository)
        {
            _roleRepository = roleRepository;
            _photoRespository = photoRespository;
            _personRepository = personRepository;
        }
        private readonly IRoleRepository _roleRepository;
        private readonly IPhotoRespository _photoRespository;
        private readonly IPersonRepository _personRepository;

        /// <summary>
        /// Gets the artist.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="artistId">The artist identifier.</param>
        public Artist GetArtist(IDataSource dataSource, Guid artistId)
        {
            if (!dataSource.IsConnected())
                return null;

            var artistTable = ArtistTable(dataSource);
            if (artistTable == null)
                return null;

            var artistQuery =
                from a in artistTable
                where a.Id == artistId
                select a;

            var artist = artistQuery.SingleOrDefault();

            if (artist == null)
                return null;

            var photoTable = PhotoTable(dataSource);
            var artistPhotoTable = ArtistPhotoTable(dataSource);
            var photosQuery =
                from ap in artistPhotoTable
                join photo in photoTable on ap.PhotoId equals photo.Id
                where ap.ArtistId == artistId
                orderby ap.Priority
                select photo;

            artist.Photos = photosQuery.ToList();

            var artistMemberTable = ArtistMemberTable(dataSource);
            var membersQuery =
                from member in artistMemberTable
                where member.ArtistId == artistId
                orderby member.Priority
                select member;

            artist.Members = membersQuery.ToList();

            if (artist.Members == null) 
                return artist;
            
            var artistMemberRoleTable = ArtistMemberRoleTable(dataSource);
            var personTable = PersonTable(dataSource);
            var roleTable = RoleTable(dataSource);
            foreach (var artistMember in artist.Members)
            {
                var am = artistMember;
                var personQuery =
                    from person in personTable
                    where person.Id == am.PersonId
                    select person;

                artistMember.Person = personQuery.SingleOrDefault();

                var rolesQuery =
                    from amr in artistMemberRoleTable
                    join role in roleTable on amr.RoleId equals role.Id
                    where amr.ArtistMemberId == am.Id
                    orderby role.Name
                    select role;

                artistMember.Roles = rolesQuery.ToList();
            }

            return artist;
        }

        /// <summary>
        /// Finds all artists with the given name.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="artistName">An artist name.</param>
        public List<Artist> FindArtist(IDataSource dataSource, string artistName)
        {
            if (!dataSource.IsConnected())
                return null;

            var artistTable = ArtistTable(dataSource);
            if (artistTable == null)
                return null;

            var artistQuery =
                from a in artistTable
                where a.Name.ToLower() == artistName.ToLower()
                select a;

            var results = artistQuery.ToList();
            return !results.Any() ? null : results;
        }

        /// <summary>
        /// Lists the artists.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        public List<Artist> ListArtists(IDataSource dataSource)
        {
            if (!dataSource.IsConnected())
                return null;

            var artistTable = ArtistTable(dataSource);
            if (artistTable == null)
                return null;

            var photoTable = PhotoTable(dataSource);
            var artistPhotoTable = ArtistPhotoTable(dataSource);
            var artistsQuery =
                from a in artistTable
                from artistPhoto in artistPhotoTable.Where(ap => ap.ArtistId == a.Id && ap.Priority == 1).DefaultIfEmpty()
                from photo in photoTable.Where(p => artistPhoto.PhotoId == p.Id).DefaultIfEmpty()
                orderby a.Name
                select new 
                {
                    Artist = a,
                    ProfileImage = photo
                };

            var results = artistsQuery.ToList();
            var artists = new List<Artist>();
            foreach (var result in results)
            {
                var artist = result.Artist;
                artist.ProfileImage = result.ProfileImage;
                artists.Add(artist);
            }

            return artists;
        }

        /// <summary>
        /// Adds an artist.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="artist">The artist.</param>
        public void AddArtist(IDataSource dataSource, Artist artist)
        {
            if (!dataSource.IsConnected())
                return;

            var artistTable = ArtistTable(dataSource);
            if (artistTable == null)
                return;

            artistTable.InsertOnSubmit(artist);
            dataSource.Save();
        }

        /// <summary>
        /// Adds an artist member.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="member">The artist member.</param>
        public void AddArtistMember(IDataSource dataSource, ArtistMember member)
        {
            if (!dataSource.IsConnected())
                return;

            if (!_personRepository.PersonExists(dataSource, member.PersonId))
                _personRepository.AddPerson(dataSource, member.Person);

            var artistMemberTable = ArtistMemberTable(dataSource);
            if (artistMemberTable == null)
                return;

            artistMemberTable.InsertOnSubmit(member);
            dataSource.Save();
        }

        /// <summary>
        /// Deletes an artist member.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="member">The artist member.</param>
        public void DeleteArtistMember(IDataSource dataSource, ArtistMember member)
        {
            if (!dataSource.IsConnected())
                return;

            var artistMemberTable = ArtistMemberTable(dataSource);
            if (artistMemberTable == null)
                return;

            if (member.Roles != null && member.Roles.Any())
                foreach (var role in member.Roles)
                    DeleteArtistMemberRole(dataSource, member, role);

            artistMemberTable.DeleteOnSubmit(member);
            dataSource.Save();
        }

        /// <summary>
        /// Adds an artist member role.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="member">The member.</param>
        /// <param name="role">The role.</param>
        public void AddArtistMemberRole(IDataSource dataSource, ArtistMember member, Role role)
        {
            if (!dataSource.IsConnected())
                return;

            if (!_roleRepository.RoleExists(dataSource, role.Id))
                _roleRepository.AddRole(dataSource, role);

            var artistMemberRoleTable = ArtistMemberRoleTable(dataSource);
            if (artistMemberRoleTable == null)
                return;

            var artistMemberRole = ArtistMemberRole.Create(member, role);

            artistMemberRoleTable.InsertOnSubmit(artistMemberRole);
            dataSource.Save();
        }

        /// <summary>
        /// Deletes an artist member role.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="member">The member.</param>
        /// <param name="role">The role.</param>
        public void DeleteArtistMemberRole(IDataSource dataSource, ArtistMember member, Role role)
        {
            if (!dataSource.IsConnected())
                return;

            var artistMemberRoleTable = ArtistMemberRoleTable(dataSource);
            if (artistMemberRoleTable == null)
                return;

            var artistMemberRoleQuery =
                from amr in artistMemberRoleTable
                where amr.ArtistMemberId == member.Id && amr.RoleId == role.Id
                select amr;

            var artistMemberRole = artistMemberRoleQuery.SingleOrDefault();
            if (artistMemberRole == null)
                return;

            artistMemberRoleTable.DeleteOnSubmit(artistMemberRole);
            dataSource.Save();
        }

        /// <summary>
        /// Adds an artist photo.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="artist">An artist.</param>
        /// <param name="photo">The photo.</param>
        /// <param name="priority">The priority.</param>
        public void AddArtistPhoto(IDataSource dataSource, Artist artist, Photo photo, int priority)
        {
            if (!dataSource.IsConnected() || photo == null)
                return;

            if (!_photoRespository.PhotoExists(dataSource, photo.Id))
                _photoRespository.AddPhoto(dataSource, photo);

            var artistPhotoTable = ArtistPhotoTable(dataSource);
            if (artistPhotoTable == null)
                return;

            var artistPhoto = ArtistPhoto.Create(artist, photo, priority);
            artistPhotoTable.InsertOnSubmit(artistPhoto);
            dataSource.Save();
        }

        /// <summary>
        /// Deletes the artist photo.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="artist">The artist.</param>
        /// <param name="photo">The photo.</param>
        public void DeleteArtistPhoto(IDataSource dataSource, Artist artist, Photo photo)
        {
            if (!dataSource.IsConnected() || photo == null)
                return;

            var artistPhotoTable = ArtistPhotoTable(dataSource);
            if (artistPhotoTable == null)
                return;

            var artistPhotoQuery =
                from ap in artistPhotoTable
                where ap.ArtistId == artist.Id && ap.PhotoId == photo.Id
                select ap;

            var personPhoto = artistPhotoQuery.SingleOrDefault();
            if (personPhoto == null)
                return;

            artistPhotoTable.DeleteOnSubmit(personPhoto);
            dataSource.Save();
        }

        /// <summary>
        /// Deletes an artist.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="artist">The artist.</param>
        public void DeleteArtist(IDataSource dataSource, Artist artist)
        {
            if (!dataSource.IsConnected())
                return;

            var artistTable = ArtistTable(dataSource);
            if (artistTable == null)
                return;

            var artistReferenceTable = ArtistReferenceTable(dataSource);
            var artistReferencesQuery =
                from ar in artistReferenceTable
                where ar.ArtistId == artist.Id
                select ar;

            artistReferenceTable.DeleteAllOnSubmit(artistReferencesQuery.AsEnumerable());
            dataSource.Save();

            var artistPhotoTable = ArtistPhotoTable(dataSource);
            var artistPhotosQuery =
                from ap in artistPhotoTable
                where ap.ArtistId == artist.Id
                select ap;

            artistPhotoTable.DeleteAllOnSubmit(artistPhotosQuery.AsEnumerable());
            dataSource.Save();

            var artistMemberTable = ArtistMemberTable(dataSource);
            var artistMembersQuery =
                from am in artistMemberTable
                where am.ArtistId == artist.Id
                select am;

            var members = artistMembersQuery.ToList();
            foreach (var member in members)
            {
                var m = member;
                var artistMemberRoleTable = ArtistMemberRoleTable(dataSource);
                var artistMemberRolesQuery =
                    from amr in artistMemberRoleTable
                    where amr.ArtistMemberId == m.Id
                    select amr;

                artistMemberRoleTable.DeleteAllOnSubmit(artistMemberRolesQuery.AsEnumerable());
                dataSource.Save();

                artistMemberTable.DeleteOnSubmit(member);
                dataSource.Save();
            }

            artistTable.DeleteOnSubmit(artist);
            dataSource.Save();
        }

        #region Tables

        private static Table<Artist> ArtistTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<Artist>();
        }

        private static Table<ArtistPhoto> ArtistPhotoTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<ArtistPhoto>();
        }

        private static Table<ArtistReference> ArtistReferenceTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<ArtistReference>();
        }

        private static Table<ArtistMember> ArtistMemberTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<ArtistMember>();
        }

        private static Table<ArtistMemberRole> ArtistMemberRoleTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<ArtistMemberRole>();
        }

        private static Table<Role> RoleTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<Role>();
        }

        private static Table<Person> PersonTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<Person>();
        }

        private static IEnumerable<Photo> PhotoTable(IDataSource dataSource)
        {
            return dataSource?.Context.GetTable<Photo>();
        }

        #endregion
    }
}
