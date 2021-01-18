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
            newLocation.offSet  = fs.Position;                                                
            newLocation.leftX           = BO.readInt32(fs);
            newLocation.topY            = BO.readInt32(fs);
            newLocation.rightX          = BO.readInt32(fs);
            newLocation.bottY           = BO.readInt32(fs);
            newLocation.stringNumber    = BO.readInt16(fs);
            newLocation.flags           = BO.readInt16(fs);
            return newLocation;
        }
        public static Unit readUnit(FileStream fs)
        {
            Unit newUnit = new Unit();
            newUnit.offSet = fs.Position;
            newUnit.instanceNum = BO.readInt32(fs);
            newUnit.x = BO.readInt16(fs);
            newUnit.y = BO.readInt16(fs);
            newUnit.ID = BO.readInt16(fs);
            newUnit.AddonNydus = BO.readInt16(fs);
            newUnit.Properties = BO.readInt16(fs);
            newUnit.mapProperties = BO.readInt16(fs);
            newUnit.PlayerNumber = Convert.ToByte(fs.ReadByte());
            newUnit.HP = Convert.ToByte(fs.ReadByte());
            newUnit.Shield = Convert.ToByte(fs.ReadByte());
            newUnit.Energy = Convert.ToByte(fs.ReadByte());
            newUnit.ResourceAmount = BO.readInt32(fs);
            newUnit.unitHangar = BO.readInt16(fs);
            newUnit.unitFlags = BO.readInt16(fs);
            newUnit.unused = BO.readInt32(fs);
            newUnit.AddonNydusLink = BO.readInt32(fs);            
            return newUnit;
        }
        public static void updateLocation(Location loc, FileStream fs)
        {
            fs.Position = loc.offSet;
            byte[] leftX        = BitConverter.GetBytes(loc.leftX);
            byte[] topY         = BitConverter.GetBytes(loc.topY);
            byte[] rightX       = BitConverter.GetBytes(loc.rightX);
            byte[] bottY        = BitConverter.GetBytes(loc.bottY);
            byte[] stringNumber = BitConverter.GetBytes(loc.stringNumber);
            byte[] flags        = BitConverter.GetBytes(loc.flags);
            byte[] fullArray = leftX.Concat(topY.Concat(rightX.Concat(bottY.Concat(stringNumber.Concat(flags)))))
                .ToArray();
            foreach (byte myByte in fullArray)
            {
                fs.WriteByte(myByte);
            }
        }

        public static void updateUnit(Unit u, FileStream fs)
        {
            fs.Position = u.offSet;

            byte[] array =
                A(u.instanceNum)
                .Concat(A(u.x))
                .Concat(A(u.y))
                .Concat(A(u.ID))
                .Concat(A(u.AddonNydus))
                .Concat(A(u.Properties))
                .Concat(A(u.mapProperties))
                .Concat(new byte[] { u.PlayerNumber })
                .Concat(new byte[] { u.HP })
                .Concat(new byte[] { u.Shield })
                .Concat(new byte[] { u.Energy })
                .Concat(A(u.ResourceAmount))
                .Concat(A(u.unitHangar))
                .Concat(A(u.unitFlags))
                .Concat(A(u.unused))
                .Concat(A(u.AddonNydusLink))
                .ToArray();

            foreach (byte myByte in array)
            {
                fs.WriteByte(myByte);
            }
        }

        public static byte[] A(dynamic i)
        {
            return BitConverter.GetBytes(i);
        }
    }
}

