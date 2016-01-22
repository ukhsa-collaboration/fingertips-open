﻿using System;
using System.Collections.Generic;
using System.Linq;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.PdfData
{
    public class HealthProfilesGroupRootSelector
    {
        private readonly List<GroupRoot> groupRoots = new List<GroupRoot>();
        private List<GroupRoot> mainGroupRoots;
        private List<GroupRoot> supportingGroupRoots;

        public IList<GroupRoot> MainGroupRoots
        {
            get { return mainGroupRoots; }
            set
            {
                groupRoots.AddRange(value);
                mainGroupRoots = value.ToList();
            }
        }

        public IList<GroupRoot> SupportingGroupRoots
        {
            get { return supportingGroupRoots; }
            set
            {
                groupRoots.AddRange(value);
                supportingGroupRoots = value.ToList();
            }
        }

        public GroupRoot Population
        {
            get { return GetGroupRoot(IndicatorIds.Population); }
        }

        public GroupRoot Deprivation
        {
            get { return GetGroupRoot(IndicatorIds.DeprivationBottom20Percent); }
        }

        public GroupRoot ChildHealth
        {
            get { return GetGroupRoot(IndicatorIds.ChildrenInPoverty); }
        }

        public GroupRoot XAxisOfHealthProfilesChartAtBottomOfPage2Male
        {
            get { return GetGroupRoot(IndicatorIds.XAxisOfHealthProfilesChartAtBottomOfPage2, SexIds.Male); }
        }

        public GroupRoot XAxisOfHealthProfilesChartAtBottomOfPage2Female
        {
            get { return GetGroupRoot(IndicatorIds.XAxisOfHealthProfilesChartAtBottomOfPage2, SexIds.Female); }
        }

        public GroupRoot LifeExpectancyAtBirthMale
        {
            get { return GetGroupRoot(IndicatorIds.LifeExpectancyAtBirth, SexIds.Male); }
        }

        public GroupRoot LifeExpectancyAtBirthFemale
        {
            get { return GetGroupRoot(IndicatorIds.LifeExpectancyAtBirth, SexIds.Female); }
        }

        public GroupRoot SlopeIndexOfInequalityForLifeExpectancyMale
        {
            get { return GetGroupRoot(IndicatorIds.SlopeIndexOfInequalityForLifeExpectancy, SexIds.Male); }
        }

        public GroupRoot SlopeIndexOfInequalityForLifeExpectancyFemale
        {
            get { return GetGroupRoot(IndicatorIds.SlopeIndexOfInequalityForLifeExpectancy, SexIds.Female); }
        }

        public GroupRoot ObesityYear6
        {
            get { return GetGroupRoot(IndicatorIds.ObesityYear6); }
        }

        public GroupRoot Under18AlcoholSpecificStays
        {
            get { return GetGroupRoot(IndicatorIds.Under18AlcoholSpecificStays); }
        }

        public GroupRoot TeenagePregnancy
        {
            get { return GetGroupRoot(IndicatorIds.TeenagePregnancy); }
        }

        public GroupRoot GCSEAchievement
        {
            get { return GetGroupRoot(IndicatorIds.GcseAchievement); }
        }

        public GroupRoot BreastfeedingInitiation
        {
            get { return GetGroupRoot(IndicatorIds.BreastfeedingInitiation); }
        }

        public GroupRoot SmokingAtTimeOfDelivery
        {
            get { return GetGroupRoot(IndicatorIds.SmokingAtTimeOfDelivery); }
        }

        public GroupRoot ObesityAdult
        {
            get { return GetGroupRoot(IndicatorIds.ObeseAdults); }
        }

        public GroupRoot AdultAlcoholAdmissions
        {
            get { return GetGroupRoot(IndicatorIds.AlcoholAdmissionsToHospital); }
        }

        public GroupRoot AdultSelfHarmAdmissions
        {
            get { return GetGroupRoot(IndicatorIds.HospitalStaysForSelfHarm); }
        }

        public GroupRoot AdultSmokingRelatedDeaths
        {
            get { return GetGroupRoot(IndicatorIds.AdultSmokingRelatedDeaths); }
        }

        public GroupRoot AdultExcessWeight
        {
            get { return GetGroupRoot(IndicatorIds.AdultExcessWeight); }
        }

        public GroupRoot AdultSmokingPrevalence
        {
            get { return GetGroupRoot(IndicatorIds.AdultSmokingPrevalence); }
        }

        public GroupRoot AdultPhysicalActivity
        {
            get { return GetGroupRoot(IndicatorIds.AdultPhysicalActivity); }
        }

        public GroupRoot AdultHipFractures
        {
            get { return GetGroupRoot(IndicatorIds.HipFractures); }
        }

        public GroupRoot AdultSTI
        {
            get { return GetGroupRoot(IndicatorIds.SexuallyTransmittedInfection); }
        }

        public GroupRoot AdultKilledAndSeriouslyInjuredOnRoads
        {
            get { return GetGroupRoot(IndicatorIds.KilledAndSeriouslyInjuredOnRoads); }
        }

        public GroupRoot AdultIncidenceOfTB
        {
            get { return GetGroupRoot(IndicatorIds.IncidenceOfTB); }
        }

        public GroupRoot AdultStatutoryHomelessness
        {
            get { return GetGroupRoot(IndicatorIds.StatutoryHomelessness); }
        }

        public GroupRoot AdultViolentCrime
        {
            get { return GetGroupRoot(IndicatorIds.ViolentCrime); }
        }

        public GroupRoot AdultLongTermUnemployment
        {
            get { return GetGroupRoot(IndicatorIds.LongTermUnemployment); }
        }

        public GroupRoot AdultIncidenceOfMalignantMelanoma
        {
            get { return GetGroupRoot(IndicatorIds.IncidenceOfMalignantMelanoma); }
        }

        public GroupRoot AdultDrugMisuse
        {
            get { return GetGroupRoot(IndicatorIds.AdultDrugMisuse); }
        }

        public GroupRoot AdultExcessWinterDeaths
        {
            get { return GetGroupRoot(IndicatorIds.ExcessWinterDeaths); }
        }

        public GroupRoot AdultUnder75MortalityRateCvd
        {
            get { return GetGroupRoot(IndicatorIds.AdultUnder75MortalityRateCvd); }
        }

        public GroupRoot AdultUnder75MortalityRateCancer
        {
            get { return GetGroupRoot(IndicatorIds.AdultUnder75MortalityRateCancer); }
        }

        public GroupRoot HealthInequalitiesEthnicity
        {
            get { return GetGroupRoot(IndicatorIds.HealthInequalitiesEmergencyHospitalAdmissionsByEthnicGroup); }
        }

        public GroupRoot PercentageOfPeoplePerDeprivationQuintile
        {
            get { return GetGroupRoot(IndicatorIds.PercentageOfPeoplePerDeprivationQuintile); }
        }

        public GroupRoot OverallPrematureDeaths
        {
            get { return GetGroupRoot(IndicatorIds.OverallPrematureDeaths); }
        }

        public GroupRoot OverallPrematureDeathsMale
        {
            get { return GetGroupRoot(IndicatorIds.OverallPrematureDeaths, SexIds.Male); }
        }

        public GroupRoot OverallPrematureDeathsFemale
        {
            get { return GetGroupRoot(IndicatorIds.OverallPrematureDeaths, SexIds.Female); }
        }

        public void AddRange(IList<GroupRoot> groupRoots)
        {
            this.groupRoots.AddRange(groupRoots);
        }

        public GroupRoot GetGroupRoot(int indicatorId, int sexId)
        {
            try
            {
                return groupRoots.First(x => x.IndicatorId == indicatorId && x.SexId == sexId);
            }
            catch (InvalidOperationException)
            {
                throw new FingertipsException("Indicator could not be found: " + indicatorId);
            }
        }

        public GroupRoot GetGroupRoot(int indicatorId)
        {
            try
            {
                return groupRoots.First(x => x.IndicatorId == indicatorId);
            }
            catch (InvalidOperationException)
            {
                throw new FingertipsException("Indicator could not be found: " + indicatorId);
            }
        }
    }
}