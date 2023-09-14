using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Pokemon
    {
        //COMO ESCRIBIR CORRECTAMENTE EL NOMBRE DE LA COLUMNA(ACENTOS, espacios, etc)
        [DisplayName("Número")]
        public int Numero { get; set; }   
        public string Nombre { get; set;}
        [DisplayName("Descripción")]
        public int Id { get; set; } 
        public string Descripcion { get; set;}
        public string UrlImagen { get; set;}  
        public Elemento Tipo { get; set;}
        public Elemento Debilidad { get; set;}
    }
}
