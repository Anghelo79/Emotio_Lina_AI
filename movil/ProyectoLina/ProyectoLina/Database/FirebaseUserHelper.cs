using Firebase.Database;
using Firebase.Database.Query;
using ProyectoLina.Commun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace ProyectoLina.Database
{
    class FirebaseUserHelper
    {
        public readonly string Child = "Usuario";
        readonly FirebaseClient firebase = new FirebaseClient("https://linaemotional.firebaseio.com/");
        public async Task AgregarUsuario(string Nombre, string Apellido,
               string FechaNacimineto, string CI, string NroMantricula, string Institucion,
                string Correo, string Password)
        {
            try
            {
                await firebase
               .Child(Child)
               .PostAsync(new Usuario() { Nombre = Nombre, Apellido = Apellido, CI = CI, Correo = Correo,
                   FechaNacimineto = FechaNacimineto,
                   Institucion = Institucion, NroMantricula = NroMantricula, Password = Password });

            }
            catch (Exception ex)
            {
                Log.Warning("FirebaseUserHelper", "no se pudo agregar coretamete al usuario" + ex.Message);
                throw;
            }
        }

        public async Task<List<Usuario>> GetAllPersons(string email, string password)
        {
            try
            {

                return (await firebase
                .Child(Child)
                .OnceAsync<Usuario>()).Select(item => new Usuario
                {
                    KeyId = item.Key,
                    Correo = item.Object.Correo,
                    Password=item.Object.Password

                }).Where(x => x.Correo == email && x.Password == password).ToList();


            }
            catch (Exception ex)
            {
                Log.Warning("FirebaseUserHelper", "no se pudo verificar si el hay un usarios" + ex.Message);
                throw;

            }
        }
        public async Task<List<Usuario>> GetUserAll(string id)
        {
            try
            {

                return (await firebase
                .Child(Child)
                .OnceAsync<Usuario>()).Select(item => new Usuario
                {
                    KeyId = item.Key,
                    Nombre = item.Object.Nombre,
                    Apellido = item.Object.Apellido,
                    CI = item.Object.CI,
                    Correo = item.Object.Correo
                }).Where(x => x.KeyId == id).ToList();


            }
            catch (Exception ex)
            {
                Log.Warning("FirebaseUserHelper", "no se pudo verificar si el hay un usarios" + ex.Message);
                throw;

            }
        }
        public async Task<List<Usuario>> EmailRecuperait(string email)
        {
            try
            {

                return (await firebase
                .Child(Child)
                .OnceAsync<Usuario>()).Select(item => new Usuario
                {
                    KeyId = item.Key,
                    Correo=item.Object.Correo,
                    Nombre=item.Object.Nombre,
                    Apellido=item.Object.Apellido,
                    CI= item.Object.CI,
                    FechaNacimineto=item.Object.FechaNacimineto,
                    Institucion= item.Object.Institucion,
                    NroMantricula= item.Object.NroMantricula,
                    Password=item.Object.Password
  
                }).Where(x => x.Correo == email).ToList();


            }
            catch (Exception ex)
            {
                Log.Warning("FirebaseUserHelper", "no se pudo verificar si el hay un usarios" + ex.Message);
                throw;

            }
        }
        public async Task UpdatePaswword(string id, string Nombre, string Apellido,
               string FechaNacimineto, string CI, string NroMantricula, string Institucion,string Correo, string Password)
        {
            try
            {
                var toUpdatePerson = (await firebase
               .Child(Child)
               .OnceAsync<Usuario>()).FirstOrDefault(a => a.Key == id);
                await firebase
                    .Child(Child)
                    .Child(toUpdatePerson.Key)
                    .PutAsync(new Usuario() {
                        Nombre = Nombre,
                        Apellido = Apellido,
                        CI = CI,
                        Correo = Correo,
                        FechaNacimineto = FechaNacimineto,
                        Institucion = Institucion,
                        NroMantricula = NroMantricula,
                        Password = Password
                    });
            }
            catch (Exception)
            {
               throw;
            }
        }

        //// para ver si el usario exite en la base de datos 
        public async Task<List<Usuario>> GetAllusuario()
        {
            return (await firebase
                .Child(Child)
                .OnceAsync<Usuario>()).Select(item => new Usuario
                {
                    Nombre = item.Object.Nombre,
                    Apellido = item.Object.Apellido,
                    Correo = item.Object.Correo,
                    NroMantricula=item.Object.NroMantricula
                    
                }).ToList();
        }

        public async Task<Usuario> ExitUserNombre (string name)
        {
            var allPersons = await GetAllusuario();
            await firebase
                .Child(Child)
                .OnceAsync<Usuario>();
            return allPersons.FirstOrDefault(a => a.Nombre == name);
        }
        public async Task<Usuario> ExitUserApellido(string apellido)
        {
            var allPersons = await GetAllusuario();
            await firebase
                .Child(Child)
                .OnceAsync<Usuario>();
            return allPersons.FirstOrDefault(a => a.Apellido == apellido);
        }
        public async Task<Usuario> ExitUserCorreo(string correo)
        {
            var allPersons = await GetAllusuario();
            await firebase
                .Child(Child)
                .OnceAsync<Usuario>();
            return allPersons.FirstOrDefault(a => a.Correo == correo);
        }
        public async Task<Usuario> ExitUserMatricula(string nromatricula)
        {
            var allPersons = await GetAllusuario();
            await firebase
                .Child(Child)
                .OnceAsync<Usuario>();
            return allPersons.FirstOrDefault(a => a.NroMantricula == nromatricula);
        }
    }
}                                         