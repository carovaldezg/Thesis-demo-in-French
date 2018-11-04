
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Kinect;
using test;
using GestureDetector.Events;
using GestureDetector.DataSources;




namespace test
{
    public partial class ReconocedorDeGestos
    {
        Device sensor;
        MainWindow ventana;
        EnviarInfoTCPIP servidor;
        MensajeOSC msgosc;
       
        //The object Device is used to work with Kinect.
        public ReconocedorDeGestos(KinectSensor kinect, MainWindow principal)
        {
            sensor = new Device(kinect.UniqueKinectId);
            ventana = principal;
            servidor = new EnviarInfoTCPIP(); //Stablishes the TCP/IP connection
        }

        //this method starts gestures recognition
        public void StartReconocimiento()
        {
            sensor.NewPerson +=sensor_NewPerson;
           //Recognition mode: seated.
          //  bool valor = sensor.Seated;
            sensor.Start();
        
        }

        public ReconocedorDeGestos getReconocedorGestosKinect()
        {
            return this;
        }
        
        //This method gives functionality to the recognized gestures
        private void sensor_NewPerson(object sender, GestureDetector.Events.NewPersonEventArgs e)
        {
            //ONsWIPE= right hand, moving to the right
            e.Person.OnSwipe += delegate(object sender1, GestureEventArgs args)
            {
                if (ventana.luzcocina.Visibility == Visibility.Visible)
                {
                    ventana.luzcocina.Visibility = Visibility.Hidden;
                    msgosc = new MensajeOSC(2, "gesture", 1, "turn off light");//The message sent to Arduino has an OSC protocol format
                    servidor.enviarMensaje(msgosc); //Sends the msg to the server. To execute the Arduino funcionality.
                }
            };
            //OnSwipeRightToLeft = right hand moving to the left
            e.Person.OnSwipeRightToLeft += delegate(object sender1, GestureEventArgs args)
            {
                if (ventana.luzcocina.Visibility == Visibility.Hidden)
                { 
                    ventana.luzcocina.Visibility = Visibility.Visible;
                    msgosc = new MensajeOSC(1, "gesto", 1, "turn on light");//The message sent to Arduino has an OSC protocol format
                    servidor.enviarMensaje(msgosc); //Sends the msg to the server. To execute the Arduino funcionality.
                }
            };

           
        }

        //This stops the sensor
        public void Stop()
        {
            sensor.Stop();
        }

    }
}
