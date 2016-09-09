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
            _timer = new System.Timers.Timer(500D);
            _timer.AutoReset = true;
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        private void Init()
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

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (inProgress)
            {
                return;
            }

            inProgress = true;

            Init();

            try
            {
                _fusUploadRepository = new UploadJobRepository();
                _coreDataRepository = new CoreDataRepository();
                _loggingRepository = new LoggingRepository();
                _uploadManager = new UploadManager(_fusUploadRepository, _coreDataRepository, _loggingRepository);
                _logger.Debug("...");
            }
            catch (Exception ex)
            {
                LogException(ex);
                _logger.Info("--- Initialisation failed ---");
                inProgress = false;
                return;
            }


            try
            {
                _uploadManager.ProcessUploadJobs();
            }
            catch (Exception ex)
            {
                LogException(ex);
                _logger.Info("--- Process jobs failed ---");
            }
            finally
            {
                _fusUploadRepository = null;
                _coreDataRepository = null;
                _loggingRepository = null;
                _uploadManager = null;
                ReaderFactory.DisposeStatic();
            }

            inProgress = false;

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

                _logger.Info("--- STOPPED ---");
            }
            catch (Exception ex)
            {
                LogException(ex);
                _logger.Info("--- Stop failed ---");
            }
        }
    }
}
