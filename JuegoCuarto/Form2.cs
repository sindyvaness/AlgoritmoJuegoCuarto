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
    public partial class Form2 : Form
    {
        public Boton1[] boton2Array;
        int numeroBotones;

        public Form2()
        {
            InitializeComponent();
            numeroBotones = 10;

            boton2Array = new Boton1[numeroBotones];

            int i;

            // Creamos en tiempo de ejecución los 10 botones
            for (i = numeroBotones; i > 0; i--)
                creaBoton(i - 1);
        }

        void creaBoton(int i)
        {

            Boton1 boton = new Boton1();  // Se crea un nuevo objeto tipo Boton1
            boton2Array[i] = boton;       // Lo ponemos en el array
            boton.indice = i;              // Le asignamos a la propiedad indice el número de boton
            boton.Text = i.ToString();
            boton.Location = new System.Drawing.Point(20, i * 20);

            // Cuando se dispare el evento Click_ConIndice del botón que acabamos de crear,  se ejecutará
            // la función Boton_Click_ConIndice que recibirá el índice del botón

            boton.Click_ConIndice += Boton_Click_ConIndice;

            // Se añade el botón creado al formulario
            this.Controls.Add(this.boton2Array[i]);

        }

        // Todos los botones ejecutaran esta función al dispararse el evento Click_ConIndice
        // Podremos saber cual de los botones ha sido pulsado mediante
        // e.valor
        void Boton_Click_ConIndice(object sender, IntEventArgs e)
        {

            MessageBox.Show("Se ha pulsado el botón número: " +e.valor.ToString());

        }

    }

    //public class IntEventArgs : EventArgs
    //{
    //    public IntEventArgs(int valor)
    //    {
    //        this.valor = valor;
    //    }
    //    public int valor;
    //}

    //// Creamos un objeto Boton1 que hereda todas las propiedades del objeto Button de Windows Forms y al que modificamos
    //// el evento On_Click para que responda con el número de índice
    //public class Boton1 : System.Windows.Forms.Button
    //{

    //    public int indice;   // Esta variable será la que contenga el índice dentro del vector de objetos

    //    public int categoria;  // En esta variable le indicamos a qué categoría pertenece

    //    public delegate void IntEventHandler(object sender, IntEventArgs e);  // Puntero hacia la clase IntEventHandler.

    //    public event IntEventHandler Click_ConIndice; // Evento

    //    public Boton1()

    //    { }

    //    //  Cambiamos el método OnClick original del objeto Button por este código, lo que se hace es que al pulsar el botón, se dispara este evento en vez del original

    //    //  y este evento contiene el índice para poder distinguirlo del resto de botones

    //    protected override void OnClick(EventArgs e)
    //    {

    //        IntEventArgs ex = new IntEventArgs(indice);

    //        if (Click_ConIndice != null)
    //            Click_ConIndice(this, ex);

    //    }

    //}

}
