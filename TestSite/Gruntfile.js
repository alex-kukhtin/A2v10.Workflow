/// <binding ProjectOpened='watch' />

module.exports = function (grunt) {
	grunt.initConfig({
		clean: ['wwwroot/js/dist/*'],
		browserify: {
			options: {
				browserifyOptions: {
					debug: false,
				},
				plugin: [
					'esmify'
				]
			},
			app: {
				files: {
					'wwwroot/js/dist/index.js': ['app/**/*.js']
				}
			}
		},
		uglify: {
			'app': {
				files: {
					"wwwroot/js/dist/index.min.js": ['wwwroot/js/dist/index.js']
				}
			}
		},
		watch: {
			files: ["app/**/*.js"],
			tasks: ["browserify"]
		}
	});

	grunt.loadNpmTasks('grunt-contrib-clean');
	grunt.loadNpmTasks('grunt-browserify');
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-contrib-watch');

	grunt.registerTask("all", ['clean', 'browserify', 'uglify']);
};