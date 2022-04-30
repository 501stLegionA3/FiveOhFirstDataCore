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
        mode: 'development',
    },
    {
        name: 'Split Interop',
        entry: path.resolve(root, './split/splitInterop.js'),
        output: {
            path: dist,
            filename: 'splitInterop.bundle.js'
        },
        mode: 'development',
    },
    {
        name: 'Drop Interop',
        entry: path.resolve(root, './drag/dropInterop.js'),
        output: {
            path: dist,
            filename: 'dropInterop.bundle.js',
        },
        mode: 'development',
    }
];

module.exports.parallelism = 1;