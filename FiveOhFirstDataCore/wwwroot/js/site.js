window.CKEditorInterop = (() => {
	const editors = {};

	return {
		init(id, dotNetRef) {
			ClassicEditor
				.create(document.querySelector('.ckeditor'), {
					toolbar: {
						items: [
							'heading',
							'|',
							'bold',
							'italic',
							'underline',
							'strikethrough',
							'subscript',
							'|',
							'superscript',
							'link',
							'bulletedList',
							'numberedList',
							'|',
							'fontColor',
							'fontBackgroundColor',
							'highlight',
							'fontSize',
							'fontFamily',
							'removeFormat',
							'|',
							'undo',
							'redo',
							'-',
							'outdent',
							'indent',
							'alignment',
							'|',
							'imageInsert',
							'blockQuote',
							'insertTable',
							'mediaEmbed',
							'specialCharacters',
							'|',
							'code',
							'codeBlock',
							'htmlEmbed',
							'sourceEditing',
							'pageBreak',
							'|',
							'findAndReplace'
						],
						shouldNotGroupWhenFull: true
					},
					language: 'en',
					image: {
						toolbar: [
							'imageTextAlternative',
							'imageStyle:inline',
							'imageStyle:block',
							'imageStyle:side'
						]
					},
					table: {
						contentToolbar: [
							'tableColumn',
							'tableRow',
							'mergeTableCells',
							'tableCellProperties',
							'tableProperties'
						]
					},
					link: {
						decorators: {
							openInNewTab: {
								mode: 'manual',
								label: 'Open in a new tab',
								attributes: {
									target: '_blank',
									rel: 'noopener noreferrer'
								}
							}
						}
					},
					licenseKey: '',



				})
				.then(editor => {
					window.editor = editor;
					editors[id] = editor;
					editor.model.document.on('change:data', () => {
						let data = editor.getData();

						const el = document.createElement('div');
						el.innerHTML = data;
						if (el.innerText.trim() == '')
							data = null;

						dotNetRef.invokeMethodAsync('EditorDataChanged', data);
					});
				})
				.catch(error => {
					console.error('Oops, something went wrong!');
					console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
					console.warn('Build id: orklzebmd98k-mu98nylwq3e2');
					console.error(error);
				});
		},
		destory(id) {
			editors[id].destory()
				.then(() => delete editors[id])
				.catch(error => console.log(error));
		}
	};
})();