const fs = require('fs');
const path = require('path');

const jsDir = path.resolve(path.join(__dirname, 'build', 'static', 'js'));
const cssDir = path.resolve(path.join(__dirname, 'build', 'static', 'css'));

process.stdout.write('\nRenaming files for webreporting\n');
process.stdout.write(`\nSearching for JS files in "${jsDir}"\n`);

const jsDirFiles = fs.readdirSync(jsDir);
let jsHash = '';
if (jsDirFiles && Array.isArray(jsDirFiles) && jsDirFiles.length) {
    const jsFileHashMatches = jsDirFiles[0].match(/^[^.]+\.([^.]+)\./);
    if (jsFileHashMatches && jsFileHashMatches.length > 1) {
        jsHash = jsFileHashMatches[1];
    }
    if (jsHash !== '') {
        process.stdout.write(` JS files hash: "${jsHash}"\n`);
        const hashRegex = new RegExp('.' + jsHash + '.', 'g');
        for (let i=0; i< jsDirFiles.length; i++) {
            if (jsDirFiles[i].match(hashRegex)) {
                const newName = jsDirFiles[i].replace(hashRegex, '.');
                process.stdout.write(` - Copying "${jsDirFiles[i]}" to "${newName}"\n`);
                fs.copyFileSync(path.join(jsDir, jsDirFiles[i]), path.join(jsDir, newName));
            }
        }
    }
}

process.stdout.write(`Searching for CSS files in "${cssDir}"\n`);
const cssDirFiles = fs.readdirSync(cssDir);
let cssHash = '';
if (cssDirFiles && Array.isArray(cssDirFiles) && cssDirFiles.length) {
    const cssFileHashMatches = cssDirFiles[0].match(/^[^.]+\.([^.]+)\./);
    if (cssFileHashMatches && cssFileHashMatches.length > 1) {
        cssHash = cssFileHashMatches[1];
    }
    if (cssHash !== '') {
        process.stdout.write(` CSS files hash "${cssHash}"\n`);
        const hashRegex = new RegExp('.' + cssHash + '.', 'g');
    
        for (let i=0; i< cssDirFiles.length; i++) {
            if (cssDirFiles[i].match(hashRegex)) {
                const newName = cssDirFiles[i].replace(hashRegex, '.');
                process.stdout.write(` - Copying "${cssDirFiles[i]}" to "${newName}"\n`);
                fs.copyFileSync(path.join(cssDir, cssDirFiles[i]), path.join(cssDir, newName));
            }
        }
    }
}
process.stdout.write('\nFinished.\n');
process.exit(0);