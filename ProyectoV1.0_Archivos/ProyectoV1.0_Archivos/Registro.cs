using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1._0_Archivos {

    public class Registro {

        #region Variables
        private List<Object> registros;
        private long dirS;
        private long dirRe;
        #endregion

        #region Propiedades
        public List<Object> Registros {
            get {
                return registros;
            }
            set {
                registros = value;
            }
        }
        public long DirSiguienteReg {
            get {
                return dirS;
            }
            set {
                dirS = value;
            }
        }
        public long DirRegistro {
            get {
                return dirRe;
            }
            set {
                dirRe = value;
            }
        }
        #endregion

        public Registro() {
            registros = new List<Object>();
        }
    }
}
