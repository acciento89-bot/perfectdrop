using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Kamilunavo.PerfectDrop.Trials
{
    public static class TrialCodec
    {
        public const int MaximumBytes=65536;
        private static readonly byte[] Magic=Encoding.ASCII.GetBytes("PDTRIALS");
        private static readonly Encoding Utf8=new UTF8Encoding(false,true);
        private static void Text(BinaryWriter writer,string text) { writer.Write(Utf8.GetBytes(text)); }
        private static string Token(BinaryReader reader)
        {
            var bytes=reader.ReadBytes(32); if(bytes.Length!=32) throw new InvalidDataException("Truncated identity.");
            var text=Utf8.GetString(bytes); TrialRules.Token(text); return text;
        }
        public static byte[] Encode(TrialBook book)
        {
            TrialRules.Validate(book);
            using(var stream=new MemoryStream())
            {
                using(var writer=new BinaryWriter(stream,Utf8,true))
                {
                    writer.Write(Magic); writer.Write(book.Schema); writer.Write(book.Revision); writer.Write(book.Records.Count);
                    foreach(var record in book.Records.OrderBy(x=>x.TrialId))
                    { writer.Write(record.TrialId);writer.Write((int)record.BestMedal);writer.Write(record.CompletedRuns);writer.Write(record.Attempts); }
                    writer.Write(book.Active!=null);
                    if(book.Active!=null)
                    {
                        var active=book.Active; Text(writer,active.RunId);Text(writer,active.AttemptId);
                        writer.Write(active.TrialId);writer.Write(active.AttemptNumber);writer.Write((int)active.State);writer.Write((int)active.Medal);
                        var snapshot=active.Snapshot;
                        writer.Write(snapshot.Placed);writer.Write(snapshot.Perfects);writer.Write(snapshot.Streak);writer.Write(snapshot.MaxStreak);writer.Write(snapshot.Phase);
                        writer.Write(snapshot.Layers.Count);
                        foreach(var layer in snapshot.Layers)
                        {writer.Write(layer.CenterX);writer.Write(layer.CenterZ);writer.Write(layer.SizeX);writer.Write(layer.SizeZ);}
                    }
                    writer.Flush();
                }
                if(stream.Length>MaximumBytes)throw new InvalidDataException("Trial archive is too large.");
                return stream.ToArray();
            }
        }
        public static TrialBook Decode(byte[] bytes)
        {
            if(bytes==null || bytes.Length<21 || bytes.Length>MaximumBytes) throw new InvalidDataException("Invalid archive length.");
            using(var stream=new MemoryStream(bytes,false))
            using(var reader=new BinaryReader(stream,Utf8,true))
            {
                if(!reader.ReadBytes(8).SequenceEqual(Magic))throw new InvalidDataException("Unknown trial archive.");
                var book=new TrialBook { Schema=reader.ReadInt32(),Revision=reader.ReadInt32() };
                if(book.Schema!=1)throw new InvalidDataException("Unsupported trial schema.");
                var count=reader.ReadInt32(); if(count<0 || count>12)throw new InvalidDataException("Invalid record count.");
                for(int i=0;i<count;i++)book.Records.Add(new TrialRecord { TrialId=reader.ReadInt32(),BestMedal=(TrialMedal)reader.ReadInt32(),CompletedRuns=reader.ReadInt32(),Attempts=reader.ReadInt32() });
                var hasActive=reader.ReadByte();if(hasActive>1)throw new InvalidDataException("Invalid active flag.");
                if(hasActive==1)
                {
                    var active=new TrialSession { RunId=Token(reader),AttemptId=Token(reader),TrialId=reader.ReadInt32(),AttemptNumber=reader.ReadInt32(),State=(TrialAttemptState)reader.ReadInt32(),Medal=(TrialMedal)reader.ReadInt32() };
                    var snapshot=new TrialSnapshot { Placed=reader.ReadInt32(),Perfects=reader.ReadInt32(),Streak=reader.ReadInt32(),MaxStreak=reader.ReadInt32(),Phase=reader.ReadSingle() };
                    var layers=reader.ReadInt32();if(layers<0 || layers>15)throw new InvalidDataException("Invalid layer count.");
                    for(int i=0;i<layers;i++)snapshot.Layers.Add(new TrialLayer { CenterX=reader.ReadSingle(),CenterZ=reader.ReadSingle(),SizeX=reader.ReadSingle(),SizeZ=reader.ReadSingle() });
                    active.Snapshot=snapshot;book.Active=active;
                }
                if(stream.Position!=stream.Length)throw new InvalidDataException("Unexpected trailing trial data.");
                TrialRules.Validate(book);
                return book;
            }
        }
    }
    public sealed class TowerTrialsFile
    {
        // Shared across instances: multiple adapters cannot bypass revision checking.
        private static readonly object Gate=new object();
        private readonly string path;
        private readonly Action<string,byte[]> write;
        public TowerTrialsFile(string path,Action<string,byte[]> writer=null)
        {
            if(string.IsNullOrWhiteSpace(path))throw new ArgumentException("Missing archive path.");
            this.path=Path.GetFullPath(path);write=writer??AtomicWrite;
        }
        private byte[] ReadBytes()
        {
            using(var stream=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                if(stream.Length<21 || stream.Length>TrialCodec.MaximumBytes)throw new InvalidDataException("Invalid archive size.");
                var bytes=new byte[(int)stream.Length];int offset=0;
                while(offset<bytes.Length) {int count=stream.Read(bytes,offset,bytes.Length-offset);if(count==0)throw new EndOfStreamException();offset+=count;}
                if(stream.ReadByte()!=-1)throw new InvalidDataException("Archive changed during read.");
                return bytes;
            }
        }
        public TrialBook Load()
        {
            lock(Gate) { return File.Exists(path)?TrialCodec.Decode(ReadBytes()):TrialBook.Empty(); }
        }
        public void Save(TrialBook proposed,int expectedRevision)
        {
            var bytes=TrialCodec.Encode(proposed);
            lock(Gate)
            {
                // Always decode CURRENT bytes, including idempotent retries. Never overwrite invalid/future files.
                byte[] currentBytes=File.Exists(path)?ReadBytes():null;
                var current=currentBytes==null?TrialBook.Empty():TrialCodec.Decode(currentBytes);
                if(currentBytes!=null && bytes.SequenceEqual(currentBytes))return;
                if(current.Revision!=expectedRevision || proposed.Revision!=checked(expectedRevision+1))
                    throw new InvalidOperationException("Trial progress changed. Reopen the saved trial.");
                write(path,bytes);
                if(!File.Exists(path) || !ReadBytes().SequenceEqual(bytes))throw new IOException("Trial save could not be verified.");
            }
        }
        private static void AtomicWrite(string destination,byte[] bytes)
        {
            var directory=Path.GetDirectoryName(destination);Directory.CreateDirectory(directory);
            var temporary=Path.Combine(directory,".perfectdrop-trials-"+Guid.NewGuid().ToString("N")+".tmp");
            try
            {
                using(var stream=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None))
                {stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
                if(!File.ReadAllBytes(temporary).SequenceEqual(bytes))throw new IOException("Temporary trial save mismatch.");
                if(File.Exists(destination))File.Replace(temporary,destination,null);
                else File.Move(temporary,destination);
            }
            finally {if(File.Exists(temporary))File.Delete(temporary);}
        }
    }
}
