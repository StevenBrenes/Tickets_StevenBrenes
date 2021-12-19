﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tickets.Formularios
{
    public partial class FrmUsuarioRecuperarContrasennia : Form
    {

        public Logica.Email MyEmail { get; set; }
        public Logica.Models.Usuario MyUser { get; set; }

        public Logica.Models.Bitacora MiBitacora { get; set; }


        public FrmUsuarioRecuperarContrasennia()
        {
            InitializeComponent();

            MyEmail = new Logica.Email();
            MyUser = new Logica.Models.Usuario();

        }

        private void FrmUsuarioRecuperarContrasennia_Load(object sender, EventArgs e)
        {
            TxtCodigoEnviado.Enabled = false;
            TxtPass1.Enabled = false;
            TxtPass2.Enabled = false;
            BtnAceptar.Enabled = false;

            TxtCodigoEnviado.Clear();
            TxtPass1.Clear();
            TxtPass2.Clear();

        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void BtnEnviarCodigo_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (!string.IsNullOrEmpty(TxtUsuario.Text.Trim()) &&
                Commons.ObjetosGlobales.ValidarEmail(TxtUsuario.Text.Trim()))
                {
                    MyUser.Email = TxtUsuario.Text.Trim();

                    if (MyUser.ConsultarPorEmail())
                    {
                        //Si el correo existe para un usuario activo se procede a enviar el correo con un 
                        //códgo de verificación que el usuario deberá digitar para comprobar que sea él 

                        //Este código se debería generar aleatoriamente. 
                        //TODO: ???

                        string CodigoVerificacion = "ABC123*";

                        if (MyUser.EnviarCodigoRecuperacion(CodigoVerificacion))
                        {
                            //Se procede a enviar el correo al usuario con el código  

                            MyEmail.Asunto = "Su código de recuperación de contraseña de Tickets Progra 5 2021-3";

                            MyEmail.CorreoDestino = MyUser.Email;

                            string Mensaje = string.Format("Su código de recuperación de contraseña es: {0}", CodigoVerificacion);

                            MyEmail.Mensaje = Mensaje;

                            if (MyEmail.EnviarCorreo_Net_Mail_SmtpClient())
                            {

                                if (MiBitacora.Agregar())
                                {

                                    MiBitacora.MiUsuario.IDUsuario = 0;
                                    string email = Convert.ToString(TxtUsuario);
                                    string descrip = "Se ha restablecido la contrase;a del usuario: ";
                                    MiBitacora.Accion = String.Format(descrip + email);
                                    //string v = DateTime.Now.ToString("MMddyyyy");
                                    //MiBitacora.FechaHora = v;
                                }

                                MessageBox.Show("Corrreo enviado correctamente", ":)", MessageBoxButtons.OK);

                                TxtCodigoEnviado.Enabled = true;
                                TxtPass1.Enabled = true;
                                TxtPass2.Enabled = true;
                                BtnAceptar.Enabled = true;

                            }
                            else
                            {
                                MessageBox.Show("El Correo no se pudo enviar!", ":(", MessageBoxButtons.OK);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("El Correo no existe o el usuario está desactivado!", "Error de validación", MessageBoxButtons.OK);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            //1. Se debe verificar que el código digitado sea el mismo que está almacenado en la tabla usuario
            //2. Las contraseñas deben ser las mismas. 
            //3. Se procede con el cambio de contraseña 

            if (!string.IsNullOrEmpty(TxtCodigoEnviado.Text.Trim()) && ValidarContrasennias() )
            {
                //el dato del email ya se había asignado en el proceso de enviar el email al usuario
                MyUser.CodigoRecuperacion = TxtCodigoEnviado.Text.Trim();
                MyUser.Contrasennia = TxtPass1.Text.Trim();

                if (MyUser.ComprobarCodigoRecuperacion())
                {
                    // se tiene permiso de modificar la contraseña 

                    if (MyUser.CambiarPassword())
                    {
                        MessageBox.Show("La Contraseña se ha actualizado correctamente", ":)", MessageBoxButtons.OK);
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("No se ha cambiado la contraseña", ":(", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("El código de verificación digitado no es correcto", ":(", MessageBoxButtons.OK);

                    TxtCodigoEnviado.BackColor = Color.Firebrick;
                }
            }
        }

        private bool ValidarContrasennias()
        {
            bool R = false;

            if (!string.IsNullOrEmpty(TxtPass1.Text.Trim()) &&
                !string.IsNullOrEmpty(TxtPass2.Text.Trim()) &&
                TxtPass1.Text.Trim() == TxtPass2.Text.Trim())
            {
                R = true;
            }
            else
            {
                if (string.IsNullOrEmpty(TxtPass1.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar la contraseña", "Error de validación", MessageBoxButtons.OK);
                    TxtPass1.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(TxtPass2.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar la confirmación de contraseña", "Error de validación", MessageBoxButtons.OK);
                    TxtPass2.Focus();
                    return false;
                }

                if (!string.IsNullOrEmpty(TxtPass1.Text.Trim()) &&
                    !string.IsNullOrEmpty(TxtPass2.Text.Trim()) &&
                    TxtPass1.Text.Trim() != TxtPass2.Text.Trim())
                {
                    MessageBox.Show("Las contraseñas no son iguales", "Error de validación", MessageBoxButtons.OK);
                    TxtPass2.Focus();
                    return false;
                }
            }
            return R;
        }
    }
}
