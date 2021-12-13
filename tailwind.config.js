module.exports = {
	mode: 'jit',
	content: [
		'../ProjectDataCore/**/*.{html,razor,razor.css,cshtml,cshtml.css}',
		'../ProjectDataCore.Components/**/*.{html,razor,razor.css,cshtml,cshtml.css}'
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
