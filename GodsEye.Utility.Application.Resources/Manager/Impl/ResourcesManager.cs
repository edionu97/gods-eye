using System.IO;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Helpers.Helpers.Paths;
using GodsEye.Utility.Application.Items.Exceptions;
using GodsEye.Utility.Application.Resources.Options;

namespace GodsEye.Utility.Application.Resources.Manager.Impl
{
    public class ResourcesManager : IResourcesManager
    {
        public async Task<FileInfo> GenerateTemplateBasedResourceAsync(
            string generateLocation, TemplateGenerationOptions generationOptions)
        {
            //treat the null or empty path case
            if (string.IsNullOrEmpty(generateLocation))
            {
                throw new PathEmptyOrWhitespaceException(generateLocation);
            }

            var generatedFileInfo = PathHelpers
                .CreatePathToFile(generateLocation);
            //delete the file if exists
            if (generatedFileInfo.Exists)
            {
                generatedFileInfo.Delete();
            }

            //destruct the options
            var (groupId, placeholderId, templateLocation, placeholderValues) = generationOptions;

            //treat the file does not exist case
            if (!templateLocation.Exists)
            {
                throw new FileNotFoundException(templateLocation.FullName);
            }

            //create the reader and the writer
            using var templateReader = templateLocation.OpenText();
            await using var generatedFileWriter = generatedFileInfo.CreateText();

            //copy text and replace the values
            string line;
            while ((line = await templateReader.ReadLineAsync()) != null)
            {
                var match = placeholderId?.Match(line);

                //if there is a match
                if (match?.Success == true && groupId != null)
                {
                    //get the value of the placeholder
                    var groupValue = match.Groups[groupId].Value;
                    placeholderValues.TryGetValue(groupValue, out var placeholderValue);

                    //replace the value on the line
                    line = placeholderId
                        .Replace(line, placeholderValue ?? string.Empty);
                }

                await generatedFileWriter.WriteLineAsync(line);
            }

            //return the file info
            return new FileInfo(generateLocation);
        }
    }
}
