using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using Newtonsoft.Json;

namespace CyberBiology.Core.Serialization
{
    public class WorldSerializer
    {
        private static string SerializeObject(object obj)
        {
            var jsonSerializerSettings = CreateJsonSerializerSettings();
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        private static JsonSerializerSettings CreateJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
            };
        }

        private static T Deserialize<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var serializer = CreateJsonSerializer();
                    return serializer.Deserialize<T>(jsonTextReader);
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
        
        public WorldInfo LoadWorldInfo(string path)
        {
            using (var zip = ZipFile.Read(path))
            {
                return LoadWorldInfo(zip);
            }
        }

        private static WorldInfo LoadWorldInfo(ZipFile zip)
        {
            if (!zip.ContainsEntry("WorldInfo"))
            {
                throw new BadImageFormatException("Wrong zip file, WorldInfo not found");
            }

            var worldEntry = zip["WorldInfo"];
            var worldInfo = Deserialize<WorldInfo>(worldEntry.OpenReader());

            return worldInfo;
        }

        public IEnumerable<WorldChunk> LoadWorldChunks(string path)
        {
            using (var zip = ZipFile.Read(path))
            {
                int chunkNumber = 0;
                var chunkName = "WorldChunk_" + chunkNumber;
                while (zip.ContainsEntry(chunkName))
                {
                    var worldChunkEntry = zip[chunkName];
                    var worldChunk = Deserialize<WorldChunk>(worldChunkEntry.OpenReader());
                    yield return worldChunk;

                    chunkNumber++;
                    chunkName = "WorldChunk_" + chunkNumber;
                }
            }
        }

        public void Save(World world, string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var worldInfo = new WorldInfo(world);
            var worldSplitter = new WorldSplitter();
            
            using (var zip = new ZipFile(Encoding.UTF8))
            {
                foreach (var worldChunk in worldSplitter.Split(world))
                {
                    worldInfo.ChunksNumber = worldChunk.Number;
                    zip.AddEntry("WorldChunk_" + worldChunk.Number, SerializeObject(worldChunk));
                }

                zip.AddEntry("WorldInfo", SerializeObject(worldInfo));

                zip.Save(path);
            }
        }
    }
}
