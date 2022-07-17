import * as monaco from 'monaco-editor';

window.LightweightMonacoInterop = (() => {
    const editors = {};

    return {
        init(guid, dotNetRef, elementId, startingText, language, theme, onChange) {
			let element = document.getElementById(elementId);
			let editor = monaco.editor.create(element, {
				value: startingText,
				language: language,
				theme: theme
			});

			editor.onDidChangeModelContent((evt) => {
				dotNetRef.invokeMethodAsync(onChange, editor.getValue('\n', false));
			});
			editor.set
			editors[guid] = editor;
		},

		changeTheme(guid, newTheme) {
			let inst = editors[guid];
			if (inst) {
				let opt = inst.getRawOptions();
				opt.theme = newTheme;
				inst.updateOptions(opt);
            }
        },

		dispose(guid) {
			let inst = editors[guid];
			if (inst) {
				inst.dispose();
				delete editors[guid];
			}
        }
    }
})();