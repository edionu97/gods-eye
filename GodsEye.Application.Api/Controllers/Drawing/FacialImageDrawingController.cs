using System;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Application.Api.Messages;
using GodsEye.Application.Services.ImageManipulator;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.Application.Api.Controllers.Drawing
{
    [ApiController]
    [Route("api/drawing/facial-image")]
    public class FacialImageDrawingController : ControllerBase
    {
        private readonly IFacialImageManipulatorService _facialImageManipulatorService;

        public FacialImageDrawingController(IFacialImageManipulatorService facialImageManipulatorService)
        {
            _facialImageManipulatorService = facialImageManipulatorService;
        }

        [HttpPost("resize")]
        public async Task<IActionResult> ResizeAsync([FromBody] ResizeImageInfo resizeImageInfo)
        {
            //treat the null case
            if (string.IsNullOrEmpty(resizeImageInfo?.ImageBase64))
            {
                return BadRequest("The image must not be null");
            }

            //resize the image
            try
            {

                //get the new image
                var newImage = await _facialImageManipulatorService
                    .ResizeFaceImageAsync(
                        resizeImageInfo.ImageBase64,
                        (resizeImageInfo.ToWidth, resizeImageInfo.ToHeight));

                //return the answer
                return Ok(new
                {
                    Image = newImage
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpPost("extract-face")]
        public async Task<IActionResult> CropFaceAsync([FromBody] FacialExtractionInfo facialExtraction)
        {
            //treat the null case
            if (string.IsNullOrEmpty(facialExtraction?.ImageBase64))
            {
                return BadRequest("The image must not be null");
            }

            //treat the null case
            if (facialExtraction.BoundingBox == null)
            {
                return BadRequest("The cropping coordinates must not be null");
            }

            //crop the face
            try
            {
                //get the new image (resized or not)
                string newImage;
                if (facialExtraction.ToWidth.HasValue && facialExtraction.ToHeight.HasValue)
                {
                    newImage = await _facialImageManipulatorService
                        .ExtractFaceAsync(
                            facialExtraction.ImageBase64,
                            facialExtraction.BoundingBox,
                            (facialExtraction.ToWidth.Value, facialExtraction.ToHeight.Value));
                }
                else
                {
                    newImage = await _facialImageManipulatorService
                        .ExtractFaceAsync(
                            facialExtraction.ImageBase64, facialExtraction.BoundingBox);
                }

                //return the answer
                return Ok(new
                {
                    Image = newImage
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpPost("roi")]
        public async Task<IActionResult> DrawFaceDetectionAsync([FromBody] FacialIdentificationInfo facialIdentification)
        {
            //treat the null case
            if (string.IsNullOrEmpty(facialIdentification?.ImageBase64))
            {
                return BadRequest("The image must not be null");
            }

            //treat the null case
            if (facialIdentification.BoundingBox == null)
            {
                return BadRequest("The cropping coordinates must not be null");
            }
            //crop the face
            try
            {
                //highlight the face in image
                var newImage = await _facialImageManipulatorService
                    .IdentifyAndDrawRectangleAroundFaceAsync(
                        facialIdentification.ImageBase64,
                        facialIdentification.BoundingBox,
                        facialIdentification.DrawingOptions,
                        facialIdentification.FaceKeypointsLocation);

                //return the answer
                return Ok(new
                {
                    Image = newImage
                });
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
