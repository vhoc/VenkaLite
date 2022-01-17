using System;

namespace VenkaLite
{
    class Program
    {
        static void Main(string[] args)
        {
            string queries;

            if( args.Length > 0 && args[0] == "--hours" )
            {
                Console.WriteLine( "[" + DateTime.Now + "] Intentando enviar los horarios de la sucursal a la nube." );
                try {
                    LocalData.SetOpeningHours();
                    Console.WriteLine( "[" + DateTime.Now + "] Horarios enviados" );
                } catch( Exception ex ) {
                    Console.WriteLine( "[" + DateTime.Now + "] ERROR: " + ex.Message );
                }
                
            }

            // Application parameter --config triggers settings prompt.
            if ( args.Length > 0 && args[0] == "--config")
            {
                Settings.Create();
            }
            else
            {
                Console.WriteLine( "[" + DateTime.Now + "] Servidor VenkaLite iniciado." );

                // Validates the configuration file's presence.
                if ( ! Settings.Check() )
                {
                    Console.WriteLine( "[" + DateTime.Now + "] ERROR: VenkaLite no está configurado. Ejecute la aplicación como 'Administrador' o 'root' con el parámetro '--config'." );
                    Environment.Exit(0);
                }

                // Obtains the SQL queries from the Venka API, exit the application if none is obtained.
                queries = RemoteQueries.Get( Settings.Value( "DbEngine" ), Security.DecryptString("1234567890123456", Settings.Value( "AuthKey" ) ) );

                if ( String.IsNullOrEmpty( queries ) )
                {
                    Environment.Exit(0);
                }

                // Sends the data obtained from the SoftRestaurant database using the queries to the Venka API.
                RemoteData.Put( LocalData.Get( queries ), Security.DecryptString("1234567890123456", Settings.Value( "AuthKey" ) ) );
            }
        }
    }
}
