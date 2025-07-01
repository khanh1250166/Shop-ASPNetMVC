namespace SV21T1020102.BusineesLayers
{

    public static class Configuration
    {
        private static string connectionString = "";
        /// <summary>
        /// Khoi tao cau hinh cho BussineesLayres
        /// </summary>
        public static void Initialize(string connectionString)
        {
            Configuration.connectionString = connectionString;
        }
        /// <summary>
        /// Chuoi tham so ket noi CSDL
        /// </summary>
        public static string ConnectionString 
        { 
            get 
            { 
                return connectionString; 
            } 
        }
    }
}
