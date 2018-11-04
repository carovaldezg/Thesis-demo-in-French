using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Speech.Recognition;
using System.IO;
using System.Windows;

namespace test
{
    class GramaticaDinamica
    {
       private MainWindow ventana;
       private List<string> commandslist;
       private Choices comandos;
       private Choices salir;
       private GrammarBuilder constructorgramatica;
       private GrammarBuilder constructorgramaticasalir;
       private Grammar gramaticacomandos;
       private Grammar gramaticasalir;
       private RecognizerInfo idioma;
       private  StreamReader stream_reader;
       //private StreamReader grammarline;
       private  string line;
       private  string grammarfilepath;

        public GramaticaDinamica(RecognizerInfo ri, MainWindow mwindow)
        {
            commandslist = new List<string>();
            comandos = new Choices();
            salir = new Choices();
            grammarfilepath = null;
            ventana = mwindow;
            idioma = ri;
            //Must load the exit words to close the application
            salir.Add("proche");
            salir.Add("laisser");
            
            try
            {
                stream_reader = new StreamReader("C:\\config.ini");
                grammarfilepath = stream_reader.ReadLine(); //reads the path from frenchgramatic.txt in config.ini
                if (grammarfilepath == null)
                    MessageBox.Show("Please check the config.ini file. It must have a path to a txt grammar file ");
                else
                    this.generarGramatica(grammarfilepath);
                stream_reader.Close(); 
            }
            catch (Exception e)
            {
                MessageBox.Show("CONFIG.INI NOT FOUND " + e.Message, "Error");
                ventana.Close();
                
            }
        }

      
        //Adds a new command (It is not used yet)
        public void agregarComando(string comando)
        {
            if (!commandslist.Contains(comando))
            {
                commandslist.Add(comando);
                if (grammarfilepath != null)
                    this.generarGramatica(grammarfilepath);
            }
        }
        //*********************************************************************************
        /* NO SE PUEDEN BORRAR COMANDOS  
        public void eliminarComando(string comando)
        {
            if (sePuedeBorrar(comando))
            {
                listadecomandos.Remove(comando);
                this.generarGramatica();
            }
        }

      
        private bool sePuedeBorrar(string comando)
        {
            if ((comando == "computadora") && (comando == "salir") && (comando == "cerrar"))
                return false;
            return true;
        }
        //************************************************************************************/
        //FIN METODOS PARA BORRAR COMANDOS (NO SE UTILIZAN)
        //************************************************************************************/



        //***********************************************************************************
        // EL SIGUIENTE METODO GENERA LA GRAMATICA DESDE EL ARCHIVO "GRAMATICA"
        //***********************************************************************************
        //Leo los comandos cargados en el archivo "gramatica" cuya ubicacion esta dentro del archivo "config.ini"
        //Por cada renglon cargo la lista de comandos y la gramatica a utilizar
        public void generarGramatica(String path)
        {
            try
            {
                 //reads "gramatica.txt"
                StreamReader grammarstream = new StreamReader(path);
                line = grammarstream.ReadLine();
                while (line != null)
                {
                    comandos.Add(line); //add the command to the choices 
                    commandslist.Add(line);//add the command to the recognized commands list
                    line = grammarstream.ReadLine();
                }
                grammarstream.Close();
                
                
               
                //el siguiente for es para mostrar los comandos disponibles
                ventana.label5.Content = "Dire: assistant + \n";
                for (int i = 0; i < commandslist.Count(); i++)
                {
                    ventana.label5.Content += commandslist.ElementAt(i) + " \n ";
                }

                constructorgramatica = new GrammarBuilder { Culture = idioma.Culture };
                constructorgramatica.Append("assistant"); //must add the keyword
                constructorgramatica.Append(comandos);//must add the commands
                gramaticacomandos = new Grammar(constructorgramatica); //Generates grammar
                constructorgramaticasalir = new GrammarBuilder { Culture = idioma.Culture };
                constructorgramaticasalir.Append(salir); //must add the exit commands
                gramaticasalir = new Grammar(constructorgramaticasalir); //Generates the exit grammar
            }
            catch (Exception e)
                {
                     MessageBox.Show("NO SE ENCONTRÓ EL ARCHIVO GRAMATICA.TXT - verifique el archivo config.ini", "Error" + e.Message );
                     ventana.Close();
                }
        }
        //***********************************************************************************
        //             FIN GENERAR GRAMATICA DESDE EL ARCHIVO "GRAMATICA"
        //***********************************************************************************


        //***********************************************************************************
        // METODOS PARA DEVOLVER LAS GRAMATICAS "GRAMATICA" Y "SALIR"
        //***********************************************************************************
        public Grammar obtenerGramaticaComandos()
        {
            return gramaticacomandos;
        }

        public Grammar obtenerGramaticaSalir()
        {
            return gramaticasalir;
        }

        public List<string> getCommandsList()
        {
            return commandslist;
        }

        //***********************************************************************************
        //     FIN METODOS PARA DEVOLVER LAS GRAMATICAS "GRAMATICA" Y "SALIR"
        //***********************************************************************************

        public int getId(string pal)
        {
            for (int i = 0; i < commandslist.Count(); i++)
                if (commandslist.ElementAt(i) == pal)
                    return i+1; //retorno +1 por q el nro de id de comando empieza en 1
            return -1;
        }

    }//END NAMESPACE
}//END CLASS
