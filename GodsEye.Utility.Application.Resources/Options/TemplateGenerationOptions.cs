using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GodsEye.Utility.Application.Resources.Options
{
    public class TemplateGenerationOptions
    {
        public string GroupId { get; set; }

        public Regex PlaceholderIdentifier { get; set; }

        public FileInfo TemplateLocation { get; set; }

        public IDictionary<string, string> PlaceholdersValues { get; set; }

        public void Deconstruct(
            out string groupId, 
            out Regex placeholderIdentifier, 
            out FileInfo templateLocation,
            out IDictionary<string, string> placeholdersValues)
        {
            groupId = GroupId;
            placeholderIdentifier = PlaceholderIdentifier;
            templateLocation = TemplateLocation;
            placeholdersValues = PlaceholdersValues;
        }
    }
}
