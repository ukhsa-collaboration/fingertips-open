using AutoMapper;
using IndicatorsUI.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IndicatorsUI.MainUI.Models.UserList;

namespace IndicatorsUI.MainUI.Configuration
{
    public class IndicatorListMappingProfile : Profile
    {
        public IndicatorListMappingProfile()
        {
            // View model to domain
            CreateMap<IndicatorListViewModel, UserAccess.IndicatorList>();
            CreateMap<IndicatorListItemViewModel, UserAccess.IndicatorListItem>();
            CreateMap<ViewIndicatorListViewModel, UserAccess.IndicatorList>();

            // Domain to view model
            CreateMap<UserAccess.IndicatorList, IndicatorListViewModel>();
            CreateMap<UserAccess.IndicatorListItem, IndicatorListItemViewModel>();
            CreateMap<UserAccess.IndicatorList, ViewIndicatorListViewModel>();
        }
    }
}