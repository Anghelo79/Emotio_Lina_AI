using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Xamarin.Forms.Internals;

namespace ProyectoLina.Commun
{
    class AddressEmail
    {
        protected string Email = "linaunivalle4@gmail.com";
        protected string Password = "imaprial";
        protected int codigo;

        public void SendEmail(string detinatario, string sujet)
        {


            try
            {
                var random = new Random();
                codigo = random.Next(1000, 9999);
                MailMessage mail = new MailMessage();
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress(Email);
                mail.To.Add(detinatario);
                mail.Subject = sujet;
                mail.Body = " Su codigo de segurida es " + codigo;

                smtpServer.Port = 587;
                smtpServer.Host = "smtp.gmail.com";
                smtpServer.EnableSsl = true;
                smtpServer.UseDefaultCredentials = false;
                smtpServer.Credentials = new System.Net.NetworkCredential(Email, Password);
                smtpServer.Send(mail);

            }
            catch (Exception ex)
            {

                Log.Warning("AddresEmail", "no se pudo enviar el correo " + ex.Message);
                throw;
            }
        }
        public bool verifycode(int CodigoConfirt)
        {
            if (CodigoConfirt == codigo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
