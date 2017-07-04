/// <binding Clean='default' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var watch = require('gulp-watch');
var gulp = require('gulp');

var source = './angular-app/dist',
    destination = './ts';

gulp.task('default', function () {
    // place code for your default task here
    gulp.src(source + '/**.js', { base: source })
    .pipe(watch(source, { base: source }))
    .pipe(gulp.dest(destination));

    gulp.src(source + '/**.js.map', { base: source })
    .pipe(watch(source, { base: source }))
    .pipe(gulp.dest(destination));
});