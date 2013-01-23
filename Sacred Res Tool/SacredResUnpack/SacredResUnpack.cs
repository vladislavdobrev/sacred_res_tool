/*
 * Games:           Sacred
 *                  Sacred Underworld
 * 
 * File extension:  res
 * 
 * Byte order:      Little Endian
 * 
 * File format:     uint32 {4}  Number of strings
 * 
 *                  // for each string
 *                  uint32 {4}  String ID
 *                  uint32 {4}  String offset
 *                  uint32 {4}  Unknown (0)
 *                  uint32 {4}  String length
 *                  
 *                  // for each string
 *                  byte {X}    Unicode string
 *                  
 * Author:          Vladislav Dobrev (xakepa)
 */

using System;
using System.IO;
using System.Text;

class SacredResUnpack
{
    static void Main(string[] args)
    {
        foreach (string item in args)
        {
            string fileName = item;

            try
            {
                FileInfo info = new FileInfo(fileName);

                FileStream input = new FileStream(fileName, FileMode.Open);
                StringBuilder sb = new StringBuilder();
                using (input)
                {
                    byte[] numberOfStrings = new byte[4];
                    input.Read(numberOfStrings, 0, 4);
                    UInt32 stringsNumber = BitConverter.ToUInt32(numberOfStrings, 0);

                    for (UInt32 i = 0; i < stringsNumber; i++)
                    {
                        byte[] stringID = new byte[4];
                        byte[] stringOff = new byte[4];
                        byte[] unknown = new byte[4];
                        byte[] stringLen = new byte[4];

                        input.Read(stringID, 0, 4);
                        input.Read(stringOff, 0, 4);
                        input.Read(unknown, 0, 4);
                        input.Read(stringLen, 0, 4);

                        byte[] realString = new byte[BitConverter.ToUInt32(stringLen, 0)];
                        Int64 currentPos = input.Position;

                        input.Seek((Int64)BitConverter.ToUInt32(stringOff, 0) + 4, SeekOrigin.Begin);
                        input.Read(realString, 0, BitConverter.ToInt32(stringLen, 0));

                        sb.Append(BitConverter.ToUInt32(stringID, 0) + "=>" + new string(Encoding.Unicode.GetChars(realString)) + Console.Out.NewLine);

                        input.Seek(currentPos, SeekOrigin.Begin);
                    }
                }

                StreamWriter sw = new StreamWriter(info.Name + ".txt", false, Encoding.GetEncoding("UTF-8"));
                using (sw)
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine("File {0} not found.", fileName);
                Console.WriteLine();
            }
            catch (DirectoryNotFoundException)
            {
                Console.Error.WriteLine("Invalid directory.");
                Console.WriteLine();
            }
            catch (IOException)
            {
                Console.Error.WriteLine("Can't open the file {0}", fileName);
                Console.WriteLine();
            }
        }
    }
}

