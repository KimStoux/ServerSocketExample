using ServerLizardFile.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lizardServer.Utils
{
    public class RSA
    {
        RSAParameters privatekey;
        privatekey paraPrivateKey;

        public RSA(RSAParameters pk, privatekey ppk)
        {
            privatekey = pk;
            paraPrivateKey = ppk;
        }

        public RSAParameters getPublicKey()
        {
            CspParameters cspParams = new CspParameters();

            RSAParameters publicKeys;

            using (var rsa = new RSACryptoServiceProvider(cspParams))
            {
                paraPrivateKey.privateKey = rsa.ExportParameters(true);
                publicKeys = rsa.ExportParameters(false);
                rsa.Clear();
            }

            return publicKeys;
        }

        public string Decrypt(byte[] encryptedData, RSAParameters rsaKeyInfo)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(rsaKeyInfo);

                byte[] decryptedData = rsa.Decrypt(encryptedData, true);

                string decryptedValue = Encoding.Default.GetString(decryptedData);

                rsa.Clear();

                return decryptedValue;
            }
        }
    }
}
