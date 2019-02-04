'use strict';

$(document).ready(function () {

    // Index page
    initTableSorter();

    $('#profile-menu')
        .change(function () {

            var selectedProfile = $(this).val();
            if (selectedProfile !== '') {
                LoadProfileDetail(selectedProfile);
            } else {
                $('#divDetail').html('');
            }
        });

    $("form").submit(function (e) {
        if ($(this).valid()) {

            // Get all selected area-types for PDF
            var selectedAreaTypes = [];
            $('#profile-area-types input:checked').each(function () {
                selectedAreaTypes.push({
                    Id: $(this).attr('id').replace('chk', ''),
                    Value: $(this).attr('value')
                });
            });
            $('#SelectedPdfAreaTypes').val(JSON.stringify(selectedAreaTypes));

            // Get all selected profile users
            var selectedUsers = [];
            $('.selectable-check-box:checked').each(function () {
                selectedUsers.push({
                    Id: $(this).attr('id').replace('chk', ''),
                    Value: $(this).attr('value')
                });
            });
            $('#ProfileUsers').val(JSON.stringify(selectedUsers));
        }
    });
});

function LoadProfileDetail(profileId) {
    $.ajax({
        type: 'GET',
        url: '/profile/NonAdminProfileDetails?profileId=' + profileId,
        success: function(response) {
            $('#divDetail').html(response);
            $('[data-toggle="tooltip"]').tooltip();
        },
        error: function(err) {
            console.log(err);
        }
    });
}

function SaveProfileDetail() {
    var name = $('#Name').val();
    var data = $('#frmProfile').serialize();

    $.post('/profile/NonAdminProfileDetails', data)
        .done(function (response) {
            if (response) {
                $('#alert-success').show();
                $('#profile-menu :selected').text(name);
            }
        })
        .fail(function (err) {
            console.log(err);
        });
}

function CancelProfileUpdate() {
    $('#profile-menu').get(0).selectedIndex = 0;
    $('#divDetail').html('');
}

function addProfileUser() {
    var data = {
        profileId: $("[name=Id]").val(),
        FpmUserId: $("[name=userId]").val()
    };
    $.post('/Profile/AddProfileUser', data)
        .done(function (result) {
            $('#user-listing').html(result);
        })
        .fail(function (err) {
            console.log(err);
        });
}

function removeProfileUser() {
    var data = {
        profileId: $("[name=Id]").val(),
        FpmUserId: $("[name=userId]").val()
    };
    $.post('/Profile/RemoveProfileUser', data)
        .done(function (result) {
            $('#user-listing').html(result);
        })
        .fail(function (err) {
            console.log(err);
        });
}