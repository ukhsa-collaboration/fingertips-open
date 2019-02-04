using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using PholioVisualisation.UserData.Repositories;

namespace PholioVisualisation.ServicesWeb.Helpers
{
    /// <summary>
    /// Interface to define the public id generator
    /// </summary>
    public interface IPublicIdGenerator
    {
        /// <summary>
        /// Method to get the area list public id
        /// </summary>
        /// <returns></returns>
        string GetAreaListPublicId();
    }

    /// <summary>
    /// Class to generate the public id
    /// </summary>
    public class PublicIdGenerator : IPublicIdGenerator
    {
        /// <summary>
        /// Defines the length of the public id
        /// </summary>
        public const int UidLength = 10;

        /// <summary>
        /// Defines the prefix for area list public id
        /// </summary>
        public const string AreaListPublicIdPrefix = "al-";

        private readonly IAreaListRepository _areaListRepository;

        /// <summary>
        /// Constructor to initialise the area list repository
        /// </summary>
        /// <param name="areaListRepository"></param>
        public PublicIdGenerator(IAreaListRepository areaListRepository)
        {
            _areaListRepository = areaListRepository;
        }

        /// <summary>
        /// Generate an unique area list public id
        /// </summary>
        /// <returns></returns>
        public string GetAreaListPublicId()
        {
            var uid = GetShortUid();

            // Ensure UID is unique
            while (_areaListRepository.GetAreaListByPublicId(uid) != null)
            {
                uid = GetShortUid();
            }

            return AreaListPublicIdPrefix + uid;
        }

        private static string GetShortUid()
        {
            var uid = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "")
                .Substring(0, UidLength);
            return uid;
        }
    }
}