using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoLina.Commun
{
    class Usuario
    {     
         public string KeyId { set; get; } 
         public string Nombre { set; get; }
         public string Apellido { set; get; }
         public string FechaNacimineto { set; get; }
         public string CI { set; get; }
         public string NroMantricula { set; get;}
         public string Institucion { set; get; }
         public string Correo { set; get; }
         public string Password { set; get; }
    }
}
