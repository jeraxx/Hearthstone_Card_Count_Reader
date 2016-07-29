using System;
using System.Windows.Forms;
using Hearthstone_Deck_Tracker.Hearthstone;
using System.Collections.Generic;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using System.Linq;

namespace Hearthstone_Card_Count_Reader
{
    public partial class Form1 : Form
    {
        public int Id { get; set; }
        public IEnumerable<Entity> PlayerEntities => _game.Entities.Values.Where(x => !x.Info.HasOutstandingTagChanges && x.IsControlledBy(Id));
        public IEnumerable<Entity> Hand => PlayerEntities.Where(x => x.IsInHand);
        public IEnumerable<Entity> Board => PlayerEntities.Where(x => x.IsInPlay);
        public IEnumerable<Entity> Deck => PlayerEntities.Where(x => x.IsInDeck);
        public bool HasCoin => Hand.Any(e => e.CardId == HearthDb.CardIds.NonCollectible.Neutral.TheCoin);
        public int HandCount => Hand.Count(x => x.IsControlledBy(Id));
        public int DeckCount => Deck.Count(x => x.IsControlledBy(Id));
        public int BoardCount => Board.Count(x => x.IsControlledBy(Id));
        public Form1()
        {
            InitializeComponent();
        }
        private GameV2 _game;
        private void button1_Click(object sender, EventArgs e)
        {
            var FriendlyHandCount = _game.Player.HandCount;
            var OpponentBoardCount = _game.OpponentMinionCount;
            var FriendlyBoardCount = _game.PlayerMinionCount;
            this.listBox1.Text = FriendlyHandCount + "," + OpponentBoardCount + "," + FriendlyBoardCount;
        }
    }
}
