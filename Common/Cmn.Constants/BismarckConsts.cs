namespace Cmn.Constants
{
    /// <summary>
    /// Общие костанты. Расшарил так для простоты решения.
    /// Иначе нужно делить и, для некоторых кейсов, дублировать настройки для внешних сервисов (Arches) и бизнесового кластера (Bismarck)
    /// </summary>
    public class BismarckConsts
    {
        public const string SqlServerConnection = "data source=DESKTOP-5F5DREF\\MSSQL2016;initial catalog=Bismarck;persist security info=True;user id=q1;password=q1;";

        public const string MinioServerHost = "localhost:24001";
        public const string MinioServerAccessKey = "AKIAIOSFODNN7EXAMPLE";
        public const string MinioServerSecretKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
    }
}
