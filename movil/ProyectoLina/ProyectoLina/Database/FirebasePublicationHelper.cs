using Firebase.Database;
using Firebase.Database.Query;
using ProyectoLina.Commun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ProyectoLina.Database
{
    class FirebasePublicationHelper
    {
        public readonly string Child = "Publicacion";
        readonly FirebaseClient firebase = new FirebaseClient("https://linaemotional.firebaseio.com/");
        
        public async Task AgregarPublicasion(string idkeyU, string descripsion,string EstadoAnimoe, string rangoEdad)
        {
            try
            {
                await firebase
                    .Child(Child)
                    .PostAsync(new Publicacion()
                    {
                        IdKeyU = idkeyU,
                        Descripcion = descripsion,
                        TipoEstadoAnimo = EstadoAnimoe,
                        RangoEdad = rangoEdad,
                        Megusta="0",NoMegusta="0",Habilitado="1"
                    });
            }
            catch (Exception ex)
            {

                Log.Warning("FirebaseUserHelper", "no se pudo agregar coretamete la publicacion" + ex.Message);
                throw;
            }
        }

        public async Task DelatePublicasion(string idkeyp)
        {
            try
            {
                var ToDeletePublicasion = (await firebase.Child(Child).OnceAsync<Publicacion>()).FirstOrDefault(a => a.Key == idkeyp);
                await firebase.Child(Child).Child(ToDeletePublicasion.Key).DeleteAsync();
            }
            catch (Exception ex )
            {
                Log.Warning("FirebaseUserHelper", "no se pudo eliminar " + ex.Message);
                throw;
            }
        }

        public async Task<List<Publicacion>> GetAllPublications(string id)
        {
            try
            {
                return (await firebase
                .Child(Child)
                .OnceAsync<Publicacion>()).Select(item => new Publicacion
                {
                    IdKeyP = item.Key,
                    IdKeyU = item.Object.IdKeyU,
                    Descripcion = item.Object.Descripcion,
                    TipoEstadoAnimo = item.Object.TipoEstadoAnimo,
                    RangoEdad = item.Object.RangoEdad,
                    Megusta = item.Object.Megusta,
                    NoMegusta = item.Object.NoMegusta,
                    Vistas = (int.Parse(item.Object.Megusta)+int.Parse(item.Object.NoMegusta)).ToString(),
                    Habilitado = item.Object.Habilitado
                }).Where(x => x.IdKeyU == id && x.Habilitado == "1").ToList();
            }
            catch (Exception ex)
            {
                Log.Warning("FirebaseUserHelper", "no se pudo verificar si el hay un usarios" + ex.Message);
                throw;
            }
        }
    }
}
