using System.IO;
using System.Threading.Tasks;
using GodsEye.Utility.Application.Resources.Options;

namespace GodsEye.Utility.Application.Resources.Manager
{
    public interface IResourcesManager
    {
        /// <summary>
        /// Generates a new resource based on a given template
        /// </summary>
        /// <param name="generateLocation">the location in which the file will be generated</param>
        /// <param name="generationOptions">the generation option</param>
        /// <returns>the file info of the generated file</returns>
        public Task<FileInfo> GenerateTemplateBasedResourceAsync(
            string generateLocation, TemplateGenerationOptions generationOptions);
    }
}
