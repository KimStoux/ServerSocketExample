using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ServerLizardFile.Bdd.Account
{
    [Table("Accounts")]
    public class Record
    {
        public string login { get; set; }
        public string password { get; set; }
        public string Nickename { get; set; }
    }
}
