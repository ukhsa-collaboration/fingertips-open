﻿using System;
using System.Collections.Generic;
using Fpm.ProfileData.Entities.LookUps;
using NHibernate;

namespace Fpm.ProfileData.Repositories
{
    public class LookUpsRepository : RepositoryBase
    {

        // poor man injection, should be removed when we use DI containers
        public LookUpsRepository()
            : this(NHibernateSessionFactory.GetSession())
        {
        }

        public LookUpsRepository(ISessionFactory sessionFactory)
            : base(sessionFactory)
        {

        }

        public virtual IEnumerable<CategoryType> GetCategoryTypes()
        {
            return CurrentSession.CreateCriteria<CategoryType>()
                .List<CategoryType>();
        }

        public IEnumerable<Skin> GetSkins()
        {
            return CurrentSession.CreateCriteria<Skin>()
                .List<Skin>();
        }
       
        public IEnumerable<KeyColour> GetKeyColours()
        {
            return CurrentSession.CreateCriteria<KeyColour>()
                .List<KeyColour>();
        }

        public IEnumerable<Sex> GetSexes()
        {
            return CurrentSession.QueryOver<Sex>()
                .OrderBy(x => x.Description).Asc
                .List();
        }

        public IEnumerable<Age> GetAges()
        {
            return CurrentSession.QueryOver<Age>()
                .OrderBy(x => x.Description).Asc
                .List();
        }

        public IEnumerable<Comparator> GetComparators()
        {
            return CurrentSession.QueryOver<Comparator>()
                .Where(x => x.IsCurrent)
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public IEnumerable<YearType> GetYearTypes()
        {
            return CurrentSession.QueryOver<YearType>()
                .OrderBy(x => x.Label).Asc
                .List();
        }

        public IEnumerable<IndicatorValueType> GetIndicatorValueTypes()
        {
            return CurrentSession.QueryOver<IndicatorValueType>()
                .OrderBy(x => x.Label).Asc
                .List();
        }

        public IEnumerable<ConfidenceIntervalMethod> GetConfidenceIntervalMethods()
        {
            return CurrentSession.QueryOver<ConfidenceIntervalMethod>()
                .Where(x => x.IsCurrent)
                .OrderBy(x => x.Name).Asc
                .List();
        }

        public IEnumerable<Unit> GetUnits()
        {
            return CurrentSession.QueryOver<Unit>()
                .Where(x => x.IsCurrent)
                .OrderBy(x => x.Label).Asc
                .List();
        }

        public IEnumerable<DenominatorType> GetDenominatorTypes()
        {
            return CurrentSession.QueryOver<DenominatorType>()
                .Where(x => x.IsCurrent)
                .OrderBy(x => x.Name).Asc
                .List();
        }
    }
}