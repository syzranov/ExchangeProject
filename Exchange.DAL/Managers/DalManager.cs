namespace Exchange.DAL.Managers
{
    public class DalManager
    {
        public static readonly DalManager Instance = new DalManager();
        
        private DalManager() { }

        private static IDbManager _dbManager;
        public IDbManager DbManager { get { return _dbManager ?? (_dbManager = new DbManager()); } }


        //private static IInsuranceFundDbManager _insuranceFundDbManager;
        //public IInsuranceFundDbManager InsuranceFundDbManager
        //{
        //    get
        //    {
        //        if (_insuranceFundDbManager == null)
        //            _insuranceFundDbManager = new InsuranceFundDbManager();
        //        return _insuranceFundDbManager;
        //    }
        //}

        //private static ISaperionDbManager _saperionManager;
        //public ISaperionDbManager SaperionDbManager
        //{
        //    get
        //    {
        //        if (_saperionManager == null)
        //            _saperionManager = new SaperionDbManager();
        //        return _saperionManager;
        //    }
        //}

        //private static ISaperionExportDbManager _saperionExportManager;
        //public ISaperionExportDbManager SaperionExportDbManager
        //{
        //    get
        //    {
        //        if (_saperionExportManager == null)
        //            _saperionExportManager = new SaperionExportDbManager();
        //        return _saperionExportManager;
        //    }
        //}

        //private static IConveraDbManager _converaDbManager;
        //public IConveraDbManager ConveraDbManager
        //{
        //    get
        //    {
        //        if (_converaDbManager == null)
        //            _converaDbManager = new ConveraDbManager();
        //        return _converaDbManager;
        //    }
        //}

        //private static IConveraExportDbManager _converaExportDbManager;
        //public IConveraExportDbManager ConveraExportDbManager
        //{
        //    get
        //    {
        //        if (_converaExportDbManager == null)
        //            _converaExportDbManager = new ConveraExportDbManager();
        //        return _converaExportDbManager;
        //    }
        //}

        //private static ILoggingManager _loggingManager;
        //public ILoggingManager LoggingManager
        //{
        //    get
        //    {
        //        if (_loggingManager == null)
        //            _loggingManager = new LoggingManager();
        //        return _loggingManager;
        //    }
        //}
    }
}
