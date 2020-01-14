'use strict';

$(document).ready(function() {

    // Index page
    initTableSorter();

    $('#profile-menu')
        .change(function() {

            var selectedProfile = $(this).val();
            if (selectedProfile !== '') {
                LoadProfileDetail(selectedProfile);
            } else {
                $('#divDetail').html('');
            }
        });

    $("form").submit(function(e) {
        if ($(this).valid()) {

            // Get all selected area-types for PDF
            var selectedAreaTypes = [];
            $('#profile-area-types input:checked').each(function() {
                selectedAreaTypes.push({
                    Id: $(this).attr('id').replace('chk', ''),
                    Value: $(this).attr('value')
                });
            });
            $('#SelectedPdfAreaTypes').val(JSON.stringify(selectedAreaTypes));

            // Get all selected profile users
            var selectedUsers = [];
            $('.selectable-check-box:checked').each(function() {
                selectedUsers.push({
                    Id: $(this).attr('id').replace('chk', ''),
                    Value: $(this).attr('value')
                });
            });
            $('#ProfileUsers').val(JSON.stringify(selectedUsers));
        }
    });

    // Set contact user id
    SetContactUserIds();

    // Multi select dropdown
    ApplySelect2Style('profile-contacts');
});

function SetContactUserIds() {
    var contactUserIds = $('#hdn-contact-user-ids').val();
    if (contactUserIds) {
        $.each(contactUserIds.split(","), function (i, e) {
            $("#ContactUserIds option[value='" + e + "']").prop("selected", true);
            $('#ContactUserIds').trigger('change.select2');
        });
    }
}

function ApplySelect2Style(element) {
    if ($('.' + element).length) {
        $('.' + element).select2();
    }

    if ($('#' + element).length) {
        $('#' + element).select2();
    }
}

function LoadProfileDetail(profileId) {
    $.ajax({
        type: 'GET',
        url: '/profiles/profile/non-admin-profile-details?profileId=' + profileId,
        success: function (response) {
            $('#divDetail').html(response);
            $('[data-toggle="tooltip"]').tooltip();

            // Set contact user id
            SetContactUserIds();

            // Multi select dropdown
            ApplySelect2Style('profile-contacts');
        },
        error: function(err) {
            console.log(err);
        }
    });
}

function SaveProfileDetail() {
    var name = $('#Name').val();
    var data = $('#frmProfile').serialize();

    $.post('/profiles/profile/non-admin-profile-details', data)
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
    $.post('/profiles/profile/add-profile-user', data)
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
    $.post('/profiles/profile/remove-profile-user', data)
        .done(function (result) {
            $('#user-listing').html(result);
        })
        .fail(function (err) {
            console.log(err);
        });
}
