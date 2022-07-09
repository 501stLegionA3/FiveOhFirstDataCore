const path = require('path');
const root = path.resolve(__dirname, './ProjectDataCore/wwwroot');
const dist = path.resolve(root, 'dist');

const TerserPlugin = require('terser-webpack-plugin');

module.exports = [
    {
        name: 'Site Utils',
        entry: path.resolve(root, './site.js'),
        output: {
            path: dist,
            filename: 'site.bundle.js',
        },
        mode: 'development',
    },
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
    },
    {
        name: 'Monaco Interop',
        entry: {
            app: path.resolve(root, './monaco/lightweightMonacoInterop.js'),
            'editor.worker': 'monaco-editor/esm/vs/editor/editor.worker.js',
            "json.worker": 'monaco-editor/esm/vs/language/json/json.worker',
            "css.worker": 'monaco-editor/esm/vs/language/css/css.worker',
            "html.worker": 'monaco-editor/esm/vs/language/html/html.worker',
            "ts.worker": 'monaco-editor/esm/vs/language/typescript/ts.worker',
        },
        output: {
            globalObject: 'self',
            path: dist,
            filename: 'lightweightMonacoInterop.bundle.js',
        },
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
        optimization: {
            minimize: true,
            minimizer: [new TerserPlugin()]
        },
        mode: 'development',
    }
];

module.exports.parallelism = 1;