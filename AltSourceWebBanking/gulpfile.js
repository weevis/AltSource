/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');

gulp.task('default', function () {
    // place code for your default task here
});

var sass = require("gulp-sass");
var paths = {
    webroot: "~" 
};

paths.scss = paths.webroot + "Content/**/*.sass"; /*watches sub folders inside wwwroot/Content/ */
gulp.task('sass', function () {
    gulp.src(paths.scss)
        .pipe(sass())
        .pipe(gulp.dest(paths.webroot + "Content"));
});
gulp.task('watch-sass', function () {
    gulp.watch(paths.scss, ['sass']);
});