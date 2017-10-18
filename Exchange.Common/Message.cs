namespace Exchange.Common
{
    public class Message
    {
        private byte[] _data;
        public byte[] Data { get { return _data ?? (_data = new byte[1024]); } set { _data = value; } }
    }
}