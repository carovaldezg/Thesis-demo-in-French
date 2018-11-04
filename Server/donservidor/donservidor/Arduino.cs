using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows;

namespace donservidor
{
    class Arduino
    {
        SerialPort puertousb;

        public Arduino()
        {
            puertousb = new SerialPort("COM6", 9600); //Please check the port where you have the Arduino connected

            if (!puertousb.IsOpen)
                try
                {
                    puertousb.Open();
                }
                catch
                {
                    MessageBox.Show("ERROR, Please connect an Arduino to port COM4");
                }
         }

        //The following method sends a message to the Arduino
        public void enviarMensaje(string id)
        {
            puertousb.Write(id);
        }


        public void Stop()
        {
            if (puertousb.IsOpen)
                puertousb.Close();
        }
    }
}
