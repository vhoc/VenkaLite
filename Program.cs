using System;

namespace VenkaLite
{
    class Program
    {
        static void Main(string[] args)
        {
            if ( args.Length > 0 && args[0] == "--config")
            {
                Settings.Create();
            }
            else
            {
                Console.WriteLine( "[" + DateTime.Now + "] Servidor VenkaLite iniciado." );

                if ( ! Settings.Check() )
                {
                    Console.WriteLine( "[" + DateTime.Now + "] ERROR: VenkaLite no está configurado. Ejecute la aplicación como 'Administrador' o 'root' con el parámetro '--config'." );
                    Environment.Exit(0);
                }

                string queries = RemoteQueries.Get( Settings.Value( "Sucursal" ), Security.DecryptString("1234567890123456", Settings.Value( "AuthKey" ) ) );

                if ( String.IsNullOrEmpty( queries ) )
                {
                    Environment.Exit(0);
                }
                
                RemoteData.Put( LocalData.Get( queries ), Security.DecryptString("1234567890123456", Settings.Value( "AuthKey" ) ) );
            }
        }
    }
}
