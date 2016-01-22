using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.Analysis;

namespace PholioVisualisation.ServiceActions
{
    public class SpcForDsrLimitsAction
    {
        public SpcForDsrLimitsResponseObject GetResponse(double comparatorValue,
            double populationMin, double populationMax, double unitValue, int yearRange)
        {
            ParameterCheck.GreaterThanZero("Comparator value", comparatorValue);
            ParameterCheck.GreaterThanZero("Population min", populationMin);
            ParameterCheck.GreaterThanZero("Population max", populationMax);
            ParameterCheck.GreaterThanZero("Unit value", unitValue);
            ParameterCheck.GreaterThanZero("Year range", yearRange);

            List<ControlLimits> points = new ControlLimitsBuilderSpcForDsr
            {
                UnitValue = unitValue,
                YearRange = yearRange
            }.GetSpcForDsrLimits(comparatorValue, populationMin, populationMax);

            return new SpcForDsrLimitsResponseObject
            {
                ComparatorValue = comparatorValue,
                Points = points
            };
        }
    }
}
