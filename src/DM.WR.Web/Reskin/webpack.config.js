const path = require("path");
const webpack = require("webpack");
module.exports = {
    entry: {
        index: "./Scripts/src/index.js",
    },
    output: {
        path: path.resolve(__dirname, "../elevateheaderexternal/eeh/build"),
        filename: "[name].js",
    },
    resolve: {
        alias: {
            process: "process/browser"
        }
    },
    plugins: [
        new webpack.ProvidePlugin({
            process: 'process/browser',
        }),
    ],
    module: {

        rules: [
            {
                use: {
                    loader: "babel-loader",
                    options: {
                        presets: ['@babel/preset-env','@babel/preset-react'],
                        plugins: ['@babel/plugin-proposal-object-rest-spread','@babel/plugin-syntax-jsx']
                    }
                },
                test: /\.js$/,
                exclude: /node_modules/, //excludes node_modules folder from being transpiled by babel. We do this because it's a waste of resources to do so.
            },
            {
                test: /\.scss$/,
                use: ["style-loader", "css-loader", "sass-loader"],
            },
        ],
    },
};
