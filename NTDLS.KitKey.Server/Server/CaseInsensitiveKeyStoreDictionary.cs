namespace NTDLS.KitKey.Server.Server
{
    internal class CaseInsensitiveKeyStoreDictionary : Dictionary<string, KeyStore>
    {
        public CaseInsensitiveKeyStoreDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
