using System;

namespace MazeMaker
{
    public class Unit
    {
        public int instanceNum;
        public Int16 x;//center of the unit
        public Int16 y;
        public Int16 ID;
        public Int16 AddonNydus;
        public Int16 Properties;
        public Int16 mapProperties;
        public Byte PlayerNumber;
        public Byte HP;
        public Byte Shield;
        public Byte Energy;
        public int ResourceAmount;
        public Int16 unitHangar;
        public Int16 unitFlags;
        public int unused;
        public int AddonNydusLink;
        public long offSet;
    }
}
