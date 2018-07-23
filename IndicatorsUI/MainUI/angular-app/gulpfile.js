var gulp = require('gulp'),
    exec = require('child_process').exec,
    shell = require('gulp-shell'),
    del = require('del'),
    fs = require('fs');

// Remove folder with compiled files
gulp.task('clean', function() {
    console.log('Clean the Distribution folder');
    return del(['../angular-app-dist'], { force: true });
});

gulp.task('test', [], shell.task([
    'ng test --watch=false --reporters=junit'
]));

// Build production
gulp.task('prod', ['clean'], shell.task([
    'ng build --prod --output-hashing none'
]));

// Build development
gulp.task('dev', ['clean'], shell.task([
    'ng build --watch'
]));

// Default task
gulp.task('default', ['dev']);