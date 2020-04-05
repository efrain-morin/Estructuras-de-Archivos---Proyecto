using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoV1._0_Archivos {
    public class indiceS {

        public object clave;
        public long apuntador;
        public List<long> cajones;

        public indiceS(bool tipo)
        {
            apuntador = -1;
            if (tipo)
            {
                clave = Int32.Parse("10000");
            }
            else
            {
                clave = "Z";
            }
            cajones = new List<long>();
            for (int i = 0; i < 131; i++)
            {
                long ini = 100000;
                cajones.Add(ini);
            }
        }

    }
}
