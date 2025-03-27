// maebleme2

using System;
using UnityEngine;

namespace StickBlast
{
   public enum ItemShapes
    {
        I,
        I2,
        L,
        L2,
        U
    }
    
    [Serializable]
    public class ItemSpawnProbability
    {

        public ItemShapes ItemShapes;
        
        [Range(0,10)]
        public int Probability;
        
    }
}