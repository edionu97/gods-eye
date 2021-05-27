using System;
using System.Linq;
using GodsEye.RemoteWorker.Workers.Messages;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;

namespace GodsEye.Application.Middleware.MessageBroadcaster.Impl
{
    public partial class MessageBroadcasterMiddleware
    {
        /// <summary>
        /// This function it is used for serializing the response
        /// </summary>
        /// <param name="message">the message that will be serialized</param>
        /// <returns>a string representing the json response</returns>
        private static string SerializeMessageInClientResponse(IRequestResponseMessage message)
        {
            switch (message)
            {
                //handle the active worker message
                case ActiveWorkerMessageResponse activeWorkerMessageResponse:
                    {
                        //get the message content
                        var (guid, activeJobs) = activeWorkerMessageResponse.MessageContent;

                        //serialize the response
                        return JsonSerializerDeserializer<dynamic>.Serialize(new
                        {
                            GeoLocation = activeWorkerMessageResponse.Geolocation,
                            WorkerInfo = new
                            {
                                WorkerId = guid,
                                RunningJobs = activeJobs 
                            },
                            activeWorkerMessageResponse.StartedAt,
                            MessageType = nameof(ActiveWorkerMessageResponse)
                        });
                    }

                //handle the person found
                case PersonFoundMessageResponse searchForPersonResponse:
                    {
                        //unpack the object
                        var (searchResponse, image, facialResult) =
                            searchForPersonResponse.MessageContent;

                        //in a picture a person can appear a single time
                        var singleFaceResponse = searchResponse.FaceRecognitionInfo.First();
                        
                        //serialize the response
                        return JsonSerializerDeserializer<dynamic>.Serialize(new
                        {
                            FindByWorkerId = searchForPersonResponse.FindByWorkerId.ToString() ,
                            ResponseId = searchForPersonResponse.MessageId,
                            SearchStartedAt = searchForPersonResponse.StartTimeUtc,
                            FoundAt = searchForPersonResponse.EndTimeUtc,
                            GeoLocation = searchForPersonResponse.FromLocation,
                            SearchResult = new
                            {
                                SearchDetails = new
                                {
                                    BoundingBox = singleFaceResponse.FaceBoundingBox,
                                    FaceKeypoints = singleFaceResponse.FacePoints
                                },
                                Frame = image,
                                FacialAnalysisResult = new
                                {
                                    facialResult.Age,
                                    facialResult.Emotion,
                                    facialResult.Gender,
                                    facialResult.Race
                                }
                            }, 
                            SearchedPersonImage = searchForPersonResponse.SearchedPersonImageBase64,
                            MessageType = nameof(PersonFoundMessageResponse)
                        });
                    }
                
                //handle the active worker failed message response
                case ActiveWorkerFailedMessageResponse activeWorkerFailedMessageResponse:
                {
                    //get the failure summary
                    var failureSummary = activeWorkerFailedMessageResponse.FailureSummary;

                    //create the failure response
                    return JsonSerializerDeserializer<dynamic>.Serialize(new
                    {
                        failureSummary?.Status,
                        failureSummary?.FailureDetails,
                        failureSummary?.ExceptionType,
                        MessageType = nameof(ActiveWorkerFailedMessageResponse)
                    });
                }

                default:
                    throw new ArgumentException(message.GetType().FullName);
            }
        }
    }
}
