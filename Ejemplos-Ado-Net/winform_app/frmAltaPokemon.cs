using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dominio;
using Negocio;
using System.Configuration;

namespace winform_app
{
    public partial class frmAltaPokemon : Form
    {
        private Pokemon pokemon = null;
        private OpenFileDialog archivo = null;
        public frmAltaPokemon()
        {
            InitializeComponent();
        }
        public frmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        //DESPLEGABLES
        private void btnAceptar_Click(object sender, EventArgs e)
        {
            //Pokemon poke = new Pokemon();  
            PokemonNegocio negocio = new PokemonNegocio();  
            try
            {
                if (pokemon == null)
                    pokemon = new Pokemon();

                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.UrlImagen = txtUrlImagen.Text;
                pokemon.Tipo = (Elemento)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cboDebilidad.SelectedItem;

                if (pokemon.Id != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Modificado exitosamente");
                }
                else
                {
                    negocio.agregar(pokemon);
                    MessageBox.Show("Agregado exitosamente");
                }

                //GUARDO IMAGEN SI LA LEVANTO LOCALMENTE
                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName,ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);


                    // QUIERO MOSTRAR EL CARTE QUE ME DIGA ELIMINADO
                    //negocio.eliminar(pokemon);
                    // MessageBox.Show("Eliminado exitosamente"
                    Close();   
                
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private void frmAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                cboTipo.DataSource = elementoNegocio.listar();
                //PRECARGA DEL DESPLEGABLE
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";  
                cboDebilidad.DataSource = elementoNegocio.listar();
                //PRECARGA DEL DESPLEGABLE
                cboDebilidad.ValueMember = "Id";
                cboDebilidad.DisplayMember = "Descripcion";    

                //PRECARGA EN EL POKEMON MODIFICAR
                if (pokemon != null)
                {
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    cargarImagen(pokemon.UrlImagen);
                    cboTipo.SelectedValue = pokemon.Tipo.Id;  
                    cboDebilidad.SelectedValue = pokemon.Debilidad.Id;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }
        // CARGA IMAGEN URL
        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
        }
        private void cargarImagen(string imagen)
        {
            try
            {
                bxPokemon.Load(imagen);

            }

            //ADVERTENCIA!!
            catch (Exception ex)
            {

                bxPokemon.Load("https://tse4.mm.bing.net/th?id=OIP.R4gEjzGdxIEBZ42eafUHfgHaHa&pid=Api&P=0&h=180");
            }
        }
        //LEVANTAR Y GUARDAR IMAGEN LOCAL
        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.pgn";
            if(archivo.ShowDialog () == DialogResult.OK)
            {
                txtUrlImagen.Text= archivo.FileName;
                cargarImagen(archivo.FileName);

                //Guardo la imagen
                //File.Copy(archivo.FileName,ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);

            }
        }
    }
}
