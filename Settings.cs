using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace VenkaLite
{

    class Settings
    {

        public string Sucursal { get; set; }
        public string DbEngine { get; set; }
        public string AuthKey { get; set; }
        public string SqlServerInstance { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectTimeout { get; set; }
        public string ConnectRetryCount { get; set; }
        public string ConnectRetryInterval { get; set; }
        public string IntegratedSecurity { get; set; }
        public string Encrypt { get; set; }

        /*
        * Settings.Create()
        * return void
        *
        * Creates the configuration file for the application.
        */
        public static void Create()
        {
            
            // Set configuration file path and file name.
            string configPath = "";
            string vkconfig = @"vkconfig";

            // Validates the operating system in which the application is running and set the configuration file path accordingly.
            // MacOS is not yet supported and will cause the application's termination at this point.
            if ( OperatingSystem.IsWindows() ) { configPath = @Environment.GetEnvironmentVariable( "SystemRoot" ) + @"\System32\drivers\etc\"; }
            if ( OperatingSystem.IsLinux() ) { configPath = @"/etc/venka/"; }
            if ( OperatingSystem.IsMacOs() )
            {
                Console.WriteLine( "[" + DateTime.Now + "] ERROR: VenkaLite no es compatible con MacOS." );
                Console.ReadKey();
                Environment.Exit(0);
            }

            string configFile = @configPath + @vkconfig;

            // Validate the configuration file directory's existence.
            // Create the directory if the validation fails.
            if( !Directory.Exists( @configPath ) )
            {
                Console.WriteLine( "[" + DateTime.Now + "] El directorio no existe, intentando crear..." );

                try
                {
                    Directory.CreateDirectory( @configPath );
                    Console.WriteLine( "[" + DateTime.Now + "] Directorio creado" );
                }
                catch( Exception )
                {
                    Console.WriteLine( "[" + DateTime.Now + "] ERROR: No se pudo crear el directorio. Intenta correr el programa como Administrador (Windows) o root (Linux)." );
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            
            }

            // Validate the configuration file's existence.
            // Create the file if the validation fails.
            if ( File.Exists( @configFile ) )
            {
                File.Delete( @configFile );
            }

            if ( !File.Exists( @configFile ) )
            {
                Console.WriteLine( "[" + DateTime.Now + "] Intentando crear archivo..." );
                try
                {
                    using ( FileStream fs = File.Create( @configFile ) )
                    {
                        byte[] info = new UTF8Encoding( true ).GetBytes( @"" );
                        Console.WriteLine( "[" + DateTime.Now + "] Archivo de configuración creado con éxito." );                        
                        fs.Write( info, 0, info.Length );
                    }

                    // Ask for input for the configuration parameters and store them in a new instance of Settings.
                    Settings settings = new Settings();
                    
                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el ID de la Sucursal:" );
                    settings.Sucursal = @Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el ID del Motor de Base de datos:" );
                    settings.DbEngine = @Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el API Key:" );
                    settings.AuthKey = Security.EncryptString( "1234567890123456", @Console.ReadLine() );

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa la Instancia de SQL SERVER:" );
                    settings.SqlServerInstance = Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el Nombre de la Base de Datos:" );
                    settings.Database = @Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el Nombre de Usuario (Ejemplo: sa):" );
                    settings.User = Security.EncryptString( "1234567890123456", @Console.ReadLine() );

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa la Contraseña:" );
                    settings.Password = Security.EncryptString( "1234567890123456", @Console.ReadLine() );

                    // Default settings that can be changed directly in the configuration file afterwards.
                    // These are proven to work on Windows and on Linux using the venka-nanoserver image.
                    settings.ConnectTimeout = "180";
                    settings.ConnectRetryCount = "255";
                    settings.ConnectRetryInterval = "10";
                    settings.IntegratedSecurity = "false";
                    settings.Encrypt = "false";

                    // Write the settings into the configuration file.
                    using( StreamWriter w = File.AppendText( @configFile ) )
                    {
                        w.WriteLine( JsonConvert.SerializeObject( settings, Formatting.Indented ) );
                        // Register the restaurant opening hours to the database.
                    }
                    LocalData.SetOpeningHours();
                }
                catch ( Exception )
                {
                    Console.WriteLine( "[" + DateTime.Now + "] ERROR: No se pudo crear el archivo. Intenta correr el programa como Administrador (Windows) o root (Linux)." );
                    Console.ReadKey();
                    Environment.Exit(0);
                }

                
            }
            

        }

        /*
        * Settings.Value( string key )
        * return string
        *
        * Fetch and return a single settings value specified in the key parameter.
        */
        public static string Value( string key )
        {
            string configPath = "";
            string fileName = @"vkconfig";

            // Validate the configuration file's existence.
            if ( OperatingSystem.IsWindows() ) { configPath = @Environment.GetEnvironmentVariable( "SystemRoot" ) + @"\System32\drivers\etc\"; }
            if ( OperatingSystem.IsLinux() ) { configPath = @"/etc/venka/"; }

            // Read the file into a Dictionary dConfig[ key ]
            // and return only the specified value.
            try
            {
                string jConfig = File.ReadAllText( configPath + fileName );
                Dictionary<string, string> dConfig = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,string>>( jConfig );

                return dConfig[ key ];
            }
            catch ( Exception ex )
            {
                Console.WriteLine( "[" + DateTime.Now + "] ERROR: " + ex.Message );
                return "";
            }
            
        }

        /*
        * Settings.Check()
        * return bool
        *
        * Validates the configuration file's existence.
        */
        public static bool Check()
        {
            string configPath = "";
            string vkconfig = @"vkconfig";

            if ( OperatingSystem.IsWindows() ) { configPath = @Environment.GetEnvironmentVariable( "SystemRoot" ) + @"\System32\drivers\etc\"; }
            if ( OperatingSystem.IsLinux() ) { configPath = @"/etc/venka/"; }

            string configFile = @configPath + @vkconfig;

            if ( ! File.Exists( @configFile ) ) return false;

            return true;


        }

    }

}