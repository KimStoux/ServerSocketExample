using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLizardFile.Network.Message
{
    public class TransfertFile : message
    {
        public override short Protocol => 2;
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Destinatire { get; set; }
        public byte[] Data { get; set; }
        public override void Deserialize(readerInterface reader)
        {
            Id = reader.ReadId();
            FileName = reader.ReadExtention();
            Destinatire = reader.ReadDestinatire();
            Data = reader.ReadBytes();
        }
    }
}
