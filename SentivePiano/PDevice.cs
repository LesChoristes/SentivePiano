using System;
using System.Collections.Generic;

namespace SentivePiano
{
    public enum PlayHand
    {
        All,
        Left,
        Right,
        Unknown,
        Known,
        NotLeft,
        NotRight
    }

    public class PDevice
    {
        private const int SystemPacketLength = 33;

        //Light key start from 22 
        //61 keys start + 12 = 34
        public void Test()
        {
            Dictionary<int, PlayHand> lights = new Dictionary<int, PlayHand>();
            lights[33] = PlayHand.Left;
            lights[33 + 61 - 1] = PlayHand.Right;

            var bytes = BuildLightsPacket(lights);
            var hex = bytes.PrintInHex();
        }

        public static byte[] BuildQueryPacket()
        {
            byte[] msg = new byte[SystemPacketLength];
            msg[0] = 0xF0;
            msg[1] = 0x7;
            msg[msg.Length - 1] = 0xF7;
            return msg;
        }

        public static byte[] BuildSingleNoteLightPacket(int note, bool on = true)
        {
            //8080f04d4c4e454c1100f7
            byte[] msg = new byte[]
                {0xf0, 0x4d, 0x4c, 0x4e, 0x45, (byte) note, (on ? (byte) 0x11 : (byte) 0x00), 0x00, 0xf7};
            return msg;
        }

        public static byte[] BuildPianoVerifyPacket()
        {
            byte[] msg = new byte[SystemPacketLength];
            msg[0] = 0xF0;
            msg[1] = 0xF;
            Random random = new Random();
            byte identifyByte0 = 0;
            while (identifyByte0 == 0)
            {
                identifyByte0 = (byte) random.Next(127);
            }

            byte identifyByte1 = 0;
            while (identifyByte1 == 0)
            {
                identifyByte1 = (byte) random.Next(127);
            }

            msg[2] = identifyByte0;
            msg[3] = identifyByte1;
            msg[msg.Length - 1] = 0xF7;
            return msg;
        }

        public static byte[] BuildLightsPacket(IDictionary<int, PlayHand> lightsMap)
        {
            byte[] msg = new byte[16];
            msg[0] = 0xF0;
            msg[1] = 0x9;
            int leftOffset = 2;
            int rightOffset = 8;
            foreach (var kv in lightsMap)
            {
                int pos = kv.Key - 20;
                if (kv.Value == PlayHand.Left && leftOffset <= 7) //original is Value == Right, maybe wrong?
                {
                    msg[leftOffset] = (byte) pos;
                    leftOffset++;
                }
                else if (rightOffset <= 13)
                {
                    msg[rightOffset] = (byte) pos;
                    rightOffset++;
                }
            }

            msg[15] = 0xF7;
            return msg;
        }
    }
}