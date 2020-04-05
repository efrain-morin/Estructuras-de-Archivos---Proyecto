using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1._0_Archivos {

    public class Atributo {

        #region Variables
        // Nombre del atributo
        private char[] nombre;
        // Dirección donde se encuentra el atributo en el diccionario de datos
        private long dirAtri;
        // Tipo de dato que el atributo contiene
        private char tipo;
        // Longitud máxima que puede tomar el atributo
        private int longitud;
        // Identificador del tipo de índice que tiene el atributo
        private int tipoIndice;
        // Dirección donde se encuentra el índice del atributo
        private long dirIndice;
        // Dirección del siguiente atributo
        private long dirSig;
        // Nombre del atributo en string
        private string nameS;
        #endregion

        #region Propiedades
        public char[] nombreAtributo {
            get {
                return nombre;
            }
            set {
                nombre = value;
            }
        }
        public string nombreSA {
            get {
                return nameS;
            }
            set {
                nameS = value;
            }
        }
        public long DirAtributo {
            get {
                return dirAtri;
            }
            set {
                dirAtri = value;
            }
        }
        public char TipoAtributo {
            get {
                return tipo;
            }
            set {
                tipo = value;
            }
        }
        public int Longitud {
            get {
                return longitud;
            }
            set {
                longitud = value;
            }
        }
        public int TipoIndice {
            get {
                return tipoIndice;
            }
            set {
                tipoIndice = value;
            }
        }
        public long DirIndice {
            get {
                return dirIndice;
            }
            set {
                dirIndice = value;
            }
        }
        public long DirSiguienteAtr {
            get {
                return dirSig;
            }
            set {
                dirSig = value;
            }
        }
        #endregion

        public Atributo (string name, long dAtr, char tAtr, int longi, int tInd, long dInd, long dSig) {
            CambiaNombre(name);
            nameS = name;
            dirAtri = dAtr;
            tipo = tAtr;
            longitud = longi;
            tipoIndice = tInd;
            dirIndice = dInd;
            dirSig = dSig;
        }

        public Atributo() {
            dirIndice = -1;
        }

        /// <summary>
        /// MÉTODO PARA CONVERTIR LA CADENA QUE RECIBE EN ARREGLO DE CARACTERES.
        /// </summary>
        /// <param name="name">Nombre de la entidad a convertir.</param>
        private void CambiaNombre(string name) {
            nombre = new char[30];
            int i = 0;
            foreach (char c in name) {
                nombre[i] = c;
                i++;
            }
        }
    }
}
