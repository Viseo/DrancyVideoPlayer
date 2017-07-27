using System.Collections.Generic;
using FluentAssertions;
using MediaPlayer.Managers;
using MediaPlayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class PlanningManagerTests
    {
        private IPlanningManager _manager;

        public PlanningManagerTests()
        {
            _manager = new PlanningManager();
        }

        [TestMethod]
        public void SetOldFlagToPreviousPlaylistItemsTest()
        {
            var oldList = new List<PlaylistItem>()
            {
                new PlaylistItem(){AccessPath = "AccessPath-1", IsDowloaded = true, Id = "id-1", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-2", IsDowloaded = true, Id = "id-2", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-3", IsDowloaded = true, Id = "id-3", IsFromPreviousPlaylist = false}
            };
            var newList = new List<PlaylistItem>()
            {
                new PlaylistItem(){AccessPath = "AccessPath1", IsDowloaded = false, Id = "id1", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath2", IsDowloaded = false, Id = "id2", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath3", IsDowloaded = false, Id = "id3", IsFromPreviousPlaylist = false}
            };
            var expectedList = new List<PlaylistItem>()
            {
                new PlaylistItem(){AccessPath = "AccessPath-1", IsDowloaded = true, Id = "id-1", IsFromPreviousPlaylist = true},
                new PlaylistItem(){AccessPath = "AccessPath-2", IsDowloaded = true, Id = "id-2", IsFromPreviousPlaylist = true},
                new PlaylistItem(){AccessPath = "AccessPath-3", IsDowloaded = true, Id = "id-3", IsFromPreviousPlaylist = true},
                new PlaylistItem(){AccessPath = "AccessPath1", IsDowloaded = false, Id = "id1", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath2", IsDowloaded = false, Id = "id2", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath3", IsDowloaded = false, Id = "id3", IsFromPreviousPlaylist = false}
            };
            _manager.SetOldFlagToPreviousPlaylistItems(oldList, newList).ShouldBeEquivalentTo(expectedList);
        }

        [TestMethod]
        public void SetOldFlagToPreviousPlaylistItemsTest2()
        {
            var oldList = new List<PlaylistItem>()
            {
                new PlaylistItem(){AccessPath = "AccessPath-1", IsDowloaded = true, Id = "id-1", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-2", IsDowloaded = true, Id = "id-2", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-3", IsDowloaded = true, Id = "id-3", IsFromPreviousPlaylist = false}
            };
            var newList = new List<PlaylistItem>()
            {
                new PlaylistItem(){AccessPath = "AccessPath-1", IsDowloaded = true, Id = "id-1", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-2", IsDowloaded = true, Id = "id-2", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-3", IsDowloaded = true, Id = "id-3", IsFromPreviousPlaylist = false}
            };
            var expectedList = new List<PlaylistItem>()
            {
                new PlaylistItem(){AccessPath = "AccessPath-1", IsDowloaded = true, Id = "id-1", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-2", IsDowloaded = true, Id = "id-2", IsFromPreviousPlaylist = false},
                new PlaylistItem(){AccessPath = "AccessPath-3", IsDowloaded = true, Id = "id-3", IsFromPreviousPlaylist = false},
            };
            _manager.SetOldFlagToPreviousPlaylistItems(oldList, newList).ShouldBeEquivalentTo(expectedList);
        }


    }
}
