using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.Models
{
    public class Bitacora
    {

        public Usuario MiUsuario { get; set; }
        public string Accion { get; set; }
        public DateTime FechaHora { get; set; }




        public bool Agregar()
        {
            bool R = false;

            Conexion MiCnnAdd = new Conexion();

            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@IDUsuario", 0));
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Accion", this.Accion));
            MiCnnAdd.ListadoDeParametros.Add(new SqlParameter("@Telefono", this.FechaHora));

            int resultado = MiCnnAdd.DMLUpdateDeleteInsert("SPBitacoraAgregar");

            if (resultado > 0)
            {
                R = true;
            }
            return R;
        }

        public bool Agregar(int IDUsuario, string Accion, string FechaHora)
        {
            bool R = false;
            return R;
        }
    }
}
