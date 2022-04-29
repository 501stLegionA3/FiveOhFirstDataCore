const path = require('path');
const root = path.resolve(__dirname, './ProjectDataCore/wwwroot');
const dist = path.resolve(root, 'dist');

module.exports = [
    {
        name: 'CK Editor Interop',
        entry: path.resolve(root, './ckeditor/interop/ckEditorInterop.js'),
        output: {
            path: dist,
            filename: 'ckEditorInterop.bundle.js',
        },
        mode: 'production'
    },
    {
        name: 'Split Interop',
        entry: path.resolve(root, './split/splitInterop.js'),
        output: {
            path: dist,
            filename: 'splitInterop.bundle.js'
        },
        mode: 'production'
    }
];

module.exports.parallelism = 1;