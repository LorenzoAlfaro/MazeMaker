using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeMaker
{
    public static class StarcraftObj
    {
        public static Location readLocation(FileStream fs)
        {
            Location newLocation = new Location();
            newLocation.offSet = fs.Position;
            byte[] size = new byte[] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int leftX = BitConverter.ToInt32(size, 0);

            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int topY = BitConverter.ToInt32(size, 0);

            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int rightX = BitConverter.ToInt32(size, 0);

            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int bottY = BitConverter.ToInt32(size, 0);

            size = new byte[] { 0, 0 };

            for (int i = 0; i < 2; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            short stringNumber = BitConverter.ToInt16(size, 0);

            for (int i = 0; i < 2; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            short flags = BitConverter.ToInt16(size, 0);
            newLocation.leftX = leftX;
            newLocation.topY = topY;
            newLocation.rightX = rightX;
            newLocation.bottY = bottY;
            newLocation.stringNumber = stringNumber;
            newLocation.flags = flags;
            return newLocation;
        }
        public static void updateLocation(Location loc, FileStream fs)
        {
            fs.Position = loc.offSet;
            byte[] leftX = BitConverter.GetBytes(loc.leftX);
            byte[] topY = BitConverter.GetBytes(loc.topY);
            byte[] rightX = BitConverter.GetBytes(loc.rightX);
            byte[] bottY = BitConverter.GetBytes(loc.bottY);
            byte[] stringNumber = BitConverter.GetBytes(loc.stringNumber);
            byte[] flags = BitConverter.GetBytes(loc.flags);
            byte[] fullArray = leftX.Concat(topY.Concat(rightX.Concat(bottY.Concat(stringNumber.Concat(flags)))))
                .ToArray();
            foreach (byte myByte in fullArray)
            {
                fs.WriteByte(myByte);
            }
        }
    }
}
