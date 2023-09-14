using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;
using Negocio;


namespace Negocio
{
     public class PokemonNegocio
    {
        public List<Pokemon> listar()
        {
            List<Pokemon> lista = new List<Pokemon>();
            SqlConnection conexion = new SqlConnection();
            SqlCommand Comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS; database=POKEDEX_DB; integrated security=true;";
                Comando.CommandType = System.Data.CommandType.Text;
                Comando.CommandText = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad,P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.id = P.IdTipo and D.id = P.IdDebilidad and p.Activo = 1";
                Comando.Connection = conexion;  

                conexion.Open();
                lector = Comando.ExecuteReader();   

                while (lector.Read())
                { 
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)lector["Id"];
                    aux.Numero = lector.GetInt32(0);
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    //VALIDAR LECTURA NULL DE DB
                    // if (lector.IsDBNull(lector.GetOrdinal("UrlImagen"))); 
                    // aux.UrlImagen = (string)lector["UrlImagen"];

                    if (!(lector["UrlImagen"]is DBNull))
                     aux.UrlImagen = (string)lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)lector["IdTipo"];    
                    aux.Tipo.Descripcion = (string)lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    lista.Add(aux);

                }
                conexion.Close();   
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public void agregar(Pokemon nuevo) 
        {
            AccesoDatos datos = new AccesoDatos();  
            try
            {
                datos.setearConsulta("insert into POKEMONS (Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen)values("+ nuevo.Numero + ", '"+ nuevo.Nombre +"' , '"+ nuevo.Descripcion+"', 1, @IdTipo, @IdDebilidad, @UrlImagen)");
                datos.setearParametro("@IdTipo", nuevo.Tipo.Id);
                datos.setearParametro("@IdDebilidad", nuevo.Debilidad.Id);
                datos.setearParametro("@UrlImagen",nuevo.UrlImagen);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            {
                datos.cerrarConexion(); 
            }
        }

        public void modificar(Pokemon poke) 
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @descripcion, UrlImagen = @imagen, IdTipo = @idTipo, IdDebilidad = @idDebilidad where id = @id");
                datos.setearParametro("@numero", poke.Numero);
                datos.setearParametro("nombre", poke.Nombre);
                datos.setearParametro("@descripcion", poke.Descripcion);
                datos.setearParametro("@imagen", poke.UrlImagen);
                datos.setearParametro("@idTipo", poke.Tipo.Id);
                datos.setearParametro("@idDebilidad ", poke.Debilidad.Id);
                datos.setearParametro("@id", poke.Id);

                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }

            finally
            { 
                datos.cerrarConexion();
            }
        }
        // ELIMINAR
        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from POKEMONS where id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void eliminarLogico(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("update POKEMONS set Activo = 0 where Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion(); 
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        // FILTRO AVANZADO CONTRA DB
        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion Tipo, D.Descripcion Debilidad, P.IdTipo, P.IdDebilidad,P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.id = P.IdTipo and D.id = P.IdDebilidad and p.Activo = 1 and ";
               
                if (campo == "Numero") 
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero >" + filtro;
                            break;
                        case "Menor a":
                            consulta += "Numero <" + filtro;
                            break;
                        default:
                            consulta += "Numero =" + filtro;
                            break;
                    }

                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%'";
                            break;
                    }
                }
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "P.Descripcion like '" + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "P.Descripcion like '%" + filtro + "'";
                            break;
                        default:
                            consulta += "P.Descripcion like '%" + filtro + "%'";
                            break;
                    }
                }
                datos.setearConsulta( consulta );
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = datos.Lector.GetInt32(0);
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector["UrlImagen"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["UrlImagen"];

                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];

                    lista.Add(aux);

                }



                // UN EJEMPLO DE SWITCH
                //switch (campo)
                //{
                //    case "Numero":
                //        switch(criterio) 
                //        {
                //            case "Mayor a":
                //                consulta += "Numero >" + filtro;
                //                break;
                //            case "Menor a":
                //                consulta += "Numero <" + filtro;
                //                break;
                //            default:
                //                consulta += "Numero =" + filtro;
                //                break;
                //        }
                //        break;
                //    case "Nombre":
                //        break;
                //    case "Descripcion":
                //        break;
                //    default:
                //        break;
                // }
                return lista;   
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
