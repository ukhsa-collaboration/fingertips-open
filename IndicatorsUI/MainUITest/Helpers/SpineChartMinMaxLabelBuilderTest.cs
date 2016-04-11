using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Profiles.DomainObjects;
using Profiles.MainUI.Helpers;
using Profiles.MainUI.Models;

namespace IndicatorsUI.MainUITest.Helpers
{
    [TestClass]
    public class SpineChartMinMaxLabelBuilderTest
    {
        [TestMethod]
        public void MinMaxLabels_Returns_WorstBest_For_RagOnlyLegendColour()
        {
            // Arrange
            SpineChartMinMaxLabel labelInput = new SpineChartMinMaxLabel();
            var keyColorId = KeyColours.RagOnly;
            var builder = new SpineChartMinMaxLabelBuilder(labelInput, keyColorId);

            // Act
            var result = builder.MinMaxLabels;

            // Assert
            Assert.IsTrue(result.Min == "Worst" && result.Max == "Best");
        }

        [TestMethod]
        public void MinMaxLabels_Returns_LowestHighest_For_BluesOnlyLegendColour()
        {
            // Arrange
            SpineChartMinMaxLabel labelInput = new SpineChartMinMaxLabel();
            var keyColorId = KeyColours.BluesOnly;
            var builder = new SpineChartMinMaxLabelBuilder(labelInput, keyColorId);

            // Act
            var result = builder.MinMaxLabels;

            // Assert
            Assert.IsTrue(result.Min == "Lowest" && result.Max == "Highest");
        }

        [TestMethod]
        public void MinMaxLabels_Returns_CorrectLabels_For_RagsAndBluesLegendColour()
        {
            // Arrange
            SpineChartMinMaxLabel labelInput = new SpineChartMinMaxLabel();
            var keyColorId = KeyColours.RagAndBlues;
            var builder = new SpineChartMinMaxLabelBuilder(labelInput, keyColorId);

            // Act
            var result = builder.MinMaxLabels;

            // Assert
            Assert.IsTrue(result.Min == "Worst/ Lowest" && result.Max == "Best/ Highest");
        }

        [TestMethod]
        public void MinMaxLabels_Returns_Default_For_NoLegendColour()
        {
            // Arrange
            SpineChartMinMaxLabel labelInput = new SpineChartMinMaxLabel();
            var keyColorId = KeyColours.Undefined;

            var builder = new SpineChartMinMaxLabelBuilder(labelInput, keyColorId);

            // Act
            var result = builder.MinMaxLabels;

            // Assert
            Assert.IsTrue(result.Min == "Worst/ Lowest" && result.Max == "Best/ Highest");
        }


    }


}
