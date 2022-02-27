window.CKEditorInterop = (() => {
	const editors = {};

	return {
		init(id, dotNetRef) {
			const watchdog = new CKSource.EditorWatchdog();
			window.watchdog = watchdog;
			watchdog.setCreator((element, config) => {
				return CKSource.Editor
					.create(element, config)
					.then(editor => {

						return editor;
					})
			});

			watchdog.setDestructor(editor => {
				return editor.destroy();
			});

			watchdog.on('error', handleError);

			watchdog
				.create(document.querySelector('.ckeditor'), {
					language: 'en',
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
					simpleUpload: {
						uploadUrl: 'https://s4.501stlegion-a3.com/api/image/upload',
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
				.catch(handleError);

			function handleError(error) {
				console.error('Oops, something went wrong!');
				console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
				console.warn('Build id: a2j0vf9bve5a-w58ttbsi94lt');
				console.error(error);
			}
		},
		destory(id) {
			editors[id].destory()
				.then(() => delete editors[id])
				.catch(error => console.log(error));
		}
	};
})();