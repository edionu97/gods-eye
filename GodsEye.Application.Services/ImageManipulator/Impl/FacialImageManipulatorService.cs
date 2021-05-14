using System;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using GodsEye.Utility.Application.Items.Enums;
using GodsEye.Utility.Application.Helpers.Helpers.Image;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Application.Services.ImageManipulator.Helpers;
using GodsEye.Utility.Application.Helpers.Helpers.Reflection;

namespace GodsEye.Application.Services.ImageManipulator.Impl
{
    public class FacialImageManipulatorService : IFacialImageManipulatorService
    {
        public async Task<string>
            IdentifyAndDrawRectangleAroundFaceAsync(
                string imageBase64,
                FaceLocationBoundingBox locationBox,
                DrawingOptions drawing = null,
                FaceKeypointsLocation keyPointsLocation = null)
        {

            //handle the case in which the image is null
            if (string.IsNullOrEmpty(imageBase64))
            {
                throw new ArgumentNullException();
            }

            //create a new memory stream for loading the image
            await using var stream = new MemoryStream(
                ImageHelpers.ConvertImageFromBase64(imageBase64));
            var loadedImage = await Image.LoadAsync<Rgba32>(stream);

            //draw the image
            var width = locationBox.BottomX - locationBox.TopX;
            var height = locationBox.BottomY - locationBox.TopY;

            //get the values from the drawing options
            var (thickness, (r, g, b)) = drawing ?? new DrawingOptions
            {
                Color = new Rgb
                {
                    Red = 0,
                    Green = 255,
                    Blue = 0
                },
                LineThickness = 2
            };

            //draw the rectangle
            loadedImage.Mutate(x =>
                x.Draw(
                    Color.FromRgb(r, g, b),
                    thickness,
                    new RectangleF(locationBox.TopX, locationBox.TopY, width, height)));

            //draw the key points
            if (keyPointsLocation != null)
            {
                //get all the properties of type face point
                var propertyValues = ReflectionHelpers
                    .GetPropertyValuesOfType<FacePoint>(keyPointsLocation);

                //iterate the property values
                foreach (var propertyValue in propertyValues)
                {
                    //draw the elipse
                    loadedImage.Mutate(x => x.Draw(
                        Color.FromRgb(r, g, b),
                        thickness,
                        new EllipsePolygon(propertyValue.X, propertyValue.Y, thickness)));
                }
            }


            //split the base64 to get the format and convert the image back into base64
            var splicedString = imageBase64.Split(',').FirstOrDefault();
            Enum.TryParse<ImageType>(splicedString, true, out var type);

            //return the image
            return loadedImage.ToBase64String(type.ToFormat());
        }

        public async Task<string>
            ExtractFaceAsync(
                string imageBase64,
                FaceLocationBoundingBox locationBox,
                (int Width, int Height)? size = null)
        {
            //handle the case in which the image is null
            if (string.IsNullOrEmpty(imageBase64))
            {
                throw new ArgumentNullException();
            }

            //create a new memory stream for loading the image
            await using var stream = new MemoryStream(
                ImageHelpers.ConvertImageFromBase64(imageBase64));
            var loadedImage = await Image.LoadAsync<Rgba32>(stream);

            //extract the roi
            var width = locationBox.BottomX - locationBox.TopX;
            var height = locationBox.BottomY - locationBox.TopY;
            var roi = new Rectangle(locationBox.TopX, locationBox.TopY, width, height);
            loadedImage.Mutate(i => i.Crop(roi));

            //split the base64 to get the format and convert the image back into base64
            var splicedString = imageBase64.Split(',').FirstOrDefault();
            Enum.TryParse<ImageType>(splicedString, true, out var type);
            var croppedFaceBase64 = loadedImage.ToBase64String(type.ToFormat());

            //if he size is specified resize the image
            if (size != null)
            {
                return await ResizeFaceImageAsync(croppedFaceBase64, size.Value);
            }

            //convert the image back into the base64 format
            return croppedFaceBase64;
        }

        public Task<string>
            ResizeFaceImageAsync(string imageBase64, (int Width, int Height) size)
        {
            //handle the case in which the image is null
            if (string.IsNullOrEmpty(imageBase64))
            {
                throw new ArgumentNullException();
            }

            //return the resized image
            return Task.FromResult(ImageHelpers.ResizeBase64Image(imageBase64, size));
        }
    }
}
