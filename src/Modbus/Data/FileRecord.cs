namespace Modbus.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class FileRecord
    {
        /// <summary>
        /// 
        /// </summary>
        public ushort FileNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort RecordNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort RecordLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="recordNumber"></param>
        /// <param name="recordLength"></param>
        public FileRecord(ushort fileNumber, ushort recordNumber, ushort recordLength)
        {
            FileNumber = fileNumber;
            RecordNumber = recordNumber;
            RecordLength = recordLength;
        }
    }
}
