using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;
using System;
using System.ServiceProcess;
using System.Threading;

namespace FingertipsUploadService
{
    public partial class FingertipsUploadService : ServiceBase
    {
        private readonly Logger _logger = LogManager.GetLogger("FingertipsUpload");
        private IFusUploadRepository _fusUploadRepository;
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;

        public FingertipsUploadService()
        {
            InitializeComponent();
            _fusUploadRepository = new UploadJobRepository();
            _coreDataRepository = new CoreDataRepository();
            _loggingRepository = new LoggingRepository();
        }

        // Used for Debug only
        internal void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("--- STARTED ---");
            EveryHalfSec();
        }

        protected override void OnStop()
        {
            _coreDataRepository.Dispose();
            _logger.Info("--- STOPPED ---");
        }

        private void EveryHalfSec()
        {
            var manager = new UploadManager(_fusUploadRepository, _coreDataRepository, _loggingRepository);

            while (true)
            {
                try
                {
                    manager.ProcessUploadJobs();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(ex.GetType().FullName);
                    _logger.Error(ex.StackTrace);
                    _logger.Error(ex.Data);
                    break;
                }

                Thread.Sleep(500);
            }
        }
    }
}
