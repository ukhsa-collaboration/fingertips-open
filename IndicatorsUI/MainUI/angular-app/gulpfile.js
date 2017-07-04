var gulp = require('gulp'),
    exec = require('child_process').exec,
    shell = require('gulp-shell'),
    del  = require('del'),
    fs = require('fs');



// Clean the Distribution folder
gulp.task('clean', function(){    
    console.log('');
  return del(['dist','../ts'], {force:true});
});

// Build angular app
gulp.task('build', ['clean'],shell.task([
    'ng build --prod --output-hashing none'
]));

// Come to TS folder
gulp.task('copyToFingertips',['build'], function(){
    gulp.src('dist/**.js').pipe(gulp.dest('../ts'));
});


gulp.task('prod',['copyToFingertips'], function(){
    console.log('Building for production environment.');
}); 

gulp.task('serve',['clean'], shell.task([
    'ng serve --watch'
]));

gulp.task('copy-dev-files-to-fingertips', ['serve'], function(){
    gulp.src('dist/**.js').pipe(gulp.dest('../ts'));
});

// Serve angular app
gulp.task('dev',['copyToFingertips'], shell.task([
    'ng serve --watch'
]));

// Default task
gulp.task('default', ['dev']);





