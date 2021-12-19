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
    public partial class FrmTicketCrear : Form
    {
        public Logica.Models.Ticket MiTicket { get; set; }
        public Logica.Models.Bitacora MiBitacora { get; set; }
        public Logica.Models.Usuario MiUsuario { get; set; }

        public FrmTicketCrear()
        {
            InitializeComponent();

            MiTicket = new Logica.Models.Ticket();

        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TxtIDUsuario_DoubleClick(object sender, EventArgs e)
        {
            FrmClienteBuscar MiFormDeBusqueda = new FrmClienteBuscar();

            DialogResult resp = MiFormDeBusqueda.ShowDialog();

            if (resp == DialogResult.OK)
            {
                TxtIDUsuario.Text = MiTicket.MiCliente.IDCliente.ToString();
                LblClienteNombre.Text = MiTicket.MiCliente.Nombre;
            }
        }

        private void FrmTicketCrear_Load(object sender, EventArgs e)
        {
            CargarCategorias();

            LimpiarForm();
        }

        private void LimpiarForm()
        {
            TxtIDUsuario.Clear();
            LblClienteNombre.Text = "";
            TxtTitulo.Clear();
            TxtDescripcion.Clear();
            CboxCategoria.SelectedIndex = -1;

            MiTicket = new Logica.Models.Ticket();
        
        }


        private void CargarCategorias()
        {
            DataTable Datos = new DataTable();
                       
            Datos = MiTicket.MiCategoria.Listar();

            CboxCategoria.ValueMember = "ID";
            CboxCategoria.DisplayMember = "Descrip";
                        
            CboxCategoria.DataSource = Datos;

            CboxCategoria.SelectedIndex = -1;
        }

        private void BtnAceptar_Click(object sender, EventArgs e)
        {
            if (Validar())
            {
                //está todo listo para agregar el ticket 

                MiTicket.MiCategoria.IDTicketCategoria = Convert.ToInt32(CboxCategoria.SelectedValue);
                MiTicket.MiCategoria.TicketCategoriaDescripcion = Convert.ToString(CboxCategoria.SelectedText);

                MiTicket.TicketTitulo = TxtTitulo.Text.Trim();
                MiTicket.TicketDescripcion = TxtDescripcion.Text.Trim();

                if (MiTicket.Agregar())
                {
                    if (MiBitacora.Agregar())
                    {

                        MiBitacora.MiUsuario.IDUsuario = Convert.ToInt32(Commons.ObjetosGlobales.MiUsuarioDeSistema.IDUsuario);
                        string email = Convert.ToString(TxtTitulo);
                        string descrip = "Se creo nuevo ticket: ";
                        MiBitacora.Accion = String.Format(descrip + email);
                       // string v = DateTime.Now.ToString("MMddyyyy");
                        //MiBitacora.FechaHora = v;
                    }

                    MessageBox.Show("Ticket Agregado Correctamente", ":)", MessageBoxButtons.OK);

                    LimpiarForm();

                    //TODO Implementar un reporte de crystal para poderlo imprimir y que quede 
                    //como atestado de creación del ticket 
                }
                


            }

        }

        private bool Validar()
        {
            bool R = false;

            if (MiTicket.MiCliente.IDCliente > 0 &&
                CboxCategoria.SelectedIndex > -1 &&
                !string.IsNullOrEmpty(TxtTitulo.Text.Trim()) &&
                !string.IsNullOrEmpty(TxtDescripcion.Text.Trim()))
            {
                R = true;
            }
            else
            {
                if (MiTicket.MiCliente.IDCliente == 0)
                {
                    MessageBox.Show("Debe seleccionar un Cliente!", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

                if (CboxCategoria.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar una categoría para el Ticket!", "Error de validación", MessageBoxButtons.OK);
                    return false;
                }

                if (string.IsNullOrEmpty(TxtTitulo.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar un Título para el Ticket!", "Error de validación", MessageBoxButtons.OK);
                    TxtTitulo.Focus();
                    return false;
                }

                if (string.IsNullOrEmpty(TxtDescripcion.Text.Trim()))
                {
                    MessageBox.Show("Debe digitar una descripción para el Ticket!", "Error de validación", MessageBoxButtons.OK);
                    TxtDescripcion.Focus();
                    return false;
                }


            }


            return R;
        
        }

    }
}
