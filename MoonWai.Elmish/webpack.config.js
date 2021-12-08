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
        contentBase: "./public",
        port: 5002,
        proxy: {
            '/api': {
                target: 'http://localhost:5000',
                changeOrigin: false
            }
        },
        publicPath: "/",
    },
    module: {
        rules: [{
            test: /\.fs(x|proj)?$/,
            use: "fable-loader"
        },
        {
            test: /\.scss$/,
            use: [{
                    loader: "style-loader"
                }, {
                    loader: "css-loader" 
                }, {
                    loader: "sass-loader"
                }]
        }]
    }
}
