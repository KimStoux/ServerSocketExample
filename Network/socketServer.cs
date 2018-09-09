using ServerLizardFile.Bdd;
using ServerLizardFile.Bdd.Account;
using ServerLizardFile.Network;
using ServerLizardFile.Network.Message;
using ServerLizardFile.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LizardFile.Network
{

    public class socketServer
    {
        private static List<Account> accounts = Navicat.Query<Account>("SELECT * FROM accounts;").ToList();
        public static string time = DateTime.Now.ToString("HH:MM");
        public const string ip = "127.0.0.1";
        private static List<EndPoint> endpoint = new List<EndPoint>();
        private static List<Tuple<EndPoint, string>> pseudoEnd = new List<Tuple<EndPoint, string>>();
        private static RSAParameters key;
        static RSAParameters privatekey;
        static privatekey privatekeyPara;


        public class StateObject
        {
            // Client  socket.  
            public Socket workSocket = null;
            // Size of receive buffer.  
            public const int BufferSize = 12000000;
            // Receive buffer.  
            public byte[] buffer = new byte[BufferSize];
        }


        public void startSocket(RSAParameters publicKey, RSAParameters pk, privatekey pkp) // le server
        {
            privatekey = pk;
            key = publicKey;
            privatekeyPara = pkp;
          //  IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
            //IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), 1088);

            Socket listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Serveur démarrer sur {0}:{1}", ip, 1088);

            listener.Bind(localEndPoint);
            listener.Listen(32);
            startAccept(listener);
            
        }

        public static void startAccept(Socket listener)
        {
            listener.BeginAccept(AccepteCallBack, listener);
        }


        public static void AccepteCallBack(IAsyncResult result)
        {
            Socket listener = (Socket)result.AsyncState;
            //var remote = listener.RemoteEndPoint.ToString();
            Socket handler = listener.EndAccept(result);
            startAccept(listener);
            //form.logger.Invoke(new MethodInvoker(delegate () { form.logger.AppendText("\n[" + time + "]" + "Nouvelle connexion : " + handler.RemoteEndPoint); }));
            Console.WriteLine("\n[" + time + "]" + "Nouvelle connexion : " + handler.RemoteEndPoint);
            endpoint.Add(handler.RemoteEndPoint);
            StateObject state = new StateObject();
            var data = state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult result)
        {
            StateObject state = (StateObject)result.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(result, out SocketError errorCode);
            string nbrDonne = bytesRead.ToString();
            //var data = readHeader(state.buffer, handler.RemoteEndPoint);
            //var pseudoS = pseudoSend(state.buffer);
            //form.logger.Invoke(new MethodInvoker(delegate () { form.logger.AppendText("\n[" + time + "]" + "Donnée envoyer par :" + handler.RemoteEndPoint + "Nombre de donnée :" + nbrDonne + "(" + pseudoS + ")"); }));
            reader reader = new reader(state.buffer);
            receive(reader, handler.RemoteEndPoint, handler, state);

            state.buffer = new byte[12000000];

           if(handler.Connected == true)
           {
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
           }
        }
        public static void receive(readerInterface Data, EndPoint endpoint, Socket handler, StateObject state)
        {
            var Id = Data.ReadId();
            if(Id == 1)
            {
                var pseudo = Data.ReadPseudo();
                HelloConnect helloConnect = new HelloConnect();
                helloConnect.Deserialize(Data);
                if(pseudoEnd.Count == 0)
                {
                    byte[] packet = key.Exponent;
                    byte[] packetFinal = packet.Concat(key.Modulus).ToArray();
                    send(handler, packetFinal, endpoint);
                    Console.WriteLine("\n[" + time + "]" + "Envoye de l'RSA key a :" + handler.RemoteEndPoint);
                }
                else
                {
                    foreach (var end in pseudoEnd)
                    {
                        if (end.Item1 == endpoint) { }
                        else
                        {
                            byte[] packet = key.Exponent;
                            byte[] packetFinal = packet.Concat(key.Modulus).ToArray();
                            send(handler, packetFinal, helloConnect.Pseudo);
                            Console.WriteLine("\n[" + time + "]" + "Envoye de l'RSA key a :" + handler.RemoteEndPoint);
                        }
                    }
                }
            }
            else if(Id == 2)
            {
                var pseudo = Data.ReadPseudo();
                TransfertFile transfertFile = new TransfertFile();
                transfertFile.Deserialize(Data);
                byte[] headerFile = BitConverter.GetBytes(transfertFile.FileName.Length);
                byte[] packet = Encoding.UTF8.GetBytes(transfertFile.FileName);
                byte[] packete = headerFile.Concat(packet).ToArray();
                byte[] packeteFinal = packete.Concat(transfertFile.Data).ToArray();
                string destination = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                using (var fs = new FileStream(String.Format(destination + @"\{0}", transfertFile.FileName), FileMode.Create, FileAccess.Write)) //Write Fichier
                {
                    fs.Write(transfertFile.Data, 0, transfertFile.Data.Length);
                }
                send(handler, packeteFinal, transfertFile.Destinatire);
               // Console.WriteLine("\n[" + time + "]" + "Donnée envoyer par :" + handler.RemoteEndPoint + "Nombre de donnée :" + nbrDonne + "(pseudoS)");
            }
            else if(Id == 3)
            {
                lizardServer.Utils.RSA rsa = new lizardServer.Utils.RSA(privatekey,privatekeyPara);
                privatekey = privatekeyPara.privateKey;
                var ndc = Data.NdcRsa();
                var mdp = Data.MdpRsa();
                var ndcDecryp = rsa.Decrypt(ndc, privatekey);
                var mdpDecryp = rsa.Decrypt(mdp, privatekey); 
                bool connection = false;
                foreach (var test in accounts)
                {
                    if(test.login == ndcDecryp && test.password == mdpDecryp)
                    {
                        var pseudo = accounts.Where(x => x.login == ndcDecryp).ToList();
                        if(test.login == ndcDecryp)
                        {
                            pseudoEnd.Add(new Tuple<EndPoint, string>(endpoint, test.Nickename));
                        }
                        connection = true;
                        var auth = BitConverter.GetBytes(connection);
                        byte[] packet = BitConverter.GetBytes(3);
                        byte[] packetFinal = packet.Concat(auth).ToArray();
                        send(handler, packetFinal, handler.RemoteEndPoint);
                        Console.WriteLine("\n[" + time + "]" + "Identification : " + handler.RemoteEndPoint + " ndc : " + ndcDecryp + " mdp : " + mdpDecryp + " Connexion accepter");
                    }
                    else
                    {
                        var auth = BitConverter.GetBytes(connection);
                        byte[] packet = BitConverter.GetBytes(3);
                        byte[] packetFinal = packet.Concat(auth).ToArray();
                        send(handler, packetFinal, handler.RemoteEndPoint);
                        Console.WriteLine("\n[" + time + "]" + "Identification : " + handler.RemoteEndPoint + "ndc : " + ndcDecryp + "mdp : " + mdpDecryp + " Connexion refusée");
                    }
                }
               
            }
            else
            {
                Console.WriteLine("\n[" + time + "]" + "Deconnection de  : " + handler.RemoteEndPoint);
                foreach (var deco in pseudoEnd)
                {
                    if(endpoint == deco.Item1)
                    {
                        pseudoEnd.Remove(deco);
                        handler.Dispose();
                        handler.Close();
                        break;
                    }
                }
            }
        }

        public static void send(Socket handler, byte[] data, string pseudo)
        {
            foreach(var endPoint in pseudoEnd)
            {
                if(endPoint.Item2 == pseudo)
                {
                    handler.SendTo(data, endPoint.Item1);
                }
            }
        }
        public static void send(Socket handler, byte[] data, EndPoint point)
        {
            if (point != null)
            {
                handler.SendTo(data, point);
            }
        }

        public static void sendCallBack(IAsyncResult result)
        {
            Socket handler = (Socket)result.AsyncState;

            handler.EndSendTo(result);
        }
    }
}
