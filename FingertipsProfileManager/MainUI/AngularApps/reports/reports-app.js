var app = angular.module("reportsApp", ["ngRoute"]);
app.config(function ($routeProvider, $httpProvider) {
    //initialize get if not there
    if (!$httpProvider.defaults.headers.get) {
        $httpProvider.defaults.headers.get = {};
    }

    // Disable IE ajax request caching
    $httpProvider.defaults.headers.get["If-Modified-Since"] = "Mon, 26 Jul 1997 05:00:00 GMT";
    $httpProvider.defaults.headers.get["Cache-Control"] = "no-cache";
    $httpProvider.defaults.headers.get["Pragma"] = "no-cache";

    $routeProvider
        .when("/",
        {
            templateUrl: "angularapps/reports/view/reports-index.html",
            controller: "reportsCtrl"
        })
        .when("/edit/:id?",
        {
            templateUrl: "angularapps/reports/view/reports-edit.html",
            controller: "editReportCtrl"
        });
});

// Web services
app.factory("reportService",
    function ($http) {
        return {
            getProfiles: function () {
                return $http.get('/profile/user-profiles');
            },
            getAllReports: function() {
                return $http.get('/api/reports');
            },
            getReportById: function (id) {
                return $http.get('api/reports/' + id);
            }
        };
    });


// Reports landing controller
app.controller("reportsCtrl", function ($scope, reportService, $location, $http) {    
    $scope.reportsForCurrentUser = [];

    // Get all available reports
    reportService.getAllReports()
        .success(function(data) {
           $scope.reportsForCurrentUser = data;
        })
        .error(function() {
            alert('failed to load all reports');
        });

    // Redirect to edit url
    $scope.editReport = function(id) {
        $location.path("edit/" + id);
    };

    // Delete report
    $scope.deleteReport = function (index) {
        console.log($scope.reportsForCurrentUser[index].id);
        $http.delete('api/reports/' + $scope.reportsForCurrentUser[index].id);
        $scope.reportsForCurrentUser.splice(index, 1);                
    };
});


// New report controller
app.controller("editReportCtrl",
    function ($http, $scope, reportService, $routeParams, $location) {
        $scope.report = {
            name:'',
            profiles:[],
            parameters: []
        };

        $scope.availableParameters = [
            {
                label: 'areaCode', val: 'areaCode'
            },
              {
                  label: 'areaTypeId', val: 'areaTypeId'
              }, {
                  label: 'groupId', val: 'groupId'

              }, {
                  label: 'parentCode', val: 'parentCode'

              }, { label: 'parentTypeId', val: 'parentTypeId' }
        ];

        $scope.hasProfiles = false;
        $scope.hasParameters = false;
        $scope.isEditMode = false;
      

        // Get Report data 
        if (!angular.isUndefined($routeParams.id)) {            
            $scope.isEditMode = true;
            reportService.getReportById($routeParams.id)
                .success(function (data) {
                    if (data.name.length > 0) {
                        $scope.report = data;                     
                    }
                })
                .error(function () {
                    alert('failed to load report details');
                });
        }


        // Get profiles for current user
        reportService.getProfiles()
            .success(function(data) {
                $scope.userProfiles = data;
            })
            .error(function() {
                alert('failed to load profiles');
            });
                
        // Add profile to list of profiles for current report
        $scope.addProfile = function() {
            var doesProfileAlreadyExist = $scope.report.profiles.indexOf($scope.selectedProfile) === -1;
            if (doesProfileAlreadyExist && !angular.isUndefined($scope.selectedProfile)) {
                $scope.report.profiles.push($scope.selectedProfile);
            }
        };        

        // Add parameter to list of parameters for current report
        $scope.addParameter = function() {
            if ($scope.report.parameters.indexOf($scope.newParameter) === -1
                && !angular.isUndefined($scope.newParameter)) {
                $scope.report.parameters.push($scope.newParameter);
            }
        };

        // Remove profile from list of profiles for current report
        $scope.removeProfileFromList = function(index) {
            $scope.report.profiles.splice(index, 1);
        };

        // Remove parameter from list of parameters
        $scope.removeParameterFromList = function(index) {
            $scope.report.parameters.splice(index, 1);
        }

        // Save report settings
        $scope.saveReport = function() {
            var data = {
                name : $scope.report.name,
                profiles: $scope.report.profiles,
                parameters: $scope.report.parameters
            }
            var res;
            if ($scope.isEditMode) {
                data.id = $routeParams.id;
                res = $http.put('api/reports/new', data);
            } else {
                res = $http.post('api/reports/new', data);
            }

            res.success(function() {
                alert('done');
            });
            res.error(function() {
                alert('Failed to save the report data.');
            });
        };
        
        // Cancel button
        $scope.cancel = function() {
            $location.path('/');
        }


        // Watcher for profiles
        $scope.$watch('report.profiles',
            function() {
                $scope.hasProfiles = $scope.report.profiles.length > 0;
            },
            true);

        // Watchers  for parameters
        $scope.$watch('report.parameters',
            function() {
                $scope.hasParameters = $scope.report.parameters.length > 0;
            },
            true);
    });