using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLizardFile.Network
{
    public interface readerInterface
    {
        byte[] Data { get; }
        int ReadId();
        string ReadPseudo();
        string ReadExtention();
        string ReadDestinatire();
        byte[] ReadBytes();
        byte[] NdcRsa();
        byte[] MdpRsa();
    }
}
