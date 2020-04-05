using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1._0_Archivos {
    public class indiceH {
        public string cajon;
        public long apuntador;
        public Dictionary<string, long> dirDatos;

        public indiceH(string numC)
        {
            cajon = numC;
            apuntador = -1;
            dirDatos = new Dictionary<string, long>();
        }
    }
}
