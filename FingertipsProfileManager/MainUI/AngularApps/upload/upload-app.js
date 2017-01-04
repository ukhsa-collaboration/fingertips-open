var app = angular.module("uploadApp", ["ngRoute"]);
// Config
app.config(function($routeProvider, $httpProvider) {

    //initialize get if not there
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }

    // Disable IE ajax request caching
    $httpProvider.defaults.headers.get["If-Modified-Since"] = "Mon, 26 Jul 1997 05:00:00 GMT";
    $httpProvider.defaults.headers.get["Cache-Control"] = "no-cache";
    $httpProvider.defaults.headers.get["Pragma"] = "no-cache";

    $routeProvider
        .when("/", {
            templateUrl: "angularapps/upload/view/upload-index.html",
            controller: "uploadCtrl"
        })
        .when("/status", {
            templateUrl: "angularapps/upload/view/upload-progress.html",
            controller: "progressCtrl"
        });
});

// Web services
app.factory("uploadService", function($http) {
    var url = "/upload/";
    return {
        getJobs: function(userId) {
            return $http.get(url + "progress/" + userId);
        },
        getJobSummary: function(guid) {
            return $http.get(url + "summary/" + guid);
        },
        updateJobStatus: function(guid, newstatuscode) { // should be a post
            return $http.get(url + "change-status/" + guid + "/" + newstatuscode);
        },
        getRowsProcessed: function(guid) {
            return $http.get(url + "rows-processed/" + guid);
        },
        getAllFpmUsers: function() {
            return $http.get("/user/all");
        }
    };
});


// New Controllers
app.controller("uploadCtrl", function($scope, $window) {
    selectTab(1);
    $scope.isFileTypeOk = true;
    $scope.fileReadyForUpload = false;
    $scope.uploadPercent = "";

    $scope.pageConst = pageConst;

    // Check if the file Ext is csv or excel
    var isFileTypeAllowed = function (filePath) {
        var isAllowed = true;
        if (filePath.length > 0) {
            var ext = filePath.substring(filePath.lastIndexOf(".") + 1).toLowerCase();
            if (ext !== "xlsx" && ext !== "xls" && ext !== "csv") {
                isAllowed = false;
            }
        }
        return isAllowed;
    };

    // Upload simple file
    $scope.simpleFileSelected = function(event) {
        uploadFilter(this.value);
    };

    // Upload batch file
    $scope.batchFileSelected = function (event) {
        uploadFilter(this.value);
    };

    // Make the UI changes according to upload file type
    var uploadFilter = function (control) {
        var isFileOk = isFileTypeAllowed(control);
        
        $scope.$apply(function() {
            $scope.isFileTypeOk = isFileOk;
            $scope.fileReadyForUpload = isFileOk;
        });        
        
    };


    // Submit form using jquery.form, we are using it as 
    // polyfill for IE9 and 10. It takes the id of the form
    // submits it.
    var uploadOptions = {
        beforeSend: function() {
            $("#upload-progress").modal();
        },
        uploadProgress: function(event, position, total, percentComplete) {
            var v = percentComplete + '%';
            $scope.uploadPercent = v;
            $scope.fileReadyForUpload = false;

            $scope.$apply();
        },
        success: function() {
            $("#upload-progress").modal('toggle');
            $scope.fileuploaded = true;
            // Redirect to status page
            $window.location.href = '#/status';
        },
        complete: function(xhr) {
            clearFileUploadInputFields();
            // This is a clean up for IE Edge
            $scope.$apply(function() {
                $scope.fileReadyForUpload = false;
            });            
        }
    };

    $('#simpleUploadForm').ajaxForm(uploadOptions);
    $('#batchUploadForm').ajaxForm(uploadOptions);
    

    // Clear file input controls
    var clearFileUploadInputFields = function () {
        var simple = $("#simple-excel-file");
        var batch = $("#batch-excel-file");
        var simpleControl = $("#upload-simple-browse-control");
        var batchControl = $("#upload-batch-browse-control");

        simpleControl.addClass("hidden");
        batchControl.addClass("hidden");

        simple.replaceWith(simple.val("").clone(true));
        batch.replaceWith(batch.val("").clone(true));

        $('.upload-simple').removeClass("selected unselected");
        $('.upload-batch').removeClass("selected unselected");
    };

});

