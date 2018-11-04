using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Net;
using System.Net.Sockets;

//This class connects a client with a server

namespace test
{
    class EnviarInfoTCPIP : ComunicacionExterna
    {
        int puerto;
        IPAddress ip;
        public Socket clientSocket;
        private byte[] byteData = new byte[1024];

        public EnviarInfoTCPIP()
        {
            puerto = 1234;
            ip = IPAddress.Parse("127.0.0.1");
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ip, puerto);

                //Connect to the server
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
                byteData = new byte[1024];
                //Start listening to the data asynchronously
                clientSocket.BeginReceive(byteData,
                                           0,
                                           byteData.Length,
                                           SocketFlags.None,
                                           new AsyncCallback(OnReceive),
                                           null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CLIENTE: error 1 SGSclient");
            }
            
        }


        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndReceive(ar);

                ASCIIEncoding enc = new ASCIIEncoding();
                string data = enc.GetString(byteData);
               // textBox1.Text = textBox1.Text + Environment.NewLine + data;
                //Accordingly process the message received

                clientSocket.BeginReceive(byteData,
                                          0,
                                          byteData.Length,
                                          SocketFlags.None,
                                          new AsyncCallback(OnReceive),
                                          null);

            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error 2 en clientTCP: ");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR 3 en cliente");
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndSend(ar);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error 4 en Client ");
            }
        }

        public override void enviarMensaje(MensajeOSC msgosc)
        {
            try
            {
                byte[] bytes = null;
                bytes = Encoding.ASCII.GetBytes(msgosc.getMensajeFormatoOSC());
             //   MessageBox.Show(msgosc.getMensajeFormatoOSC()+ "tamaño del buffer: "+ bytes.Length);
                //Send it to the server
                clientSocket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "ERROR 5 EN Client ");
            } 
      
        }
    }
}
