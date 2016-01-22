using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PholioVisualisation.PholioObjects;

namespace PholioVisualisation.DataConstruction
{
    public class EthnicityLabelBuilder
    {
        public const int EthnicityCategoryTypeId = CategoryTypeIds.EthnicGroups5;
        public const double ThreshholdForDisplayingLabel = 1;

        private StringBuilder sb;
        private double displayedEthnicity;
        private IList<CoreDataSet> dataList;
        private IList<Category> categories;

        public EthnicityLabelBuilder(IList<CoreDataSet> dataList, IList<Category> categories)
        {
            this.dataList = dataList;
            this.categories = categories;
        }

        public bool IsLabelRequired
        {
            get { return dataList.Any(); }
        }

        public string Label
        {
            get
            {
                if (IsLabelRequired)
                {
                    sb = new StringBuilder();
                    displayedEthnicity = 0;

                    AddIfRequired(CategoryIds.EthnicityMixed);
                    AddIfRequired(CategoryIds.EthnicityAsian);
                    AddIfRequired(CategoryIds.EthnicityBlack);

                    var white = GetDataByCategoryId(CategoryIds.EthnicityWhite);

                    if (displayedEthnicity > 0)
                    {
                        var nonWhiteEthnic = 100 - white.Value - displayedEthnicity;

                        if (nonWhiteEthnic >= ThreshholdForDisplayingLabel)
                        {
                            sb.Append(string.Format(", {0}other non-white ethnic groups",
                                Percentage(nonWhiteEthnic)));
                        }
                    }
                    else
                    {
                        sb.Append(string.Format("{0}non-white ethnic groups",
                                                Percentage(100 - white.Value)));
                    }
                }

                return sb.ToString();
            }
        }

        private void AddIfRequired(int categoryId)
        {
            var data = GetDataByCategoryId(categoryId);
            if (data != null &&
                data.IsValueValid &&
                data.Value >= ThreshholdForDisplayingLabel)
            {
                Add(data);
            }
        }

        private CoreDataSet GetDataByCategoryId(int categoryId)
        {
            return dataList.FirstOrDefault(x => x.CategoryId == categoryId);
        }

        private void Add(CoreDataSet data)
        {
            if (displayedEthnicity > 0)
            {
                sb.Append(", ");
            }

            displayedEthnicity += data.Value;

            sb.Append(Percentage(data.Value));

            var category = categories.First(x => x.CategoryId == data.CategoryId);
            sb.Append(category.ShortName.ToLower());
        }

        private static string Percentage(double val)
        {
            var rounded = Math.Round(val, 1, MidpointRounding.AwayFromZero);
            return string.Format("{0:0.0}% ", rounded);
        }

    }
}
