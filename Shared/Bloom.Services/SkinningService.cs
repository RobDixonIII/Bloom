﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace Bloom.Services
{
    /// <summary>
    /// Service for managing named collections of brushes and icons.
    /// </summary>
    /// <seealso cref="Bloom.Services.ISkinningService" />
    public class SkinningService : ISkinningService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkinningService"/> class.
        /// </summary>
        public SkinningService()
        {
            _skins = new Dictionary<string, string>
            {
                {"rock", "/Bloom.Resources;component/Skins/Rock/Skin.xaml"},
                {"pop", "/Bloom.Resources;component/Skins/Pop/Skin.xaml"},
                {"goth", "/Bloom.Resources;component/Skins/Goth/Skin.xaml"}
            };
        }
        private readonly Dictionary<string, string> _skins;

        /// <summary>
        /// Sets the skin.
        /// </summary>
        /// <param name="skinName">The name of the skin.</param>
        public void SetSkin(string skinName)
        {
            if (!_skins.ContainsKey(skinName.ToLower()))
                return;

            if (!UriParser.IsKnownScheme("pack"))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);

            var skinUri = new Uri(_skins[skinName.ToLower()], UriKind.Relative);
            var skin = new ResourceDictionary { Source = skinUri };

            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(skin);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/Bloom.Resources;component/Resources.xaml", UriKind.RelativeOrAbsolute)
            });
        }
    }
}
