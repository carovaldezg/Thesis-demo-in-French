using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.IO.Ports;

namespace donservidor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Arduino arduino;
        Socket serverSocket;
        // private Socket clientSocket;
        private Socket sending;
        byte[] byteData = new byte[1024];
        IPEndPoint ipEndPoint;
        int puerto;
        IPAddress ip;

        public MainWindow()
        {
            InitializeComponent();
            puerto = 1234;
            ip = IPAddress.Any;
            arduino = new Arduino();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                serverSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);


                ipEndPoint = new IPEndPoint(ip, puerto);

                //Bind and listen on the given address
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(4);

                //Accept the incoming clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR 1. cannot listen in the port");
            }
        }


        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);
                sending = clientSocket;

                //Start listening for more clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                //Once the client connects then start receiving the commands from her
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error 2 in Server. cannot receive data");
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);
                ASCIIEncoding enc = new ASCIIEncoding();
                string data = enc.GetString(byteData);
                MessageBox.Show(data);
                string id = obtenerIdMensaje(data); //Obtains the id of the funcionality executed by a gesture/voice command
                arduino.enviarMensaje(id); //Excecutes a functionality in Arduino.
                textBlock1.Text = data;

                //  byte[] bytes = Encoding.ASCII.GetBytes(data);
                //clientSocket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None,
                //                  new AsyncCallback(OnSend), clientSocket);
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error. problems during reception"); 
            }
        }


        //returns the id of the osc message
        private string obtenerIdMensaje(string data) //Fijarse si el id es de 1 o mas digitos, para eso hay q ver si el char array
                                                    //Empieza en 0 o en 1. Recorrerlo hasta llegar a la coma y despues retornar el substring
        {
            
            char[] id = data.ToCharArray();
            return (id[1]).ToString();

        }

    }
}
