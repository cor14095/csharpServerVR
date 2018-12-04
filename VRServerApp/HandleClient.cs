using System;
using System.Text;
using System.CodeDom;
using Microsoft.CSharp;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using System.CodeDom.Compiler;

namespace VRServerApp
{
    class HandleClient
    {
        Socket clientSocket;
        string clNo;
        public void StartClient(Socket inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(Compile);
            ctThread.Start();
        }

        private void Compile()
        {
            byte[] msg = new byte[1024];
            string data = "";
            string response;
            Byte[] sendBytes;

            byte[] msg2 = new byte[1024];
            string data2 = "";
            string response2;
            Byte[] sendBytes2;

            try
            {
                int k = clientSocket.Receive(msg);

                data = System.Text.Encoding.ASCII.GetString(msg);
                data = data.Split('$')[1];
                Console.WriteLine("Receiving Data");

                response = "515 confirmed task -- calculate: " + data;
                Console.WriteLine(response);
                sendBytes = Encoding.ASCII.GetBytes(response);
                // Tell the client that we have started.
                clientSocket.Send(sendBytes);

                HandleCompile compiler = new HandleCompile(data);
                var method = compiler.compileCode() as MethodInfo;

                Console.WriteLine("Evaluation start...");
                while (data2 != "quit")
                {
                    // Console.WriteLine("________________________");
                    int k2 = clientSocket.Receive(msg2);

                    data2 = System.Text.Encoding.ASCII.GetString(msg2);
                    int index = data2.IndexOf("~");
                    if (index > 0)
                        //Console.WriteLine(data2);
                        data2 = data2.Substring(0, index);
                    //Console.WriteLine(data2);
                    if (data2.IndexOf("quit") > 0)
                    {
                        Console.WriteLine("done");
                        break;
                    }

                    //Console.WriteLine(data2);
                    string data3 = data2.Split('$')[1];

                    var ps = data3.Split('@');

                    var px = float.Parse(ps[0],
                        System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    var pz = float.Parse(ps[1],
                        System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                    var py = float.Parse(ps[2],
                        System.Globalization.CultureInfo.InvariantCulture.NumberFormat);

                    var p = (double)method.Invoke(compiler.instance, new object[] { px, pz, py });

                    response2 = p.ToString();
                    sendBytes2 = Encoding.ASCII.GetBytes(response2);
                    clientSocket.Send(sendBytes2);
                    Console.WriteLine(" - Responding: " + response2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("End Of evaluation.");
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
