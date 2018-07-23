var gulp = require('gulp'),
    del = require('del'),
    shell = require('gulp-shell');

// Clean up distribution folder
gulp.task('clean', function() {
    console.log('Clean up');
    return del('[../angular-app-dist]', {force: true});
});

// Build for prodction
gulp.task('prod', ['clean'], shell.task([
    'ng build --prod --output-hashing=none --aot=false'
]));

// Build for development
gulp.task('dev', ['clean'], shell.task([
    'ng build -w'
]));

// Default task
gulp.task('default', ['dev']);