using System;
using System.IO;
using ConsoleApp1.Config;
using System.Threading.Tasks;
using Gods.Eye.Server.Artificial.Intelligence.Messaging;
using GodsEye.Application.Middleware;
using GodsEye.Application.Middleware.MessageBroadcaster;
using GodsEye.Application.Middleware.WorkersMaster;
using GodsEye.Application.Persistence.Models;
using GodsEye.Application.Persistence.Repository;
using GodsEye.Application.Services.ImageManipulator.Helpers;
using GodsEye.Application.Services.ImageManipulator.Impl;
using GodsEye.Application.Services.UserService;
using Microsoft.Extensions.DependencyInjection;
using GodsEye.RemoteWorker.Workers.Messages.Responses;
using GodsEye.Utility.Application.Helpers.Helpers.Serializers.JsonSerializer;
using GodsEye.Utility.Application.Items.Geolocation.Model;

namespace ConsoleApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            //var service = new FacialImageManipulatorService();

            //var img = await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\eduard.txt");

            //var sw = JsonSerializerDeserializer<DrawingOptions>.Serialize(new DrawingOptions
            //{
            //    LineThickness = 12
            //});

            //var i = await service.IdentifyAndDrawRectangleAroundFaceAsync(img, new FaceLocationBoundingBox
            //{
            //    TopX = 896,
            //    TopY = 332,
            //    BottomX = 1278,
            //    BottomY = 806
            //},
            //    keyPointsLocation: new FaceKeypointsLocation
            //    {
            //        LeftEyePoint = new FacePoint
            //        {
            //            X = 1007,
            //            Y = 516
            //        },
            //        MouthLeftPoint = new FacePoint
            //        {
            //            X = 1010,
            //            Y = 700
            //        },
            //        MouthRightPoint = new FacePoint
            //        {
            //            X = 1162,
            //            Y = 699
            //        },
            //        RightEyePoint = new FacePoint
            //        {
            //            X = 1180,
            //            Y = 516
            //        },
            //        NosePoint = new FacePoint
            //        {
            //            X = 1085,
            //            Y = 630
            //        }
            //    });

            //get the service provider
            var serviceProvider = Bootstrapper.Load();


            var userRepo = serviceProvider.GetService<IUserService>();


            var a = await userRepo.CreateAccountAsync("eduard", "onu");





            return;
            //get the message queue
            var workersMaster =
                serviceProvider
                    .GetService<IWorkersMasterMiddleware>()
                ?? throw new ArgumentNullException();


            //get the base64 image
            var searchedFaceBase64Img =
                await File.ReadAllTextAsync(@"C:\Users\Eduard\Desktop\eduard.txt");


            //await workersMaster.StartSearchingAsync("eduard", searchedFaceBase64Img);
            //await workersMaster.StartSearchingAsync("onu", searchedFaceBase64Img);

            await workersMaster.PingWorkersAsync("eduard");
            await workersMaster.PingWorkersAsync("onu");

            Console.WriteLine("Press any key to stop...");
            Console.ReadKey();

            //await workersMaster.StopSearchingAsync("eduard", searchedFaceBase64Img);
            //await workersMaster.StopSearchingAsync("onu", searchedFaceBase64Img);
        }
    }
}
