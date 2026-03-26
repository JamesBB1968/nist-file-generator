public class NistFileCreator
{
    // Constants for field separator and record separator
    private const byte FS = 0x1C; // Field Separator
    private const byte RS = 0x1E; // Record Separator

    public byte[] BuildType1RecordBytes(string fieldNumber, string fieldName, string fieldValue)
    {
        return CreateRecordBytes("1", fieldNumber, fieldName, fieldValue);
    }

    public byte[] BuildType2RecordBytes(string fieldNumber, string fieldName, string fieldValue)
    {
        return CreateRecordBytes("2", fieldNumber, fieldName, fieldValue, true);
    }

    public byte[] BuildType10HeaderBytes(string fieldNumber, string fieldName, string fieldValue)
    {
        return CreateRecordBytes("10", fieldNumber, fieldName, fieldValue, true);
    }

    private byte[] CreateRecordBytes(string recordType, string fieldNumber, string fieldName, string fieldValue, bool addRecordSeparator = false)
    {
        // Create the record components
        var record = new List<byte>();
        record.AddRange(System.Text.Encoding.ASCII.GetBytes(recordType));
        record.Add(FS);
        record.AddRange(System.Text.Encoding.ASCII.GetBytes(fieldNumber));
        record.Add(FS);
        record.AddRange(System.Text.Encoding.ASCII.GetBytes(fieldName));
        record.Add(FS);
        record.AddRange(System.Text.Encoding.ASCII.GetBytes(fieldValue));

        if (addRecordSeparator)
        {
            record.Add(RS);
        }

        return record.ToArray();
    }
}