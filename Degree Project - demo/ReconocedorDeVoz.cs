using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Speech.Recognition;
using System.Windows;
using Microsoft.Speech.AudioFormat;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;


namespace test
{
    class ReconocedorDeVoz 
    {   
        SpeechRecognitionEngine motordereconocimiento;
        MainWindow ventana;
        KinectSensor kinect;
       // comunicacionArduino arduino; 
        float niveldeconfianza;
        bool audioiniciado;
        string estado;
        string palabraclave;
        ComunicacionExterna servidor;
        GramaticaDinamica gramaticadinamica;
        List<string> commandslist;
        MensajeOSC msgosc;
        int idcomando;
       

        public ReconocedorDeVoz(KinectSensor sensor, MainWindow visual) 
        {
            
            kinect = sensor;
            ventana = visual; //In the future the UI will be in the Server side
            servidor = new EnviarInfoTCPIP(); //Stablishes the TCP/IP connection
            commandslist = new List<string>();
            palabraclave = "assistant"; //The keyword to avoid executing false possitives 
            this.audioiniciado = false;
        }


        //The following method returns the language pack installed in the computer. Please check you have french-France version
        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "fr-FR".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }




       //********************************************************
        // This method starts kinect voice recognition
        //*******************************************************
        public void StartReconocimiento()
        {
            motordereconocimiento = this.obtenerMotorDeReconocimiento();
       
        }

