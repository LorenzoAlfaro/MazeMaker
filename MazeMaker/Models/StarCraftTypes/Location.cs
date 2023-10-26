using System;

namespace MazeMaker
{
    public class Location
    {
        public int leftX; //u32
        public int topY; //u32
        public int rightX; //u32
        public int bottY;
        public Int16 stringNumber; //u16
        public Int16 flags; //u16
        public long offSet;
        // Location is 20 bytes
        // 4+4+4+4+2+2=20
    }
}
