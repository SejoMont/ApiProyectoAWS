namespace ApiCoreProyectoEventos.Models
{
    public class KeysModel
    {
        public string ConnectionString { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public string ApiUrl { get; set; }
        public string BucketName { get; set; }
        public string BucketUrl { get; set; }
    }
}
