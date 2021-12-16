﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.Models
{
    public class Usuario : ICrudBase, IPersona
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Activo { get; set; }

        public bool Agregar()
        {
            bool R = false;

            //1.6.1 y 1.6.2 
            Conexion MiCnnAdd = new Conexion();

            //agregar los parámetros para el SP 
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Cedula", this.Cedula));
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Telefono", this.Telefono));
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Email", this.Email));
            
            Crypto MiEncriptador = new Crypto();
            string PassEncriptado = MiEncriptador.EncriptarEnUnSentido(this.Contrasennia);

            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Contrasennia", PassEncriptado));

            //debemos enviar el valor del id del rol, usando la composición de la clase UsuarioRol
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@IdRol", this.MiRol.IDUsuarioRol));
  
            //1.6.3 y 1.6.4
            int resultado = MiCnnAdd.DMLUpdateDeleteInsert("SPUsuarioAgregar");

            //1.6.5
            if (resultado > 0)
            {
                R = true;
            }

            return R;
        }

        public bool Editar()
        {
            bool R = false;

            Conexion MiCnn = new Conexion();

            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Cedula", this.Cedula));
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Nombre", this.Nombre));
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Telefono", this.Telefono));
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Email", this.Email));

            Crypto MiEncriptador = new Crypto();
            string PassEncriptado = string.Empty;

            if (!string.IsNullOrEmpty(this.Contrasennia))
            {
                PassEncriptado = MiEncriptador.EncriptarEnUnSentido(this.Contrasennia);
            }
                       
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Contrasennia", PassEncriptado));

            MiCnn.ListadoDeParametros.Add(new SqlParameter("@IdRol", this.MiRol.IDUsuarioRol));
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@ID", this.IDUsuario));

            int retorno = MiCnn.DMLUpdateDeleteInsert("SPUsuarioEditar");

            if (retorno == 1)
            {
                R = true;
            }

            return R;
        }

        public bool Eliminar()
        {
            bool R = false;

            Conexion MiCnn = new Conexion();
                      
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@ID", this.IDUsuario));

            int retorno = MiCnn.DMLUpdateDeleteInsert("SPUsuarioEliminar");

            if (retorno == 1)
            {
                R = true;
            }

            return R;
        }

        public bool Activar()
        {
            bool R = false;

            Conexion MiCnn = new Conexion();

            MiCnn.ListadoDeParametros.Add(new SqlParameter("@ID", this.IDUsuario));

            int retorno = MiCnn.DMLUpdateDeleteInsert("SPUsuarioActivar");

            if (retorno == 1)
            {
                R = true;
            }

            return R;
        }


        //adicionales
        public int IDUsuario { get; set; }
        public string CodigoRecuperacion { get; set; }

        public string Contrasennia { get; set; }

        //composición del rol del usuario 
        public UsuarioRol MiRol  { get; set; }

        //constructor
        public Usuario()
        {
            MiRol = new UsuarioRol();
        }

        //funciones adicionales
        public bool Agregar(string cedula, string nombre, string telefono, string email, string contrasennia)
        {
            bool R = false;

            return R;
        }

        public Usuario ConsultarPorID(int ID)
        {
            Usuario R = new Usuario();

            Conexion MiCnn = new Conexion();

            MiCnn.ListadoDeParametros.Add(new SqlParameter("@ID", ID));

            DataTable DatosUsuario = new DataTable();

            DatosUsuario = MiCnn.DMLSelect("SPUsuarioConsultarPorID");

            if (DatosUsuario != null && DatosUsuario.Rows.Count == 1)
            {
                DataRow Fila = DatosUsuario.Rows[0];

                R.IDUsuario = ID;
                R.Nombre = Convert.ToString(Fila["Nombre"]);
                R.Cedula = Convert.ToString(Fila["Cedula"]);
                R.Telefono = Convert.ToString(Fila["Telefono"]);
                R.Email = Convert.ToString(Fila["Email"]);
                R.Contrasennia = string.Empty;
                //R.Contrasennia = Convert.ToString(Fila["Contrasennia"]);
                R.MiRol.IDUsuarioRol = Convert.ToInt32(Fila["IDUsuarioRol"]);
            }

            return R;
        }

        public bool ConsultarPorCedula(string cedula)
        {
            bool R = false;

            //paso 1.3.1 y 1.3.2
            Conexion MiConexion = new Conexion();

            //En este caso y de forma didactica se decidió implementar un parámetro para la cédula
            //este valor debe agregarse como parámetro que debe llegar hasta el SP. 
            MiConexion.ListadoDeParametros.Add(new SqlParameter("@Cedula", cedula));
            
            //paso 1.3.3 y 1.3.4
            DataTable retorno = MiConexion.DMLSelect("SPUsuarioConsultarPorCedula");

            //paso 1.3.5
            if (retorno != null && retorno.Rows.Count > 0)
            {
                R = true;
            }

            return R;
        }

        public bool ConsultarPorEmail()
        {
            bool R = false;

            //paso 1.4.1 y 1.42
            Conexion MiCnn = new Conexion();

            //agregar el parámetro que debe llegar con el valor del email a consultar
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Email", this.Email));

            // 1.4.3 y 1.4.4
            DataTable resultado = MiCnn.DMLSelect("SPUsuarioConsultarPorEmail");

            //1.4.5
            if (resultado != null && resultado.Rows.Count > 0)
            {
                R = true;
            }

            return R;
        }

        public DataTable Listar(bool VerActivos, string Filtro = "")
        {
            DataTable R = new DataTable();

            Conexion MiCnn = new Conexion();

            MiCnn.ListadoDeParametros.Add(new SqlParameter("@VerActivos", VerActivos));
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@Filtro", Filtro));

            R = MiCnn.DMLSelect("SPUsuariosListar");

            return R;
        }

        public bool EnviarCodigoRecuperacion(string CodigoVerif)
        {
            bool R = false;

            try
            {

                Conexion MyCnn = new Conexion();

                MyCnn.ListadoDeParametros.Add(new SqlParameter("@Email", this.Email));
                MyCnn.ListadoDeParametros.Add(new SqlParameter("@CodigoVerif", CodigoVerif));

                int Resultado = MyCnn.DMLUpdateDeleteInsert("SPUsuarioGuardarCodigoVerificacion");

                if (Resultado > 0)
                {
                    R = true;
                }


            }
            catch (Exception)
            {

                throw;
            }

            return R;
        }

        public bool CambiarPassword()
        {
            bool R = false;

            try
            {
                Conexion MyCnn = new Conexion();

                MyCnn.ListadoDeParametros.Add(new SqlParameter("@Email", this.Email));

                Crypto MiEncriptador = new Crypto();

                string ContrasenniaEncriptada = MiEncriptador.EncriptarEnUnSentido(this.Contrasennia);

                MyCnn.ListadoDeParametros.Add(new SqlParameter("@Contrasennia", ContrasenniaEncriptada));

                int Resultado = MyCnn.DMLUpdateDeleteInsert("SPUsuarioActualizarContrasennia");

                if (Resultado > 0)
                {
                    R = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return R;
        }

        public Usuario ValidarIngreso(string user, string password)
        {
            Usuario R = new Usuario();

            Conexion MiCnn = new Conexion();
            Crypto MiEncriptador = new Crypto();

            string PassEncriptado = MiEncriptador.EncriptarEnUnSentido(password);

            MiCnn.ListadoDeParametros.Add(new SqlParameter("@user", user));
            MiCnn.ListadoDeParametros.Add(new SqlParameter("@pass", PassEncriptado));

            DataTable DatosUsuario = MiCnn.DMLSelect("SPUsuarioValidarIngreso");

            if (DatosUsuario != null && DatosUsuario.Rows.Count == 1)
            {
                DataRow Fila = DatosUsuario.Rows[0];

                R.IDUsuario = Convert.ToInt32(Fila["ID"]);
                R.Nombre = Convert.ToString(Fila["Nombre"]);
                R.Cedula = Convert.ToString(Fila["Cedula"]);
                R.Telefono = Convert.ToString(Fila["Telefono"]);
                R.Email = Convert.ToString(Fila["Email"]);
                R.Contrasennia = string.Empty;
                R.MiRol.IDUsuarioRol = Convert.ToInt32(Fila["IDUsuarioRol"]);
            }
            
            return R;
        }

        public bool ComprobarCodigoRecuperacion()
        {
            bool R = false;

            try
            {
                Conexion MyCnn = new Conexion();

                MyCnn.ListadoDeParametros.Add(new SqlParameter("@Email", this.Email));
                MyCnn.ListadoDeParametros.Add(new SqlParameter("@CodigoVerif", this.CodigoRecuperacion));

                DataTable Resultado = MyCnn.DMLSelect("SPUsuarioComprobarCodigoVerificacion");

                if (Resultado != null && Resultado.Rows.Count > 0)
                {
                    R = true;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return R;
        }

    }
}
