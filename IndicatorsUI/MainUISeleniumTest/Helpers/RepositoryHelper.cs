using System.Collections.Generic;

namespace IndicatorsUI.MainUISeleniumTest.Helpers
{
    public class RepositoryHelper
    {
        private readonly IRepositoryContainer _repositoryContainer;

        public RepositoryHelper(IRepositoryContainer repositoryContainer)
        {
            _repositoryContainer = repositoryContainer;
        }

        public RepositoryHelper()
        {
            _repositoryContainer = new RepositoryContainer();
        }

        public int ExecuteInsert(string query)
        {
            return _repositoryContainer.ExecuteInsert(query);
        }

        public void ExecuteUpdate(string query)
        {
            _repositoryContainer.ExecuteUpdate(query);
        }

        public IEnumerable<object> ExecuteQuery(string query)
        {
            return _repositoryContainer.ExecuteQuery(query);
        }
    }
}
