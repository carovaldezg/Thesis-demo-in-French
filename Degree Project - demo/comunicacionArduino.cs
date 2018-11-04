using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO.Ports;


//This class is used only when Arduino runs locally
namespace test
{
    class comunicacionArduino : ComunicacionExterna
    {
        
        SerialPort puertousb;


        public comunicacionArduino()
        {
            
            puertousb = new SerialPort("COM4", 9600); //Please check the Arduino port before running
           
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

        public override void enviarMensaje(MensajeOSC msg)
        {
           //msg = new MensajeOSC ( id,  tipo,  confidence, comando);
           MessageBox.Show("The message is: "+msg.getMensajeFormatoOSC());
           puertousb.Write(/*id.ToString()*/"1");
        }


        public void Stop()
        {
            if (puertousb.IsOpen)
                puertousb.Close();
        }
    
    
    }
}
