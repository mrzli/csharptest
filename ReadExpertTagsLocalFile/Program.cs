using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadExpertTagsLocalFile
{
    class Program
    {
        // read from file in this format:
        // René P.Jucker
        // EX: Projektmanagement; SW Entwicklung; Project Management; Agile; Scrum; internationale Projekte
        // E1: Agile Software Entwicklung; Scrum; Web Development; App Development; Assessment Center; Projekt Management
        // E2: internationale Projekte; SW Development; SW Entwicklung
        // E3: 

        // training tags go like this
        // T86: SEO, Digital Marketing, SEM

        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("expert_tags.txt", Encoding.UTF8);

            List<TagData> tagList = ExtractTags(lines);
            List<ExpertTagData> expertTagList = ExtractExpertTags(lines, tagList);
            List<TrainingTagData> trainingTagList = ExtractTrainingTags(lines, tagList);

            List<string> tagLines = ConvertToInserts(tagList);
            List<string> expertTagLines = ConvertToInserts(expertTagList);
            List<string> trainingTagLines = ConvertToInserts(trainingTagList);

            List<string> allLines = new List<string>();
            allLines.AddRange(tagLines);
            allLines.Add("");
            allLines.AddRange(expertTagLines);
            allLines.Add("");
            allLines.AddRange(trainingTagLines);

            File.WriteAllLines("output.txt", allLines.ToArray(), Encoding.UTF8);
        }

        private static List<TagData> ExtractTags(string[] lines)
        {
            lines = lines
                .Where(l => Regex.IsMatch(l, @"^((E[X123]:)|(T(\d+):))"))
                .Select(l => GetTagNamesInLine(l))
                .SelectMany(l => l)
                .OrderBy(l => l)
                .Distinct()
                .ToArray();

            List<TagData> tagList = lines.Select((l, i) => new TagData { Id = i + 1, Name = l }).ToList();

            return tagList;
        }

        private static List<ExpertTagData> ExtractExpertTags(string[] lines, List<TagData> tagList)
        {
            List<ExpertTagData> expertTagList = new List<ExpertTagData>();
            int id = 1;

            int currentExpertId = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Match match = Regex.Match(line, @"^(\d+):.*");
                if (match.Success)
                {
                    string num = match.Groups[1].Value;
                    currentExpertId = int.Parse(num);
                }
                else
                {
                    if (currentExpertId == -1)
                    {
                        throw new Exception();
                    }

                    int type = GetExpertTagType(line);
                    if (type >= 0)
                    {
                        IEnumerable<string> tagNames = GetTagNamesInLine(line);
                        foreach (string tagName in tagNames)
                        {
                            int tagId = tagList.First(t => t.Name == tagName).Id;
                            ExpertTagData expertTagData = new ExpertTagData { Id = id, ExpertId = currentExpertId, TagId = tagId, Type = type };
                            id++;
                            expertTagList.Add(expertTagData);
                        }
                    }
                }
            }

            return expertTagList;
        }

        private static List<TrainingTagData> ExtractTrainingTags(string[] lines, List<TagData> tagList)
        {
            List<TrainingTagData> trainingTagList = new List<TrainingTagData>();
            int id = 1;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                Match match = Regex.Match(line, @"^T(\d+):.*");
                if (match.Success)
                {
                    string num = match.Groups[1].Value;
                    int trainingId = int.Parse(num);

                    IEnumerable<string> tagNames = GetTagNamesInLine(line);
                    foreach (string tagName in tagNames)
                    {
                        int tagId = tagList.First(t => t.Name == tagName).Id;
                        TrainingTagData trainingTagData = new TrainingTagData { Id = id, TrainingId = trainingId, TagId = tagId };
                        id++;
                        trainingTagList.Add(trainingTagData);
                    }
                }
            }

            return trainingTagList;
        }

        private static List<string> ConvertToInserts(List<TagData> tagList)
        {
            List<string> lines = tagList
                .Select(t => string.Format("INSERT INTO tag (id, name) VALUES ({0}, '{1}');", t.Id, t.Name))
                .ToList();

            return lines;
        }

        private static List<string> ConvertToInserts(List<ExpertTagData> expertTagList)
        {
            List<string> lines = expertTagList
                .Select(t => string.Format("INSERT INTO expert_tag (id, id_expert, id_tag, expert_tag_type) VALUES ({0}, {1}, {2}, {3});", t.Id, t.ExpertId, t.TagId, t.Type))
                .ToList();

            return lines;
        }

        private static List<string> ConvertToInserts(List<TrainingTagData> trainingTagList)
        {
            List<string> lines = trainingTagList
                .Select(t => string.Format("INSERT INTO training_tag (id, id_training, id_tag) VALUES ({0}, {1}, {2});", t.Id, t.TrainingId, t.TagId))
                .ToList();

            return lines;
        }

        private static IEnumerable<string> GetTagNamesInLine(string line)
        {
            IEnumerable<string> tagNames = line
                .Substring(line.IndexOf(':') + 1)
                .Split(',', ';')
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim());

            return tagNames;
        }

        private static int GetExpertTagType(string line)
        {
            if (line.Length < 3)
            {
                return -1;
            }

            string lineStart = line.Substring(0, 3);
            switch (lineStart)
            {
                case "EX:":
                    return 0;

                case "E1:":
                    return 1;

                case "E2:":
                    return 2;

                case "E3:":
                    return 3;

                default:
                    return -1;
            }
        }

        private class TagData
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override string ToString()
            {
                return Id + ", " + Name;
            }
        }

        private class ExpertTagData
        {
            public int Id { get; set; }
            public int ExpertId { get; set; }
            public int TagId { get; set; }
            public int Type { get; set; }

            public override string ToString()
            {
                return Id + ", " + ExpertId + ", " + TagId + ", " + Type;
            }
        }

        private class TrainingTagData
        {
            public int Id { get; set; }
            public int TrainingId { get; set; }
            public int TagId { get; set; }

            public override string ToString()
            {
                return Id + ", " + TrainingId + ", " + TagId;
            }
        }
    }
}
