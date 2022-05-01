const path = require('path');
const root = path.resolve(__dirname, './ProjectDataCore/wwwroot');
const dist = path.resolve(root, 'dist');

module.exports = [
    {
        name: 'Site Utils',
        entry: path.resolve(root, './site.js'),
        output: {
            path: dist,
            filename: 'site.bundle.js',
        },
        mode: 'production',
    },
    {
        name: 'CK Editor Interop',
        entry: path.resolve(root, './ckeditor/interop/ckEditorInterop.js'),
        output: {
            path: dist,
            filename: 'ckEditorInterop.bundle.js',
        },
        mode: 'production',
    },
    {
        name: 'Split Interop',
        entry: path.resolve(root, './split/splitInterop.js'),
        output: {
            path: dist,
            filename: 'splitInterop.bundle.js'
        },
        mode: 'production',
    },
    {
        name: 'Drop Interop',
        entry: path.resolve(root, './drag/dropInterop.js'),
        output: {
            path: dist,
            filename: 'dropInterop.bundle.js',
        },
        mode: 'production',
    }
];

module.exports.parallelism = 1;