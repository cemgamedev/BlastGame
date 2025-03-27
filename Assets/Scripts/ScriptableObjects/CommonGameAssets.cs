using System.Collections.Generic;
using System.Linq;
using BlastRoot.Extensions;
using BlastRoot.Utility;
using StickBlast;
using UnityEngine;
using StickBlast.Models;

namespace BlastRoot
{
    [CreateAssetMenu(fileName = "CommonGameAssets", menuName = "BlastRoot/CommonGameAssets", order = 0)]
    public class CommonGameAssets : SingletonScriptableObject<CommonGameAssets>
    {
        [SerializeField]
        private List<Item> items;


        public List<Item> GetAllItems()
        {
            return items;
        }

        public Item GetItemByType(ItemTypes type)
        {
            return items.SingleOrDefault(p => p.ItemType == type);
        }

        public Item GetRandomItem()
        {
            return items.GetRandomElement();
        }

        public Item GetRandomItemByType(ItemTypes type)
        {
            return items.Where(p => p.ItemType == type).GetRandomElement();
        }

        public List<Item> GetRandomItems()
        {
            Dictionary<ItemShapes, List<ItemTypes>> types = new Dictionary<ItemShapes, List<ItemTypes>>()
            {
                { ItemShapes.I, new List<ItemTypes>() { ItemTypes.I, ItemTypes.FlatI } },
                { ItemShapes.I2, new List<ItemTypes>() { ItemTypes.I2, ItemTypes.FlatI2 } },
                { ItemShapes.L, new List<ItemTypes>() { ItemTypes.LeftBottomL, ItemTypes.LeftTopL, ItemTypes.RightBottomL, ItemTypes.RightTopL } },
                { ItemShapes.L2, new List<ItemTypes>() { ItemTypes.BottomRightL2, ItemTypes.RightBottomL2, ItemTypes.RightTopL2, ItemTypes.TopRightL2, ItemTypes.BottomLeftL2, ItemTypes.TopLeftL2, ItemTypes.LeftBottomL2, ItemTypes.LeftTopL2 } },
                { ItemShapes.U, new List<ItemTypes>() { ItemTypes.BottomU, ItemTypes.LeftU, ItemTypes.TopU, ItemTypes.RightU } },
            };

            List<Item> items = new List<Item>();

            var probabilities = GameConfigs.Instance.ItemSpawnProbabilities;


            int sum = 0;

            for (int i = 0; i < GameConfigs.Instance.ItemSpawnCount; i++)
            {
                var randomInt = Random.Range(0, 11);
                foreach (var probability in probabilities)
                {
                    sum += probability.Probability;

                    if (sum >= randomInt)
                    {
                        var itemTypes = types[probability.ItemShapes];
                        var rndType = itemTypes.GetRandomElement();
                        items.Add(CommonGameAssets.Instance.GetRandomItemByType(rndType));
                        
                        break;
                    }
                }
            }

            return items.GetRandomElements(GameConfigs.Instance.ItemSpawnCount);
        }
    }
}