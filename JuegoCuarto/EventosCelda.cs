using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuegoCuarto
{
    // Creamos la clase IntEventArgs, derivada de EventArgs y que recibe una variable tipo int
    public class IntEventArgs : EventArgs
    {
        public IntEventArgs(int valor)
        {
            this.valor = valor;
        }
        public int valor;

    }

    // Creamos un objeto Boton1 que hereda todas las propiedades del objeto Button de Windows Forms y al que modificamos

    // el evento On_Click para que responda con el número de índice

    public class Boton1 : System.Windows.Forms.Button
    {

        public int indice;   // Esta variable será la que contenga el índice dentro del vector de objetos

        public int categoria;  // En esta variable le indicamos a qué categoría pertenece

        public delegate void IntEventHandler(object sender, IntEventArgs e);  // Puntero hacia la clase IntEventHandler.

        public event IntEventHandler Click_ConIndice; // Evento

        public Boton1()//constructor vacio
        { }


        //  Cambiamos el método OnClick original del objeto Button por este código, lo que se hace es que al pulsar el botón, se dispara este evento en vez del original
         //  y este evento contiene el índice para poder distinguirlo del resto de botones
        protected override void OnClick(EventArgs e)
        {

            IntEventArgs ex = new IntEventArgs(indice);

            if (Click_ConIndice != null)
                Click_ConIndice(this, ex);

        }

    }
    
}
