using JuegoCuarto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JuegoCuarto
{
    public partial class Form1 : Form
    {
        //banderas para hacer aleatorio la primera jugada
        int primeraFichaBand = 0;
        int primeraCeldaBand = 0;
        
        Boton1[,] cel = new Boton1[4, 4];//matriz que contiene el tablero de juego
        //indices para la seleccion de la celda en el trablero
        int indiceH = -1;
        int indiceV = -1;

        string ficha;//string que contiene las caracteristicas de la ficha
        string[] fichas = new string[16];//vector que contiene las fichas disponibles en cada jugada
        int[] cntFichasH = new int[4];//almacenar la cantidad de fichas que se tiene en cada vector horizontal
        int[] cntFichasV = new int[4];//almacenar la cantidad de fichas que se tiene en cada vector vertical

        string[] vp = new string[4];//vector de caracteristicas de la ficha asociada a una celda

        int[] cvh = new int[4];
        int[] cvv = new int[4];

        
        
        public Form1()
        {
            InitializeComponent();
            CrearTablero();
            CargarFichas();
            primeraFichaBand = 1;
            primeraCeldaBand = 1;

            if (SeleccionarPrimerJugador())
            {
                //COMIENZA LA MAQUINA
                ActivarInactivarFichas(false);         
                SeleccionarFicha();
            }
            else 
            {
                //COMIENZA JUGANDO EL USUARIO
                lbl_jugador.Text = "Usuario selecciona ficha";
                ActivarInactivarFichas(true);
            }
        }

        private void ProcesarJugada()
        {
            TomarVectorCaracteristicasActual(ficha);
            ColocarFichaVertical();
            ColocarFichaHorizontal();
            ContarFichasVector();
            SeleccionarCelda();
            InutilizarFicha();
            lbl_jugador.Text = "Usuario coloca ficha";
        }

        private void ContarFichasVector()//con esta funcion se valida que vectores le falta una ficha para ganar
        {
            //vectores horizontales
            for (int i = 0; i < 4; i++)
            {
                cntFichasH[i] = 4;

                for (int j = 0; j < 4; j++)
                {
                    if (!String.IsNullOrEmpty(cel[i,j].Text))
                    {
                        cntFichasH[i] -= 1; 
                    }
                }

                if (cntFichasH[i] == 0)
                {
                    if (ValidarGanador(i, 1))
                    {
                        //entramos aqui hubo ganador
                    }
                }
            }


            //vectores verticales
            for (int j = 0; j < 4; j++)
            {
                cntFichasV[j] = 4;

                for (int i = 0; i < 4; i++)
                {
                    if (!String.IsNullOrEmpty(cel[i, j].Text))
                    {
                        cntFichasV[i] -= 1;
                    }
                }

                if (cntFichasV[j] == 0)
                {
                    if (ValidarGanador(j, 0))
                    {
                        //entramos aqui hubo ganador
                    }
                   
                }
            }
        }

        private void CargarFichas()
        {
            string fmt = "0000.##";
            for (int i = 0; i < 16; i++)
            {
                var binario = Int16.Parse(Convert.ToString(i, 2));
                fichas[i] = binario.ToString(fmt);
            }
        }

        private void SeleccionarFichaInicial()
        {
            var seed = Environment.TickCount;
            var random = new Random(seed + 333);
            var value = random.Next(0, 15);
            
            if (!string.IsNullOrEmpty(fichas[value]))
            {
                ficha = fichas[value];
                fichas[value] = null;
            }
            else
            {
                SeleccionarFichaInicial();
            }            
        }

        private void SeleccionarFicha()
        {
            if (primeraFichaBand == 1)//si es la primera jugada de la maquina
            {
                SeleccionarFichaInicial();
                primeraFichaBand = 0;
            }
            else
            {
                int[] totalFicha = new int[16];//ponderacion total de la ficha
                int[] minFicha = new int[16];//menor ponderacion de la ficha
                int[] maxFicha = new int[16];//mayor ponderacion de la ficha

                for (int i = 0; i < 16; i++)//evaluacion de ficha por ficha de las disponibles
                {
                    minFicha[i] = 4;
                    maxFicha[i] = 0;
                    if (!String.IsNullOrEmpty(fichas[i]))
                    {
                        //se puede usar esta ficha
                        TomarVectorCaracteristicasActual(fichas[i]);
                        ColocarFichaVertical(); //cvv[j]
                        ColocarFichaHorizontal(); //cvh[i]
                        ContarFichasVector(); //cntFichasH[i] y cntFichasV[j]

                        //validar los vectores que solo le faltan una ficha para ganar

                        //validando vectores horizontales que le falten 1 ficha
                        for (int j = 0; j < 4; j++)
                        {
                            if (cntFichasH[j] == 1 && cvh[j] > 0)//si el vector le falta una ficha y hay posiblidad de merge por la horizontal
                            {
                                //descartar la ficha totalmente
                                totalFicha[i] = 1000;
                                maxFicha[i] = 1000;
                            }
                            else if (cntFichasV[j] == 1 && cvv[j] > 0)//si el vector le falta una ficha y hay posiblidad de merge por la vertical
                            {
                                //descartar la ficha totalmente
                                totalFicha[i] = 1000;
                                maxFicha[i] = 1000;
                            }
                        }
                        if (totalFicha[i] != 1000)
                        {
                            //se continua el analisis de la ficha
                            foreach (var item in cvh)
                            {
                                totalFicha[i] = totalFicha[i] + item;//la suma total de posiblidades de  los vectores horizontales
                                minFicha[i] = item < minFicha[i] ? item : minFicha[i];
                                maxFicha[i] = item > maxFicha[i] ? item : maxFicha[i];
                            }

                            foreach (var item in cvv)
                            {
                                totalFicha[i] = totalFicha[i] + item;//la suma total de posibilidades de los vectores verticales
                                minFicha[i] = item < minFicha[i] ? item : minFicha[i];
                                maxFicha[i] = item > maxFicha[i] ? item : maxFicha[i];
                            }

                        }
                    }
                }

                var indiceFicha = 0;

                //recorremos nuevamente las fichas para buscar las qeu tienen menor ponderacion
                for (int i = 0; i < 16; i++)
                {
                    var total = 100;
                    var totalMax = 100;

                    if (!String.IsNullOrEmpty(fichas[i]))
                    {

                        if (totalFicha[i] < total && maxFicha[i] < totalMax)
                        {
                            indiceFicha = i;
                        }
                    }
                }

                ficha = fichas[indiceFicha];
                fichas[indiceFicha] = null;
            }

            /////////////////////////////

            var nombre = "img" + ficha;

            foreach (Control item in this.Controls)
            {
                if (item.Name == nombre)
                {
                    item.BackgroundImage = null;
                }
            }

            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");            
            lbl_jugador.Text = "Usuario coloca la ficha";
            ActivarInactivarFichas(true);
        }

        private void InutilizarFicha()
        {
            for (int i = 0; i < 16; i++)
            {
                if (ficha == fichas[i])
                {
                    fichas[i] = null;
                }
            }
        }

        private bool SeleccionarPrimerJugador()
        {
            var seed = Environment.TickCount;
            var random = new Random(seed);
            var value = random.Next(0, 5);

            return value % 2 == 0 ? true : false;            
        }

        private void ColocarFichaVertical()
        {

            string[,] vk = new string[4, 4];            

            for (int j = 0; j < 4; j++)//se recorren las filas
            {
                cvv[j] = 4;
                vk = new string[4, 4];

                bool igual = true;

                for (int i = 0; i < 4; i++)//se recorren las columnas
                {
                    if (!String.IsNullOrEmpty(cel[i, j].Text))
                    {
                        for (int k = 0; k < cel[i, j].Text.Length; k++)//se recorren las caracteristicas
                        {
                            vk[i, k] = cel[i, j].Text.Substring(k, 1);
                        }
                    }
                }

                //cuando termine el ciclo de j, se compararn los valores de k
                //compraramos los valores de k
                for (int k = 0; k < 4; k++)
                {
                    igual = true;
                    for (int i = 0; i < 4; i++)
                    {

                        if (!String.IsNullOrEmpty(vk[i, k]) && (vk[i, k] != vp[k]))
                        {
                            igual = false;
                        }
                    }

                    if (!igual)
                    {
                        cvv[j] -= 1;
                    }
                }
            }
        }
            
        private void ColocarFichaHorizontal()
        {
            
            string[,] vk = new string[4,4];
          

            for (int i = 0; i < 4; i++)//se recorren las filas
            {
                cvh[i] = 4;
                vk = new string[4, 4];

                bool igual = true;

                for (int j = 0; j < 4; j++)//se recorren las columnas
                {                    
                    if (!String.IsNullOrEmpty(cel[i, j].Text))
                    {
                        for (int k = 0; k < cel[i, j].Text.Length; k++)//se recorren las caracteristicas
                        {
                            vk[j,k] = cel[i, j].Text.Substring(k, 1);                                            
                        }
                    }
                }

                //cuando termine el ciclo de j, se compararn los valores de k
                //compraramos los valores de k
                for (int k = 0; k < 4; k++)
                {
                    igual = true;                    

                    for (int j = 0; j < 4; j++)
                    {

                        if (!String.IsNullOrEmpty(vk[j, k]) && (vk[j, k] != vp[k]) )
                        {
                            igual = false;
                        }
                    }

                    if (!igual)
                    {
                        cvh[i] -= 1;
                    }
                }
            }
        }
        
        private void SeleccionarCeldaInicial()
        {
            var seed = Environment.TickCount;
            var random = new Random(seed + 333);
            indiceH = random.Next(0, 3);

            seed = Environment.TickCount;
            random = new Random(seed + 999);
            indiceV = random.Next(0, 3);

        }
        
        //cntFichasH
        private void SeleccionarCelda()
        {
            int max = 0;
            
            ColocarFichaVertical();
            ColocarFichaHorizontal();
            ContarFichasVector();


            if (primeraCeldaBand == 1)
            {
                SeleccionarCeldaInicial();
                primeraCeldaBand = 0;
            }
            else
            {
                //validando vectores horizontales que le falten 1 ficha
                for (int i = 0; i < 4; i++)
                {
                    if (cntFichasH[i] == 1 && cvh[i] > 0)//si queda una fila horizontal con solo 1 ficha faltante y tiene por lo menos una caracteristica habilitada
                    {
                        indiceH = i;
                        for (int j = 0; j < 4; j++)
                        {
                            if (String.IsNullOrEmpty(cel[i, j].Text))
                            {
                                indiceV = j;
                            }
                        }
                    }
                }

                if (indiceH >= 0 && indiceV >= 0 && cel[indiceH, indiceV].Enabled)//si se tiene por lo menos un vector horizontal que le falta 1 ficha y que la ficha seleccionada haga match
                {
                    //se usa esa celda
                    cel[indiceH, indiceV].BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");
                    cel[indiceH, indiceV].Text = ficha;
                }
                else // de lo contrario, se busca por los vectores horizontales
                {
                    //validando vectores verticales
                    for (int j = 0; j < 4; j++)
                    {
                        if (cntFichasH[j] == 1 && cvh[j] > 0)
                        {
                            indiceH = j;
                            for (int i = 0; i < 4; i++)
                            {
                                if (String.IsNullOrEmpty(cel[i, j].Text))
                                {
                                    indiceV = i;
                                }

                            }
                        }
                    }
                }

                if (indiceH >= 0 && indiceV >= 0 && cel[indiceH, indiceV].Enabled)//si se tiene por lo menos un vector vertical que le falta 1 ficha y que la ficha seleccionada haga match
                {
                    //se usa esa celda
                    cel[indiceH, indiceV].BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");
                    cel[indiceH, indiceV].Text = ficha;
                }  //si tampoco se tiene por un vector vertical, se busca otra celda que tenga mayores posiblidades de exito
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (String.IsNullOrEmpty(cel[i, j].Text))
                            {
                                if ((cvh[i] + cvv[j]) > max)
                                {
                                    max = cvh[i] + cvv[j];
                                    //celda con mayor probabilidad
                                    indiceH = i;
                                    indiceV = j;
                                }
                            }
                        }
                    }
                }
            }

            cel[indiceH, indiceV].BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");
            cel[indiceH, indiceV].Text = ficha;
            cel[indiceH, indiceV].Enabled = false;

            ficha = null;
            img_act.BackgroundImage = null;  

            //////////////////////////////////////////
            ActivarInactivarFichas(false);
            //si la maquina empieza a jugar
            //la maquina selecciona una ficha

            SeleccionarFicha();
        }
        
        private void CrearTablero()
        {
            int x = 10;
            int y = 30;

            


            for (int i = 0; i < 4; i++)
            {                
                x = 50;

                for (int j = 0; j < 4; j++)
                {
                    Point newLoc = new Point(x, y);

                    Boton1 boton = new Boton1();
                    cel[i, j] = boton;
                    boton.BackColor = System.Drawing.SystemColors.ControlLight;
                    boton.BackgroundImage = null; //((System.Drawing.Image)(resources.GetObject("cel11.BackgroundImage")));
                    boton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                    boton.Location = newLoc;
                    boton.Name = "cel" + i.ToString() + j.ToString();
                    boton.Size = new System.Drawing.Size(140, 140);
                    boton.TabIndex = 16;
                    boton.TabStop = false;
                    boton.indice = Int32.Parse(i.ToString() + j.ToString());
                    boton.Click_ConIndice += Boton_Click_ConIndice;                    

                    Controls.Add(cel[i, j]);                   
                    x = x + 141;
                }
                y = y + 141;
            }
        }

        // Todos los botones ejecutaran esta función al dispararse el evento Click_ConIndice
        // Podremos saber cual de los botones ha sido pulsado mediante
        // e.valor
        void Boton_Click_ConIndice(object sender, IntEventArgs e)
        {

            //MessageBox.Show("Se ha pulsado el botón número: " +e.valor.ToString());
            Boton1 boton = new Boton1();
            boton.Name = "cel" + e.valor.ToString();

            if (e.valor >= 10 && ficha != null)
            {
                string indice = e.valor.ToString();
                var i = int.Parse(indice.Substring(0, 1));
                var j = int.Parse(indice.Substring(1, 1));

                cel[i, j].BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");
                cel[i, j].Enabled = false;
                cel[i, j].Text = ficha;                
            }
            else
            {
                if (ficha != null)
                {
                    cel[0, e.valor].BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");
                    cel[0, e.valor].Enabled = false;
                    cel[0, e.valor].Text = ficha;
                }                
            }

            ficha = null;
            img_act.BackgroundImage = null;
            lbl_jugador.Text = "Usuario selecciona ficha";
            ActivarInactivarFichas(true);
        }

        private void TomarVectorCaracteristicasActual(string valFicha)
        {
            vp[0] = valFicha.Substring(0, 1);
            vp[1] = valFicha.Substring(1, 1);
            vp[2] = valFicha.Substring(2, 1);
            vp[3] = valFicha.Substring(3, 1);
        }
                
        private void ActivarInactivarFichas(bool value)
        {
            img0000.Enabled = value;
            img0001.Enabled = value;
            img0010.Enabled = value;
            img0011.Enabled = value;
            img0100.Enabled = value;
            img0101.Enabled = value;
            img0110.Enabled = value;
            img0111.Enabled = value;
            img1000.Enabled = value;
            img1001.Enabled = value;
            img1010.Enabled = value;
            img1011.Enabled = value;
            img1100.Enabled = value;
            img1101.Enabled = value;
            img1110.Enabled = value;
            img1111.Enabled = value;
        }

        private bool ValidarGanador(int indice, int horizontal)
        {
            int[] caracteristica = new int[4];
            bool igualAnt = false;

            if (horizontal == 1)
            {
                for (int k = 0; k < 4; k++)
                {
                    for (int j  = 0; j < 4; j++)
                    {
                        if (j > 0)
                        {
                            if (cel[indice, j].Text.Substring(k, 1) == cel[indice, j - 1].Text.Substring(k, 1))
                            {
                                igualAnt = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (igualAnt) //si la corrida termino en true
                    {
                        break;
                    }
                }
            }
            else //vertical
            {
                for (int k = 0; k < 4; k++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (i > 0)
                        {
                            if (cel[i, indice].Text.Substring(k, 1) == cel[i, indice - 1].Text.Substring(k, 1))
                            {
                                igualAnt = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (igualAnt) //si la corrida termino en true
                    {
                        break;                        
                    }
                }
            }
            
            return igualAnt;
        }

        #region EventosControlFicha

        private void img0000_Click(object sender, EventArgs e)
        {
            ficha = "0000";
            img0000.BackgroundImage = null;
            img0000.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }        
        
        private void img0001_Click(object sender, EventArgs e)
        {
            ficha = "0001";
            img0001.BackgroundImage = null;
            img0001.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img0010_Click(object sender, EventArgs e)
        {
            ficha = "0010";
            img0010.BackgroundImage = null;
            img0010.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img0011_Click(object sender, EventArgs e)
        {
            ficha = "0011";
            img0011.BackgroundImage = null;
            img0011.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img0100_Click(object sender, EventArgs e)
        {
            ficha = "0100";
            img0100.BackgroundImage = null;
            img0100.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img0101_Click(object sender, EventArgs e)
        {
            ficha = "0101";
            img0101.BackgroundImage = null;
            img0101.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img0110_Click(object sender, EventArgs e)
        {
            ficha = "0110";
            img0110.BackgroundImage = null;
            img0110.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img0111_Click(object sender, EventArgs e)
        {
            ficha = "0111";
            img0111.BackgroundImage = null;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1000_Click(object sender, EventArgs e)
        {
            ficha = "1000";
            img1000.BackgroundImage = null;
            img1000.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1001_Click(object sender, EventArgs e)
        {
            ficha = "1001";
            img1001.BackgroundImage = null;
            img1001.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1010_Click(object sender, EventArgs e)
        {
            ficha = "1010";
            img1010.BackgroundImage = null;
            img1010.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1011_Click(object sender, EventArgs e)
        {
            ficha = "1011";
            img1011.BackgroundImage = null;
            img1011.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1100_Click(object sender, EventArgs e)
        {
            ficha = "1100";
            img1100.BackgroundImage = null;
            img1100.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1101_Click(object sender, EventArgs e)
        {
            ficha = "1101";
            img1101.BackgroundImage = null;
            img1101.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1110_Click(object sender, EventArgs e)
        {
            ficha = "1110";
            img1110.BackgroundImage = null;
            img1110.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        private void img1111_Click(object sender, EventArgs e)
        {
            ficha = "1111";
            img1111.BackgroundImage = null;
            img1111.Enabled = false;
            img_act.BackgroundImage = Image.FromFile("../../img/" + ficha + ".jpg");

            ProcesarJugada();
        }

        #endregion
    }

}
