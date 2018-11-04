using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Speech.Recognition;


namespace test
{
    class Gramatica
    {
        Choices comandos = new Choices();
        Choices salir = new Choices();
        GrammarBuilder constructorgramatica;
        GrammarBuilder constructorgramaticasalir;
        Grammar gramaticacomandos;
        Grammar gramaticasalir;

        public Gramatica(RecognizerInfo ri)
        {
                   
            comandos.Add("luz");
            comandos.Add("apagar luz");
            comandos.Add("mata a flanders");
            
            salir.Add("cerrar");
            salir.Add("salir");

            constructorgramatica = new GrammarBuilder { Culture = ri.Culture };
            constructorgramatica.Append("computadora");
            constructorgramatica.Append(comandos);
            gramaticacomandos = new Grammar(constructorgramatica);

            constructorgramaticasalir = new GrammarBuilder { Culture = ri.Culture};
            constructorgramaticasalir.Append(salir);
            gramaticasalir = new Grammar(constructorgramaticasalir);
        }

        public Grammar obtenerGramaticaComandos()
        {
            return gramaticacomandos;
        }

        public Grammar obtenerGramaticaSalir()
        {
            return gramaticasalir;
        }

    }
}
