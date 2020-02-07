using System.IO;
using MessagePack;

namespace Olm
{
    [MessagePackObject]

    public class Hdr
    {
        [Key(0)]
        public int Num { get; set; }
        [Key(1)]
        public string White { get; set; }
        [Key(2)]
        public string W_Elo { get; set; }
        [Key(3)]
        public string Black { get; set; }
        [Key(4)]
        public string B_Elo { get; set; }
        [Key(5)]
        public string Result { get; set; }
        [Key(6)]
        public string Date { get; set; }
        [Key(7)]
        public string Event { get; set; }
        [Key(8)]
        public string Round { get; set; }
        [Key(9)]
        public string Site { get; set; }
        public override string ToString()
        {
            var nl = System.Environment.NewLine;
            return "[Event \"" + Event + "\"]" + nl +
            "[Site \"" + Site + "\"]" + nl +
            "[Date \"" + Date + "\"]" + nl +
            "[Round \"" + Round + "\"]" + nl +
            "[White \"" + White + "\"]" + nl +
            "[Black \"" + Black + "\"]" + nl +
            "[WhiteElo \"" + W_Elo + "\"]" + nl +
            "[BlackElo \"" + B_Elo + "\"]" + nl +
            "[Result \"" + Result + "\"]" + nl;
        }

    }
    [MessagePackObject]
    public class ChessPack
    {
        [Key(0)]
        public Hdr[] Hdrs { get; set; }
        [Key(1)]
        public string[] MvsStrs { get; set; }
        [Key(2)]
        public uint[][] Mvss { get; set; }
        [Key(3)]
        public System.Collections.Generic.Dictionary<string, int[]> Indx { get; set; }

        public void Save(string fn)
        {
            var stream = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.None);
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray); 
            MessagePackSerializer.Serialize(stream, this, lz4Options);
            stream.Close();
        }
        public static ChessPack Load(string fn)
        {
            var stream = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            var lz4Options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
            var cp = MessagePackSerializer.Deserialize<ChessPack>(stream,lz4Options);
            stream.Close();
            return cp;
        }

    }
}
