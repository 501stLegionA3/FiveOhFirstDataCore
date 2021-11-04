module.exports = {
	purge: {
		enabled: true,
		content: [
			'./**/*.html',
			'./**/*.razor',
			'../ProjectDataCore.Components/**/*.razor'
		],
	},
	darkMode: false, // or 'media' or 'class'
	theme: {
		extend: {},
	},
	variants: {
		extend: {},
	},
	plugins: [],
}
