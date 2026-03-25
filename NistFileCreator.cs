using System;
using System.IO;
using System.Text;

namespace NistFileGenerator
{
    /// <summary>
    /// Creates NIST (ANSI/NIST-ITL 1-2000) files with embedded JPEG images.
    /// Supports Type 1 (Transaction), Type 2 (Descriptive), and Type 10 (Image) records.
    /// </summary>
    public class NistFileCreator
    {
        // NIST ASCII Delimiters per ANSI/NIST-ITL 1-2000
        private const char FS = (char)0x1C;  // File Separator
        private const char GS = (char)0x1D;  // Group Separator
        private const char RS = (char)0x1E;  // Record Separator
        private const char US = (char)0x1F;  // Unit Separator

        /// <summary>
        /// Creates a NIST file containing transaction info, descriptive text, and an embedded JPEG image.
        /// </summary>
        public static void CreateNistFile(
            string jpgPath,
            string nistOutputPath,
            string caseNumber = "CASE001",
            string agencyId = "BMS",
            string originatingAgency = "BE/TEST",
            string photoDate = "20251212")
        {
            if (!File.Exists(jpgPath))
                throw new FileNotFoundException($"JPEG file not found: {jpgPath}");

            // Load JPEG as binary
            byte[] jpgBytes = File.ReadAllBytes(jpgPath);

            // Build Type 1 Record (Transaction Information)
            string type1Record = BuildType1Record(caseNumber, agencyId, originatingAgency, photoDate);

            // Build Type 2 Record (Descriptive Text)
            string type2Record = BuildType2Record(caseNumber, photoDate);

            // Build Type 10 Record (Image Data)
            string type10Header = BuildType10Header(jpgBytes.Length);

            // Write all to binary file
            using (var fs = new FileStream(nistOutputPath, FileMode.Create, FileAccess.Write))
            {
                // Write Type 1
                byte[] type1Bytes = Encoding.ASCII.GetBytes(type1Record);
                fs.Write(type1Bytes, 0, type1Bytes.Length);

                // Write Type 2
                byte[] type2Bytes = Encoding.ASCII.GetBytes(type2Record);
                fs.Write(type2Bytes, 0, type2Bytes.Length);

                // Write Type 10 Header
                byte[] type10HeaderBytes = Encoding.ASCII.GetBytes(type10Header);
                fs.Write(type10HeaderBytes, 0, type10HeaderBytes.Length);

                // Write JPEG binary data
                fs.Write(jpgBytes, 0, jpgBytes.Length);

                // End with record separator
                fs.WriteByte((byte)RS);
            }

            Console.WriteLine($"✓ NIST file created: {nistOutputPath}");
            Console.WriteLine($"  Case: {caseNumber} | Agency: {agencyId} | Date: {photoDate}");
            Console.WriteLine($"  Image size: {jpgBytes.Length} bytes");
        }

        private static string BuildType1Record(string caseNumber, string agencyId, string originatingAgency, string photoDate)
        {
            var sb = new StringBuilder();
            sb.Append("1.001:LEN:160").Append(FS);
            sb.Append("1.002:VER:0502").Append(FS);
            sb.Append("1.003:CNT:3").Append(FS);
            sb.Append("1.004:TOT:EES").Append(FS);
            sb.Append("1.005:DAT:").Append(photoDate).Append(FS);
            sb.Append("1.006:PRY:1").Append(FS);
            sb.Append("1.007:DAI:").Append(agencyId).Append(FS);
            sb.Append("1.008:ORI:").Append(originatingAgency).Append(FS);
            sb.Append("1.009:TCN:").Append(caseNumber).Append(RS);
            return sb.ToString();
        }

        private static string BuildType2Record(string caseNumber, string photoDate)
        {
            var sb = new StringBuilder();
            sb.Append("2.001:LEN:80").Append(FS);
            sb.Append("2.002:IDC:0").Append(FS);
            sb.Append("2.003:SYS:0100").Append(FS);
            sb.Append("2.004:DAR:").Append(photoDate).Append(FS);
            sb.Append("2.005:CNO:").Append(caseNumber).Append(RS);
            return sb.ToString();
        }

        private static string BuildType10Header(int jpgByteLength)
        {
            // Type 10 is Image Record
            int recordLength = jpgByteLength + 150; // Estimate with header overhead
            
            var sb = new StringBuilder();
            sb.Append("10.001:LEN:").Append(recordLength).Append(FS);
            sb.Append("10.002:IDC:1").Append(FS);
            sb.Append("10.003:IMT:FACE").Append(FS);
            sb.Append("10.004:SRC:AT").Append(FS);
            sb.Append("10.005:PHD:20100526").Append(FS);
            sb.Append("10.006:HLL:1199").Append(FS);
            sb.Append("10.007:VLL:1600").Append(FS);
            sb.Append("10.008:SLC:0").Append(FS);
            sb.Append("10.009:THPS:1").Append(FS);
            sb.Append("10.010:TVPS:1").Append(FS);
            sb.Append("10.011:CGA:JPEGB").Append(FS);
            sb.Append("10.012:CSP:RGB").Append(FS);
            sb.Append("10.013:SAP:13").Append(FS);
            sb.Append("10.014:POS:F").Append(FS);
            sb.Append("10.015:IMG:"); // Binary image data follows
            return sb.ToString();
        }
    }
}