app.controller("progressCtrl", function($scope, uploadService, $interval, $window) {
    selectTab(2);

    $scope.loading = true;
    $scope.nofiles = false;
    $scope.selectedUserId;
    $scope.isCurrentUserAdmin = currentUser.IsAdministrator;
    // Get all fpm users
    uploadService.getAllFpmUsers().success(function(data) {
        $scope.users = data;
    });
        

    // Calls progress web service, which contains list of jobs 
    var getJobs = function () {
        var userId = _.isUndefined($scope.selectedUserId) ? currentUser.Id : $scope.selectedUserId;

        uploadService.getJobs(userId).success(function (data) {

            if ($scope.progress !== data) {
                $scope.loading = false;
                if (data.Jobs.length === 0) {
                    $scope.nofiles = true;
                } else {
                    $scope.progress = data;
                }

            }
        }).error(function() {
            // do nothing
        });
    };

    // Auto refresh jobs list
    $interval(function () {
        getJobs();
    }, 2000);


    // Return the css class for row according to job status
    $scope.getRowClass = function(job) {
        var rowClass;
        switch (job.Status) {
        case 0:
            rowClass = "in-queue-bg";
            break;
        case 200:
            rowClass = "in-progress-bg";
            break;
        case 300:
            rowClass = "awaiting-confirmation-bg";
            break;
        default:
            rowClass = "default-bg";
            break;
        }
        return rowClass;
    };

    // Show summary as modal dialog
    $scope.showSummary = function(job) {
        if (job.Status === 500) {
            var unexpectedErrorSummary = {
                ErrorJson: null,
                ErrorText: "Sorry, this upload has unexpectedly failed. Please contact profilefeedback@phe.gov.uk.",
                ErrorType: null,
                JobStatus: 500
            };

            $scope.summary = unexpectedErrorSummary;

            $("#summary").modal();
        } else {
            // Make ajax call for job summary
            uploadService.getJobSummary(job.Guid).success(function(data) {
                $scope.summary = data;
                // As we store error json as string in db, convert 
                // it into object before we can loop over it.
                $scope.summary.ErrorJson = JSON.parse(data.ErrorJson);
                // show summary modal with error detail.
                $("#summary").modal();

            }).error(function() {
                // do nothing
            });
        }
    };

    // Change status
    $scope.updateStatus = function (job, statusCode) {
        uploadService.updateJobStatus(job.Guid, statusCode).success(function(data) {
        }).error(function () {
            // do nothing
        });
    };

    $scope.getProgress = function(job) {

        var progress = {};
        switch (job.ProgressStage) {
        case 201:
            progress.percent = 5;
            progress.text = "Validating worksheets";
            break;
        case 202:
            progress.percent = 15;
            progress.text = "Validating data";
            break;
        case 203:
            progress.percent = 25;
            progress.text = "Checking permission";
            break;
        case 204:
            progress.percent = 35;
            progress.text = "Duplication check in file";
            break;
        case 205:
            progress.percent = 60;
            progress.text = "Duplication check in DB";
            break;
        case 206:
            progress.percent = 85;
            progress.text = "Writing to DB";
            break;
        default:
            progress = 0;
            progress.text = "";
            break;
        }
        return progress;
    };

    $scope.switchUser = function () {
        $scope.nofiles = false;
        getJobs();

    }
});


// Angular doesn't support onChange event  for
// file input. This directive will capture on change 
// event. Usage <input type="file" custom-on-change="uploadFile" />
// where uploadFile is $scope.uploadFile = function(){}
app.directive("customOnChange", function() {
    return {
        restrict: "A",
        link: function(scope, element, attrs) {
            var onChangeHandler = scope.$eval(attrs.customOnChange);
            element.bind("change", onChangeHandler);
        }
    };
});


// Make seleted tab active
var selectTab = function(tabnumber) {
    if (tabnumber === 1) {
        $("#upload-tab").addClass("active");
        $("#progress-tab").removeClass("active");
    } else {
        $("#progress-tab").addClass("active");
        $("#upload-tab").removeClass("active");
    }
};

function toggleBrowseControl() {
    $(".select-spreadsheet").hide();
    $(".browse-control").removeClass("hidden");
    $(".upload-browse").hide();
}

function uploadSimple() {
    toggleBrowseControl();
    $(".upload-batch").removeClass("selected").addClass("unselected");
    $(".upload-simple").addClass("selected").removeClass("unselected");
    $("#upload-batch-browse-control").addClass("hidden");
    $("#upload-simple-browse-control").removeClass("hidden");
}

function uploadBatch() {
    toggleBrowseControl();
    $(".upload-simple").removeClass("selected").addClass("unselected");
    $(".upload-batch").addClass("selected").removeClass("unselected");
    $("#upload-simple-browse-control").addClass("hidden");
    $("#upload-batch-browse-control").removeClass("hidden");
}