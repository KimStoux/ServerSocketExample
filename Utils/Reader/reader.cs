using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLizardFile.Network
{

    public class reader : readerInterface
    {
        private byte[] _data;
        private int totalHeader;
        private int HeaderPseudo;
        private int HeaderRsa;
        public byte[] Data
        {
            get => _data;
            private set => _data = value;
        }
        public reader(byte[] data)
        {
            _data = data;
        }

        public int ReadId()
        {
            return BitConverter.ToInt16(Data, 0);
        }
        public string ReadPseudo()
        {
            var header = BitConverter.ToInt16(Data, 4);
            return Encoding.UTF8.GetString(Data, 8, header);
        }
        public string ReadExtention()
        {
            var header = BitConverter.ToInt16(Data, 4);
            return Encoding.UTF8.GetString(Data, 8, header);
        }
        public string ReadDestinatire()
        {
            int header = BitConverter.ToInt16(Data, 4);
            totalHeader = header + 8;
            HeaderPseudo = BitConverter.ToInt16(Data, totalHeader);
            return Encoding.UTF8.GetString(Data, totalHeader + 4, HeaderPseudo);
        }
        public byte[] ReadBytes()
        {
            var header = totalHeader + HeaderPseudo + 4;
            List<byte> packet = new List<byte>();
            for(int index = header; index < Data.Length; index++)
                packet.Add(Data[index]);
            return packet.ToArray();
        }
        public byte[] NdcRsa()
        {
            HeaderRsa = BitConverter.ToInt16(Data, 4);
            List<byte> packet = new List<byte>();
            for (int index = 8; index < HeaderRsa + 8; index++)
                packet.Add(Data[index]);
            return packet.ToArray();
        }
        public byte[] MdpRsa()
        {
            int headerMdpRsa = BitConverter.ToInt16(Data, HeaderRsa + 8);
            List<byte> packet = new List<byte>();
            for (int index = HeaderRsa + 12; index < HeaderRsa * 2 + 12; index++)
                packet.Add(Data[index]);
            return packet.ToArray();
        }
    }
}