        //*************************************************************************************
        // The following methods start kinect voice recognition and loads the words dictionary
        //************************************************************************************
        public SpeechRecognitionEngine obtenerMotorDeReconocimiento()
        {
            //obtains the installed language pack
            RecognizerInfo idioma = GetKinectRecognizer();
            if (idioma == null)
            {
                ventana.label2.Content = "Language pack not found :/";
                return null;
            }
            else 
            {
                SpeechRecognitionEngine reconocedoraux;
                try
                {
                    reconocedoraux = new SpeechRecognitionEngine(idioma.Id);
                    
                }
                catch
                {
                    MessageBox.Show(@"Problem found. Please install Microsoft Speech SDK.","Microsoft Speech Error");
                    ventana.Close();
                    return null;
                }
                try
                {
                    //I use a dynamic grammar to load the words to be recognized from a txt file.
                    gramaticadinamica = new GramaticaDinamica(idioma, ventana);
                    //This obtains the commands (words) saved in the txt file
                    Grammar comandosgrammar = gramaticadinamica.obtenerGramaticaComandos();
                    //I have 2 different grammars, the words grammar and the exit comands grammar
                    reconocedoraux.LoadGrammar(gramaticadinamica.obtenerGramaticaComandos());
                    reconocedoraux.LoadGrammar(gramaticadinamica.obtenerGramaticaSalir());
                    //The folowing lines belongs to voice recognition
                    reconocedoraux.SpeechRecognized += this.SpeechRecognized;
                    reconocedoraux.SpeechRecognitionRejected += this.SpeechRejected;

                    audioiniciado = true;
                    KinectAudioSource fuentedeaudio = this.kinect.AudioSource;
                    fuentedeaudio.BeamAngleMode = BeamAngleMode.Adaptive;
                    Stream audiostream = fuentedeaudio.Start();
                    reconocedoraux.SetInputToAudioStream(audiostream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                    reconocedoraux.RecognizeAsync(RecognizeMode.Multiple);
                    return reconocedoraux;
                }
                catch (Exception e)
                {
                    ventana.Close();
                    MessageBox.Show("Error. The grammar could not be loaded " +e.Message);
                    return null;
                }

            }//end else
        }//end funcion obtenerReconocedor



        //**************************************************************************
        //  This method matches the voice commands with the words of the txt
        //**************************************************************************
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        { 
           // int pos;
            string comandoreconocido = null;
            commandslist = gramaticadinamica.getCommandsList();
          // Speech utterance confidence below which we treat speech as if it hadn't been heard
            niveldeconfianza = e.Result.Confidence;
            if (e.Result.Confidence >= 0.7)  //confidence level: 0 to 1. For more accuracy results please change tge 0.7 for 0.9 or 0.95.
            {//If the voice command is "salir" (exit) or "cerrar" (close) The application will close itself
                if ((e.Result.Words[0].Text == "proche") || (e.Result.Words[0].Text == "laisser"))
                    ventana.Close();
                else
                    //If the first word pronounced is "assistant" (keyword to avoid false possitives)
                    if (e.Result.Words[0].Text == palabraclave) //Palabra clave = "assistant" you can change it in the variables section at the beginning of this class.
                    {
                        comandoreconocido = getRecognizedCommand(e.Result.Words); //Obtains all the words recognized
                        ventana.label6.Content = comandoreconocido;
                        if (hayMatching(e.Result.Words, commandslist))//if the word detected is a command from the txt file/ grammar
                        {   
                            int pos =  gramaticadinamica.getId(comandoreconocido);//This is to obtain the id (line) of the command recognized. It is used as a parameter to execute an Arduino function
                            MessageBox.Show("You have said "+ comandoreconocido);
                            msgosc = new MensajeOSC(pos, "voix", e.Result.Confidence, comandoreconocido);//The message sent to Arduino has an OSC protocol format
                            servidor.enviarMensaje(msgosc); //Sends the msg to the server. To execute the Arduino funcionality.
                        }
                        else
                        { //If the command is not recognized but the keyward was, the audiostream is rejected
                            estado = "The command could not be recognized";
                            RejectSpeech(estado);
                        }
                    }
                    else
                    { //If the word was not "assistant" all the recognized stream is rejected
                        estado = "Plsease, remember using the keyword: " + palabraclave;
                        RejectSpeech(estado);
                    }
                
            }
            else
            { //If the confidence detected is too low, all the audio stream is rejected
                estado = "Rejected " + e.Result.Text + " The conficende level was too low " + e.Result.Confidence;
                RejectSpeech(estado);
            }
         }

        //The following function chekcs if there is matching between the words from the dictionary (txt file) and the 
        //stream
        public bool hayMatching(ReadOnlyCollection<RecognizedWordUnit> frase, List<string> listadecomandos)
        {
            
            string []diccionario;
            for (int i = 0; i < listadecomandos.Count; i++)
            {
                diccionario = listadecomandos.ElementAt(i).Split(" ".ToCharArray());
                if (frase.Count - 1 == diccionario.Count()) //The first word is "assistant"
                {

                    if (sonIguales(frase, diccionario))
                        return true;
                }
            }
            return false;
            
        }

        //The following function checks if 2 words are =
        public bool sonIguales(ReadOnlyCollection<RecognizedWordUnit> frase, string[] diccionario)
        {//The fist word must not be considered, it is the keyword "assistant" 
           int i=0;
            for (i= 0; i < diccionario.Count(); i++)
            {
                if (frase[i+1].Text != diccionario.ElementAt(i))
                    return false;
            }
            idcomando = i;
            return true;
        }


        //returns the recognized command without the keyword
        public string getRecognizedCommand(ReadOnlyCollection<RecognizedWordUnit> words)
        {
            string comandoreconocido = null;
            for (int i = 1; i < words.Count; i++)
            {
                comandoreconocido +=words.ElementAt(i).Text+" ";
            }
            return comandoreconocido.Remove(comandoreconocido.Length-1);
        }

        //returns command's id 
        public int getCommandId(List<string> list, string word)
        {
            for (int i = 0; i < list.Count(); i++)
                if (word == list.ElementAt(i))
                    return i+1;
            return -1;

        }

        //Rejected command
        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            estado = "Rejected command, confidence level: "+ e.Result.Confidence;
            this.RejectSpeech(estado);
        }


        private void RejectSpeech(string estado)
        { 
         ventana.label2.Content = estado;
        }

        //This method stops the voice recognition
        public void Stop()
        {
            if (audioiniciado)
            {
                this.kinect.AudioSource.Stop();
               // this.arduino.Stop();
            }
               if (this.motordereconocimiento != null)
               {
                this.motordereconocimiento.SpeechRecognized -= SpeechRecognized;
                this.motordereconocimiento.SpeechRecognitionRejected -= SpeechRejected;
                this.motordereconocimiento.RecognizeAsyncStop();
               }
        }

    } //END NAMESPACE
    
}//END CLASS
