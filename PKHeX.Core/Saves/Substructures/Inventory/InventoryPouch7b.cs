using System;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Inventory Pouch used by <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed class InventoryPouch7b : InventoryPouch
    {
        public bool SetNew { get; set; }
        private InventoryItem7b[] OriginalItems = Array.Empty<InventoryItem7b>();

        public override InventoryItem GetEmpty(int itemID = 0, int count = 0) => new InventoryItem7b { Index = itemID, Count = count };

        public InventoryPouch7b(InventoryType type, ushort[] legal, int maxCount, int offset, int size)
            : base(type, legal, maxCount, offset, size) { }

        public override void GetPouch(byte[] data)
        {
            var items = new InventoryItem7b[PouchDataSize];
            for (int i = 0; i < items.Length; i++)
            {
                uint val = BitConverter.ToUInt32(data, Offset + (i * 4));
                items[i] = InventoryItem7b.GetValue(val);
            }
            Items = items;
            OriginalItems = items.Select(i => i.Clone()).ToArray();
        }

        public override void SetPouch(byte[] data)
        {
            if (Items.Length != PouchDataSize)
                throw new ArgumentException("Item array length does not match original pouch size.");

            var items = (InventoryItem7b[])Items;
            for (int i = 0; i < items.Length; i++)
            {
                uint val = items[i].GetValue(SetNew, OriginalItems);
                BitConverter.GetBytes(val).CopyTo(data, Offset + (i * 4));
            }
        }

        /// <summary>
        /// Checks pouch contents for bad count values.
        /// </summary>
        /// <remarks>
        /// Certain pouches contain a mix of count-limited items and uncapped regular items.
        /// </remarks>
        internal void SanitizeCounts()
        {
            foreach (var item in Items)
                item.Count = GetSuggestedCount(Type, item.Index, item.Count);
        }

        public static int GetSuggestedCount(InventoryType t, int item, int requestVal)
        {
            switch (t)
            {
                // mixed regular battle items & mega stones
                case InventoryType.BattleItems when item > 100:
                // mixed regular items & key items
                case InventoryType.Items when Legal.Pouch_Regular_GG_Key.Contains((ushort)item):
                    return Math.Min(1, requestVal);

                default:
                    return requestVal;
            }
        }
    }
}
