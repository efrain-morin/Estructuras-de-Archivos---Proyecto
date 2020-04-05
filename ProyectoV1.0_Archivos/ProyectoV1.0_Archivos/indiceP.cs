using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1._0_Archivos {
    public class indiceP {
        public object clave;
        public long apuntador;

        public indiceP(bool tipo)
        {
            apuntador = -1;
            if (tipo)
            {
                clave = Int32.Parse("10000");
            } else
            {
                clave = "Z";
            }
        }
        
        

    }
}
