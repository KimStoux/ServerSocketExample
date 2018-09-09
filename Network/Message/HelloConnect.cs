using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLizardFile.Network
{
    public class HelloConnect : message
    {
        public override short Protocol => 1;
        public int Id { get; set; }
        public string Pseudo { get; set; }
        public override void Deserialize(readerInterface reader)
        {
            Id = reader.ReadId();
            Pseudo = reader.ReadPseudo();
        }
    }
}
