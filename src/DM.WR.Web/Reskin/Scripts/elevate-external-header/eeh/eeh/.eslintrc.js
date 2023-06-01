module.exports = {
    root: true,
    parserOptions: {
        ecmaVersion: 2020,
        sourceType: 'module',
    },
    plugins: ['@typescript-eslint', 'react', 'react-hooks'],
    extends: [
        'react-app',
        'plugin:react/recommended',
        'plugin:react-hooks/recommended',
        // 'plugin:@typescript-eslint/recommended',
    ],
    rules: {
        'import/no-extraneous-dependencies': [
            'error',
            {
                packageDir: __dirname,
            },
        ],
        'react-hooks/exhaustive-deps': 0,
        'no-use-before-define': [0],
        '@typescript-eslint/no-use-before-define': [1],
        indent: 0,
        quotes: ['error', 'single'],
        semi: ['error', 'always'],
        eqeqeq: 2,
        'react-hooks/exhaustive-deps': 0,
        '@typescript-eslint/no-explicit-any': 2,
        'no-console': 1,
        arguments: 0,
        'jsx-a11y/anchor-is-valid': 0,
        'valid-jsdoc': 'error',
    },
    globals: {
        'elevateConfiguration': 'writable',
    },
    ignorePatterns: ['.eslintrc.js', 'node_modules', 'dist'],
};
