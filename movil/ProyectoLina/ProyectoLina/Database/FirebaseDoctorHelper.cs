using Firebase.Database;
using ProyectoLina.Commun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ProyectoLina.Database
{
    class FirebaseDoctorHelper
    {
        public readonly string Child = "Medicos";
        readonly FirebaseClient firebase = new FirebaseClient("https://medicossc-38138.firebaseio.com/");
        public async Task<List<doctor>> GetAllMedicos()
        {
            try
            {
                return (await firebase.Child(Child).OnceAsync<doctor>()).Select(item => new doctor
                {
                    IdKey = item.Key,
                    Nombre = item.Object.Nombre,
                    Apellido = item.Object.Apellido,
                    Especialidad = item.Object.Especialidad,
                    NroMatricula = item.Object.NroMatricula
                }).ToList();
            }
            catch (Exception ex)
            {
                Log.Warning("FirebaseDiocoresHelper", "no se pudo cargar a al usuario " + ex.Message);
                throw;
            }
          
        }
        public async Task<doctor> GetNombreM(String Nombre)
        {
            try
            {
            
            var allPersons = await GetAllMedicos();
            await firebase
                .Child(Child)
                .OnceAsync<doctor>();
            return allPersons.FirstOrDefault(a => a.Nombre == Nombre);
        }
            catch (Exception ex )
            {
                Log.Warning("FirebaseDiocoresHelper", "no se pudo buscar al doctor pos su nombre  corectamente" + ex.Message);
                throw;
            }
        }
        public async Task<doctor> GetApelidoM(String Apellido)
        {
            try
            {
                var allPersons = await GetAllMedicos();
                await firebase
                    .Child(Child)
                    .OnceAsync<doctor>();
                return allPersons.FirstOrDefault(a => a.Apellido == Apellido);
            }
            catch (Exception ex) 
            {
                Log.Warning("FirebaseDiocoresHelper", "no se pudo buscar al doctor pos su Apellido   corectamente" + ex.Message);
                throw;
            }
           
        }
        public async Task<doctor> GetMatricula(String NroMatricula)
        {
            try
            {
                var allPersons = await GetAllMedicos();
                await firebase
                    .Child(Child)
                    .OnceAsync<doctor>();
                return allPersons.FirstOrDefault(a => a.NroMatricula == NroMatricula);
            }
            catch (Exception ex )
            {
                Log.Warning("FirebaseDiocoresHelper", "no se pudo buscar al doctor pos su nro Matricula   corectamente" + ex.Message);
                throw;
            }
           
        }




    }
}
