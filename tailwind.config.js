module.exports = {
	mode: 'jit',
	purge: [
		'../**/*.{html,razor,razor.css,cshtml,cshtml.css}'
	],
	theme: {
		extend: {},
	},
	variants: {
		extend: {},
	},
	plugins: [
		require('@tailwindcss/forms'),
	],
}
