namespace Modbus.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadFileSubRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public byte ReferenceType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort FuleNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort RecordNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort RecordLength { get; set; }
    }
}
