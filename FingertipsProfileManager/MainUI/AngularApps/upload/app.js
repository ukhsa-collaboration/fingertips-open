var app = angular.module("uploadApp", []);

// Config
app.config(['$httpProvider', function ($httpProvider) {
    //initialize get if not there
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }

    // Answer edited to include suggestions from comments
    // because previous version of code introduced browser-related errors
    // disable IE ajax request caching
    $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
    $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
    $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
}]);

// Web services
var url = "/upload/";
app.factory("uploadService", function($http) {
    return {
        getJobs: function() {
            return $http.get(url + "progress");
        },
        getJobSummary: function(guid) {
            return $http.get(url + "summary/" + guid);
        },
        updateJobStatus: function(guid, newstatuscode) { // should be a post
            return $http.get(url + "change-status/" + guid + "/" + newstatuscode);
        },
        getRowsProcessed: function(guid) {
            return $http.get(url + "rows-processed/" + guid);
        }
    };
});

// Controller
app.controller("IndexCtrl", function($scope, $location, uploadService, $interval) {
    var tabs = { First: 1, Second: 2 };

    $scope.fileUploadStatus = "";
    $scope.selected_tab = tabs.First;

    // Determine which tab is active
    $scope.isActive = function(route) {
        if (route === "" || route === "/" || route === "/#") {
            return true;
        }
        return route === $location.path();
    };

    // Execute code for each tab upon click
    $scope.select = function(tab) {
        $scope.selected_tab = tab;

        if (tab === tabs.Second) {
            getJobs();
        }
    };

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
    };

    // Auto refresh jobs list
    $interval(function() {
        if ($scope.selected_tab == tabs.Second) {
            getJobs();
        }
    }, 2000);

    // Change status
    $scope.updateStatus = function(job, statusCode) {
        uploadService.updateJobStatus(job.Guid, statusCode).success(function(data) {
            $scope.select(2);
        }).error(function() {
            // do nothing
        });
    };

    // Check if the file Ext is csv or excel
    var isFileTypeAllowed = function (filePath) {
        var isAllowed;
        var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
            if (ext != 'xlsx' && ext != 'xls' && ext != 'csv') {
                isAllowed = false;
            } else {
                isAllowed = true;
            }
        return isAllowed;
    };

    // Upload simple file
    $scope.uploadSimpleFile = function (event) {
        var isFileTypeOk = isFileTypeAllowed(this.value);
        if (isFileTypeOk) {
            $("form#validateSimpleSpreadsheetForm").submit();
            $scope.switchToProgressTab(true);
            $scope.fileTypeAllowed = false;
        } else {
            $scope.fileTypeAllowed = true;
        }        
    };

    $scope.uploadBatchFile = function (event) {
        var isFileTypeOk = isFileTypeAllowed(this.value);
        if (isFileTypeOk) {
            $("form#validateBatchSpreadsheetForm").submit();
            $scope.switchToProgressTab(false);
            $scope.fileTypeAllowed = false;
        } else {
            $scope.fileTypeAllowed = true;
        }      
    };

    var clearFileUploadInputField = function (isSimple) {        
        var fileUploader = isSimple ? $("#simple-excel-file") : $("#batch-excel-file");
        var fileUploadControl = isSimple ? $("#upload-simple-browse-control") : $("#upload-batch-browse-control");

        fileUploadControl.addClass("hidden");
        fileUploader.replaceWith(fileUploader.val("").clone(true));

        var selectedControlOption = isSimple ? $('.upload-simple') : $('.upload-batch');
        selectedControlOption.removeClass("selected").addClass("unselected");
    };

    $scope.switchToProgressTab = function (isSimple) {
        clearFileUploadInputField(isSimple);
        // Change the tab to progress
        $("#progress-tab").tab("show");
        $scope.select(tabs.Second);
    };

    $scope.getProgress = function (job) {
        
        var progress={};
        switch (job.ProgressStage) {
        case 201:
            progress.percent = 5;
            progress.text ='Validating worksheets';
            break;
        case 202:
            progress.percent = 15;
            progress.text = 'Validating data';
            break;
        case 203:
            progress.percent = 25;
            progress.text = 'Checking permission';
            break;
        case 204:
            progress.percent = 35;
            progress.text = 'Duplication check in file';
            break;
        case 205:
            progress.percent = 60;
            progress.text = 'Duplication check in DB';
            break;
        case 206:
            progress.percent = 85;
            progress.text = 'Writing to DB';
            break;
        default:
            progress = 0;
            progress.text ='';
            break;
        }
        return progress;
    };

    // Calls progress web service, which contains list of jobs 
    var getJobs = function() {
        uploadService.getJobs().success(function(data) {
            $scope.progress = data;
        }).error(function() {
            // do nothing
        });
    };

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


/* This code can go to template using angular*/

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