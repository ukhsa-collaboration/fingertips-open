using FingertipsUploadService.ProfileData;
using FingertipsUploadService.ProfileData.Repositories;
using NLog;
using System;
using System.ServiceProcess;

namespace FingertipsUploadService
{
    public partial class FingertipsUploadService : ServiceBase
    {
        private System.Timers.Timer _timer;

        private Logger _logger;
        private IFusUploadRepository _fusUploadRepository;
        private CoreDataRepository _coreDataRepository;
        private LoggingRepository _loggingRepository;
        private UploadManager _uploadManager;

        private bool inProgress;

        public FingertipsUploadService()
        {
            InitializeComponent();
        }

        // Used for Debug only
        internal void Start()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            _timer = new System.Timers.Timer(1000D);
            _timer.AutoReset = true;
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        public void InitLogger()
        {
            if (_logger == null)
            {
                _logger = LogManager.GetLogger("FingertipsUpload");
            }
        }

        private void LogException(Exception ex)
        {
            _logger.Error(ex.Message);
            _logger.Error(ex.GetType().FullName);
            _logger.Error(ex.StackTrace);
            _logger.Error(ex.Data);

            if (ex.InnerException != null)
            {
                LogException(ex.InnerException);
            }
        }

        public void CheckJobs()
        {
            // Are there already jobs being processed?
            if (inProgress)
            {
                return;
            }
            inProgress = true;

            // Init
            InitLogger();
            try
            {
                InitDependencies();
            }
            catch (Exception ex)
            {
                LogException(ex);
                Log("--- Initialisation failed ---");
                inProgress = false;
                return;
            }

            // Process jobs
            try
            {
                _loggingRepository.UpdateFusCheckedJobs();
                _uploadManager.ProcessUploadJobs();
            }
            catch (Exception ex)
            {
                LogException(ex);
                Log("--- Process jobs failed ---");
            }
            finally
            {
                CleanUpDependencies();
                ReaderFactory.DisposeStatic();
            }

            inProgress = false;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckJobs();
        }

        private void InitDependencies()
        {
            _fusUploadRepository = new UploadJobRepository();
            _coreDataRepository = new CoreDataRepository();
            _loggingRepository = new LoggingRepository();
            _uploadManager = new UploadManager(_fusUploadRepository, _coreDataRepository, _loggingRepository);
        }

        private void CleanUpDependencies()
        {
            _fusUploadRepository = null;
            _coreDataRepository = null;
            _loggingRepository = null;
            _uploadManager = null;
        }

        protected override void OnStop()
        {
            try
            {
                _timer.Stop();
                _timer = null;

                if (_coreDataRepository != null)
                {
                    _coreDataRepository.Dispose();
                }

                Log("--- STOPPED ---");
            }
            catch (Exception ex)
            {
                LogException(ex);
                Log("--- Stop failed ---");
            }
        }

        public void Log(string message)
        {
            _logger.Info(message);
        }

    }
}
