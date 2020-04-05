using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ProyectoV1._0_Archivos {

    public partial class Form1 : Form {

        #region Variables
        // Contiene el apuntador a la dirección de la primera entidad.
        private long cabecera;
        // Contiene la colección de entidades del diccionario de datos.
        private List<Entidad> Entidades;
        // Contiene el nombre del archivo que estamos editando.
        private string nameFile;
        // Contiene nuestro archivo actual en edición.
        private FileStream archivo;
        // Bandera de control para saber si hemos creado un archivo.
        private bool creado;
        // Contiene la entidad seleccionada para agregar atributos.
        private char[] entSel;
        // Contiene el atributo seleccionado para modificar.
        private char[] atrSel;
        // Contiene la entidad seleccionada.
        private string entS;
        // Contiene la entidad seleccionada.
        private string entS2;
        // Contiene el atributo seleccionado.
        private string entA;
        // Contiene el índice del atributo que queremos capturar el dato.
        private int atrSele;
        // Contiene los datos de manera temporal del registro a editar.
        private string[] auxR;
        // Contiene el número de atributos de la entidad seleccionada.
        private int nA = 0;
        // Contiene el índice del registro que queremos borrar o modificar.
        private int indReg = 0;
        // Contiene el índice de la entidad seleccionada.
        private int entReg = 0;
        private int NumCajPrim = 0;
        private int NumCajSec = 0;
        private int tamPrim = 0;
        #endregion

        public Form1() {
            InitializeComponent();
            Entidades = new List<Entidad>();
            cabecera = -1;                      // Dirección por defecto al crear el archivo.
            nameFile = "";
            creado = false;
            comboBox2.Items.Add('E');
            comboBox2.Items.Add('C');
            comboBox3.Items.Add(0);
            comboBox3.Items.Add(1);
            comboBox3.Items.Add(2);
            comboBox3.Items.Add(3);
            comboBox3.Items.Add(2);
            comboBox7.Items.Add("Secuencial");
            comboBox7.Items.Add("Secuencial Indexado");
            dataGridView3.Visible = false;
            label6.Text = "";
            label7.Text = "";
            //Console.WriteLine(cabecera);
        }

        /****************************Botones para la edición del DD****************************/
        /// <summary>
        /// BOTÓN PARA CREAR UN NUEVO DICCIONARIO DE DATOS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e) {
            dataGridView1.Rows.Clear();
            textBox1.Text = "";
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Diccionario de Datos (*.bin)|*.bin";     // Extensión .bin
            if (save.ShowDialog() != DialogResult.OK)
                return;
            nameFile = save.FileName;
            archivo = File.Create(nameFile);
            archivo.Close();
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Write, FileShare.None);
            BinaryWriter writer = new BinaryWriter(archivo);
            writer.Write(cabecera);
            archivo.Close();
            creado = true;
            this.Text = "Diccionario de Datos | " + nameFile;
            label7.Text = cabecera.ToString();
        }

        /// <summary>
        /// BOTÓN PARA AGREGAR UNA NUEVA ENIDAD AL ARCHIVO.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e) {
            if (textBox1.Text != "" && creado) {
                AgregaEntidad(textBox1.Text);
            }
        }

        /// <summary>
        /// BOTÓN PARA AGREGAR UN NUEVO ATRIBUTO A LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e) {
            if (textBox2.Text != "" && !AtributoExiste(textBox2.Text)) {
                if (Entidades[comboBox1.SelectedIndex].DirDato == -1) {
                    AgregaAtributo(textBox2.Text);
                } else {
                    MessageBox.Show("Imposible agregar atributos, ya existen registros.", "Agregar Atributos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox2.Text = "";
                    comboBox2.Text = "";
                    textBox3.Text = "";
                    comboBox3.Text = "";
                }
            } else
            {
                MessageBox.Show("El atributo ya existe con ese nombre", "Agregar Atributos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Text = "";
                comboBox2.Text = "";
                textBox3.Text = "";
                comboBox3.Text = "";
            }
        }

        private bool AtributoExiste(string nombre)
        {
            for(int i = 0; i < Entidades[indexAtributo].lAtributos.Count; i++)
            {
                if (Entidades[indexAtributo].lAtributos[i].nombreSA == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// BOTÓN PARA MODIFICAR ENTIDADES DEL DICCIONARIO DE DATOS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e) {
            if (textBox1.Text != "") {
                CambiaEntidad(textBox1.Text);
            }
        }

        /// <summary>
        /// BOTÓN PARA ELIMINAR LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e) {
            if (comboBox1.SelectedItem != null && Entidades.Count > 0 & Entidades[comboBox1.SelectedIndex].DirDato == -1) {    //checar si ya hay registros

                EliminaEntidad((string)comboBox1.SelectedItem);
            }
            else
            {
                MessageBox.Show("Entidad ya contiene registros");
            }
        }

        /// <summary>
        /// BOTÓN PARA MODIFICAR LOS METADATOS DEL ATRIBUTO SELECCIONADO.
        /// </summary>
        /// SANJUANA
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e) {
            if (textBox2.Text != "" && comboBox4.SelectedItem != null && !AtributoExiste(textBox2.Text))
            {
                CambiaAtributo(textBox2.Text, (char)comboBox2.SelectedItem, (int)comboBox3.SelectedItem);
            }
            else
            {
                textBox2.Text = "";
                comboBox2.Text = "";
                textBox3.Text = "";
                comboBox3.Text = "";
                MessageBox.Show("Atributo ya existe");
            }
        }

        /// <summary>
        /// BOTÓN PARA ELIMINAR EL ATRIBUTO SELECCIONADO DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e) {
            if (comboBox4.SelectedItem != null) {
                EliminaAtributo(entS, entA);
            }
        }

        /// <summary>
        /// BOTÓN PARA CREAR AL ARCHIVO DE DATOS DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e) {
            CreaArchivoDato();
            inicializaIndicePrimario();
            inicializaIndiceSecundario();
            inicializaHash();
            CreaColumnasRegistro();
            auxR = new string[nA];
            for(int i = 0; i < nA; i++) {
                auxR[i] = "";
            }
        }

        /// <summary>
        /// BOTÓN PARA CAPTURAR EL DATO EN EL REGISTRO EN MEMORIA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e) {
            if (textBox4.Text != "" && comboBox5.SelectedItem != null) {
                auxR[atrSele] = textBox4.Text;
                textBox4.Text = "";
            }
            textBox5.Text = "";
            comboBox5.Text = "";
            for (int i = 0; i < auxR.Length; i++) {
                textBox5.Text += auxR[i] + " ";
            }
        }

        /// <summary>
        /// BOTÓN PARA GUARDAR EL REGISTRO EN EL ARCHIVO DE DATOS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e) {
            bool completo = true;
            for(int i = 0; i < auxR.Length; i++) {
                if (auxR[i] == "") {
                    completo = false;
                }
            }
            if (completo) {
                AgregaRegistro();
                if (indicePrimDisp)
                {
                    AgregaIndiceP(dirNuReg, keyN);
                }   
                AgregaIndiceS(dirNuReg, keyNS);
                AgregaIndiceH(keyH, dirNuReg);
                for(int i = 0; i < nA; i++) {
                    auxR[i] = "";
                }
            } else {
                MessageBox.Show("Faltan datos por completar en el regitro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            textBox5.Clear();
        }

        /// <summary>
        /// BOTÓN PARA MODIFICAR EL REGISTRO SELECCIONADO.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e) {
            Entidad ent = Entidades[entReg];
            if (indReg < ent.Datos.Count) {
                ModificaClaves();
                ModificaRegistro(ent);
            } else {
                MessageBox.Show("Imposible moodificar el registro seleccionado.", "Modificar Registros", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModificaClaves()
        {
            posClavePrim = 0;
            posClaveSec = 0;
            posClaveH = 0;
            for (int i = 0; i < Entidades[entReg].lAtributos.Count; i++)
            {
                if (Entidades[entReg].lAtributos[i].TipoIndice == 2)
                {
                    posClavePrim = i;
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 3)
                {
                    posClaveSec = i;
                }
                //modificado clave
                if (Entidades[entReg].lAtributos[i].TipoIndice == 4)
                {
                    posClaveH = i;
                }
            }
            string claveP = auxR[posClavePrim];
            string claveS = auxR[posClaveSec];
            string claveHN = auxR[posClaveH];
            Atributo atrP = new Atributo();
            Atributo atrS = new Atributo();
            foreach (Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 2)
                {
                    atrP = a;
                }
                if (a.TipoIndice == 3)
                {
                    atrS = a;
                }
            }
            
            for (int i = 0; i < Entidades[entReg].lAtributos.Count; i++) {
                if (Entidades[entReg].lAtributos[i].TipoIndice == 2) {
                    ModificaPrimario(claveP, atrP, dirD);
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 3) {
                    ModificaSecundario(claveS, atrS, dirD);
                }
                //modificado clave
                if (Entidades[entReg].lAtributos[i].TipoIndice == 4) {
                    if (!ifHashExists(claveHN)) {
                        ModificaHash(claveHN, dirD);
                    }
                }
            }
        }

        private void ModificaPrimario(string clave, Atributo a, long dirReg)
        {
            for(int i = 0; i < Entidades[entReg].indPrimario.Count; i++)
            {
                if (dirReg == Entidades[entReg].indPrimario[i].apuntador)
                {
                    if (clave != Entidades[entReg].indPrimario[i].clave.ToString() && !ifKeyExists(clave))
                    {
                        if (a.TipoAtributo == 'E')
                        {
                            Entidades[entReg].indPrimario[i].clave = Int32.Parse(clave);
                        } else
                        {
                            Entidades[entReg].indPrimario[i].clave = clave;
                        }
                        Entidades[entReg].indPrimario = Entidades[entReg].indPrimario.OrderBy(k => k.clave).ToList();
                        GuardaIndiceMod(a.DirIndice, a.TipoAtributo, a.Longitud);
                        ActualizaDGV5(a);
                    }
                }
            }
        }
        private void GuardaIndiceMod(long dir, char a, int longA) {
            arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
            arcInd.Position = dir;
            BinaryWriter writer = new BinaryWriter(arcInd);

            for (int i = 0; i < Entidades[entReg].indPrimario.Count; i++) {
                if (a == 'E') {
                    int entero = Int32.Parse(Entidades[entReg].indPrimario[i].clave.ToString());
                    writer.Write(entero);
                    writer.Write(Entidades[entReg].indPrimario[i].apuntador);
                } else {
                    char[] cadena = new char[longA];
                    int ind = 0;
                    foreach (char c in (string)Entidades[entReg].indPrimario[i].clave) {
                        cadena[ind] += c;
                        ind++;
                    }
                    writer.Write(cadena);
                    writer.Write(Entidades[entReg].indPrimario[i].apuntador);
                }
            }
            arcInd.Close();
        }
        private void ModificaSecundario(string clave, Atributo a, long dirReg)
        {
            string cveOri = "";
            for(int i = 0; i < Entidades[entReg].indSecundario.Count; i++)
            {
                for(int j = 0; j < Entidades[entReg].indSecundario[i].cajones.Count; j++)
                {
                    if (dirReg == Entidades[entReg].indSecundario[i].cajones[j])
                    {
                        cveOri = Entidades[entReg].indSecundario[i].clave.ToString();
                        if (clave != cveOri)
                        {
                            if (ifForeingKeyExists(clave))     //ya existe, no se crea cajon
                            {
                                modificaCajoncito(clave, dirReg);
                                eliminaCajoncito(cveOri, dirReg, a);
                            }
                            else
                            {
                                AgregaIndiceS(dirReg, clave);
                                eliminaCajoncito(cveOri, dirReg, a);
                            }
                        }
                    }
                }
            }
            GuardaSecMod(a);
        }
        private void GuardaSecMod(Atributo a) {
            arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
            arcInd.Position = a.DirIndice;
            BinaryWriter writer = new BinaryWriter(arcInd);
            for(int i = 0; i < Entidades[entReg].indSecundario.Count; i++) {
                if (a.TipoAtributo == 'E') {
                    int entero = Int32.Parse(Entidades[entReg].indSecundario[i].clave.ToString());
                    writer.Write(entero);
                    writer.Write(Entidades[entReg].indSecundario[i].apuntador);
                } else {
                    char[] cadena = new char[a.Longitud];
                    int ind = 0;
                    foreach (char c in (string)Entidades[entReg].indSecundario[i].clave) {
                        cadena[ind] += c;
                        ind++;
                    }
                    writer.Write(cadena);
                    writer.Write(Entidades[entReg].indSecundario[i].apuntador);
                }
            }
            for(int i = 0; i < Entidades[entReg].indSecundario.Count; i++) {
                if (Entidades[entReg].indSecundario[i].apuntador != -1) {
                    arcInd.Position = Entidades[entReg].indSecundario[i].apuntador;
                    for(int j = 0; j < Entidades[entReg].indSecundario[i].cajones.Count; j++) {
                        writer.Write(Entidades[entReg].indSecundario[i].cajones[j]);
                    }
                }
            }
            arcInd.Close();
        }
        private void modificaCajoncito(string clave, long dirDato)
        {
            for(int i=0; i < Entidades[entReg].indSecundario.Count; i++)
            {
                if (clave == Entidades[entReg].indSecundario[i].clave.ToString())
                {
                    int llenos = 0;
                    for(int j = 0; j < Entidades[entReg].indSecundario[i].cajones.Count; j++)
                    {
                        if (Entidades[entReg].indSecundario[i].cajones[j] != 100000)
                        {
                            llenos++;
                        }
                    }
                    Entidades[entReg].indSecundario[i].cajones[llenos] = dirDato;
                }
            }
        }
        private void eliminaCajoncito(string clave, long dirDato, Atributo a)
        {
            foreach(indiceS index in Entidades[entReg].indSecundario)
            {
                if (index.clave.ToString() == clave)
                {
                    int llenos = 0;
                    for(int j = 0; j < index.cajones.Count; j++)
                    {
                        if (index.cajones[j] == dirDato)
                        {
                            index.cajones[j] = 100000;
                        }
                    }
                    index.cajones = index.cajones.OrderBy(p => p).ToList();
                    foreach (long l in index.cajones)
                    {
                        if (l != 100000)
                        {
                            llenos++;
                        }
                    }
                    
                    if (llenos == 0)
                    {
                        if (a.TipoAtributo == 'E')
                        {
                            index.clave = Int32.Parse("10000");
                        } else
                        {
                            index.clave = "Z";
                        }
                        index.apuntador = -1;
                        Entidades[entReg].indSecundario = Entidades[entReg].indSecundario.OrderBy(p => p.clave).ToList();
                        ActualizaDGV6(a);
                    }
                }
            }
        }
        private bool ifForeingKeyExists(string key)
        {
            foreach (indiceS index in Entidades[entReg].indSecundario)
            {
                if (key == index.clave.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private bool ifKeyExists(string key)
        {
            foreach(indiceP index in Entidades[entReg].indPrimario)
            {
                if (index.clave.ToString() == key)
                {
                    sePuedeModificar = false;
                    return true;
                } else
                {
                    sePuedeModificar = true;
                }
            }
            return false;
        }
        private void ActualizaDGV5(Atributo a)
        {
            dataGridView5.Rows.Clear();
            foreach(indiceP index in Entidades[entReg].indPrimario)
            {
                if (a.TipoAtributo == 'E')
                {
                    if (Int32.Parse(index.clave.ToString()) != 10000)
                    {
                        dataGridView5.Rows.Add(index.clave, index.apuntador);
                    }
                    else
                    {
                        dataGridView5.Rows.Add("", "");
                    }
                }
                else
                {
                    if (index.clave.ToString() != "Z")
                    {
                        dataGridView5.Rows.Add(index.clave, index.apuntador);
                    }
                    else
                    {
                        dataGridView5.Rows.Add("", "");
                    }
                }
            }
        }
        private void ActualizaDGV6(Atributo a)
        {
            dataGridView6.Rows.Clear();
            foreach (indiceS index in Entidades[entReg].indSecundario)
            {
                if (a.TipoAtributo == 'E')
                {
                    if (Int32.Parse(index.clave.ToString()) != 10000)
                    {
                        dataGridView6.Rows.Add(index.clave, index.apuntador);
                    }
                    else
                    {
                        dataGridView6.Rows.Add("", "");
                    }
                }
                else
                {
                    if (index.clave.ToString() != "Z")
                    {
                        dataGridView6.Rows.Add(index.clave, index.apuntador);
                    }
                    else
                    {
                        dataGridView6.Rows.Add("", "");
                    }
                }
            }
        }

        /// <summary>
        /// BOTÓN PARA BORRAR EL REGISTRO SELECCIONADO.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e) {
            Entidad ent = Entidades[entReg];
            if (indReg < ent.Datos.Count) {
                EliminaRegistros(ent);
            } else {
                MessageBox.Show("Imposible eliminar registro.", "Eliminar Registro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// SELECCIONADOR DE LA ENTIDAD A LA QUE QUEREMOS AGREAGAR ATRIBUTOS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        int indexAtributo = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {
            label6.Text = (string)comboBox1.SelectedItem;
            entS = (string)comboBox1.SelectedItem;
            indexAtributo = comboBox1.SelectedIndex;
            int i = 0;
            entSel = new char[30];
            foreach(char c in (string)comboBox1.SelectedItem) {
                entSel[i] = c;
                i++;
            }
            ActualizaDataGrid2(entSel);
        }

        /// <summary>
        /// SELECCIONADOR DEL TIPO DE DATO DEL ATRIBUTO, RESTRINGE EL CAMPO DE LONGITUD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) {
            if ((char)comboBox2.SelectedItem == 'C') {
                textBox3.Enabled = true;
            } else {
                textBox3.Enabled = false;
            }
        }

        /// <summary>
        /// SELECCIONADOR DEL ATRIBUTO A MODIFICAR.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e) {
            string n = (string)comboBox4.SelectedItem;
            entA = (string)comboBox4.SelectedItem;
            int i = 0;
            atrSel = new char[30];
            foreach(char c in (string)comboBox4.SelectedItem) {
                atrSel[i] = c;
                i++;
            }
        }

        /// <summary>
        /// SELECCIONADOR DEL ATRIBUTO QUE QUEREMOS CAPTURAR EL REGISTRO.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e) {
            atrSele = comboBox5.SelectedIndex;
        }

        /// <summary>
        /// SELECCIONADOR DE LA ENTIDAD A EDITAR REGISTROS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e) {
            //Console.WriteLine(comboBox6.SelectedIndex);
            entReg = comboBox6.SelectedIndex;
            comboBox5.Items.Clear();
            entS2 = (string)comboBox6.SelectedItem;
            Console.WriteLine("Combo " + comboBox6.SelectedIndex);
            Console.WriteLine("entS " + entS);
            CreaColumnasRegistro();
            Entidad ent = null;
            string hola = null;
            for(int i = 0; i < Entidades.Count; i++) {
                if (Entidades[i].nombreS == entS2) {
                    ent = Entidades[i];
                }
            }
            auxR = new string[nA];
            for (int i = 0; i < nA; i++) {
                auxR[i] = "";
            }
            ActualizaDataGrid3(ent);
        }

        /// <summary>
        /// SELECCIONADOR DEL REGISTRO A BORRAR O MODIFICAR.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e) {
            Console.WriteLine(dataGridView3.CurrentRow.Index);
            indReg = dataGridView3.CurrentRow.Index;
            textBox5.Text = "";
            if (indReg < Entidades[entReg].Datos.Count) {
                for(int i = 0; i < nA; i++) {
                    auxR[i] = Entidades[entReg].Datos[indReg].Registros[i].ToString();

                }
                for (int i = 0; i < auxR.Length; i++) {
                    textBox5.Text += auxR[i] + " ";
                }
                dirD = Entidades[entReg].Datos[indReg].DirRegistro;
            }
        }

        /// <summary>
        /// BOTÓN PARA ABRIR UN DICCIONARIO DE DATOS CREADO.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e) {
            Entidades = new List<Entidad>();
            creado = true;
            LeeCabecera();
        }

        /// <summary>
        /// BOTÓN PARA CERRAR EL ARCHIVO ACTUAL Y RESETEAR LOS DATOS.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cerrarArchivoToolStripMenuItem1_Click(object sender, EventArgs e) {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            comboBox1.Items.Clear();
            comboBox1.Text = "";
            comboBox4.Items.Clear();
            comboBox4.Text = "";
            Entidades = new List<Entidad>();
            nameFile = "";
            cabecera = -1;
            label7.Text = "";
            label6.Text = "";
            this.Text = "Diccionario de Datos";
            creado = false;
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView3.Visible = false;
            comboBox6.Items.Clear();
            comboBox5.Items.Clear();
            comboBox6.Text = "";
        }

        /// <summary>
        /// BOTÓN PARA SALIR DEL SISTEMA.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void salirToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }
        /****************************Botones para la edición del DD****************************/

        /***************************Métodos de Apoyo***************************/
        /// <summary>
        /// MÉTODO PARA ENCONTRAR LA DIRECCIÓN PARA GUARDAR LA SIGUIENTE ENTIDAD.
        /// </summary>
        /// <returns>Dirección de la siguiente entidad.</returns>
        private long EncuentraDireccion() {
            long aux = -1;
            archivo = File.OpenRead(nameFile);
            aux = archivo.Length;
            archivo.Close();
            return aux;
        }

        /// <summary>
        /// MÉTODO PARA ENCONTRAR LA DIRECCIÓN PARA GUARDAR EL SIGUIENTE REGISTRO.
        /// </summary>
        /// <returns>Dirección del siguiente registro.</returns>
        private long EncuentraDirRegistro() {
            long aux = -1;
            for(int i = 0; i < Entidades.Count; i++) {
                if (Entidades[i].nombreS == entS2) {
                    Entidades[i].Registro = File.OpenRead(Entidades[i].Ruta);
                    aux = Entidades[i].Registro.Length;
                    Entidades[i].Registro.Close();
                }
            }
            return aux;
        }

        /// <summary>
        /// MÉTODO DE APOYO PARA VISUALIZAR EL NOMBRE EN EL DATAGRID.
        /// </summary>
        /// <param name="n">Recibe el nombre de la entidad.</param>
        /// <returns>Regresa la cadena para poder visualizarla completa en el DataGrid</returns>
        private string NombreString(char[] n) {
            string name = "";
            foreach (char c in n) {
                if (char.IsLetter(c)) {
                    name += c;
                }
            }
            return name;
        }

        /// <summary>
        /// MÉTODO DE APOYO PARA CAMBIAR UN STRING A UN ARREGLO DE CARACTERES.
        /// </summary>
        /// <param name="n">Cadena a convertir.</param>
        /// <returns>Nombre convertido a un arreglo de caracteres.</returns>
        private char[] NombreChars(string n) {
            char[] aux = new char[30];
            int i = 0;
            foreach(char c in n) {
                aux[i] = c;
                i++;
            }
            return aux;
        }
       
        /// <summary>
        /// MÉTODO PARA AGREGAR UNA ENTIDAD EN MEMORIA.
        /// </summary>
        /// <param name="nombre">Nombre de la entidad.</param>
        private void AgregaEntidad(string nombre) {
            long dir = EncuentraDireccion();
            Entidad ent = new Entidad(nombre, dir, -1, -1, -1);
            if (!ifExists(nombre)) {
                Entidades.Add(ent);
                GuardaEntidad(ent);
                Entidades = Entidades.OrderBy(p => p.nombreS).ToList();
                ActualizaCabecera(Entidades[0].DirEntidad);
                ActualizaEntidades();
                ActualizaDataGrid();
                textBox1.Text = "";
            } else {
                MessageBox.Show("La entidad ya existe en el diccionario de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "";
            }
        }
        
        /// <summary>
        /// MÉTODO PARA GUARDAR UNA ENTIDAD AL ARCHIVO.
        /// </summary>
        /// <param name="ent">Contiene la entidad a agregar al archivo.</param>
        private void GuardaEntidad(Entidad ent) {
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Write, FileShare.None);
            archivo.Position = archivo.Length;
            BinaryWriter write = new BinaryWriter(archivo);
            write.Write(ent.nombreEntidad);
            write.Write(ent.DirEntidad);
            write.Write(ent.DirAtributo);
            write.Write(ent.DirDato);
            write.Write(ent.DirSiguienteEnt);
            archivo.Close();
        }

        /// <summary>
        /// MÉTODO PARA AGREGAR UN ATRIBUTO EN MEMORIA.
        /// </summary>
        /// <param name="nombre">Nombre del atributo.</param>
        private void AgregaAtributo(string nombre) {
            long dir = EncuentraDireccion();
            char tipoAt = (char)comboBox2.SelectedItem;
            int tam;
            int tipoIn = (int)comboBox3.SelectedItem;
            
            if (tipoAt == 'C') {
                tam = Int32.Parse(textBox3.Text);
            } else {
                tam = 4;
            }
            Atributo atr = new Atributo(nombre, dir, tipoAt, tam, tipoIn, -1, -1);
            
            for(int i = 0; i < Entidades.Count; i++) {
                if (isEquals(Entidades[i].nombreEntidad, entSel)) {
                    if (Entidades[i].DirAtributo == -1) {               // Cambia la dirección del primer atributo
                        Entidades[i].DirAtributo = atr.DirAtributo;     // cuando no existen atributos.
                        ModificaEntidad(Entidades[i]);
                        ActualizaDataGrid();
                    }
                    if (Entidades[i].lAtributos.Count >= 1) {
                        int last = Entidades[i].lAtributos.Count;
                        Entidades[i].lAtributos[last - 1].DirSiguienteAtr = atr.DirAtributo;
                        ModificaAtributo(Entidades[i].lAtributos[last - 1]);
                    }
                    Entidades[i].lAtributos.Add(atr);
                }
            }
            ActualizaDataGrid2(entSel);
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            GuardaAtributo(atr);
        }

        /// <summary>
        /// MÉTODO PARA GUARDAR UN ATRIBUTO AL ARCHIVO.
        /// </summary>
        /// <param name="at">Nombre del atributo.</param>
        private void GuardaAtributo(Atributo at) {
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Write, FileShare.None);
            archivo.Position = archivo.Length;
            BinaryWriter write = new BinaryWriter(archivo);
            write.Write(at.nombreAtributo);
            write.Write(at.DirAtributo);
            write.Write(at.TipoAtributo);
            write.Write(at.Longitud);
            write.Write(at.TipoIndice);
            write.Write(at.DirIndice);
            write.Write(at.DirSiguienteAtr);
            archivo.Close();
        }

        /// <summary>
        /// ACTUALIZA CUANDO AGREGAMOS EL PRIMER ATRIBUTO DE LA ENTIDAD.
        /// </summary>
        /// <param name="e">Entidad que vamos a modificar su dirección del atributo.</param>
        private void ModificaEntidad(Entidad e) {
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Write, FileShare.None);
            archivo.Seek(e.DirEntidad, SeekOrigin.Begin);
            BinaryWriter write = new BinaryWriter(archivo);
            write.Write(e.nombreEntidad);
            write.Write(e.DirEntidad);
            write.Write(e.DirAtributo);
            write.Write(e.DirDato);
            write.Write(e.DirSiguienteEnt);
            archivo.Close();
        }

        /// <summary>
        /// MÉTODO PARA ACTUALIZAR CUANDO SE MODIFIQUE UN ATRIBUTO.
        /// </summary>
        /// <param name="at">Atributo a modificar.</param>
        private void ModificaAtributo(Atributo at) {
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Write, FileShare.None);
            archivo.Seek(at.DirAtributo, SeekOrigin.Begin);
            BinaryWriter write = new BinaryWriter(archivo);
            write.Write(at.nombreAtributo);
            write.Write(at.DirAtributo);
            write.Write(at.TipoAtributo);
            write.Write(at.Longitud);
            write.Write(at.TipoIndice);
            write.Write(at.DirIndice);
            write.Write(at.DirSiguienteAtr);
            archivo.Close();
        }

        /// <summary>
        /// MÉTODO PARA MODIFICAR METADATOS DE UNA ENTIDAD.
        /// </summary>
        /// <param name="nombre">El nuevo nombre de la entidad.</param>
        private void CambiaEntidad(string nombre) {
            if (!ifExists(nombre)) {
                for (int i = 0; i < Entidades.Count; i++) {
                    if (isEquals(Entidades[i].nombreEntidad, entSel) && Entidades[i].DirDato == -1) {
                        Entidad e = new Entidad(nombre, Entidades[i].DirEntidad, Entidades[i].DirAtributo, Entidades[i].DirDato, Entidades[i].DirSiguienteEnt);
                        Entidades.Remove(Entidades[i]);
                        Entidades.Add(e);
                        Entidades = Entidades.OrderBy(p => p.nombreS).ToList();
                        cabecera = Entidades[0].DirEntidad;
                        ActualizaCabecera(cabecera);
                        label7.Text = cabecera.ToString();
                        ActualizaEntidades();
                        ActualizaDataGrid();
                    }
                }
            } else {
                MessageBox.Show("Imposible modificar la entidad seleccionada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            comboBox1.Text = "";
            textBox1.Text = "";
        }

        /// <summary>
        /// MÉTODO PARA MODIFICAR ALGÚN METADATO DEL ATRIBUTO SELECCIONADO DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="n">Nombre nuevo.</param>
        /// <param name="tD">Tipo de atributo nuevo.</param>
        /// <param name="ind">Tipo de indice nuevo.</param>
        private void CambiaAtributo(string n, char tD, int ind) {
            int tam;
            if (tD == 'C') {
                tam = Int32.Parse(textBox3.Text);
            } else {
                tam = 4;
            }

            if (!ifExists(n)) {
                for (int i = 0; i < Entidades.Count; i++) {
                    if (isEquals(Entidades[i].nombreEntidad, entSel) && Entidades[i].DirDato == -1) {
                        for(int j = 0; j < Entidades[i].lAtributos.Count; j++) {
                            if (isEquals(Entidades[i].lAtributos[j].nombreAtributo, atrSel)) {
                                Entidades[i].lAtributos[j].nombreAtributo = NombreChars(n);
                                Entidades[i].lAtributos[j].TipoAtributo = tD;
                                Entidades[i].lAtributos[j].Longitud = tam;
                                Entidades[i].lAtributos[j].TipoIndice = ind;
                                ModificaAtributo(Entidades[i].lAtributos[j]);
                                ActualizaDataGrid2(entSel);
                            }
                        }
                    }
                }
            } else {
                MessageBox.Show("Imposible modificar el atributo seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox4.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        /// <summary>
        /// MÉTODO PARA ELIMINAR LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="nombre">Nombre de la entidad que queremos borrar.</param>
        private void EliminaEntidad(string nombre) {
            bool borrado = false;
            long dir = -1;
            for(int i = 0; i < Entidades.Count; i++) {
                if (string.Compare(Entidades[i].nombreS, nombre) == 0) {
                    dir = Entidades[i].DirEntidad;
                }
            }
            for(int i = 0; i < Entidades.Count - 1; i++) {
                if (dir == Entidades[i].DirSiguienteEnt) {
                    Entidades[i].DirSiguienteEnt = Entidades[i + 1].DirSiguienteEnt;
                    ModificaEntidad(Entidades[i]);
                    Entidades.Remove(Entidades[i + 1]);
                    dir = -1;
                    borrado = true;
                }
            }
            if (Entidades.Count == 1 && !borrado) {
                cabecera = -1;
                Entidades.RemoveAt(0);
                ActualizaCabecera(cabecera);
            }
            if (Entidades.Count > 1 && dir == Entidades[0].DirEntidad) {
                cabecera = Entidades[0].DirSiguienteEnt;
                ActualizaCabecera(cabecera);
                Entidades.Remove(Entidades[0]);
            }
            dataGridView2.Rows.Clear();
            comboBox1.Items.Clear();
            comboBox4.Items.Clear();
            label6.Text = "";
            comboBox1.Text = "";
            ActualizaDataGrid();
        }

        /// <summary>
        /// MÉTODO PARA ELIMINAR EL ATRIBUTO SELECCIONADO DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="ent">Entidad seleccionada.</param>
        /// <param name="atr">Atributo seleccionado.</param>
        private void EliminaAtributo(string ent, string atr) {
            bool borrado = false;
            long dir = -1;
            for(int i = 0; i < Entidades.Count; i++) {
                for(int j = 0; j < Entidades[i].lAtributos.Count; j++) {
                    if (string.Compare(Entidades[i].nombreS, ent) == 0 && string.Compare(Entidades[i].lAtributos[j].nombreSA, atr) == 0) {
                        dir = Entidades[i].lAtributos[j].DirAtributo;
                    }
                }
            }
            for (int i = 0; i < Entidades.Count; i++) {
                for (int j = 0; j < Entidades[i].lAtributos.Count - 1; j++) {
                    if (dir == Entidades[i].lAtributos[j].DirSiguienteAtr) {
                        Entidades[i].lAtributos[j].DirSiguienteAtr = Entidades[i].lAtributos[j + 1].DirSiguienteAtr;
                        ModificaAtributo(Entidades[i].lAtributos[j]);
                        Entidades[i].lAtributos.Remove(Entidades[i].lAtributos[j + 1]);
                        dir = -1;
                        borrado = true;
                    }
                }
            }
            for(int i = 0; i < Entidades.Count; i++) {
                if (string.Compare(Entidades[i].nombreS, entS) == 0) {
                    if (Entidades[i].lAtributos.Count == 1 && !borrado) {
                        Entidades[i].DirAtributo = -1;
                        Entidades[i].lAtributos.RemoveAt(0);
                        ModificaEntidad(Entidades[i]);
                        ActualizaDataGrid();
                    }
                }
            }
            for(int i = 0; i < Entidades.Count; i++) {
                if (string.Compare(Entidades[i].nombreS, entS) == 0) {
                    if (Entidades[i].lAtributos.Count > 1 && dir == Entidades[i].lAtributos[0].DirAtributo) {
                        Entidades[i].DirAtributo = Entidades[i].lAtributos[0].DirSiguienteAtr;
                        ModificaEntidad(Entidades[i]);
                        Entidades[i].lAtributos.RemoveAt(0);
                        ActualizaDataGrid();
                    }
                }
            }
            comboBox4.Text = "";
            ActualizaDataGrid2(entSel);
        }

        long dirNuReg;
        string keyN, keyNS, keyH;
        bool indicePrimDisp = true;
        /// <summary>
        /// MÉTODO PARA AGREGAR UN REGISTRO EN MEMORIA.
        /// </summary>
        private void AgregaRegistro() {
            dirNuReg = -1;
            keyN = "";
            keyNS = "";
            keyH = "";
            posClavePrim = 0;
            posClaveSec = 0;
            posClaveH = 0;
            for (int i = 0; i < Entidades[entReg].lAtributos.Count; i++)
            {
                if (Entidades[entReg].lAtributos[i].TipoIndice == 2)
                {
                    posClavePrim = i;
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 3)
                {
                    posClaveSec = i;
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 4)
                {
                    posClaveH = i;
                }
            }
            for (int h = 0; h < Entidades.Count; h++)
            {
                if (Entidades[h].nombreS == entS2 /*&& !EncuentraClavePrimaria(Entidades[h])*/)
                {

                    int rowCount = dataGridView3.Rows.Count - 1;
                    int colCount = dataGridView3.Columns.Count - 1;
                    Registro reg = new Registro();
                    reg.DirRegistro = EncuentraDirRegistro();
                    dirNuReg = reg.DirRegistro;
                    keyN = auxR[posClavePrim];
                    keyNS = auxR[posClaveSec];
                    keyH = auxR[posClaveH];
                    reg.DirSiguienteReg = -1;
                    /*for(int i = 0; i < nA; i++) {
                        reg.Registros.Add(auxR[i]);
                    }*/
                    bool yaExiste = false;
                    indicePrimDisp = true;
                    foreach (indiceP index in Entidades[entReg].indPrimario)
                    {
                        if (keyN == index.clave.ToString())
                        {
                            yaExiste = true;
                            indicePrimDisp = false;
                        }
                    }
                    if (!yaExiste && !EncuentraClavePrimaria(Entidades[entReg]))
                    {
                        for (int i = 0; i < Entidades.Count; i++)
                        {
                            if (Entidades[i].nombreS == entS2)
                            {
                                for (int j = 0; j < nA; j++)
                                {
                                    if (Entidades[i].lAtributos[j].TipoAtributo == 'E')
                                    {
                                        reg.Registros.Add((Int32.Parse(auxR[j])));
                                    }
                                    else
                                    {
                                        reg.Registros.Add(auxR[j]);
                                    }
                                }
                            }
                        }
                        dataGridView3.Rows.Add(1);
                        dataGridView3.Rows[rowCount].Cells[0].Value = reg.DirRegistro;
                        for (int i = 0; i < nA; i++)
                        {
                            dataGridView3.Rows[rowCount].Cells[i + 1].Value = auxR[i];
                        }
                        dataGridView3.Rows[rowCount].Cells[colCount].Value = reg.DirSiguienteReg;
                        for (int i = 0; i < Entidades.Count; i++)
                        {
                            if (Entidades[i].nombreS == entS2 /*&& !EncuentraClavePrimaria(Entidades[i])*/)
                            {
                                Entidades[i].Datos.Add(reg);
                                MeteRegistro(Entidades[i], reg);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("La clave primaria ya existe.");
                    }
                }
            }
        }

        int posClavePrim, posClaveSec, posClaveH;
        private bool EncuentraClavePrimaria(Entidad e)
        {
            int indice = 0;
            bool band = false;
            for(int i = 0; i < e.lAtributos.Count; i++)
            {
                if (e.lAtributos[i].TipoIndice == 1)
                {
                    band = true;
                    indice = i;
                }
            }
            if (band) {
                foreach(Registro r in e.Datos)
                {
                    if (r.Registros[indice].ToString() == auxR[indice])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// MÉTODO PARA GUARDAR UN REGISTRO EN EL ARCHIVO DE DATOS.
        /// </summary>
        /// <param name="e">Entidad seleccionada.</param>
        /// <param name="r">Registro recien creado para guardar.</param>
        private void MeteRegistro(Entidad e, Registro r) {
            e.Registro = File.Open(e.Ruta, FileMode.Open, FileAccess.Write, FileShare.None);
            e.Registro.Position = e.Registro.Length;
            BinaryWriter writer = new BinaryWriter(e.Registro);
            writer.Write(r.DirRegistro);
            for(int i = 0; i < r.Registros.Count; i++) {
                if (e.lAtributos[i].TipoAtributo == 'E') {
                    int entero = Int32.Parse(r.Registros[i].ToString());
                   // int entero = r.Registros[i];
                    writer.Write(entero);
                } else {
                    char[] cadena = new char[e.lAtributos[i].Longitud];
                    int ind = 0;
                    foreach(char c in (string)r.Registros[i]) {
                        cadena[ind] += c;
                        ind++;
                    }
                    writer.Write(cadena);
                }
            }
            writer.Write(r.DirSiguienteReg);
            //comboBox3.Items.Add(0);
            e.Registro.Close();
            VerificaRegistros(e);
        }

        /// <summary>
        /// MÉTODO QUE SELECCIONA COMO GUARDAR LAS DIRECCIONES DE LOS REGISTROS.
        /// </summary>
        /// <param name="e">Entidad seleccionda.</param>
        private void VerificaRegistros(Entidad e) {
            int index = 0;
            for(int i = 0; i < e.lAtributos.Count; i++) {
                if (e.lAtributos[i].TipoIndice == 1) {      //cambiar a 1 para secuencial
                    index = i;
                }
            }
            if (ClaveBusqueda(e)) {
                OrdenaRegistros(e, index, true);
            } else {
                OrdenaRegistros(e, 0, false);
            }
        }

        /// <summary>
        /// MÉTODO PARA SABER SI ALGÚN ATRIBUTO TIENE CLAVE DE BÚSQUEDA.
        /// </summary>
        /// <param name="e">Entidad seleccionada.</param>
        /// <returns>True si encuentra algún atributo con clave de búsqueda.</returns>
        private bool ClaveBusqueda(Entidad e) {
            foreach(Atributo a in e.lAtributos) {
                if (a.TipoIndice == 1) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// MÉTODO QUE ORDENA LOS REGISTROS DEPENDIENDO SI HAY O NO CLAVE DE BÚSQUEDA.
        /// </summary>
        /// <param name="e">Entidad seleccionada.</param>
        /// <param name="indice">Posición del atributo por el cual se va a ordenar.</param>
        /// <param name="ordena">Bandera de control para saber si hay necesidad de ordenar.</param>
        private void OrdenaRegistros(Entidad e, int indice, bool ordena) {
            if (ordena) {
                e.Datos = e.Datos.OrderBy(p => p.Registros[indice]).ToList();
            }
            e.DirDato = e.Datos[0].DirRegistro;
            ModificaEntidad(e);
            ActualizaDataGrid();
            for(int i = 0; i < e.Datos.Count - 1; i++) {
                e.Datos[i].DirSiguienteReg = e.Datos[i + 1].DirRegistro;
                Registro reg = new Registro();
                reg.DirRegistro = e.Datos[i].DirRegistro;
                reg.DirSiguienteReg = e.Datos[i].DirSiguienteReg;
                for (int j = 0; j < nA; j++) {
                    reg.Registros.Add(e.Datos[i].Registros[j]);
                }
                ActualizaRegistros(e, reg);
            }
            int dat = e.Datos.Count;
            e.Datos[dat - 1].DirSiguienteReg = -1;
            Registro reg1 = new Registro();
            reg1.DirRegistro = e.Datos[dat - 1].DirRegistro;
            reg1.DirSiguienteReg = e.Datos[dat - 1].DirSiguienteReg;
            for(int k = 0; k < nA; k++) {
                reg1.Registros.Add(e.Datos[dat - 1].Registros[k]);
            }
            ActualizaRegistros(e, reg1);
            ActualizaDataGrid3(e);
        }

        /// <summary>
        /// MÉTODO PARA ACTUALIZAR LOS DATOS DE UN REGISTRO MODIFICADO.
        /// </summary>
        /// <param name="e">Entidad seleccionada.</param>
        /// <param name="r">Registro modificado a sobreescribir.</param>
        private void ActualizaRegistros(Entidad e, Registro r) {
            e.Registro = File.Open(e.Ruta, FileMode.Open, FileAccess.Write, FileShare.None);
            e.Registro.Seek(r.DirRegistro, SeekOrigin.Begin);
            BinaryWriter writer = new BinaryWriter(e.Registro);
            writer.Write(r.DirRegistro);
            for (int i = 0; i < r.Registros.Count; i++) {
                if (e.lAtributos[i].TipoAtributo == 'E') {
                    int entero = Int32.Parse(r.Registros[i].ToString());
                    writer.Write(entero);
                } else {
                    char[] cadena = new char[e.lAtributos[i].Longitud];
                    int ind = 0;
                    foreach (char c in (string)r.Registros[i]) {
                        cadena[ind] += c;
                        ind++;
                    }
                    writer.Write(cadena);
                }
            }
            writer.Write(r.DirSiguienteReg);
            e.Registro.Close();
           // ActualizaDataGrid3(e);
        }

        /// <summary>
        /// MÉTODO PARA ACTUALIZAR LAS DIRECCIONES SIGUIENTES DE LAS ENTIDADES.
        /// </summary>
        private void ActualizaEntidades() {
            for(int i = 0; i < Entidades.Count - 1; i++) {
                Entidades[i].DirSiguienteEnt = Entidades[i + 1].DirEntidad;
                ModificaEntidad(Entidades[i]);
            }
            int last = Entidades.Count - 1;
            Entidades[last].DirSiguienteEnt = -1;
            ModificaEntidad(Entidades[last]);

        }

        /// <summary>
        /// MÉTODO QUE ACTUALIZA LA CABECERA QUE APUNTA A LA PRIMERA ENTIDAD ORDENADA.
        /// </summary>
        /// <param name="dir">Dirección de la entidad a la que hará referencia.</param>
        private void ActualizaCabecera(long dir) {
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Write, FileShare.None);
            archivo.Seek(0, SeekOrigin.Begin);
            BinaryWriter write = new BinaryWriter(archivo);
            write.Write(dir);
            archivo.Close();
            label7.Text = dir.ToString();
            //Console.WriteLine(dir);
        }

        /// <summary>
        /// MÉTODO PARA EXTRAER LA CABECERA DEL ARCHIVO PARA LEER LAS ENTIDADES.
        /// </summary>
        private void LeeCabecera() {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Diccionario de Datos (*.bin)|*.bin";
            if (open.ShowDialog() != DialogResult.OK)
                return;
            nameFile = open.FileName;
            this.Text = "Diccionario de Datos | " + nameFile;
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.None);
            archivo.Seek(0, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(archivo);
            cabecera = reader.ReadInt64();
            archivo.Close();
            label7.Text = cabecera.ToString();
            LeeArchivo(cabecera);
        }

        /// <summary>
        /// MÉTODO PARA LEER LAS ENTIDADES QUE TENGO EN MI ARCHIVO.
        /// </summary>
        /// <param name="dir">Dirección de la primera entidad.</param>
        private void LeeArchivo(long dir) {
            long dire = dir;
            archivo = File.Open(nameFile, FileMode.Open, FileAccess.Read, FileShare.None);
            while(dire != -1) {
                archivo.Seek(dire, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(archivo);
                char[] nombre = reader.ReadChars(30);
                long direccion = reader.ReadInt64();
                long dirA = reader.ReadInt64();
                long dirD = reader.ReadInt64();
                long dirS = reader.ReadInt64();
                string n = NombreString(nombre);
                Entidad enti = new Entidad(n, direccion, dirA, dirD, dirS);
                Entidades.Add(enti);
                if (dirA != -1) {
                    LeeAtributos(enti);
                }
                dire = dirS;
            }
            ActualizaDataGrid();
            archivo.Close();
            foreach(Entidad enti in Entidades) {
                if (enti.DirDato != -1) {
                    LeeRegistros(enti);
                }
            }
            foreach(Entidad enti in Entidades) {
                nomEnt = enti.nombreS + ".idx";
                if (hasPrimaryIndex(enti)) {
                    enti.indPrimario = new List<indiceP>();
                    arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Read, FileShare.None);
                    arcInd.Position = atrPivot.DirIndice;
                    BinaryReader reader = new BinaryReader(arcInd);
                    tamIndPrima(atrPivot.Longitud);
                    Console.WriteLine("Cajones primario" + NumCajPrim);
                    for(int i = 0; i < NumCajPrim; i++) {
                        
                        if (atrPivot.TipoAtributo == 'E') {
                            indiceP index = new indiceP(true);
                            index.clave = reader.ReadInt32();
                            index.apuntador = reader.ReadInt64();
                            enti.indPrimario.Add(index);
                        } else {
                            indiceP index = new indiceP(false);
                            int tamIndAtr = atrPivot.Longitud;
                            char[] cadena = reader.ReadChars(tamIndAtr);
                            string cadena2 = NombreString(cadena);
                            index.clave = cadena2;
                            index.apuntador = reader.ReadInt64();
                            enti.indPrimario.Add(index);
                        }
                    }
                    ActualizaDGV5(atrPivot);
                    arcInd.Close();
                }
            }
            foreach(Entidad enti in Entidades) {
                nomEnt = enti.nombreS + ".idx";
                if (hasSecondaryIndex(enti)) {
                    enti.indSecundario = new List<indiceS>();
                    arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Read, FileShare.None);
                    arcInd.Position = atrPivot.DirIndice;
                    BinaryReader reader = new BinaryReader(arcInd);
                    tamIndSec(atrPivot.Longitud);
                    for(int i = 0; i < NumCajSec; i++) {
                        if (atrPivot.TipoAtributo == 'E') {
                            indiceS index = new indiceS(true);
                            index.clave = reader.ReadInt32();
                            index.apuntador = reader.ReadInt64();
                            enti.indSecundario.Add(index);
                        } else {
                            indiceS index = new indiceS(false);
                            int tamIndAtr = atrPivot.Longitud;
                            char[] cadena = reader.ReadChars(tamIndAtr);
                            string cadena2 = NombreString(cadena);
                            index.clave = cadena2;
                            index.apuntador = reader.ReadInt64();
                            enti.indSecundario.Add(index);
                        }
                    }
                    //arcInd.Close();
                    for(int i = 0; i < enti.indSecundario.Count; i++) {
                        if (enti.indSecundario[i].apuntador != -1) {                            
                            //arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Read, FileShare.None);
                            arcInd.Position = enti.indSecundario[i].apuntador;
                            BinaryReader reader2 = new BinaryReader(arcInd);
                            for(int j = 0; j < 131; j++) {
                                enti.indSecundario[i].cajones[j] = reader2.ReadInt64();
                            }
                        } 
                    }
                    ActualizaDGV6(atrPivot);
                    arcInd.Close();
                }
            }
        }
        Atributo atrPivot;
        private bool hasPrimaryIndex(Entidad e) {
            foreach(Atributo a in e.lAtributos) {
                if (a.TipoIndice == 2) {
                    atrPivot = new Atributo();
                    atrPivot = a;
                    return true;
                }
            }
            return false;
        }
        private bool hasSecondaryIndex(Entidad e) {
            foreach(Atributo a in e.lAtributos) {
                if (a.TipoIndice == 3) {
                    atrPivot = new Atributo();
                    atrPivot = a;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// MÉTODO PARA LEER LOS ATRIBUTOS DE CADA ENTIDAD
        /// </summary>
        /// <param name="e"></param>
        private void LeeAtributos(Entidad e) {
            long dir = e.DirAtributo; 
            while(dir != -1) {
                archivo.Seek(dir, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(archivo);
                char[] nombre = reader.ReadChars(30);
                long dirA = reader.ReadInt64();
                char tipoA = reader.ReadChar();
                int longi = reader.ReadInt32();
                int tipoI = reader.ReadInt32();
                long dirI = reader.ReadInt64();
                long dirS = reader.ReadInt64();
                string n = NombreString(nombre);
                Atributo a = new Atributo(n, dirA, tipoA, longi, tipoI, dirI, dirS);
                e.lAtributos.Add(a);
                dir = dirS;
            }
        }

        /// <summary>
        /// MÉTODO PARA LEER LOS REGISTROS DEL ARCHIVO DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="e">Entidad seleccionada.</param>
        private void LeeRegistros(Entidad e) {
            e.Ruta = e.nombreS + ".dat";
            e.Registro = File.Open(e.Ruta, FileMode.Open, FileAccess.Read, FileShare.None);
            long dir = 0;
            dir = e.DirDato;
            nA = e.lAtributos.Count;
            while(dir != -1) {
                Registro reg = new Registro();
                e.Registro.Seek(dir, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(e.Registro);
                reg.DirRegistro = reader.ReadInt64();
                for(int i = 0; i < nA; i++) {
                    if (e.lAtributos[i].TipoAtributo == 'E') {
                        int entero = reader.ReadInt32();
                        reg.Registros.Add(entero);
                    } else {
                        int tam = e.lAtributos[i].Longitud;
                        char[] cadena = reader.ReadChars(tam);
                        string cadena2 = NombreString(cadena);
                        reg.Registros.Add(cadena2);
                    }
                }
                reg.DirSiguienteReg = reader.ReadInt64();
                dir = reg.DirSiguienteReg;
                e.Datos.Add(reg);
            }
            //CreaColumnasRegistro();
            //ActualizaDataGrid3(e);


            e.Registro.Close();
        }

        /// <summary>
        /// MÉTODO PARA ELIMINAR EL REGISTRO SELECCIONADO.
        /// </summary>
        /// <param name="e">Entidad seleccionada.</param>
        private void EliminaRegistros(Entidad e) {
            EliminaClaves();
            if (e.Datos.Count > 1) {
                e.Datos.RemoveAt(indReg);
                VerificaRegistros(e);
            } else if (e.Datos.Count == 1) {
                e.Datos.RemoveAt(0);
                e.DirDato = -1;
                ActualizaEntidades();
                ActualizaDataGrid();
                ActualizaDataGrid3(e);
            }
        }

        long dirD;
        private void EliminaClaves()
        {
            posClavePrim = 0;
            posClaveSec = 0;
            posClaveH = 0;
            for (int i = 0; i < Entidades[entReg].lAtributos.Count; i++)
            {
                if (Entidades[entReg].lAtributos[i].TipoIndice == 2)
                {
                    posClavePrim = i;
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 3)
                {
                    posClaveSec = i;
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 4)
                {
                    posClaveH = i;
                }
            }
            string claveP = auxR[posClavePrim];
            string claveS = auxR[posClaveSec];
            string claveH = auxR[posClaveH];
            Atributo atrP = new Atributo();
            Atributo atrS = new Atributo();
            Atributo atrH = new Atributo();
            foreach(Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 2)
                {
                    atrP = a;
                }
                if (a.TipoIndice == 3)
                {
                    atrS = a;
                }
                if (a.TipoIndice == 4)
                {
                    atrH = a;
                }
            }
            
            for (int i = 0; i < Entidades[entReg].lAtributos.Count; i++) {
                if (Entidades[entReg].lAtributos[i].TipoIndice == 2) {
                    EliminaPrimario(claveP, atrP);
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 3) {
                    EliminaSecundario(claveS, atrS, dirD);
                }
                if (Entidades[entReg].lAtributos[i].TipoIndice == 4) {
                    EliminaHash(claveH);
                }
            }
        }

        private void EliminaPrimario(string clave, Atributo a)
        {
            if (a.DirIndice != -1)
            {
                dataGridView5.Rows.Clear();
                for(int i = 0; i < Entidades[entReg].indPrimario.Count; i++)
                {
                    if (clave == Entidades[entReg].indPrimario[i].clave.ToString())
                    {
                        Entidades[entReg].indPrimario[i].apuntador = -1;
                        if (a.TipoAtributo == 'E')
                        {
                            Entidades[entReg].indPrimario[i].clave = Int32.Parse("10000");
                        } else
                        {
                            Entidades[entReg].indPrimario[i].clave = "Z";
                        }
                    }
                }
                Entidades[entReg].indPrimario = Entidades[entReg].indPrimario.OrderBy(p => p.clave).ToList();
                GuardaIndiceMod(a.DirIndice, a.TipoAtributo, a.Longitud);
                foreach(indiceP index in Entidades[entReg].indPrimario)
                {
                    if (a.TipoAtributo == 'E')
                    {
                        if (index.clave.ToString() == "10000")
                        {
                            dataGridView5.Rows.Add("", "");
                        } else
                        {
                            dataGridView5.Rows.Add(index.clave, index.apuntador);
                        }
                    } else
                    {
                        if (index.clave.ToString() == "Z")
                        {
                            dataGridView5.Rows.Add("", "");
                        }
                        else
                        {
                            dataGridView5.Rows.Add(index.clave, index.apuntador);
                        }
                    }
                }
            }
        }
        private void EliminaSecundario(string clave, Atributo a, long dirDato)
        {
            if (a.DirIndice != -1)
            {
                //dataGridView6.Rows.Clear();
                for(int i = 0; i < Entidades[entReg].indSecundario.Count; i++)
                {
                    if (clave == Entidades[entReg].indSecundario[i].clave.ToString())
                    {
                        int llenos = numCajonesIngSec(Entidades[entReg].indSecundario[i].cajones);
                        if (llenos > 1)
                        {
                            for(int j = 0; j < Entidades[entReg].indSecundario[i].cajones.Count; j++)
                            {
                                if (dirDato == Entidades[entReg].indSecundario[i].cajones[j])
                                {
                                    Entidades[entReg].indSecundario[i].cajones[j] = 100000;
                                    Entidades[entReg].indSecundario[i].cajones = Entidades[entReg].indSecundario[i].cajones.OrderBy(p => p).ToList();
                                }
                            }
                        } else
                        {
                            dataGridView6.Rows.Clear();
                            for (int x = 0; x < Entidades[entReg].indSecundario.Count; x++)
                            {
                                if (clave == Entidades[entReg].indSecundario[x].clave.ToString())
                                {
                                    Entidades[entReg].indSecundario[x].apuntador = -1;
                                    if (a.TipoAtributo == 'E')
                                    {
                                        Entidades[entReg].indSecundario[x].clave = Int32.Parse("10000");
                                    }
                                    else
                                    {
                                        Entidades[entReg].indSecundario[x].clave = "Z";
                                    }
                                }
                            }
                            Entidades[entReg].indSecundario = Entidades[entReg].indSecundario.OrderBy(p => p.clave).ToList();
                            foreach (indiceS index in Entidades[entReg].indSecundario)
                            {
                                if (a.TipoAtributo == 'E')
                                {
                                    if (index.clave.ToString() == "10000")
                                    {
                                        dataGridView6.Rows.Add("", "");
                                    }
                                    else
                                    {
                                        dataGridView6.Rows.Add(index.clave, index.apuntador);
                                    }
                                }
                                else
                                {
                                    if (index.clave.ToString() == "Z")
                                    {
                                        dataGridView6.Rows.Add("", "");
                                    }
                                    else
                                    {
                                        dataGridView6.Rows.Add(index.clave, index.apuntador);
                                    }
                                }
                            }
                        }
                    }
                }
                GuardaSecMod(a);
            }
        }
        private void EliminaHash(string clave)
        {
            int numCajonH = getHash(clave);
            Entidades[entReg].indHash[numCajonH - 1].dirDatos.Remove(clave);
            Entidades[entReg].indHash[numCajonH - 1].dirDatos = Entidades[entReg].indHash[numCajonH - 1].dirDatos.OrderBy(p => p.Key).ToDictionary(e => e.Key, e => e.Value);
        }

        private int numCajonesIngSec(List<long> cajones)
        {
            int num = 0;
            foreach(long l in cajones)
            {
                if (l != 100000)
                {
                    num++;
                }
            }
            return num;
        }

        bool sePuedeModificar = true;
        private void ModificaRegistro(Entidad e) {
            if (sePuedeModificar)
            {
                Registro reg = new Registro();
                reg.DirRegistro = e.Datos[indReg].DirRegistro;
                reg.DirSiguienteReg = e.Datos[indReg].DirSiguienteReg;
                for (int j = 0; j < nA; j++)
                {
                    if (e.lAtributos[j].TipoAtributo == 'E')
                    {
                        reg.Registros.Add((Int32.Parse(auxR[j])));
                    }
                    else
                    {
                        reg.Registros.Add(auxR[j]);
                    }
                }
                e.Datos.RemoveAt(indReg);
                e.Datos.Add(reg);
                VerificaRegistros(e);
                ActualizaDataGrid3(e);
            }
            else
                MessageBox.Show("Imposible modificar registro, la nueva clave primaria ya existe.", "Modificar Registro");
        }

        /// <summary>
        /// MÉTODO PARA REDIBUJAR EL DATAGRID CUANDO SE HAGAN CAMBIOS EN EL SISTEMA.
        /// </summary>
        private void ActualizaDataGrid() {
            dataGridView1.Rows.Clear();
            comboBox1.Items.Clear();
            comboBox6.Items.Clear();
            foreach(Entidad e in Entidades) {
                string n = NombreString(e.nombreEntidad);
                comboBox1.Items.Add(n);
                comboBox6.Items.Add(n);
                dataGridView1.Rows.Add(e.nombreS, e.DirEntidad, e.DirAtributo, e.DirDato, e.DirSiguienteEnt);
            }
        }

        /// <summary>
        /// MÉTODO PARA VISUALIZAR SOLO LOS ATRIBUTOS DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="c">Entidad seleccionada.</param>
        private void ActualizaDataGrid2(char[] c) {
            dataGridView2.Rows.Clear();
            comboBox4.Items.Clear();
            //comboBox5.Items.Clear();
            foreach (Entidad e in Entidades) {
                if (isEquals(e.nombreEntidad, c)) {
                    foreach(Atributo a in e.lAtributos) {
                        string n = NombreString(a.nombreAtributo);
                        dataGridView2.Rows.Add(n, a.DirAtributo, a.TipoAtributo, a.Longitud, a.TipoIndice, a.DirIndice, a.DirSiguienteAtr);
                        comboBox4.Items.Add(n);
                        //comboBox5.Items.Add(n);
                    }
                }
            }
        }

        /// <summary>
        /// MÉTODO PARA VISUALIZAR LOS DATOS DE LOS REGISTROS DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        /// <param name="e"></param>
        private void ActualizaDataGrid3(Entidad e) {
            dataGridView3.Visible = true;
            dataGridView3.Rows.Clear();
            comboBox5.Items.Clear();
            for(int i = 0; i < e.Datos.Count; i++) {
                int rowCount = dataGridView3.Rows.Count - 1;
                int colCount = dataGridView3.Columns.Count - 1;
                dataGridView3.Rows.Add(1);
                dataGridView3.Rows[rowCount].Cells[0].Value = e.Datos[i].DirRegistro;
                for (int j = 0; j < nA; j++) {
                    dataGridView3.Rows[rowCount].Cells[j + 1].Value = e.Datos[i].Registros[j];
                }
                dataGridView3.Rows[rowCount].Cells[colCount].Value = e.Datos[i].DirSiguienteReg;
            }
            foreach(Atributo a in e.lAtributos) {
                comboBox5.Items.Add(a.nombreSA);
            }
        }

        /// <summary>
        /// MÉTODO PARA CREAR EL ARCHIVO DE DATOS DE LA ENTIDAD SELECCIONADA.
        /// </summary>
        public string nomEnt;
        private void CreaArchivoDato() {
            nomEnt = "";
            Console.WriteLine(comboBox6.SelectedIndex);
            if (Entidades[comboBox6.SelectedIndex].DirDato == -1) {
                dataGridView3.Visible = true;
                dataGridView3.Rows.Clear();
                dataGridView3.Columns.Clear();
                string dataName = "";
                Entidad ex = null;
                for (int i = 0; i < Entidades.Count; i++) {
                    if (Entidades[i].nombreS == entS2) {
                        ex = Entidades[i];
                        dataName = Entidades[i].nombreS;
                        nomEnt = Entidades[i].nombreS;
                        nomEnt += ".idx";
                        dataName += ".dat";
                    }
                }
                //Console.WriteLine(dataName);
                ex.Ruta = dataName;
                ex.Registro = File.Create(dataName);
                ex.Registro.Close();
            } else {
                MessageBox.Show("Ya existe un archivo de datos de la entidad seleccionada.", "Crear Registros", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// MÉTODO PARA GENERAR LAS COLUMNAS DE LOS REGISTROS DE MANERA DINÁMICA.
        /// </summary>
        private void CreaColumnasRegistro() {
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView4.Rows.Clear();
            dataGridView4.Columns.Clear();
            int numAtr = 0;
            Entidad ex = null;
            for(int i = 0; i < Entidades.Count; i++) {
                if (Entidades[i].nombreS == entS2) {
                    numAtr = Entidades[i].lAtributos.Count;
                    nA = numAtr;
                    ex = Entidades[i];
                }
            }
            dataGridView3.Columns.Add("DirRegistro", "DirRegistro");
            dataGridView4.Columns.Add("DirRegistro", "DirRegistro");
            for (int i = 0; i < numAtr; i++) {
                DataGridViewColumn columna = new DataGridViewColumn();
                columna.Name = ex.lAtributos[i].nombreSA;
                columna.HeaderText = ex.lAtributos[i].nombreSA;
                columna.ReadOnly = true;
                columna.CellTemplate = dataGridView3.Columns[0].CellTemplate;
                dataGridView3.Columns.Add(columna);
                DataGridViewColumn columna2 = new DataGridViewColumn();
                columna2.Name = ex.lAtributos[i].nombreSA;
                columna2.HeaderText = ex.lAtributos[i].nombreSA;
                columna2.ReadOnly = true;
                columna2.CellTemplate = dataGridView3.Columns[0].CellTemplate;
                dataGridView4.Columns.Add(columna2);
            }
            dataGridView3.Columns.Add("DirRegistroSig", "DirRegistroSig");
            dataGridView4.Columns.Add("DirRegistroSig", "DirRegistroSig");
        }
        
        /// <summary>
        /// MÉTODO QUE COMPARA SI SON IGUALES DOS ARREGLOS DE CARACTERES.
        /// </summary>
        /// <param name="a">Primer arreglo de caracteres.</param>
        /// <param name="b">Segundo arreglo de caracteres.</param>
        /// <returns></returns>
        private bool isEquals(char[] a, char[] b) {
            bool aux = true;

            for(int i = 0; i < 30; i++) {
                if (a[i] != b[i]) {
                    return false;
                }
            }

            return aux;
        }

        /// <summary>
        /// MÉTODO PARA SABER SI LA ENTIDAD A AGREGAR YA EXISTE EN EL DICCIONARIO DE DATOS.
        /// </summary>
        /// <param name="n">Nombre de la entidad a buscar.</param>
        /// <returns></returns>
        private bool ifExists(string n) {
            for(int i = 0; i < Entidades.Count; i++) {
                if(string.Compare(Entidades[i].nombreS, n) == 0) {
                    return true;
                }
            }
            return false;
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buttonbusdat_Click(object sender, EventArgs e)
        {
            dataGridView4.Rows.Clear();
            foreach (Registro r in Entidades[entReg].Datos)
            {
                foreach(var s in r.Registros)
                {
                    if (s.ToString() == textbusqdat.Text)
                    {
                        //DataGridViewRow renglon = new DataGridViewRow();
                        int rowCount = dataGridView4.Rows.Count - 1;
                        dataGridView4.Rows.Add(1);
                        dataGridView4.Rows[rowCount].Cells[0].Value = r.DirRegistro;
                        dataGridView4.Rows[rowCount].Cells[dataGridView4.Columns.Count - 1].Value = r.DirSiguienteReg;
                        for (int i = 0; i < r.Registros.Count; i++)
                        {
                            dataGridView4.Rows[rowCount].Cells[i + 1].Value = r.Registros[i].ToString();
                        }
                        
                    }
                }
            }
        }
        /***************************Métodos de Apoyo***************************/
        public void tamIndPrima(int tamClave)
        {
            int valor, resultado;
            valor = 1048 / (tamClave + 8);
            resultado = valor * (tamClave + 8) + 8;
            if (resultado > 1048)
            {
                valor += -1;
                resultado = valor * (tamClave + 8) + 8;
            }
            NumCajPrim = valor;
             
        }
        public void tamIndSec(int tamClave)
        {
            int valor, resultado;
            valor = 1048 / (tamClave + 8);
            resultado = valor * (tamClave + 8) + 8;
            if (resultado > 1048)
            {
                valor += -1;
                resultado = valor * (tamClave + 8) + 8;
            }
            NumCajSec = valor;
        }

        FileStream arcInd;
        private void inicializaIndicePrimario()
        {
            int tam = 0;
            Atributo atr = new Atributo();
            foreach(Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 2)
                {
                    tam = a.Longitud;
                    atr = a;
                }
            }
            Console.WriteLine("Tam: " + tam);
            if (tam != 0)
            {
                tamIndPrima(tam);
                if (!File.Exists(nomEnt))
                {
                    arcInd = File.Create(nomEnt);
                    arcInd.Close();
                }
                arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                arcInd.Position = arcInd.Length;
                atr.DirIndice = arcInd.Length;
                ModificaAtributo(atr);
                ActualizaDataGrid2(Entidades[entReg].nombreEntidad);
                BinaryWriter writer = new BinaryWriter(arcInd);
                for(int i = 0; i < NumCajPrim; i++)
                {
                    if (tam == 4)
                    {
                        indiceP aux = new indiceP(true);
                        Entidades[entReg].indPrimario.Add(aux);
                        int entero = 0;
                        writer.Write(entero);
                        writer.Write(aux.apuntador);
                    }
                    else
                    {
                        indiceP aux = new indiceP(false);
                        Entidades[entReg].indPrimario.Add(aux);
                        char[] cadena = new char[tam];
                        int ind = 0;
                        foreach (char c in (string)aux.clave)
                        {
                            cadena[ind] += c;
                            ind++;
                        }
                        writer.Write(cadena);
                        writer.Write(aux.apuntador);
                    }
                }
                long cabe = -1;
                writer.Write(cabe);
                arcInd.Close();
            }
        }
        private void inicializaIndiceSecundario()
        {
            int tam = 0;
            Atributo atr = new Atributo();
            foreach (Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 3)
                {
                    tam = a.Longitud;
                    atr = a;
                }
            }
            Console.WriteLine("Tam: " + tam);
            if(tam != 0)
            {
                tamIndSec(tam);
                if (!File.Exists(nomEnt))
                {
                    arcInd = File.Create(nomEnt);
                    arcInd.Close();
                }
                arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                arcInd.Position = arcInd.Length;
                atr.DirIndice = arcInd.Length;
                ModificaAtributo(atr);
                ActualizaDataGrid2(Entidades[entReg].nombreEntidad);
                BinaryWriter writer = new BinaryWriter(arcInd);
                for(int i = 0; i < NumCajSec; i++)
                {
                    if (tam == 4)
                    {
                        indiceS aux = new indiceS(true);
                        Entidades[entReg].indSecundario.Add(aux);
                        int entero = 0;
                        writer.Write(entero);
                        writer.Write(aux.apuntador);
                    }
                    else
                    {
                        //indiceP aux = new indiceP(false);
                        //Entidades[entReg].indPrimario.Add(aux);
                        indiceS aux = new indiceS(false);
                        Entidades[entReg].indSecundario.Add(aux);
                        char[] cadena = new char[tam];
                        int ind = 0;
                        foreach (char c in (string)aux.clave)
                        {
                            cadena[ind] += c;
                            ind++;
                        }
                        writer.Write(cadena);
                        writer.Write(aux.apuntador);
                    }
                }
                long cabe = -1;
                writer.Write(cabe);
                arcInd.Close();
            }
        }
        private void inicializaHash()
        {
            int tam = 0;
            Atributo atr = new Atributo();
            foreach (Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 4)
                {
                    tam = a.Longitud;
                    atr = a;
                }
            }
            Console.WriteLine("Tam: " + tam);
            if (tam == 4 && atr.TipoAtributo == 'E')        //es un atributo entero únicamente       
            {
                dataGridView8.Rows.Clear();
                for(int i = 0; i < 7; i++)
                {
                    Entidades[entReg].indHash.Add(new indiceH((i + 1).ToString()));
                }
                if (!File.Exists(nomEnt))
                {
                    arcInd = File.Create(nomEnt);
                    arcInd.Close();
                }
                arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                arcInd.Position = arcInd.Length;
                atr.DirIndice = arcInd.Length;
                ModificaAtributo(atr);
                ActualizaDataGrid2(Entidades[entReg].nombreEntidad);
                BinaryWriter writer = new BinaryWriter(arcInd);
                for(int ca = 0; ca < 7; ca++)
                {
                    long d = -1;
                    writer.Write(d);
                }
                for (int j = 0; j < 7; j++)
                {
                    arcInd.Position = arcInd.Length;
                    Entidades[entReg].indHash[j].apuntador = arcInd.Length;
                    
                    dataGridView8.Rows.Add(Entidades[entReg].indHash[j].cajon, (j * 1040) + 56);
                    long l = -1;
                    for (int i = 0; i < 86; i++)
                    {
                        int n = 0;
                        long d = -1;
                        writer.Write(n);
                        writer.Write(d);
                    }
                    writer.Write(l);
                }
                arcInd.Close();
            }
        }

        private void AgregaIndiceP(long dirReg, string clave)
        {
            dataGridView5.Rows.Clear();
            int tam = 0;
            Atributo atr = new Atributo();
            foreach (Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 2)
                {
                    tam = a.Longitud;
                    atr = a;
                }
            }
            if (tam != 0)
            {
                int numReg = Entidades[entReg].Datos.Count;     // número de registros almecenados
                Console.WriteLine("Ya almacenados: " + numReg);
                if (numReg < NumCajPrim)                        // aún hay espacio disponible
                {
                    Entidades[entReg].indPrimario[numReg].apuntador = dirReg;
                    if (atr.TipoAtributo == 'E')
                    {
                        Entidades[entReg].indPrimario[numReg].clave = Int32.Parse(clave);
                    } else
                    {
                        Entidades[entReg].indPrimario[numReg].clave = clave;
                    }
                    
                }
                Entidades[entReg].indPrimario = Entidades[entReg].indPrimario.OrderBy(p => p.clave).ToList();
                
                arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                arcInd.Position = atr.DirIndice;
                BinaryWriter writer = new BinaryWriter(arcInd);
                for (int i = 0; i < Entidades[entReg].indPrimario.Count; i++)
                {
                    if (atr.TipoAtributo == 'E')
                    {
                        int entero = Int32.Parse(Entidades[entReg].indPrimario[i].clave.ToString());
                        writer.Write(entero);
                        writer.Write(Entidades[entReg].indPrimario[i].apuntador);
                        if (entero != 10000)
                        {
                            dataGridView5.Rows.Add(entero, Entidades[entReg].indPrimario[i].apuntador);
                        } else
                        {
                            dataGridView5.Rows.Add("", "");
                        }
                    }
                    else
                    {
                        char[] cadena = new char[atr.Longitud];
                        int ind = 0;
                        foreach (char c in (string)Entidades[entReg].indPrimario[i].clave)
                        {
                            cadena[ind] += c;
                            ind++;
                        }
                        writer.Write(cadena);
                        writer.Write(Entidades[entReg].indPrimario[i].apuntador);
                        if (cadena[0] != 'Z')
                        {
                            dataGridView5.Rows.Add(Entidades[entReg].indPrimario[i].clave.ToString(), Entidades[entReg].indPrimario[i].apuntador);
                        } else
                        {
                            dataGridView5.Rows.Add("", "");
                        }
                    }
                }
                arcInd.Close();
            }
        }
        private void AgregaIndiceS(long dirReg, string clave)
        {
            
            int tam = 0;
            long dirCajonSec = -1;
            Atributo atr = new Atributo();
            foreach (Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 3)
                {
                    tam = a.Longitud;
                    atr = a;
                }
            }
            if (tam != 0)
            {
                int numReg = cajonesSecundariosLlenos(atr.TipoAtributo);

                if (!ifIndexExists(clave))       //se tiene que crear el cajon secundario
                {
                    dataGridView6.Rows.Clear();
                    arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                    //arcInd.Position = arcInd.Length;
                    dirCajonSec = arcInd.Length;        //direccion donde se almacena el bloque secundario
                    Entidades[entReg].indSecundario[numReg].apuntador = dirCajonSec;
                    if (atr.TipoAtributo == 'E')
                    {
                        Entidades[entReg].indSecundario[numReg].clave = Int32.Parse(clave);
                    }
                    else
                    {
                        Entidades[entReg].indSecundario[numReg].clave = clave;
                    }
                    Entidades[entReg].indSecundario = Entidades[entReg].indSecundario.OrderBy(p => p.clave).ToList();

                    arcInd.Position = atr.DirIndice;
                    BinaryWriter writer = new BinaryWriter(arcInd);
                    for(int i = 0; i < Entidades[entReg].indSecundario.Count; i++)
                    {
                        if (atr.TipoAtributo == 'E')
                        {
                            int entero = Int32.Parse(Entidades[entReg].indSecundario[i].clave.ToString());
                            writer.Write(entero);
                            writer.Write(Entidades[entReg].indSecundario[i].apuntador);
                            if (entero != 10000)
                            {
                                dataGridView6.Rows.Add(entero, Entidades[entReg].indSecundario[i].apuntador);
                            }
                            else
                            {
                                dataGridView6.Rows.Add("", "");
                            }
                        }
                        else
                        {
                            char[] cadena = new char[atr.Longitud];
                            int ind = 0;
                            foreach (char c in (string)Entidades[entReg].indSecundario[i].clave)
                            {
                                cadena[ind] += c;
                                ind++;
                            }
                            writer.Write(cadena);
                            writer.Write(Entidades[entReg].indSecundario[i].apuntador);
                            if (cadena[0] != 'Z')
                            {
                                dataGridView6.Rows.Add(Entidades[entReg].indSecundario[i].clave.ToString(), Entidades[entReg].indSecundario[i].apuntador);
                            }
                            else
                            {
                                dataGridView6.Rows.Add("", "");
                            }
                        }
                    }
                    arcInd.Position = arcInd.Length;
                    for(int i = 0; i < 131; i++)         //si se requieren más cambiar a 131
                    {
                        long datoSe = 100000;
                        writer.Write(datoSe);
                    }
                    arcInd.Close();
                    //meter la clave al bloque secundario
                    IngresaClaveSecundaria(clave, dirReg);
                }
                else                            //el cajon secundario ya fue creado
                {
                    IngresaClaveSecundaria(clave, dirReg);
                }
            }
        }
        private int cajonesSecundariosLlenos(char tipoI)
        {
            int num = 0;
            foreach(indiceS index in Entidades[entReg].indSecundario)
            {
                if (tipoI == 'E')
                {
                    if ((int)index.clave != 10000)
                    {
                        num++;
                    }
                }
                else
                {
                    if (index.clave.ToString() != "Z")
                    {
                        num++;
                    }
                }
            }
            return num;
        }

        private void dataGridView6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private bool ifIndexExists(string clave)
        {
            foreach(indiceS index in Entidades[entReg].indSecundario)
            {
                if (clave == index.clave.ToString())
                {
                    return true;
                }
            }
            return false;
        }

        private void dataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int pos = dataGridView6.CurrentRow.Index;
            dataGridView7.Rows.Clear();
            for (int i = 0; i < 131; i++)
            {
                if (Entidades[entReg].indSecundario[pos].cajones[i] == 100000)
                {
                    dataGridView7.Rows.Add("");
                }
                else
                {
                    dataGridView7.Rows.Add(Entidades[entReg].indSecundario[pos].cajones[i]);
                }
            }
        }
        
        private void IngresaClaveSecundaria(string clave, long dirReg)
        {
            for(int i = 0; i < Entidades[entReg].indSecundario.Count; i++)
            {
                if (clave == Entidades[entReg].indSecundario[i].clave.ToString())
                {
                    int numBloqSec = cajonesBloqueSecundarioIS(Entidades[entReg].indSecundario[i]);
                    if(numBloqSec < 131)
                    {
                        //si es menor significa que puede insertarlo
                        Entidades[entReg].indSecundario[i].cajones[numBloqSec] = dirReg;
                        arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                        arcInd.Position = Entidades[entReg].indSecundario[i].apuntador;
                        BinaryWriter writer = new BinaryWriter(arcInd);
                        for (int j = 0; j < 131; j++)         //si se requieren más cambiar a 131
                        {
                            writer.Write(Entidades[entReg].indSecundario[i].cajones[j]);
                        }
                        arcInd.Close();
                    }
                }
            }
        }

        private void dataGridView8_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = dataGridView8.CurrentRow.Index;
            dataGridView9.Rows.Clear();
            indiceH ind = Entidades[entReg].indHash[index];
            
            foreach(string key in ind.dirDatos.Keys)
            {
                dataGridView9.Rows.Add(key, ind.dirDatos[key]);
            }
            
        }

        private int cajonesBloqueSecundarioIS(indiceS index)
        {
            int num = 0;
            foreach(long dat in index.cajones)
            {
                if(dat != 100000)
                {
                    num++;
                }
            }
            return num;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            int orden = comboBox7.SelectedIndex;
            int index = 0;
            for (int i = 0; i < Entidades[entReg].lAtributos.Count; i++)
            {
                if (Entidades[entReg].lAtributos[i].TipoIndice == (orden + 1))
                {      //cambiar a 1 para secuencial
                    index = i;
                }
            }
            OrdenaRegistros(Entidades[entReg], index, true);
        }

        private void tabPage6_Click(object sender, EventArgs e) {

        }

        /***************************Métodos de Apoyo***************************/

        private void AgregaIndiceH(string clave, long dirDato)
        {
            int tam = 0;
            Atributo atr = new Atributo();
            foreach (Atributo a in Entidades[entReg].lAtributos)
            {
                if (a.TipoIndice == 4)
                {
                    tam = a.Longitud;
                    atr = a;
                }
            }
            if (tam != 0)
            {
                int numCajonHash = getHash(clave);
                if (Entidades[entReg].indHash[numCajonHash - 1].apuntador != -1)        //ya esta creado
                {
                    if (!ifHashExists(clave))       //se puede agregar
                    {
                        Entidades[entReg].indHash[numCajonHash - 1].dirDatos.Add(clave, dirDato);
                        Entidades[entReg].indHash[numCajonHash - 1].dirDatos = Entidades[entReg].indHash[numCajonHash - 1].dirDatos.OrderBy(p => p.Key).ToDictionary(e => e.Key, e => e.Value);
                    }
                }
                else
                {
                    arcInd = File.Open(nomEnt, FileMode.Open, FileAccess.Write, FileShare.None);
                    arcInd.Position = arcInd.Length;
                    Entidades[entReg].indHash[numCajonHash - 1].apuntador = arcInd.Length;
                    BinaryWriter writer = new BinaryWriter(arcInd);
                    for (int i = 0; i < 86; i++)
                    {
                        int key = 0;
                        long dir = -1;
                        writer.Write(key);
                        writer.Write(dir);
                    }
                    arcInd.Close();
                    dataGridView8.Rows.Clear();
                    for (int z = 0; z < 7; z++)
                    {
                        dataGridView8.Rows.Add(Entidades[entReg].indHash[z].cajon, Entidades[entReg].indHash[z].apuntador);
                    }
                }
            }
        }
        private int getHash(string clave)
        {
            int f = 0;
            f = (Int32.Parse(clave) % 7) + 1;
            Console.WriteLine("Cajon: " + f);
            return f;
        }
        private bool ifHashExists(string clave)
        {
            foreach(indiceH index in Entidades[entReg].indHash)
            {
                foreach(string key in index.dirDatos.Keys)
                {
                    if (clave == key)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void ModificaHash(string clave, long dirDato)
        {
            string cveOri = "";
            foreach (indiceH indice in Entidades[entReg].indHash)
            {
                foreach(string key in indice.dirDatos.Keys)
                {
                    if (dirDato == indice.dirDatos[key])
                    {
                        cveOri = key;
                    }
                }
            }
            EliminaHash(cveOri);
            AgregaIndiceH(clave, dirDato);
        }
    }
}
