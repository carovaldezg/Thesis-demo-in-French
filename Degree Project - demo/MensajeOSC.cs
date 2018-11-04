using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace test
{
    class MensajeOSC
    {
        private string mensajeProtocoloOSC;
        string tipo32bits;
        string comando32bits;

        //Generates an OSC format message
        public MensajeOSC(Int32 id, string tipo, float confidence, string comando)
        {

            tipo32bits = formatStringTo32bits(tipo);
            comando32bits = formatStringTo32bits(comando);
            mensajeProtocoloOSC = "/" + id + "," + tipo32bits + "," + confidence + "," + comando32bits;

        }

        //The following method returns a string if its length is mod32. 
        //If it is 32, 64, 128, etc it returns the same string. If it is not, it returns the string followed by as much 0 as it needs.
        private string formatStringTo32bits(string palabra)
        {
            int agregarceros;
            int resto;
            string palabrade32bits = null;

            if (palabra.Length % 32 == 0)
                return palabra;
            else
            {
                resto = palabra.Length % 32;
                agregarceros = 32 - resto;
                palabrade32bits += new String('0', agregarceros);
                return palabra+palabrade32bits;
            }
        }

        //Returns the string of the message
        public string getMensajeFormatoOSC()
        {
            return mensajeProtocoloOSC;
        }

    }
}
