using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLizardFile.Network
{
    public abstract class message
    {
        public abstract short Protocol { get; }
        public abstract void Deserialize(readerInterface reader);

        public void Unpack(readerInterface reader)
        {
            Deserialize(reader);
        }
    }
}
