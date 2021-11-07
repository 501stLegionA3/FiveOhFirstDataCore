module.exports = {
	mode: 'jit',
	purge: {
		enabled: true,
		content: [
			'../**/*.{html,razor,razor.css,cshtml,cshtml.css}',
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
