using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace FingertipsUploadService
{
    public partial class FingertipsUploadService : ServiceBase
    {
        private readonly Logger _logger = LogManager.GetLogger("FingertipsUpload");
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
            _logger.Info("--- STARTED ---");
            EveryHalfSec();
        }

        protected override void OnStop()
        {
            _logger.Info("--- STOPPED ---");
        }

        private void EveryHalfSec()
        {
            var timer = new Timer(
            e => _logger.Info("every half sec."),
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(.5));   
        }
    }
}
