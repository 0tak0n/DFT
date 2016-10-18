using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DFT
{
    public class WAV
    {
        struct WavHeader
        {
            public byte[] riffID; // "riff"
            public uint size;  // ファイルサイズ-8
            public byte[] wavID;  // "WAVE"
            public byte[] fmtID;  // "fmt "
            public uint fmtSize; // fmtチャンクのバイト数
            public ushort format; // フォーマット
            public ushort channels; // チャンネル数
            public uint sampleRate; // サンプリングレート
            public uint bytePerSec; // データ速度
            public ushort blockSize; // ブロックサイズ
            public ushort bit;  // 量子化ビット数
            public byte[] dataID; // "data"
            public uint dataSize; // 波形データのバイト数
        }

        public static IList<short> Read(string filename)
        {
            WavHeader Header = new WavHeader();
            List<short> lDataList = new List<short>();
            List<short> rDataList = new List<short>();

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                try
                {
                    Header.riffID = br.ReadBytes(4);
                    Header.size = br.ReadUInt32();
                    Header.wavID = br.ReadBytes(4);
                    Header.fmtID = br.ReadBytes(4);
                    Header.fmtSize = br.ReadUInt32();
                    Header.format = br.ReadUInt16();
                    Header.channels = br.ReadUInt16();
                    Header.sampleRate = br.ReadUInt32();
                    Header.bytePerSec = br.ReadUInt32();
                    Header.blockSize = br.ReadUInt16();
                    Header.bit = br.ReadUInt16();
                    Header.dataID = br.ReadBytes(4);
                    Header.dataSize = br.ReadUInt32();

                    // for (int i = 0; i < Header.dataSize / Header.blockSize; i++)
                    while (fs.Position < fs.Length)
                    {
                        lDataList.Add((short)(((short)br.ReadUInt16())));

                        if (Header.channels == 2 && fs.Position < fs.Length)
                            rDataList.Add((short)br.ReadUInt16());
                    }
                }
                finally
                {
                    if (br != null)
                    {
                        br.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }


            return lDataList;
        }


        public static void Write(string filename, IList<short> data , uint sampleRate)
        {
            //ここで加工（とりあえず素通り）

            WavHeader Header = new WavHeader();

            //Header.dataSize = (uint)Math.Max(lNewDataList.Count, rNewDataList.Count) * 4;
            Header.dataSize = (uint)Math.Max(data.Count, 0) * 2;
            Header.blockSize = 2;
            Header.bit = 16;
            Header.channels = 1;
            Header.sampleRate = sampleRate;
            Header.bytePerSec = 44100;
            Header.fmtSize = 16;
            Header.format = 1;
            Header.size = (uint)Math.Max(data.Count, 0) * 2 + 36;

            Header.riffID = new byte[] { 82, 73, 70, 70 };
            Header.wavID = new byte[] { 87, 65, 86, 69 };
            Header.fmtID = new byte[] { 102, 109, 116, 32 };
            Header.dataID = new byte[] { (byte)'d', (byte)'a', (byte)'t', (byte)'a' };


            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                try
                {
                    /*
                    Action<uint> writeBEint = (uint beint) =>
                    {
                        bw.Write(new byte[]{
                            (byte)((beint & 0xFF000000)>>24),
                            (byte)((beint & 0x00FF0000)>>16),
                            (byte)((beint & 0x0000FF00)>>8),
                            (byte)((beint & 0x000000FF))
                         });
                    };

                    Action<ushort> writeBEshort = (ushort beshort) =>
                    {
                        bw.Write(new byte[]{
                            (byte)((beshort & 0x0000FF00)>>8),
                            (byte)((beshort & 0x000000FF))
                         });
                    };*/

                    bw.Write(Header.riffID);
                    bw.Write(Header.size);
                    bw.Write(Header.wavID);
                    bw.Write(Header.fmtID);
                    bw.Write(Header.fmtSize);
                    bw.Write(Header.format);
                    bw.Write(Header.channels);
                    bw.Write(Header.sampleRate);
                    bw.Write(Header.bytePerSec);
                    bw.Write(Header.blockSize);
                    bw.Write(Header.bit);
                    bw.Write(Header.dataID);
                    bw.Write(Header.dataSize);

                    for (int i = 0; i < Header.dataSize / Header.blockSize; i++)
                    {
                        if (i < data.Count)
                        {
                            bw.Write((ushort)data[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                        /*
                        if (i < rNewDataList.Count)
                        {
                            bw.Write((ushort)rNewDataList[i]);
                        }
                        else
                        {
                            bw.Write(0);
                        }
                         */
                    }
                }
                finally
                {
                    if (bw != null)
                    {
                        bw.Close();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
            }


        }
    }
}
