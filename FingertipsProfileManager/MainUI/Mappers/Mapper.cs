using System.Collections.Generic;
using System.Linq;
using Fpm.MainUI.ViewModels;
using Fpm.MainUI.ViewModels.Profile;
using Fpm.ProfileData.Entities.LookUps;
using Fpm.ProfileData.Entities.Profile;
using Fpm.ProfileData.Entities.User;

namespace Fpm.MainUI.Mappers
{
    public static class Mapper
    {
        public static ProfileViewModel ToProfileViewModel(this ProfileDetails profileDetails)
        {
            var profileViewModel = new ProfileViewModel();
            AutoMapper.Mapper.Map(profileDetails, profileViewModel);
                
            return profileViewModel;
        }

        public static ProfileDetails ToProfileDetails(this ProfileViewModel profileViewModel)
        {
            var profileDetails = new ProfileDetails();
            AutoMapper.Mapper.Map(profileViewModel, profileDetails);
            
            profileDetails.ArePdfs = profileViewModel.SelectedPdfAreaTypes != null &&
                                     profileViewModel.SelectedPdfAreaTypes.Any();


            if(profileViewModel.ProfileUsers != null)
            {
                profileDetails.UserPermissions = profileViewModel.ProfileUsers
                        .Select(user => new UserGroupPermissions()
                        {
                            UserId = user.Id,
                            FpmUser = new FpmUser()
                            {
                                Id = user.Id, 
                                DisplayName = user.Name
                            }
                        });
            }
            
            if (profileViewModel.SelectedPdfAreaTypes != null)
            {
                profileDetails.PdfAreaTypes = profileViewModel.SelectedPdfAreaTypes
                    .Select(areaType => new AreaType() {Id = areaType.Id});
            }
            return profileDetails;
        }

        public static IEnumerable<ProfileUser> ToProfileUserList(this IEnumerable<FpmUser> fpmUsers)
        {
            return fpmUsers.OrderBy(user => user.DisplayName)
                                .Select(user => new ProfileUser()
                                {
                                    Id = user.Id,
                                    Name = user.DisplayName
                                });
        }

        public static IEnumerable<ProfileUser> ToProfileUserList(this IEnumerable<UserGroupPermissions> userGroupPermissions )
        {
            return userGroupPermissions
                                .OrderBy((s=>s.FpmUser.DisplayName))
                                .Select(user => new ProfileUser()
                                {
                                    Id = user.FpmUser.Id,
                                    Name = user.FpmUser.DisplayName
                                });
        }

        public static IEnumerable<ProfileAreaType> ToProfileAreaTypeList(this IEnumerable<AreaType> areaTypes)
        {
            return areaTypes
                                .Select(areaType => new ProfileAreaType()
                                {
                                    Id = areaType.Id,
                                    Value = areaType.Name
                                });
        }       
    }

   

}