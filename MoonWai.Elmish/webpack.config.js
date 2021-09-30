var path = require("path");

module.exports = {
    mode: "development",
    devtool: 'eval-source-map',
    entry: "./src/MoonWai.Elmish.fsproj",
    output: {
        path: path.join(__dirname, "./public"),
        filename: "bundle.js",
    },
    devServer: {
        publicPath: "/",
        contentBase: "./public",
        port: 5000,
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        }]
    }
}
