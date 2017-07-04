using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using PholioVisualisation.DataAccess;
using PholioVisualisation.Formatting;
using PholioVisualisation.PholioObjects;
using ServicesWeb.Helpers;
using ValueType = PholioVisualisation.PholioObjects.ValueType;

namespace ServicesWeb.Controllers
{
    [RoutePrefix("api")]
    public class EntitiesController : BaseController
    {
        /// <summary>
        /// Get a list of all value notes
        /// </summary>
        /// <remarks>
        /// Value notes are used to provide more information about specific data points
        /// </remarks>
        [HttpGet]
        [Route("value_notes")]
        public IList<ValueNote> GetValueNotes()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllValueNotes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a list of the recent trends
        /// </summary>
        [HttpGet]
        [Route("recent_trends")]
        public IList<TrendMarkerLabel> GetTrendMarkers(int polarityId = PolarityIds.NotApplicable)
        {
            try
            {
                return new TrendMarkerLabelProvider(polarityId).Labels;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all defined age categories
        /// </summary>
        [HttpGet]
        [Route("ages")]
        public IList<Age> GetAges()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllAges();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a specific age category.
        /// </summary>
        /// <param name="id">Age ID</param>
        [HttpGet]
        [Route("age")]
        public Age GetAge(int id)
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAgeById(id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all defined sex categories
        /// </summary>
        [HttpGet]
        [Route("sexes")]
        public IList<Sex> GetSexes()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllSexes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all category types
        /// </summary>
        [HttpGet]
        [Route("category_types")]
        public IList<CategoryType> GetCategoryTypes()
        {
            try
            {
                return ReaderFactory.GetAreasReader().GetAllCategoryTypes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the categories for a specific category type
        /// </summary>
        /// <param name="category_type_id">Category type ID</param>
        [HttpGet]
        [Route("categories")]
        public IList<Category> GetCategories(int category_type_id)
        {
            try
            {
                return ReaderFactory.GetAreasReader().GetCategories(category_type_id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all units
        /// </summary>
        [HttpGet]
        [Route("units")]
        public IList<Unit> GetUnits()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllUnits();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all year types
        /// </summary>
        [HttpGet]
        [Route("year_types")]
        public IList<YearType> GetYearTypes()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllYearTypes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a specific year type
        /// </summary>
        /// <param name="id">Year type ID</param>
        [HttpGet]
        [Route("year_type")]
        public YearType GetYearType(int id)
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetYearType(id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all value types
        /// </summary>
        [HttpGet]
        [Route("value_types")]
        public IList<ValueType> GetValueTypes()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllValueTypes();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all indicator benchmarking significance polarities
        /// </summary>
        [HttpGet]
        [Route("polarities")]
        public IList<Polarity> GetPolarities()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllPolarities();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all confidence interval methods
        /// </summary>
        [HttpGet]
        [Route("confidence_interval_methods")]
        public IList<ConfidenceIntervalMethod> GetConfidenceIntervalMethods()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllConfidenceIntervalMethods();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }


        /// <summary>
        /// Get a specific confidence interval method
        /// </summary>
        [HttpGet]
        [Route("confidence_interval_method")]
        public ConfidenceIntervalMethod GetConfidenceIntervalMethod(int id)
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetConfidenceIntervalMethod(id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get all comparator methods
        /// </summary>
        [HttpGet]
        [Route("comparator_methods")]
        public IList<ComparatorMethod> GetComparatorMethods()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllComparatorMethods();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a specific comparator/benchmarking method
        /// </summary>
        /// <param name="id">Comparator method ID</param>
        [HttpGet]
        [Route("comparator_method")]
        public ComparatorMethod GetComparatorMethod(int id)
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetComparatorMethod(id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get list of all comparators/benchmarks
        /// </summary>
        [HttpGet]
        [Route("comparators")]
        public IList<Comparator> GetComparators()
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllComparators();
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Get a specific comparator/benchmark
        /// </summary>
        /// <param name="id">Comparator ID</param>
        [HttpGet]
        [Route("comparator")]
        public Comparator GetComparator(int id)
        {
            try
            {
                return ReaderFactory.GetPholioReader().GetAllComparators()
                    .First(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the comparator significances for similarity calculations
        /// </summary>
        /// <param name="polarity_id">Polarity ID</param>
        [HttpGet]
        [Route("comparator_significances")]
        public IList<ComparatorSignificance> GetComparatorSignificances(int polarity_id)
        {
            try
            {
                var significances = new SignificanceCollection();

                if (polarity_id == PolarityIds.BlueOrangeBlue)
                {
                    significances.Add(Significance.None, "None");
                    significances.Add(Significance.Worse, "Lower");
                    significances.Add(Significance.Same, "Similar");
                    significances.Add(Significance.Better, "Higher");
                }
                else if (polarity_id == PolarityIds.RagHighIsGood ||
                         polarity_id == PolarityIds.RagLowIsGood)
                {
                    significances.Add(Significance.None, "None");
                    significances.Add(Significance.Worse, "Worse");
                    significances.Add(Significance.Same, "Similar");
                    significances.Add(Significance.Better, "Better");
                } else if (polarity_id == PolarityIds.NotApplicable)
                {
                    significances.Add(Significance.None, "None");
                }

                return significances.Significances;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }
    }
}