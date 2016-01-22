using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.DataConstruction;

namespace PholioVisualisation.Cache
{
    public class PracticeAxisRepository : ObjectWebCache<PracticeAxis>
    {
        private static object lockObject = new object();

        private const string KeyFormat = "{0}-{1}-{2}-{3}";

        protected override string RepositoryKey
        {
            get { return "1"; }
        }

        public PracticeAxis GetAxis(int indicatorId, int groupId, int sexId, int dataPointOffset)
        {
            if (UseWebCache)
            {
                string key = string.Format(KeyFormat, indicatorId, sexId, groupId, dataPointOffset);
                lock (lockObject)
                {
                    var d = Dictionary;
                    return d.ContainsKey(key) ?
                         d[key] :
                         null;
                }
            }
            return null;
        }

        public void AddAxis(int indicatorId, int groupId, int sexId, int dataPointOffset, PracticeAxis axis)
        {
            if (UseWebCache && axis != null)
            {
                string key = string.Format(KeyFormat, indicatorId, sexId, groupId, dataPointOffset);

                lock (lockObject)
                {
                    Dictionary<string, PracticeAxis> d = Dictionary;
                    if (d.ContainsKey(key) == false)
                    {
                        d[key] = axis;
                    }
                }
            }
        }
    }
}
