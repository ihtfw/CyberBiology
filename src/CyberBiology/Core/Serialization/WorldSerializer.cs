using System;
using System.IO;
using System.Text;
using Ionic.Zip;
using Newtonsoft.Json;

namespace CyberBiology.Core.Serialization
{
    public class WorldSerializer
    {
        public void Serialize(World world, Stream stream)
        {
            var worldDto = new WorldDto(world);

            using (var writer = new StreamWriter(stream))
            {
                using (var jsonTextWriter = new JsonTextWriter(writer))
                {
                    var serializer = CreateJsonSerializer();
                    serializer.Serialize(jsonTextWriter, worldDto);
                }
            }
        }

        public static WorldDto Deserialize(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = CreateJsonSerializer();
                    return serializer.Deserialize<WorldDto>(jsonTextReader);
                }
            }
        }

        private static JsonSerializer CreateJsonSerializer()
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
            };
            return serializer;
        }
        
        public WorldDto Load(string path)
        {
            using (var zip = ZipFile.Read(path))
            {
                if (!zip.ContainsEntry("World"))
                {
                    throw new BadImageFormatException("Wrong zip file");
                }

                var worldEntry = zip["World"];
                using (var memoryStream = new MemoryStream())
                {
                    worldEntry.Extract(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return Deserialize(memoryStream);
                }
            }
        }

        public void Save(World world, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            using (var memoryStream = new MemoryStream())
            {
                var worldDto = new WorldDto(world);
                var serializer = CreateJsonSerializer();

                using (var writer = new StreamWriter(memoryStream))
                {
                    using (var jsonTextWriter = new JsonTextWriter(writer))
                    {
                        serializer.Serialize(jsonTextWriter, worldDto);
                        jsonTextWriter.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        using (var zip = new ZipFile(Encoding.UTF8))
                        {
                            zip.AddEntry("World", memoryStream);
                            zip.Save(path);
                        }
                    }
                }
            }
        }
    }
}
