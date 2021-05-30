using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace ejemploArchivoBinario
{

    class Program
    {
        /*   UNIVERSIDAD NACIONAL DE SAN ANTONIO ABAD DEL CUSCO
        ESCUELA PROFESIONAL DE INGENIERIA INFORMATICA Y DE SISTEMAS
        #------------------------------------------------
        #Semestre                : 2019-2V
        #Asignatura              : BASE DE DATOS II
        #Tema                    : REGISTROS SECUENCIALES DE LONGITUD FIJA
        #Fecha                   : 06-03-2020
        #Codigo                  : 091613
        #Apellido-nombres        : Ramírez-Apaza-Ebert
        #------------------------------------------------*/

        static void Main(string[] args)
        {   
            Console.SetWindowSize(Console.LargestWindowWidth-20, Console.LargestWindowHeight-20);
            
            Console.SetWindowPosition(Console.WindowLeft,Console.WindowTop);

            //Fichero para apuntar lógicamente al fichero
            FileStream Fichero  ;
            // codificacion de strings ASCCI
            // SE ESTÁ GUARDANDO LOS DATOS EN ARCHIVOS DE LONGITUD FIJA DE 93 BYTES
            // CODIGO = CHAR(5)                       ->   5 BYTES
            // TITULO = CHAR(50)                      ->  50 BYTES
            // AUTOR  = CHAR(30)                      ->  30 BYTES
            // AÑO    = integer                       ->   4 BYTES
            // BORRADO= integer                       ->   4 BYTES
            // TOTAL DE ESPACIO PARA 1 REGISTRO DE LIBRO = 93 BYTES
            //ruta o path del data
            string ruta = "libros.dat";
            string directorioSecuencias = "Secuencias";
            string directorioMezclas = "Mezclas";
            //CUANDO NO HAY NINGUN LIBRO crea y llena con los 100 libros de prueba
           //SE ELIMINAN LAS CARPETAS DE MEZCLAS Y SECUENCIAS AL INICIAR LA APLICACION
            if (!File.Exists(ruta))
            {
                Fichero = File.Open(ruta, FileMode.Create);
                crearFichero(ruta, ref Fichero);
                Fichero.Close();
                insertarLibros(ruta);
            }
            if (Directory.Exists(directorioSecuencias))
            { Directory.Delete(directorioSecuencias, true); }


            if (Directory.Exists(directorioMezclas))
            { Directory.Delete(directorioMezclas, true); }

            limpiarCrearDirecoriosMezclaSecuencia(directorioSecuencias,directorioMezclas);
            /* */
           
            //    BLOQUE  SWITCH DEL MENU 
            ConsoleKeyInfo op;
            do
            {
                menuPrincipal();
                op = Console.ReadKey(true);
                switch (op.Key)
                {
                    case ConsoleKey.D1:
                        {//_____________ BLOQUE PARA INSERTAR___________-
                            Fichero = File.Open(ruta, FileMode.Append);
                            //Console.WriteLine("tamaño {0}  {1}", Fichero.Length, Fichero.Position);                            
                            escribirCampo("Codigo",5,5, ref Fichero);
                            escribirCampo("Titulo",50,50, ref Fichero);
                            escribirCampo("Autor",30,30, ref Fichero);
                            escribirCampo("Año", 4, ref Fichero);
                            //escribirCampo("Borrado", 4, ref Fichero);
                            escribirCampoBorrado(ref Fichero);
                            Fichero.Flush();
                            Fichero.Close();
                            break;
                        }
                    case ConsoleKey.D2:
                        {//_____________ BLOQUE PARA ELIMINAR___________-
                            Console.WriteLine("Eliminar");
                            Console.WriteLine("Introduzca codigo del libro a eliminar:");
                            string codigo = Console.ReadLine();
                            Fichero = File.OpenRead(ruta);
                            eliminarLibro(ref Fichero,codigo,ruta);
                            Fichero.Close();
                            break;
                        }
                    case ConsoleKey.D3:
                        {//_____________ BLOQUE PARA MODIFICAR___________-
                            Console.WriteLine("Modificar");
                            Console.WriteLine("Introduzca codigo del libro a modificar:");
                            string codigo = Console.ReadLine();
                            Fichero = File.OpenRead(ruta);
                            modificarLibro(ref Fichero,codigo,ruta);
                            Fichero.Close();
                            break;
                        }
                    case ConsoleKey.D4:
                        {//_____________ BLOQUE PARA MOSTRAR LIBROS GUARDADOS___________-
                            Fichero = File.Open(ruta, FileMode.Open);
                            mostrarLibros(ref Fichero);
                            Fichero.Close();
                            break;
                        }
                    case ConsoleKey.D5:
                        {//_____________ BLOQUE PARA LISTAR BORRADOS ___________-
                            if (File.Exists(ruta))
                            {
                                Fichero = File.Open(ruta, FileMode.Open);
                                mostrarLibrosBorrados(ref Fichero);
                                Fichero.Close();
                            }
                            break;
                        }
                    case ConsoleKey.D6:
                        {//_____________ BLOQUE PARA MOSTRAR TODOS LOS LIBROS BORRADOS Y NO BORRADOS___________-
                            Console.WriteLine("**********************   TODOS LOS LIBROS (borrados y no borrados)  *****************************************");
                            Console.WriteLine("*********************************************************************************");
                            Console.WriteLine("CODIGO       TITULO                                                  AUTOR                          AÑO      BORRADO");
                            Console.WriteLine("====================================================================================================================");
                            Fichero = File.OpenRead(ruta);
                            byte[] buffer = new byte[93];
                            while (Fichero.Position < Fichero.Length)
                            {
                                Fichero.Read(buffer, 0, 93);
                                mostrarRegistro(buffer);
                            }
                            Fichero.Close();
                            Console.WriteLine("====================================================================================================================");
                            break;
                        }
                    case ConsoleKey.D7:
                        {//_____________ mezcla externa por codigo libro ___________-
                            //limpiarCrearDirecoriosMezclaSecuencia(directorioSecuencias,directorioMezclas);
                            Console.WriteLine("Mezcla por CODIGO Libro");
                            Console.Write("Ingrese numero de Marcos M :");
                            int M = int.Parse(Console.ReadLine());
                            Console.Write("Ingrese factor de bloqueo fr:");
                            int fr = int.Parse(Console.ReadLine());
                            int N = generarSecuenciasOrdenadas(ruta, M, fr, "C");
                            Console.WriteLine("---------SECUENCIAS ORDENADAS  GENERADAS {0}  ----------", N);
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine("Presione cualquier tecla para MEZCLARLAS...");
                            Console.ReadKey();
                            procesarSecuenciasOrdenadas(M,N,fr,"C"); 
                        }
                        break;
                    case ConsoleKey.D8:
                        {//_____________ mezcla externa por titulo de libro ___________-
                            Console.WriteLine("Mezcla por TITULO Libro");
                            Console.Write("Ingrese numero de Marcos M :");
                            int M = int.Parse(Console.ReadLine());
                            Console.Write("Ingrese factor de bloqueo fr:");
                            int fr = int.Parse(Console.ReadLine());
                            int N= generarSecuenciasOrdenadas(ruta, M, fr, "T");
                            Console.WriteLine("---------SECUENCIAS ORDENADAS  GENERADAS {0}  ----------",N);
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine("Presione cualquier tecla para MEZCLARLAS...");
                            Console.ReadKey();
                            procesarSecuenciasOrdenadas(M,N,fr,"T"); 
                        }
                        break;
                    case ConsoleKey.D9:
                        {
                            
                        }
                        break;
                }
            } while (op.Key != ConsoleKey.Escape);
            
        }
        #region MODULOS_DEL_PROYECTO_ANTERIOR
        public static void menuPrincipal()
        {// DESPLIEGUE DEL MENU
            Console.WriteLine("*********************************************************************************");
            Console.WriteLine("======Introduzca opcion (no usar los numPad) =====");
            Console.WriteLine("1 .- INSERTAR libro  ");
            Console.WriteLine("2 .- ELIMINAR libro ");
            Console.WriteLine("3 .- MODIFICAR libro ");
            Console.WriteLine("4 .- MOSTRAR libros  HABILITADOS ");
            Console.WriteLine("5 .- MOSTRAR libros  ELIMINADOS ");
            Console.WriteLine("6 .- MOSTRAR TODO (cabecera, habilitados y eliminados)");
            Console.WriteLine("7 .- Ordenación MEZCLA-EXTERNA  por CODIGO libro");
            Console.WriteLine("8 .- Ordenación MEZCLA-EXTERNA  por TITULO libro");
            var salir = @"Presione  << ESCAPE >>  para Salir";
            Console.WriteLine(salir);
        }
        public static void escribirCampo(string campo,int lengthValido,int tamanioBytes,ref FileStream Fichero)
        {//PRECONDICION{ fichero abierto Modo.Append para escritura} 
         //POSTCONDICION{ strings guardados en la posicion actual del filestream}
            byte [] buffer = new byte [tamanioBytes];
            Console.WriteLine("(Máximo {0} caracteres) {1} : ", lengthValido, campo);
            string dato = Console.ReadLine();
            // VALIDAR 
            while (dato.Length > lengthValido || dato.Trim().Length ==0 )
            {// si es mayor volvemos a leer con el mensaje respectivo
                Console.WriteLine("Máximo {0} caracteres , ingrese nuevamente {1} :", lengthValido,campo);
                dato = Console.ReadLine();
            }
            int i = dato.Length;
            // llenamos con espacios vacios si los campos son menores que su longitud prevista
            while (i < lengthValido)
            {
                dato = dato + " ";
                i++;
            }
            buffer = Encoding.ASCII.GetBytes(dato);
            Fichero.Write(buffer,0,buffer.Length);
                
        }
        public static void escribirCampo(string campo, int lengthValido, int tamanioBytes, ref FileStream Fichero,string dato)
        {//PRECONDICION{ fichero abierto Modo.Append para escritura} 
            //POSTCONDICION{ strings guardados en la posicion actual del filestream}
            byte[] buffer = new byte[tamanioBytes];
            
            // VALIDAR 
            while (dato.Length > lengthValido || dato.Trim().Length == 0)
            {// si es mayor volvemos a leer con el mensaje respectivo
                Console.WriteLine("Máximo {0} caracteres , ingrese nuevamente {1} :", lengthValido, campo);
                dato = Console.ReadLine();
            }
            int i = dato.Length;
            // llenamos con espacios vacios si los campos son menores que su longitud prevista
            while (i < lengthValido)
            {
                dato = dato + " ";
                i++;
            }
            buffer = Encoding.ASCII.GetBytes(dato);
            Fichero.Write(buffer, 0, buffer.Length);

        }
        public static void escribirCampoBorrado(ref FileStream Fichero)
        {// escribe un 0 en el campo borrado
            byte [] buffer = new  byte [4];
            buffer = BitConverter.GetBytes((int)0);
           Fichero.Write(buffer, 0, buffer.Length);
           // Console.WriteLine("se ingreso 0 en el campo Borrado");
        }
        public static void escribirCampo(string campo,int tamanioBytes,ref FileStream Fichero)
        {//PRECONDICION{ fichero abierto Modo.Append para escritura} 
         //POSTCONDICION{guarda entero de 4 bytes, guardados en la posicion actual del filestream}
              byte[] buffer = new byte[tamanioBytes];
              //ingresamos los valores de acuerdo a CAMPO validando longitud de caracteres (bytes )
              Console.Write("{0} : ", campo);
              string dato = Console.ReadLine();
              int numero;
              bool esEntero = Int32.TryParse(dato, out numero);
              while (!esEntero)
              {
                     Console.WriteLine("Ingrese un número válido ...");
                     Console.Write("{0} : ", campo);
                     dato = Console.ReadLine();
                     esEntero = Int32.TryParse(dato, out numero);
              }
               buffer = BitConverter.GetBytes(numero);
               Fichero.Write(buffer, 0, buffer.Length);
        }
        public static void escribirCampo(string campo, int tamanioBytes, ref FileStream Fichero,int dato)
        {//PRECONDICION{ fichero abierto Modo.Append para escritura} 
            //POSTCONDICION{guarda entero de 4 bytes, guardados en la posicion actual del filestream}
            byte[] buffer = new byte[tamanioBytes];
            //ingresamos los valores de acuerdo a CAMPO validando longitud de caracteres (bytes )
            buffer = BitConverter.GetBytes(dato);
            Fichero.Write(buffer, 0, buffer.Length);
        }
        public static void crearFichero(string ruta,ref FileStream Fichero)
        {//PRECONDICION {fichero abierto con create }
         //POSTCONDICION{fichero creado o limpiado con peso 93bytes de cabecera}
            //Console.WriteLine("posicion al crear {0} lengt al crear {1 }", Fichero.Position, Fichero.Position);
            //Console.WriteLine("archivo vacío , creando cabecera...");
            byte[] buffer = new byte[85];
            string dato = "";
            for (int i = 0; i < 85; i++)
            {
                dato = dato + " ";
                //Console.WriteLine(dato.Length);
            }
            buffer = Encoding.ASCII.GetBytes(dato);
            Fichero.Write(buffer, 0, 85);
            // campo año
            buffer = BitConverter.GetBytes((int)0);
            Fichero.Write(buffer, 0, 4);
            //campo listaBorrado
            buffer = BitConverter.GetBytes((int)-1);
            Fichero.Write(buffer, 0, 4);
            //Console.WriteLine("posicion al insertando {0} lengt al crear {1 }", Fichero.Position, Fichero.Position);
        }

        public static void mostrarLibros(ref FileStream Fichero)
        {// PRECONDICION {Fichero abierto para  Lectura}
           //POSTCONDICION {Libros No borrados mostrados }
            Fichero.Position = 93; // comenzamos por el primer registro
            int tamanio = (int)Fichero.Length;
            byte [] buffer = new byte [93];
            Console.WriteLine("**********************   LISTAR LIBROS  *****************************************");
            Console.WriteLine("**********************                  ******************************************");
            Console.WriteLine("CODIGO       TITULO                                                  AUTOR                          AÑO      BORRADO");
            Console.WriteLine("====================================================================================================================");
            while (Fichero.Position < tamanio) 
            {
                Fichero.Read(buffer,0,93);
                int borrado = BitConverter.ToInt32(buffer, 89);
                if (borrado == 0)// mostramos los registros habilitados
                    {
                        mostrarRegistro(buffer);
                    }
            }
            Console.WriteLine("====================================================================================================================");
        }
        public static void mostrarRegistro(byte [] buffer)
        {//PRECONDICION{ fichero abierto para lectura} 
         //POSTCONDICION{ escribe en pantalla los datos del registro actual del buffer}
            Console.Write("{0} - ", Encoding.ASCII.GetString(buffer, 0, 5).PadRight(8));
            Console.Write("{0} - ", (Encoding.ASCII.GetString(buffer, 5, 50)).PadRight(50));
            Console.Write("{0} - ", (Encoding.ASCII.GetString(buffer, 55, 30)).PadRight(32));
            Console.Write("{0} - ", (BitConverter.ToInt32(buffer, 85))); Console.Write("    ");
            Console.WriteLine("{0}", BitConverter.ToInt32(buffer, 89));

        }
        public static void mostrarLibrosBorrados(ref FileStream Fichero)
        {//PRECONDICION{ fichero abierto para lectura} 
         //POSTCONDICION{ muestra en pantalla los libros borrados }
            byte [] buffer = new byte[93];
            int nroRegistroConMarcaFin = 0;
            //Console.WriteLine("registro encontrado en el nro : {0}", nroRegistro);
            //Fichero.Position = 0;
            //buscamos el nro de registro con un -1 en su campo borrado
            //Console.WriteLine("posicon inical{0}", Fichero.Position);
            bool fin = false;
            Console.WriteLine("**********************   LIBROS BORRADOS   *****************************************");
            Console.WriteLine("*********************************************************************************");
            Console.WriteLine("CODIGO       TITULO                                                  AUTOR                          AÑO      BORRADO");
            Console.WriteLine("====================================================================================================================");
            while (!fin)
            {// leemos la cabecera que valor hay en su campo BORRADO
                Fichero.Read(buffer, 0, 93);
                nroRegistroConMarcaFin = BitConverter.ToInt32(buffer, 89);
                if(nroRegistroConMarcaFin > 0 )
                {
                    mostrarRegistro(buffer);
                    Fichero.Position = nroRegistroConMarcaFin * 93;
                }
                if (nroRegistroConMarcaFin == -1)
                {
                    mostrarRegistro(buffer);
                    fin = true;
                }
            }
            Console.WriteLine("====================================================================================================================");
        }
        public static void modificarLibro(ref FileStream Fichero, string codigo,string ruta)
        {//PRECONDICION{ fichero abierto para lectura} 
         //POSTCONDICION{ registro con el  codigo ingresado, modificado }
            byte[] buffer = new byte[93];
            
            int nroRegistroModificar = 0;
            bool encontrado = false;
            string codigoComparativo  ;
            while (Fichero.Position < Fichero.Length && encontrado == false )
            {//necesitamos encontrar el registro a modificar en que numero de registro esta
                Fichero.Read(buffer, 0, 93);
                codigoComparativo = Encoding.ASCII.GetString(buffer, 0, 5).Trim();
                // el registro debe encontrarse de acuerd a su codigo y su campo borrado debe ser 0
                encontrado = (codigoComparativo == codigo && BitConverter.ToInt32(buffer, 89) == 0);
                //Console.WriteLine("codigo buffer {0}  codigo parametro {1} ", codigoComparativo, codigo);
                //Console.Write("posicion {0} ", Fichero.Position);
                //fueBorrado = (BitConverter.ToInt32(buffer, 89) > 0 || BitConverter.ToInt32(buffer, 89) == -1);
               // Console.ReadKey();
            }
            if (encontrado)
            {
                nroRegistroModificar = (((int)Fichero.Position / 93) - 1);
                //Console.WriteLine("Encontrado en posicion : {0} ",nroRegistroModificar);
                Fichero.Close();
                //Fichero = File.OpenWrite("libros.dat");
                Fichero = File.OpenWrite(ruta);
                Fichero.Position = nroRegistroModificar * 93;
                //Console.WriteLine("direccion {0} para escribir el {1} en el registro actualmente borrado  ", Fichero.Position,nroRegistroEliminar);
                escribirCampo("Codigo", 5, 5, ref Fichero);
                escribirCampo("Titulo", 50, 50, ref Fichero);
                escribirCampo("Autor", 30, 30, ref Fichero);
                escribirCampo("Año", 4, ref Fichero);
                escribirCampoBorrado(ref Fichero);
                Fichero.Flush();
            }
        }
        public static void eliminarLibro(ref FileStream Fichero,string codigo,string ruta)
        {//PRECONDICION{ fichero abierto para lectura} 
         //POSTCONDICION{ registro elininado lógicamente ,archivo abierto}
            byte [] buffer = new byte[93];
            bool encontrado = false ;
            int nroRegistroEliminar = 0;
            int nroRegistroPuntero = 0;
            string codigoComparativo;
            //necesitamos encontrar el registro a eliminar,en que numero de registro esta en este caso
            while (Fichero.Position < Fichero.Length && encontrado == false)
            {
                Fichero.Read(buffer, 0, 93);
                codigoComparativo = Encoding.ASCII.GetString(buffer, 0, 5).Trim();
                //debemos encontrar el registro  HABILITADO a eliminar 
                encontrado = (codigoComparativo == codigo && BitConverter.ToInt32(buffer, 89) == 0);
                //Console.WriteLine("codigo buffer {0}  codigo parametro {1} ",codigoComparativo, codigo);
                
            }
            if (encontrado) 
            {// si lo encontramos procedemos a llenar el campo registro.borrado = -1
             // y marcamos el ultimo registro.borrado = Numero De Registro a eliminar
                //Console.Write("nro de registro {0} en el que está el codigo: {1} ", (Fichero.Position*93)-1,codigo);     
                nroRegistroEliminar = (((int)Fichero.Position / 93)-1);
                //Console.WriteLine("registro encontrado en el nro : {0}", nroRegistroEliminar);
                //Fichero.Position = 0;
                //buscamos el nro de registro con un -1 en su campo borrado
                //Console.WriteLine("posicon inical{0}", Fichero.Position);
                //comenzamos a leer desde la cabecera el campo borrado y seguir los registros que éste marca
                Fichero.Read(buffer, 0, 93);
                encontrado = (BitConverter.ToInt32(buffer, 89) ==-1);
                while (encontrado == false)
                {
                    nroRegistroPuntero= (BitConverter.ToInt32(buffer, 89));   
                    // aqui vamos a la DIRECCION DONDE ESTA GUARDADO EL registro al cual apuntan la lista de borrados
                    Fichero.Position = nroRegistroPuntero * 93;
                    Fichero.Read(buffer, 0, 93);
                    //Console.WriteLine("nro registro siguiente : {0}" ,(Fichero.Position/93) -1);
                    // if (registro.borrado == -1 )
                    if (BitConverter.ToInt32(buffer, 89) == -1)
                    {
                        //Console.Write("posicion {0}  para insertar  registro  :{1}  ", Fichero.Position, nroRegistroEliminar);
                        encontrado = true;
                    }
                }
                //Console.WriteLine(" direccion {0} bytes donde se encontro el -1  {1} ", Fichero.Position, (Fichero.Position / 93) - 1);
                nroRegistroPuntero= (int)(Fichero.Position / 93) - 1;
                //Console.WriteLine("nro registro para colocar marca fin {0}  en posicion {1}  bytes ", nroRegistroPuntero,Fichero.Position);
                Fichero.Close();
                Fichero = File.OpenWrite(ruta);
                Fichero.Position = (nroRegistroEliminar * 93) +89;
                //Console.WriteLine("direccion {0} para escribir el {1} en el registro actualmente borrado  ", Fichero.Position,nroRegistroEliminar);
                buffer = BitConverter.GetBytes((int)-1);
                Fichero.Write(buffer, 0, 4);
                Fichero.Position = (nroRegistroPuntero * 93) + 89;
                buffer = BitConverter.GetBytes(nroRegistroEliminar); ;
                Fichero.Write(buffer, 0, 4);   
            }
        }
        #endregion
        static int generarSecuenciasOrdenadas(string rutaOrigen,int M,int fr,string campo)
        {//PRECONDICION{M :marcos de pagina >=1 , fr>0 enteros, campo="T" o "C" dependiendo del campo a ordenar}
        //POSTCONDICION{secuencias guardadas en carpeta SECUENCIAS/SOT_nro O SECUENCIAS/SOC_nro}

            FileStream Fichero = File.Open(rutaOrigen, FileMode.Open);
            //Console.WriteLine("tamaño" + ((Fichero.Length / 93) - 1));
            string nombreCampo;
            nombreCampo = (campo == "T" ? "TITULO" : "CODIGO");
            int nroRegistros = (int)(Fichero.Length / 93) - 1;// -1 restando la cabecera
            //Console.WriteLine("nro Registros"+nroRegistros);
            int nroBloques = nroRegistros / fr;
            
            Byte[] memoriaIntermedia = new Byte[M * fr * 93];
            FileStream FicheroSecuencias;
            int N = -1;
            if (nroRegistros > 0)
            {
                Fichero.Position = 93;// situamos en el primer registro
                int nroBloquesLlenos = nroBloques / M;
                //Console.WriteLine("llenos" + nroBloquesLlenos);
                //int nroBloquesRestantes = nroBloques % M;
                int nroRegistrosRestantes = nroRegistros - nroBloquesLlenos * M * fr;//100-96
                int contador = 0;
                for (int i = 0; i < nroBloquesLlenos; i++)
                {
                    String secuencia = "Secuencias/SO" + campo + "_" + contador+".dat";
                    Fichero.Read(memoriaIntermedia, 0, M * fr * 93);
                    //FicheroSecuencias = File.Open(secuencia, FileMode.Append);
                    FicheroSecuencias = File.Open(secuencia, FileMode.Create);
                    ordenar(ref memoriaIntermedia,campo);
                    FicheroSecuencias.Write(memoriaIntermedia, 0, memoriaIntermedia.Length);
                    FicheroSecuencias.Close();
                    Console.WriteLine("===================================================================");
                    Console.WriteLine("SECUENCIA ORDENADA por{"+nombreCampo+"} Path: " + secuencia);
                    Console.WriteLine("===================================================================");
                    mostrarSecuenciaOrdenada(secuencia);
                    Console.WriteLine("===================================================================");
                    contador++;
                }
                if (nroRegistrosRestantes > 0)
                {
                    byte[] resto = new byte[nroRegistrosRestantes * 93];
                    //Console.WriteLine("registros restantes" + nroRegistrosRestantes);
                    String secuencia = "Secuencias/SO" + campo + "_" + contador+".dat";
                    Console.WriteLine("===========================================================================");
                    Console.WriteLine("SECUENCIA ORDENADA por{" + nombreCampo + "} Path: " + secuencia);
                    Console.WriteLine("============================================================================");
                    Fichero.Read(resto,0, nroRegistrosRestantes*93);
                    FicheroSecuencias = File.Open(secuencia, FileMode.Create);
                    ordenar(ref resto, campo);
                    FicheroSecuencias.Write(resto, 0, resto.Length);
                    FicheroSecuencias.Close();
                    mostrarSecuenciaOrdenada(secuencia);
                    Console.WriteLine("===========================================================================");
                    contador++;
                }
                N = contador;
                Fichero.Close();
            }
            else
                Console.WriteLine("No hay registros para ordenar...");
            return N;
        }
        static void mostrarSecuenciaOrdenada(string ruta)
        {
            //MUESTRA EL CONTENIDO DE UN ARCHIVO SIN CABECERA
            FileStream Fichero = File.OpenRead(ruta);
            //Console.WriteLine("posicion" + Fichero.Position);
            int tamanio = (int)Fichero.Length;
            byte[] buffer = new byte[93];
            while (Fichero.Position < tamanio)
            {
                Fichero.Read(buffer, 0, 93);
                mostrarRegistro(buffer);
            }
            Fichero.Close();
        }
        static void mostrarSecuenciaAbierta(FileStream Fichero)
        {//precondicion{archivo abierto modo open}
        //postcondicion{muestra en pantalla los registros , fichero sigue abierto}
         //MUESTRA EL CONTENIDO DE UN ARCHIVO SIN CABECERA
            int tamanio = (int)Fichero.Length;
            byte[] buffer = new byte[93];
            while (Fichero.Position < tamanio)
            {
                Fichero.Read(buffer, 0, 93);
                mostrarRegistro(buffer);
            }
        }
        static void ordenar(ref byte [] memoriaIntermedia,string campo)
        {//precondicion{ORDENAR POR campo = "T" de titulo , "C" por codigo de libro}
         //postcondicion{SECUENCIA ORDENADA POR CAMPO POR EL METODO DE INSERCION } 
            byte []auxiliar= new byte [93];         
            int nroRegistros = memoriaIntermedia.Length/93;
            int j;
            for (int i = 0; i < nroRegistros; i++)
            {
                auxiliar = registroIesimo(i,memoriaIntermedia);
                j = i - 1;
                while (j >= 0 && compararRegistro(registroIesimo(j,memoriaIntermedia),auxiliar,campo)>0)
                {
                    ponerRegistro(j+1,registroIesimo(j,memoriaIntermedia),ref memoriaIntermedia);
                    j--;
                }
                ponerRegistro(j+1,auxiliar,ref memoriaIntermedia);
            }
        }
        static void ponerRegistro(int pos,byte [] registroInsertar,ref byte [] memoria)
        {//intercambia  registros en memoria
            int k = 0;
            for (int j = pos * 93; j < (pos + 1) * 93; j++)
            {
                memoria[j] = registroInsertar[k];
                k++;
            }
        }
        static int compararRegistro(byte[] reg1, byte[] reg2,string campo)
        {//comparacion por orden lexicografico de los campos strings dados
        //POSTCONDICION{1 si reg1 >reg2 , 0 si reg1=reg2, -1 si reg1<reg2}
            int inicio=-1, tamanio=-1;
            if (campo == "T")
            {
                inicio = 5;
                tamanio = 50;
            }
            if (campo == "C")
            {
                inicio = 0;
                tamanio = 5;
            }
            string cadena1 = Encoding.ASCII.GetString(reg1,inicio,tamanio);
            string cadena2 = Encoding.ASCII.GetString(reg2, inicio, tamanio);
            //Console.WriteLine("comparando {0}  y {1}", cadena1, cadena2);
            return cadena1.CompareTo(cadena2);
        }
        static int compararRegistroMostrandoComparaciones(byte[] reg1, byte[] reg2, string campo)
        {
            {//comparacion por orden lexicografico de los campos strings dados
                //POSTCONDICION{1 si reg1 >reg2 , 0 si reg1=reg2, -1 si reg1<reg2}
                int inicio = -1, tamanio = -1;
                if (campo == "T")
                {
                    inicio = 5;
                    tamanio = 50;
                }
                if (campo == "C")
                {
                    inicio = 0;
                    tamanio = 5;
                }
                string cadena1 = Encoding.ASCII.GetString(reg1, inicio, tamanio);
                string cadena2 = Encoding.ASCII.GetString(reg2, inicio, tamanio);
                int res = cadena1.CompareTo(cadena2);
                Console.WriteLine("comparando  {0}  y  {1}", cadena1, cadena2);
                return res;
            }
        }
        static byte [] registroIesimo(int i,byte [] array)
        {//recupera el registro iésimo indexable desde 0 a longitudarray/93
            
            byte [] resul = new byte [93];
            int k=0;
            for (int j=i*93; j < (i+1)*93; j++)
            {
                resul[k]=array[j] ;
                k++;
            }
            return resul;
        }
        static void procesarSecuenciasOrdenadas(int M,int N,int fr,string campo)
        {//PRECONDICION{Secuencias ordenadas creadas /Secuencias}
         //POSTCONDICION{PROCESO TOTAL PARA REALIZAR LA ORDENACION POR MEZCLA}
            int contador = 0;
            int numeroDeArchivoMezcla = 0;
            bool flag = false;
            int i = 0;
            int Nanterior;
            while(N>M)
            {
                Nanterior = N;
                int k = 0;
                int nroSecuenciasCompletas = N / (M - 1);
                while(k<nroSecuenciasCompletas)
                {//mezclamos como dice el algoritmo tomando de M-1 secuencias
                    MezclarDemo(i, M - 1, numeroDeArchivoMezcla, flag,M,fr,campo);
                    MezclarSecuencias(i, M - 1, numeroDeArchivoMezcla, flag, M, fr, campo);
                    contador++;
                    numeroDeArchivoMezcla++;
                    i = i + M - 1;
                    k++;
                }
                int nroSecuenciasRestantes = N % (M - 1);
                N = nroSecuenciasCompletas;
                if(nroSecuenciasRestantes>0)
                {
                    //Mezclamos las secuencias restantes
                    MezclarDemo(i, nroSecuenciasRestantes, numeroDeArchivoMezcla, flag, M, fr,campo);
                    MezclarSecuencias(i, nroSecuenciasRestantes, numeroDeArchivoMezcla, flag, M, fr, campo);
                    numeroDeArchivoMezcla++;
                    contador++;
                    N = N + 1;                    
                }
                if (flag == false)
                {//estamos procesando Secuencias ordenadas, reiniciamos contadores para tratar Secuencias Mezcla
                    i = 0;
                    contador = 0;
                }
                else
                {
                    numeroDeArchivoMezcla = contador + Nanterior;
                    contador = contador + k;
                    i = numeroDeArchivoMezcla - N;
                }
                flag = true;
            }
            //ultima o primera mezcla , se ejecuta directamente cuando N<=M
             MezclarDemo(i, N, numeroDeArchivoMezcla , flag, M, fr,campo);
             MezclarSecuencias(i, N, numeroDeArchivoMezcla, flag, M, fr, campo);
             Console.WriteLine("ARCHIVO ORDENADO EXITOSAMENTE....ver ultimo archivo en carpeta bin/debug/Mezclas...");
        }
        static void MezclarDemo(int inicio,int nroSecuenciasProcesar,int numeracionMezclaSalida,bool flag,int M,int fr,string campo)
        {// funcion que solo muestra qué secuencias mezclar y que producen 
            string rutaSalida = "SM" + campo + "_" + numeracionMezclaSalida;
            string rutaSalidaAbsoluta = "Mezclas/SM" + campo + "_" + numeracionMezclaSalida+".dat";
            if(!flag)//FAlso caso en el que agarramos las secuencias ordenadas
            {
                Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
                string rutaInicial = "SO" + campo + "_" + inicio;//SOT_0 +o SOTC_0
                // ruta inicial sin el indice
                string rutaInicialAbsoluta = "Secuencias/SO" + campo + "_"; //SOT_ +o SOTC_
                Console.WriteLine("Mezclar(secuencia {0} ,{1}= nrosecuencias ,{2}= numeracionMezcla,{3}->Secuencias,{4} Marcos ,{5}= Factor de bloqueo)", inicio, nroSecuenciasProcesar, numeracionMezclaSalida, flag, M, fr);
                Console.WriteLine(" =======>  Mezclamos " + nroSecuenciasProcesar + " SECUENCIAS ORDENADAS desde " + rutaInicial + " hasta SO" + campo + "_" + (inicio + nroSecuenciasProcesar - 1) + "Para generar el archivo " + rutaSalida );//+" "+rutaInicialAbsoluta+" "+rutaSalidaAbsoluta);
                //Mezclar(rutaInicialAbsoluta,inicio,nroSecuenciasProcesar,rutaSalidaAbsoluta,M,fr,campo)
            }
            else// caso en el que agarramos las Mezclas
            {
                Console.WriteLine("|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
                string rutaInicial = "SM" + campo + "_" + inicio;//SOCM_0 +o SOTM_0
                // ruta inicial sin el indice
                string rutaInicialAbsoluta = "Mezclas/SM" + campo + "_";//SMT_ +o SMTC_
                Console.WriteLine("Mezclar(secuencia {0} ,{1}= nrosecuencias ,{2}= numeracionMezcla,{3}->Mezclas,{4} Marcos ,{5}= Factor de bloqueo)", inicio, nroSecuenciasProcesar, numeracionMezclaSalida, flag, M, fr);
                Console.WriteLine(" =======> Mezclamos " + nroSecuenciasProcesar + " SECUENCIAS  MEZCLA desde " +rutaInicial+" hasta SM" + campo + "_" + (inicio + nroSecuenciasProcesar - 1) + "Para generar el archivo " + rutaSalida); //+ " " +" "+rutaInicialAbsoluta+" "+rutaSalidaAbsoluta);
                //Mezclar(rutaInicialAbsoluta, inicio, nroSecuenciasProcesar, rutaSalidaAbsoluta, M, fr,campo);
            }
        }
        static void MezclarSecuencias(int inicio,int nroSecuenciasProcesar,int numeracionMezclaSalida,bool flag,int M,int fr,string campo)
        {//funcion que mezcla propiamente las secuencias
            string rutaSalida = "SM" + campo + "_" + numeracionMezclaSalida;
            string rutaSalidaAbsoluta = "Mezclas/SM" + campo + "_" + numeracionMezclaSalida+".dat";
            if(!flag)//FAlso caso en el que agarramos las secuencias ordenadas
            {
                string rutaInicial = "SO" + campo + "_" + inicio;//SOT_0 +o SOTC_0
                // ruta inicial sin el indice
                string rutaInicialAbsoluta = "Secuencias/SO" + campo + "_"; //SOT_ +o SOTC_
                
                /*Console.Write("Mezclar({0},{1},{2},{3},{4},{5})", inicio, nroSecuenciasProcesar, numeracionMezclaSalida, flag, M, fr);
                Console.WriteLine(" =======>  Mezclamos " + nroSecuenciasProcesar + " SECUENCIAS ORDENADAS desde " + rutaInicial + " hasta SO" + campo + "_" + (inicio + nroSecuenciasProcesar - 1) + "Para generar el archivo " + rutaSalida );//+" "+rutaInicialAbsoluta+" "+rutaSalidaAbsoluta);*/
                Mezclar(rutaInicialAbsoluta,inicio,nroSecuenciasProcesar,rutaSalidaAbsoluta,M,fr,campo);
            }
            else// caso en el que agarramos las Mezclas
            {
                string rutaInicial = "SM" + campo + "_" + inicio;//SOCM_0 +o SOTM_0
                // ruta inicial sin el indice
                string rutaInicialAbsoluta = "Mezclas/SM" + campo + "_";//SMT_ +o SMTC_
                /*Console.Write("Mezclar({0},{1},{2},{3},{4},{5})", inicio, nroSecuenciasProcesar, numeracionMezclaSalida, flag, M, fr);
                Console.WriteLine(" =======> Mezclamos " + nroSecuenciasProcesar + " SECUENCIAS  MEZCLA desde " + rutaInicial + " hasta SM" + campo + "_" + (inicio + nroSecuenciasProcesar - 1) + "Para generar el archivo " + rutaSalida); //+ " " +" "+rutaInicialAbsoluta+" "+rutaSalidaAbsoluta);*/
                Mezclar(rutaInicialAbsoluta, inicio, nroSecuenciasProcesar, rutaSalidaAbsoluta, M, fr, campo);
                /*Console.WriteLine("Presione cualquier tecla para seguir Mezclando.....");
                Console.ReadKey();*/
            }
        }
        static void Mezclar(string rutaInicial,int inicio, int nroSecuenciasProcesar, string rutaMezclaSalida,int M, int fr,string campo)
        {
            
            if(nroSecuenciasProcesar==1)
            {// mezclar una secuencia es igual a la misma secuencia, ordena el archivo completamente en memoria si el numero de marcos lo permite
                File.Copy(rutaInicial + inicio+".dat", rutaMezclaSalida);
                Console.WriteLine("Archivo Mezcla "+rutaMezclaSalida +"Es .......");
                mostrarSecuenciaOrdenada(rutaMezclaSalida);
            }
            else
            {
                //variables
                int indice=inicio;//indice para mezclar las secuencias 
                int fin = nroSecuenciasProcesar;
                byte [] memoria=new byte [M*fr*93];
                //matriz auxiliar para saber cuales bloques de memoria estan vacios
                bool [] ocupado = new bool [M*fr];
                for (int k = 0; k < ocupado.Length; k++)
                {
                    ocupado[k] = false;
                }
                //crear fichero de salida
                FileStream FicheroSalida = File.Open(rutaMezclaSalida,FileMode.Create);
                FileStream [] Ficheros = new FileStream[nroSecuenciasProcesar];
                // cargamos las n secuencias en un arreglo de Ficheros
                Console.WriteLine("Archivos a mezclar : ");
                for (int n = 0; n < Ficheros.Length; n++)
                {
                    Console.WriteLine("Secuencia " + n +"-ésima "+ rutaInicial+indice+".dat");
                    Ficheros[n] = File.Open(rutaInicial+indice+".dat",FileMode.Open);
                    
                    mostrarSecuenciaAbierta(Ficheros[n]);
                    Ficheros[n].Position = 0;
                    //Console.WriteLine("Ficheros posi" + Ficheros[n].Position);
                    indice++;
                }
                Console.WriteLine("Presione cualquier tecla para VER EL PROCESO DE MEZCLA...");
                Console.ReadKey();
                //cargar a memoria principal las primeras tuplas de las N-Secuencias
                int indiceMemoria = 0;
                int j = 0;
                for (int k = 0; k < Ficheros.Length; k++)
                {
                    //Console.WriteLine("k es {0} lengthficheros {1}", k, Ficheros.Length);
                    //Console.WriteLine("se quedo j = " + j);
                    //Console.WriteLine("indiceMemoria= {0} ",indiceMemoria);
                    while (Ficheros[k].Position < Ficheros[k].Length && j < fr)
                    {
                        //Console.WriteLine("Ficheros "+k +"position " + Ficheros[k].Position);
                        //Console.WriteLine("ocupado " + indiceMemoria + " a true");
                        ocupado[indiceMemoria] = true;
                        Ficheros[k].Read(memoria, indiceMemoria * 93, 93);
                        //Console.WriteLine("Ficheros["+k+"].read(memoria,{0},{1}",indiceMemoria*93,93);// j*93
                        //Console.WriteLine("Ficheros[" + k + "].read(memoria,{0}-ésimo ,{1} bytes   ===> ", indiceMemoria, 93);// j*93
                        //Console.WriteLine("Ficheros["+k+"] position luego leer" + Ficheros[k].Position);
                        indiceMemoria++;
                        j++;
                        
                    }
                    j = 0;
                }
                Console.WriteLine("=================== PRIMERAS {0} BLOQUES DE LAS {1} SECUENCIAS CARGADAS EN MEMORIA INTERMEDIA EXITOSAMENTE===============",M,nroSecuenciasProcesar);
                Console.WriteLine("=================== LAS COMPARACIONES, ESCRITURA Y LECTURA SE HACEN EN MEMORIA     =============== ");
                /*for (int l = 0; l < ocupado.Length; l++)
                {
                    Console.WriteLine(ocupado[l]);
                }*/
                bool termina = termino(ocupado);
                while(termina)// BUCLE PARA PROCESAR EN MEMORIA LAS SECUENCIAS
                {
                    //encontramos la posicion correspondiente al menor elemento,suponiendo que el primer habilitado es el menor 
                    // y haciendo las comparaciones con los demas elementos  en memoria intermedia
                    int posicionMenor=0;
                    bool encontrado=false;
                    int i=0;
                    while(!encontrado && i<ocupado.Length)
                    {
                        if (ocupado[i])
                        {
                            posicionMenor = i;
                            encontrado = true;
                            Console.WriteLine("indice de elemento candidato a ser el MENOR " + i);
                        }
                        i++;
                    }
                    
                    while ( i < ocupado.Length)
                    {
                        if (ocupado[i])
                        {
                            int comparacion = compararRegistroMostrandoComparaciones(registroIesimo(posicionMenor, memoria), registroIesimo(i, memoria), campo);
                            if (comparacion >=0)//registromenor > reg-i
                            {
                                Console.WriteLine("indice nuevo candidato a ser el MENOR = " + i);
                                posicionMenor = i;
                            }
                        }
                        i++;
                    }
                    
                    Console.WriteLine("indice del elemento MENOR "+posicionMenor);
                    Console.WriteLine("!!!!! ESCRITURA EN EL ARCHIVO {0}",rutaMezclaSalida);
                    Console.WriteLine("ESCRIBIENDO EN {0} EL REGISTRO  COMPARADO POR el indice {1} ",rutaMezclaSalida,posicionMenor);
                    //escribimos el registro de memoria intermedia de posicion=posicionMenor{ memoria indexable de 0-M*fr}
                    FicheroSalida.Write(memoria, posicionMenor * 93, 93);
                    //BUCLE PARA SABER LA SOBRE QUE FICHERO VAMOS AGREGAR para M=3 y fr=3 posiciones{0,1,2,3,4,5,6,7,8}
                    //  para fichero[0] le corresponde el bloque consistente desde las posiciones 0-->2 y asi sucesivamente para los demás
                    int m = 0;
                    int k = 0;
                    while(m<Ficheros.Length)
                    {
                        if (m * fr <= posicionMenor && posicionMenor <= (m+1)*fr)
                            k = m;
                        m++;
                    }
                    Console.WriteLine("Existen más bloques o registros en el Fichero[{0}]??     ",k);
                    //
                    if (Ficheros[k].Position < Ficheros[k].Length)
                    {
                        Console.WriteLine("Sí");
                        /*Console.WriteLine("antes : "+Ficheros[k].Position);
                        Console.WriteLine("Fichero{0}.read(memoria,{1},{2}",k,posicionMenor*93,93);*/
                        Ficheros[k].Read(memoria, posicionMenor * 93, 93);
                        //Console.WriteLine("despues : "+Ficheros[k].Position);
                    }
                    else
                    {
                        Console.WriteLine("NO");
                        ocupado[posicionMenor] = false;
                    }

                    termina = termino(ocupado);
                    if(termina)
                        Console.WriteLine("Aún hay elementos en memoria intermedia ");
                    else
                        Console.WriteLine("NO hay elementos en memoria intermedia, fin de la mezcla");
                }
                //mostrarSecuenciaAbierta(FicheroSalida);

                FicheroSalida.Close();
                Console.WriteLine("***************SECUENCIA MEZCLA "+rutaMezclaSalida+ " Es:   ********************************************");
                mostrarSecuenciaOrdenada(rutaMezclaSalida);
                Console.WriteLine("*********************************************************************************************************");
                Console.WriteLine();
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
        static bool termino(bool [] ocupado)
        {//funcion para saber si la memoria intermedia esta vacia , termina cuando todos son FALSE
            int i=0;
            bool res = false;
            while(i<ocupado.Length)
            {
                res = res || ocupado[i];
                i++;
            }
            return res;
        }
        static void insertarLibros(string ruta)
        {
             FileStream Fichero = File.Open(ruta, FileMode.Append);
             object[] libros = { "L0100", "Tristram Shandy", "Laurence Sterne", 1759, "L0066", "El guardian entre el centeno", "Jerome David Salinger", 1951, "L0065", "El ruido y la furia", "William Faulkner", 1929 
             ,"L0078","Por quien doblan las campanas","Ernest Hemingway",1940,"L0062","En busca del tiempo perdido","Marcel Proust",1913,"L0042","La montana magica","Thomas Mann",1924,"L0095","La insoportable levedad del ser","Milan kundera",1984
             ,"L0083","Hijos de la medianoche","Salman Rushdie",1980,"L0027","Alicia en el pais de las maravillas","Lewis Carroll",1865,"L0017","Don quijote de la mancha","Miguel de Cervantes Saavedra",1605
             ,"L0004","La semana laboral de 4 horas","Timothy Ferriss",2007,"L0039","Memorias de una geisha","Arthur Golden",1977,"L0055","Romeo y julieta","William Shakespeare",1975,"L0003","Piense y hagase rico","Napoleon Hill",1937
             ,"L0013","El cerebro y la inteligencia emocional","Daniel Goleman",1995,"L0063","Mujercitas","Louisa May Alcott",1868,"L0060","Almas muertas","Nikolai Gogol",1982,"L0043","Cronica de una muerte anunciada","Gabriel Garcia Marquez",1981
             ,"L0050","Grandes esperanzas","Charles Dickens",1860,"L0002","Juan Salvador Gaviota","Richard Bach",1970,"L0011","Yo estoy bien, tu estas bien","Thomas Anthony Harris",1969,"L0094","La tierra baldia","Thomas Stearns Eliot",1922
             ,"L0087","Jacques el fatalista y su maestro","Denis Diderot",1780,"L0054","Pantaleon y las visitadoras","Mario Vargas Llosa",1973,"L0034","Cumbres borrascosas","Ellis Bell",1847,"L0007","El poder del pensamiento positivo","Norman Vicent Peale",1952
             ,"L0006","Quien se ha llevado mi queso","Spencer Jhonson M. D.",1998,"L0045","Harry Potter y la piedra filosofal","J.K.Rowling",1997,"L0052","Dona barbara","Romulo gallegos",1929,"L0064","El proceso","Franz Kafka",1925
             ,"L0080","Asi hablo zaratustra","Friedrich Nietzsche",1885 ,"L0015","Los hombres son de marte,las mujeres de venus","Jhon Gray",1992 ,"L0009","Zen y el arte del mantenimiento de la motocicleta","Robert M. Pirsig",1974
             ,"L0036","Mil soles esplendidos","Khaled Hosseine",2007,"L0049","La ciudad y los perros","Mario Vargas Llosa",1963,"L0071","La ladrona de libros","Markus Zusak",2005,"L0001","El hombre en busca de sentido","Victor Frankl",1946
             ,"L0010","Pensar rapido, pensar despacio","Daniel Kahneman",2011,"L0038","Los viajes de Gulliver","Jonathan Swift",1726,"L0051","La casa de munecas","Henrik Ibsen",1879,"L0041","La isla misteriosa","Julio Verne",1875
             ,"L0005","La ciencia de hacerse rico","Wallace D. Wattles",1910,"L0014","Tus zonas erroneas","Wayne W. Dyer",1976,"L0092","Romancero gitano","Federico Garcia",1928,"L0090","Cometas en el cielo","Khaled Hosseini",2003
             ,"L0088","Las ciudades invisibles","Italo Calvino",1972,"L0086","La metamorfosis","Franz Kafka",1916,"L0093","El alquimista","Paulo Coelho",1988,"L0091","El hombre invisible","Herbert George Wells",1897,"L0098","La Eneida","Publio Virgilio Maron",-30
             ,"L0097","La naranja mecanica","Anthony Burgess",1962,"L0008","Anatomia de la melancolia","Alberto Manguel",1621,"L0012","Siddhartha","Hermann hesse",1922,"L0016","Cien anos de soledad","Gabriel Garcia Marquez",1967
             ,"L0018","Hamlet","William Shakespeare",1599,"L0040","El diario de Ana Frank","Ana Frank",1942,"L0031","El conde de Montecristo","Alexandre Dumas",1844,"L0026","Rebelion en la granja","George Orwell",1945
             ,"L0023","Lo que el viento se llevo","Margaret Mitchell",1936,"L0022","El retrato de dorian grey","Oscar Wilde",1890,"L0019","El principito","Antoine de Saint Exupery",1943,"L0096","El laberinto de la soledad","Octavio Paz",1950
             ,"L0089","El codigo da vinci","Dan Brown",2003,"L0085","Emma","Jane Austen",1815,"L0069","Ana Karenina","Leon tolstoi",1877,"L0082","El hombre sin atributos","Robert Musil",1930,"L0068","Las uvas de ira","Jhon Steinbeck",1939
             ,"L0079","El corazon de las tinieblas","Joseph Conrad",1899,"L0077","Historia de dos cuidades","Charles Dickens",1859,"L0076","El tambor de hojalata","Gunter Grass",1959,"L0072","Los juegos del hambre","Suzanne Collins",2008,"L0070","Los pilares de la tierra","Ken Foollett",1989
             ,"L0073","El nombre de la rosa","Umberto Eco",1980,"L0081","Memorias de Adriano","Marguerite Yourcenar",1951,"L0074","Las aventuras de Huckleberry Finn","Mark Twain",1885,"L0075","Decameron","Giovanni Boccaccio",1351,"L0047","La casa de los espiritus","Isabel Allende",1982
             ,"L0046","El viejo y el mar","Ernest Miller Hemingway",1952,"L0044","Los miserables","Victor Hugo",1862,"L0057","Todo se desmorona","Chinua Achebe",1958,"L0056","La vuelta al mundo en 80 dias","Julio Verne",1872
             ,"L0059","Lolita","Vladimir Nabokov",1955,"L0067","Ulises","James Joyce",1922,"L0084","Rojo y negro","Stendha",1830,"L0061","El faro","P.D.James",2006,"L0053","Edipo Rey","Sofocles",-430
             ,"L0058","El extranero","Albert Camus",1942,"L0035","Matar a un ruisenor","Harper Lee",1960,"L0037","Frankenstein","Mary Shelley",1818,"L0033","Un mundo feliz","Aldous Huxley",1932,"L0032","Madame Bovary","Gustave Flaubert",1957,"L0024","Moby-Dick","Herman Melville",1851
             ,"L0029","El senor de los anillos","J.R.R Tolkien",1954,"L0048","Dracula","Bram Stoker",1897,"L0025","Guerra y paz","Leon Tolstoi",1869,"L0028","El gran Gabtsby","F. Scott Fitzgerald",1925,"L0020","Orgullo y Prejuicio","Jane Auste",1813
             ,"L0030","Crimen y Castigo","Fiodor Dostoievski",1866,"L0021","1984","George Orwell",1984,"L0101","La palabra del mudo","Juan Ramon Ribeyro",1973 };
             //Console.WriteLine("nro de libros"+libros.Length/4); //100 libros
             for (int i = 0; i <=libros.Length-4; i=i+4)
             {
                 escribirCampo("Codigo", 5, 5, ref Fichero, (string)libros[i]);
                 escribirCampo("Titulo", 50, 50, ref Fichero, (string)libros[i+1]);
                 escribirCampo("Autor", 30, 30, ref Fichero, (string)libros[i+2]);
                 escribirCampo("Año", 4, ref Fichero, (int)libros[i+3]);
                 escribirCampoBorrado(ref Fichero);
             } 
             Fichero.Close(); 
        }
        public static void limpiarCrearDirecoriosMezclaSecuencia(string directorioSecuencias,string directorioMezclas)
        {// funcion para evitar conflictos cuando se vuelven a ejecutar las mezclas externas más de una vez
            if (Directory.Exists(directorioSecuencias))
            { Directory.Delete(directorioSecuencias, true); }
            
            Directory.CreateDirectory(directorioSecuencias); 
            if (Directory.Exists(directorioMezclas))
            { Directory.Delete(directorioMezclas, true); }
             Directory.CreateDirectory(directorioMezclas); 
        }
    }
}
