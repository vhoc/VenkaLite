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

        public static void Create()
        {
            
            // Set configuration file path
            string configPath = "";
            string vkconfig = @"vkconfig";

            if ( OperatingSystem.IsWindows() ) { configPath = @Environment.GetEnvironmentVariable( "SystemRoot" ) + @"\System32\drivers\etc\"; }
            if ( OperatingSystem.IsLinux() ) { configPath = @"/etc/venka/"; }
            if ( OperatingSystem.IsMacOs() )
            {
                Console.WriteLine( "[" + DateTime.Now + "] ERROR: VenkaLite no es compatible con MacOS." );
                Console.ReadKey();
                Environment.Exit(0);
            }

            string configFile = @configPath + @vkconfig;

            // First, checks if directory exists
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

            // Checks if the config file exists, if not, create it.
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

                    // Create a class Settings instance to fill in with the settings paratemers afterwards.
                    Settings settings = new Settings();
                    
                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el ID de la Sucursal:" );
                    settings.Sucursal = @Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el ID del Motor de Base de datos:" );
                    settings.DbEngine = @Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el API Key:" );
                    settings.AuthKey = Security.EncryptString( "origin ladder as", @Console.ReadLine() );

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa la Instancia de SQL SERVER:" );
                    settings.SqlServerInstance = Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el Nombre de la Base de Datos:" );
                    settings.Database = @Console.ReadLine();

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa el Nombre de Usuario (Ejemplo: sa):" );
                    settings.User = Security.EncryptString( "origin ladder as", @Console.ReadLine() );

                    Console.WriteLine( "[" + DateTime.Now + "] Ingresa la Contraseña:" );
                    settings.Password = Security.EncryptString( "origin ladder as", @Console.ReadLine() );

                    settings.ConnectTimeout = "180";
                    settings.ConnectRetryCount = "255";
                    settings.ConnectRetryInterval = "10";
                    settings.IntegratedSecurity = "false";
                    settings.Encrypt = "false";

                    // Append the jsonSettings string into the configuration file.
                    using( StreamWriter w = File.AppendText( @configFile ) )
                    {
                        w.WriteLine( JsonConvert.SerializeObject( settings, Formatting.Indented ) );
                    }
                }
                catch ( Exception )
                {
                    Console.WriteLine( "[" + DateTime.Now + "] ERROR: No se pudo crear el archivo. Intenta correr el programa como Administrador (Windows) o root (Linux)." );
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                
            }
            

        }

        public static string Value( string key )
        {
            string configPath = "";
            string fileName = @"vkconfig";

            if ( OperatingSystem.IsWindows() ) { configPath = @Environment.GetEnvironmentVariable( "SystemRoot" ) + @"\System32\drivers\etc\"; }
            if ( OperatingSystem.IsLinux() ) { configPath = @"/etc/venka/"; }

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