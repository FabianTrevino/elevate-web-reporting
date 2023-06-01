/* eslint-disable @typescript-eslint/no-var-requires */
const webpack = require('webpack');
const reactAppRewirePostcss = require('react-app-rewire-postcss');
const postcssNesting = require('postcss-nesting');

const {
    override,
    // adjustStyleLoaders,
    // overrideDevServer,
    // setWebpackStats,
    // watchAll,
} = require('customize-cra');

module.exports = {
    webpack: override((config) => {
        // adjustStyleLoaders(({ use: [ , css, postcss, resolve, processor ] }) => {
        //     // css.options.sourceMap = true;         // css-loader
        //     postcss.options.sourceMap = true;     // postcss-loader
        //     console.log(postcss);
        //     // when enable pre-processor,
        //     // resolve-url-loader will be enabled too
        //     // if (resolve) {
        //     //     resolve.options.sourceMap = true;   // resolve-url-loader
        //     // }
        //     // // pre-processor
        //     // if (processor && processor.loader.includes('sass-loader')) {
        //     //     processor.options.sourceMap = true; // sass-loader
        //     // }
        // });


    reactAppRewirePostcss(config, {
            plugins: () => [postcssNesting()],
        });

        // setWebpackStats('detailed');

        if (typeof config.resolve.fallback === 'undefined') {
            config.resolve.fallback = {};
        }
        config.resolve.fallback = {
            ...config.resolve.fallback,
            crypto: require.resolve('crypto-browserify'),
            stream: require.resolve('stream-browserify'),
            util: require.resolve('util/'),
        };

        if (typeof config.resolve.alias === 'undefined') {
            config.resolve.alias = {};
        }

        config.resolve.alias.process = 'process/browser';

        config.plugins.push(
            new webpack.ProvidePlugin({
                process: 'process/browser',
            }),
        );

        // watchAll();
        //
        if (typeof config.watchOptions !== 'undefined') {
            config.watchOptions.aggregateTimeout = 300;
        }

        return config;
    }),

    jest: (config) => {
        return config;
    },

    devServer: (configFunction) => (proxy, allowedHost) => {
        const config = configFunction(proxy, allowedHost);
        return config;
    },

    paths: (paths, env) => {
        return paths;
    },
};
