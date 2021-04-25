using System.Threading;
using System.Threading.Tasks;
using GodsEye.RemoteWorker.Worker.FacialAnalysis.StartingInfo;

namespace GodsEye.RemoteWorker.Worker.FacialAnalysis
{
    public interface IFacialAnalysisAndRecognitionWorker
    {
        public FarwStartingInformation AnalysisSummary { get; }

        public Task StartSearchingForPersonAsync(FarwStartingInformation startingInformation, CancellationToken cancellationToken);
    }
}
