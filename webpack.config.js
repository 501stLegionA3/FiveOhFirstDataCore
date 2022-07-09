const path = require('path');
const root = path.resolve(__dirname, './ProjectDataCore/wwwroot');
const dist = path.resolve(root, 'dist');

const TerserPlugin = require('terser-webpack-plugin');

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
                app: path.resolve(root, './monaco/lightweightMonacoInterop.js'),
                "editor.worker": 'monaco-editor/esm/vs/editor/editor.worker.js',
                "json.worker": 'monaco-editor/esm/vs/language/json/json.worker',
                "css.worker": 'monaco-editor/esm/vs/language/css/css.worker',
                "html.worker": 'monaco-editor/esm/vs/language/html/html.worker',
                "ts.worker": 'monaco-editor/esm/vs/language/typescript/ts.worker',
            },
            output: {
                globalObject: 'self',
                path: path.resolve(dist, './monaco'),
                filename: '[name].js',
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
            optimization: {
                minimize: true,
                minimizer: [new TerserPlugin()]
            }
        }
    ];
};

module.exports.parallelism = 1;