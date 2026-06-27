using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace NJG.Runtime.Entity
{
    public static class GhostRunSerializer
    {
        public static string ExportToCode(GhostRunData runData)
        {
            string json = JsonUtility.ToJson(runData);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            using MemoryStream output = new();
            using (GZipStream gzip = new(output, CompressionMode.Compress))
            {
                gzip.Write(jsonBytes, 0, jsonBytes.Length);
            }
            return Convert.ToBase64String(output.ToArray());
        }

        public static bool TryImportFromCode(string code, out GhostRunData runData)
        {
            runData = null;
            
            try
            {
                byte[] compressedBytes = Convert.FromBase64String(code);
                
                using MemoryStream input = new(compressedBytes);
                using GZipStream gzip = new(input, CompressionMode.Decompress);
                using MemoryStream output = new();
                
                gzip.CopyTo(output);
                
                string json = Encoding.UTF8.GetString(output.ToArray());
                runData = JsonUtility.FromJson<GhostRunData>(json);
                
                return runData?.Frames is { Count: > 0 };
            }
            catch
            {
                return false;
            }
        }
    }
}