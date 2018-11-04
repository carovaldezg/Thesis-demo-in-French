//*****************************************************************************
//                      Carolina Valdez Gándara
//                        DEGREE PROJECT DEMO
//*****************************************************************************


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
using Microsoft.Kinect;
using System.Threading;
using GestureDetector;





namespace test
{

    
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EnviarInfoTCPIP cliente = new EnviarInfoTCPIP();
        ReconocedorDeVoz reconocedorvoz;
        public static ReconocedorDeGestos reconocedorgestos;
        KinectSensor kinect;

        public MainWindow()
        {
           
            InitializeComponent();
           
        }

        //*************************************************************************
        //      EVENTOS AL INICIAR Y CERRAR LA VENTANA DEL PROGRAMA
        //*************************************************************************
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            inicializarKinect();
            if (this.kinect != null)
            {
                if (kinect.ElevationAngle != 0)
                    kinect.ElevationAngle = 0;
                //POR UN BUG DE KIINECT FOR WINDOWS, EL ORDEN DE LLAMADOS TIENE Q SER GESTOS, LUEGO AUDIO, SINO SE CORTA EL STREAMING DE AUDIO
                reconocedorgestos = new ReconocedorDeGestos(kinect, this);
                reconocedorgestos.StartReconocimiento();


                habilitarStreamingDeVideo();
                reconocedorvoz = new ReconocedorDeVoz(kinect, this);
                reconocedorvoz.StartReconocimiento();             
            }
  }

       


        public void finalizarSensor()
        {

            reconocedorvoz.Stop();
            reconocedorgestos.Stop();
            image2.Source = null; 
            this.kinect.Stop();
            this.kinect = null;
        }

       
        public void Window_Closed(object sender, EventArgs e)
        {
            if (kinect != null)
            {
                finalizarSensor();
            }
        }

        //*************************************************************************
        //     FIN EVENTOS AL INICIAR Y CERRAR LA VENTANA DEL PROGRAMA
        //*************************************************************************


        //*************************************************************************
        //                  INICIALIZACIÓN DEL DISPOSITIVO KINECT
        //*************************************************************************
        //Inicializo el sensor, PRIMERO se habilita la salida, despues se le da start.
        public void inicializarKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                 if (potentialSensor.Status == KinectStatus.Connected)
                 {
                    this.kinect = potentialSensor;
                    break;
                 }
            }
            if (this.kinect != null)
            {
                label1.Content = "A Kinect sensor has been found";
               // habilitarStreamingDeVideo();
                this.kinect.Start();
            }
            else
                label1.Content = "Please connect a Kinect sensor";
        }


        private void habilitarStreamingDeVideo()
        {
            
            {
                this.kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                this.kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this.kinect.SkeletonStream.Enable();
                kinect.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinect_ColorFrameReady);
            }
        }


        void kinect_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null) 
                    return;
                byte[] colorData = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(colorData);
                image2.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, // image dimensions
                                                    96, 96, // resolution - 96 dpi for video frames
                                                    PixelFormats.Bgr32, // video format
                                                    null, // palette - none
                                                    colorData, // video data
                                                    colorFrame.Width * colorFrame.BytesPerPixel // stride
                                                    );
            }
        }

      

        //*************************************************************************
        //      FIN INICIALIZACIÓN DEL DISPOSITIVO KINECT
        //*************************************************************************

    }//end namespace
}//END CLASS
