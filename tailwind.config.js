function withOpacityValue(variable) {
	return ({ opacityValue }) => {
		if (opacityValue === undefined) {
			return `rgb(var(${variable}))`
		}
		return `rgb(var(${variable}) / ${opacityValue})`
	}
}

module.exports = {
	mode: 'jit',
	content: [
		'../ProjectDataCore/**/*.{html,razor,razor.css,cshtml,cshtml.css}',
		'../ProjectDataCore.Components/**/*.{html,razor,razor.css,cshtml,cshtml.css}'
	],
	theme: {
		extend: {
			colors: {
				// Roster colors
				roster_primary: withOpacityValue('--roster-primary'),
				roster_primary_t: withOpacityValue('--roster-primary-text'),

				roster_secondary: withOpacityValue('--roster-secondary'),
				roster_secondary_t: withOpacityValue('--roster-secondary-text'),

				roster_tertiary: withOpacityValue('--roster-tertiary'),
				roster_tertiary_t: withOpacityValue('--roster-tertiary-text'),

				// Utility Colors
				util_action: withOpacityValue('--util-action'),
				util_action_t: withOpacityValue('--util-action-text'),

				// Operation Colors
				op_danger: withOpacityValue('--op- danger'),
				op_danger_t: withOpacityValue('--op-danger-text'),
				
				op_success: withOpacityValue('--op-success'),
				op_success_t: withOpacityValue('--op-success-text'),
			},
		},
	},
	variants: {
		extend: {},
	},
	plugins: [
		require('@tailwindcss/forms'),
	],
}
