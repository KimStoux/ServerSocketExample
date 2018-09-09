using ServerLizardFile.Utils;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace serveurLizardLinux
{
    class Program
    {
        static void Main(string[] args)
        {
            RSAParameters pk = new RSAParameters();
            privatekey privatekey = new privatekey();
            Console.WriteLine("Server démarrer !");
            lizardServer.Utils.RSA rsa = new lizardServer.Utils.RSA(pk, privatekey);
            LizardFile.Network.socketServer socket = new LizardFile.Network.socketServer();
            var publicKey = rsa.getPublicKey();
            socket.startSocket(publicKey, pk, privatekey);
            Console.Read();
           // Thread startingServ = new Thread() => Program.dopeul(5);
            startingServ.Start();
        }
        
        

        
    }
}
