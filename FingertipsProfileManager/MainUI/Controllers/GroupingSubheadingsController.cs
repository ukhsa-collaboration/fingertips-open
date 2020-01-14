using Fpm.MainUI.ViewModels.ProfilesAndIndicators;
using Fpm.ProfileData;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Fpm.MainUI.Controllers
{
    [RoutePrefix("grouping-subheadings")]
    public class GroupingSubheadingsController : Controller
    {
        private readonly IProfilesReader _reader;
        private readonly IProfileRepository _profileRepository;

        public GroupingSubheadingsController(IProfilesReader reader, IProfileRepository profileRepository)
        {
            _reader = reader;
            _profileRepository = profileRepository;
        }

        /// <summary>
        /// Get grouping subheadings by area type id and group id
        /// </summary>
        /// <param name="areaTypeId">Area type id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>List of grouping subheadings</returns>
        [Route("by-area-type-and-group")]
        [HttpGet]
        public ActionResult GetGroupingSubheadings(int areaTypeId, int groupId)
        {
            IList<GroupingSubheading> groupingSubheadings = _profileRepository.GetGroupingSubheadings(areaTypeId, groupId);
            return Json(groupingSubheadings, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get grouping subheadings by profile id
        /// </summary>
        /// <param name="profileId">Profile id</param>
        /// <returns>List of grouping subheadings</returns>
        [Route("by-profile")]
        [HttpGet]
        public ActionResult GetGroupingSubheadingsForProfile(int profileId)
        {
            var groupIds = _reader.GetGroupingIds(profileId);

            IList<GroupingSubheading> groupingSubheadings = _profileRepository.GetGroupingSubheadingsByGroupIds(groupIds);
            return Json(groupingSubheadings, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Add grouping subheadings
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Route("add-grouping-subheadings")]
        [HttpPost]
        public ActionResult AddGroupingSubheadings(FormCollection formCollection)
        {
            var profileId = Convert.ToInt32(formCollection["profileId"]);
            var profileUrlKey = formCollection["profileUrlKey"];
            var domainSequence = Convert.ToInt32(formCollection["domainSequence"]);
            var areaTypeId = Convert.ToInt32(formCollection["areaTypeId"]);
            var groupId = Convert.ToInt32(formCollection["groupId"]);
            var subheading = formCollection["subheading"];
            var sequence = -1;

            var groupingSubheading = new GroupingSubheading
            {
                GroupId = groupId,
                AreaTypeId = areaTypeId,
                Sequence = sequence,
                Subheading = subheading
            };

            _profileRepository.SaveGroupingSubheading(groupingSubheading);

            var reorderIndicatorsViewModel = new ReorderIndicatorsViewModel
            {
                ProfileId = profileId,
                ProfileUrlKey = profileUrlKey,
                DomainSequence = domainSequence,
                AreaTypeId = areaTypeId,
                GroupId = groupId
            };

            return View("~/Views/ProfilesAndIndicators/ReorderIndicators.cshtml", reorderIndicatorsViewModel);
        }

        /// <summary>
        /// Edit grouping subheadings
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [Route("edit-grouping-subheadings")]
        [HttpPost]
        public ActionResult EditGroupingSubheadings(FormCollection formCollection)
        {
            var profileId = Convert.ToInt32(formCollection["profileId"]);
            var profileUrlKey = formCollection["profileUrlKey"];
            var domainSequence = Convert.ToInt32(formCollection["domainSequence"]);
            var areaTypeId = Convert.ToInt32(formCollection["areaTypeId"]);
            var groupId = Convert.ToInt32(formCollection["groupId"]);
            var sequence = Convert.ToInt32(formCollection["sequence"]);
            var subheading = formCollection["subheading"];
            var subheadingId = Convert.ToInt32(formCollection["subheadingId"]);

            var groupingSubheading = new GroupingSubheading
            {
                GroupId = groupId,
                AreaTypeId = areaTypeId,
                Sequence = sequence,
                Subheading = subheading,
                SubheadingId = subheadingId
            };

            _profileRepository.UpdateGroupingSubheading(groupingSubheading);

            var reorderIndicatorsViewModel = new ReorderIndicatorsViewModel
            {
                ProfileId = profileId,
                ProfileUrlKey = profileUrlKey,
                DomainSequence = domainSequence,
                AreaTypeId = areaTypeId,
                GroupId = groupId
            };

            return View("~/Views/ProfilesAndIndicators/ReorderIndicators.cshtml", reorderIndicatorsViewModel);
        }

        /// <summary>
        /// Delete grouping subheadings
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete-grouping-subheadings")]
        public ActionResult DeleteGroupingSubheadings(FormCollection formCollection)
        {
            var profileId = Convert.ToInt32(formCollection["profileId"]);
            var profileUrlKey = formCollection["profileUrlKey"];
            var domainSequence = Convert.ToInt32(formCollection["domainSequence"]);
            var areaTypeId = Convert.ToInt32(formCollection["areaTypeId"]);
            var groupId = Convert.ToInt32(formCollection["groupId"]);
            var groupingSubheadingId = Convert.ToInt32(formCollection["groupingSubheadingId"]);

            _profileRepository.DeleteGroupingSubheading(groupingSubheadingId);

            var reorderIndicatorsViewModel = new ReorderIndicatorsViewModel
            {
                ProfileId = profileId,
                ProfileUrlKey = profileUrlKey,
                DomainSequence = domainSequence,
                AreaTypeId = areaTypeId,
                GroupId = groupId
            };

            return View("~/Views/ProfilesAndIndicators/ReorderIndicators.cshtml", reorderIndicatorsViewModel);
        }
    }
}