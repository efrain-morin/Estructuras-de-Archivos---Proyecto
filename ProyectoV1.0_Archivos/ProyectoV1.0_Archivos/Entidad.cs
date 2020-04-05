using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ProyectoV1._0_Archivos {

    public class Entidad {

        #region Variables
        // Nombre de la entidad
        private char[] nombre;
        // Dirección donde se encuetra la entidad en el diccionario de datos
        private long dirEnt;
        // Dirección donde se encuentra el primer atributo de la entidad
        private long dirAtri;
        // Dirección donde se encuentra el primer dato
        private long dirDat;
        // Dirección de la siguiente entidad
        private long dirSig;
        // Atributos que pertenecen a la entidad
        private List<Atributo> Atributos;
        // Variable auxiliar para guardar el nombre y ordenar.
        private string nS;
        // Contiene los registros de datos de la entidad.
        private List<Registro> arcDat;
        // Contiene el archivo de datos de la entidad.
        private FileStream archivo;
        // Contiene la ruta del archivo de datos.
        private string rutaA;
        public List<indiceP> indPrimario;
        public List<indiceS> indSecundario;
        public List<indiceH> indHash;
        #endregion

        #region Propiedades
        public char[] nombreEntidad {
            get {
                return nombre;
            }    
            set {
                nombre = value;
            }
        }
        public string Ruta {
            get {
                return rutaA;
            }
            set {
                rutaA = value;
            }
        }
        public FileStream Registro {
            get {
                return archivo;
            }
            set {
                archivo = value;
            }
        }
        public string nombreS {
            get {
                return nS;
            }
            set {
                nS = value;
            }
        }
        public long DirEntidad {
            get {
                return dirEnt;
            }
            set {
                dirEnt = value;
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
        public long DirDato {
            get {
                return dirDat;
            }
            set {
                dirDat = value;
            }
        }
        public long DirSiguienteEnt {
            get {
                return dirSig;
            }
            set {
                dirSig = value;
            }
        }
        public List<Atributo> lAtributos {
            get {
                return Atributos;
            }
            set {
                Atributos = value;
            }
        }
        public List<Registro> Datos {
            get {
                return arcDat;
            }
            set {
                arcDat = value;
            }
        }
        #endregion

        public Entidad (string name, long dEnt, long dAtr, long dDat, long dSig) {
            CambiaNombre(name);
            nS = name;
            dirEnt = dEnt;
            dirAtri = dAtr;
            dirDat = dDat;
            dirSig = dSig;
            Atributos = new List<Atributo>();
            arcDat = new List<Registro>();
            indPrimario = new List<indiceP>();
            indSecundario = new List<indiceS>();
            indHash = new List<indiceH>();
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
