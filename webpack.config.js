const path = require('path');
const root = path.resolve(__dirname, './ProjectDataCore/wwwroot');
const dist = path.resolve(root, 'dist');

const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin');

module.exports = mode => {
    return [
        {
            name: 'Site Utils',
            entry: path.resolve(root, './site.js'),
            output: {
                path: dist,
                filename: 'site.bundle.js',
            },
            mode: mode,
        },
        {
            name: 'CK Editor Interop',
            entry: path.resolve(root, './ckeditor/interop/ckEditorInterop.js'),
            output: {
                path: dist,
                filename: 'ckEditorInterop.bundle.js',
            },
            mode: mode,
        },
        {
            name: 'Split Interop',
            entry: path.resolve(root, './split/splitInterop.js'),
            output: {
                path: dist,
                filename: 'splitInterop.bundle.js'
            },
            mode: mode,
        },
        {
            name: 'Drop Interop',
            entry: path.resolve(root, './drag/dropInterop.js'),
            output: {
                path: dist,
                filename: 'dropInterop.bundle.js',
            },
            mode: mode,
        },
        {
            name: 'Monaco Interop',
            entry: {
                path: path.resolve(root, './monaco/lightweightMonacoInterop.js'),
            },
            output: {
                path: path.resolve(dist, './monaco'),
                filename: 'app.js',
            },
            mode: mode,
            module: {
                rules: [
                    {
                        test: /\.css$/,
                        use: ['style-loader', 'css-loader']
                    },
                    {
                        test: /\.ttf$/,
                        use: ['file-loader']
                    }
                ]
            },
            plugins: [new MonacoWebpackPlugin()]
        }
    ];
};

module.exports.parallelism = 1;