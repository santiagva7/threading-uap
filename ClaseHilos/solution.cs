namespace ClaseHilos
{
    internal class Producto
    {
        public string Nombre { get; set; }
        public decimal PrecioUnitarioDolares { get; set; }
        public int CantidadEnStock { get; set; }

        public Producto(string nombre, decimal precioUnitario, int cantidadEnStock)
        {
            Nombre = nombre;
            PrecioUnitarioDolares = precioUnitario;
            CantidadEnStock = cantidadEnStock;
        }
    }
    internal class Solution //reference: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/lock
    {

        //LISTA DE PRODUCTOS
        static List<Producto> productos = new List<Producto>
        {
            new Producto("Camisa", 10, 50),
            new Producto("Pantalón", 8, 30),
            new Producto("Zapatilla/Champión", 7, 20), // Ehh? Cómo champión Amilcar querido estamos en Arg., jajajaja.
            new Producto("Campera", 25, 100),
            new Producto("Gorra", 16, 10)
        };

        //TAREA 1:
        static void Tarea1()
        {
            lock (productos)
             {
                 foreach (var p in productos)
                 {
                     try
                     {
                        p.CantidadEnStock += 10;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"+10 en Stock de '{p.Nombre}'. Antes: {p.CantidadEnStock - 10} / Ahora: {p.CantidadEnStock}");
                        Console.ResetColor();
                    }
                    catch (Exception e)
                     {
                         Console.WriteLine("Error en la actualización de stock. Error: \n" + e.Message);
                         throw new NotImplementedException();
                     }
                 }
             }
        }

        //TAREA 2:
        static int precio_dolar = 500;
        static void Tarea2()
        {
            Thread.Sleep(1000);lock (productos)
            try
                
            {
                Console.Write($"\nPrecio actual USD/ARS: ${precio_dolar}\n");
                Console.Write("Ingrese el nuevo valor para precio_dolar: $");
                string? input = Console.ReadLine(); //Validamos que sea null porque ya en el if se comprueba que != null

                if (int.TryParse(input, out int nuevoPrecioDolar))
                {
                    precio_dolar = nuevoPrecioDolar;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"El precio del dólar ha sido actualizado correctamente. Nuevo precio: ${precio_dolar}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("El valor ingresado no es válido.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error al actualizar el precio del dólar: {e.Message}");
            }
        }

        //TAREA 3:
        static void Tarea3()
        {
            Thread.Sleep(1000);
            Console.WriteLine("\n Informe de productos: \n");
            Console.WriteLine("---------------------------------------------------");
            productos.ForEach(p =>
            {
                try
                {
                    lock (p)
                    {
                        Console.WriteLine($"El producto '{p.Nombre}', cuenta con un stock de {p.CantidadEnStock} unidades, \nEsto representa un valor total de  ARS ${p.CantidadEnStock * p.PrecioUnitarioDolares * precio_dolar}.");
                        Console.WriteLine("---------------------------------------------------");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error en la generación de informe. Error: \n" + e.Message);
                    throw new NotImplementedException();
                }
            });
        }

        //TAREA 3Bis:
        static decimal valorTotalInventario = 0;
        static void Tarea3bis()
        {
            decimal aux = 0;
            productos.ForEach(p =>
            {
                try
                {
                    lock (p)
                    {
                        aux += p.PrecioUnitarioDolares * p.CantidadEnStock;
                        valorTotalInventario = aux * precio_dolar;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error en la actualización del valor total del inventario. Error: \n" + e.Message);
                    throw new NotImplementedException();
                }
            });
            Console.WriteLine($"El valor total del inventario es de ARS ${valorTotalInventario}.\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Informe realizado por: Kampmann, Frick, Villar.\n");
        }

        //TAREA: INFLACIÓN
        static void ActualizarPrecios()
        {
            lock (productos)
            {
                foreach (var p in productos)
                {
                    try
                    {
                        p.PrecioUnitarioDolares *= 1.1m; 
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Inflación del +10% para '{p.Nombre}'. Antes: ${p.PrecioUnitarioDolares / 1.1m} / Ahora: ${p.PrecioUnitarioDolares}");
                        Console.ResetColor();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error en la actualización de precios. Error: \n" + e.Message);
                        throw;
                    }
                }
            }
        }

        //MAIN
        internal static void Execute()
        {
            do
            {
                Barrier barrier = new Barrier(4); // Inicializamos la barrera con 4 hilos

                Thread t1 = new Thread(() =>
                {
                    Tarea1();
                    barrier.SignalAndWait();
                });

                Thread t2 = new Thread(() =>
                {
                    Tarea2();
                    barrier.SignalAndWait();
                });

                Thread updatePrices = new Thread(() =>
                {
                    ActualizarPrecios(); 
                    barrier.SignalAndWait();
                });

                Thread t3 = new Thread(() =>
                {
                    barrier.SignalAndWait(); // Espera a que los hilos 1, 2 y updatePrices terminen
                    Tarea3();
                });

                t1.Start();
                t2.Start();
                updatePrices.Start();
                t3.Start();

                t1.Join();
                t2.Join();
                updatePrices.Join();
                t3.Join(); 

                Thread t3bis = new Thread(() =>
                {
                    Tarea3bis(); // Realizamos el informe final
                });
                t3bis.Start();
                t3bis.Join(); 

                Console.WriteLine("Presione cualquier tecla para continuar o 'q' para salir...");
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }

    }
}