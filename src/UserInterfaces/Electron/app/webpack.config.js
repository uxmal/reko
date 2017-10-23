var path = require('path')
var webpack = require('webpack')

const css_options = {
  alias: {
    'element-default.css': 'element-ui/lib/theme-default/index.css',
    'bootstrap.min.css': 'bootstrap/dist/css/bootstrap.min.css'
  },
  import: true,
  modules: true,
  importLoaders: true
};

module.exports = {
  entry: './app.ts',
  output: {
    path: path.resolve(__dirname, './generated'),
    publicPath: '/',
    filename: '[name].js'
  },
  target: "electron-renderer",
  module: {
    rules: [
      {
        test: /\.ts$/,
        loader: 'ts-loader',
        exclude: /node_modules/,
        options: {
          appendTsSuffixTo: [/\.vue$/]
        }
      },
      {
        test: /\.tpl$/,
        loader: 'handlebars-loader',
        exclude: /node_modules/,
        options: {
          helperResolver : function(helper, callback){
            const fs = require("fs");
            var path = __dirname + `/views/helpers/${helper}.ts`;
            var error = fs.existsSync(path) ? null : true;
            callback(error, path);
          },
          debug: true,
          rootRelative: __dirname + "/views/",
          helperDirs: [ __dirname + "/views/helpers/" ],
          extensions: [ '.tpl' ]
        },
      },
      {
        test: /\.js$/,
        loader: 'babel-loader',
        exclude: /node_modules/
      },
      {
        test: /\.(png|jpg|gif|svg)$/,
        loader: 'url-loader'
      },
      {
        test: /\.(woff|woff2|eot|ttf|otf)$/,
        loader: 'url-loader'
      },
      {
        test: /\.vue$/,
        use: [
          {
            loader: 'vue-loader',
            options: {
              //cssModules: css_options,
              /*loaders: {
                // Since sass-loader (weirdly) has SCSS as its default parse mode, we map
                // the "scss" and "sass" values for the lang attribute to the right configs here.
                // other preprocessors should work out of the box, no loader config like this necessary.
                'scss': 'vue-style-loader!css-loader!sass-loader',
                'sass': 'vue-style-loader!css-loader!sass-loader?indentedSyntax'
              },*/
              esModule: true
              // other vue-loader options go here
            }
          }
        ],
      },
      {
        test: /\.css$/,
        use: [
          { loader: "style-loader" },
          { loader: "css-loader", options: css_options }
        ]
      }
    ]
  },
  resolve: {
    extensions: ['.ts', '.js', '.vue', ],
    alias: {
      'element-default.css$': 'element-ui/lib/theme-default/index.css',
      'bootstrap.min.css$': 'bootstrap/dist/css/bootstrap.min.css',
      'vue$': 'vue/dist/vue.esm.js'
    }
  },
  devServer: {
    historyApiFallback: true,
    noInfo: true,
    overlay: true
  },
  performance: {
    hints: false
  },
  devtool: '#eval-source-map'
}

if (process.env.NODE_ENV === 'production') {
  module.exports.devtool = '#source-map'
  // http://vue-loader.vuejs.org/en/workflow/production.html
  module.exports.plugins = (module.exports.plugins || []).concat([
    new webpack.DefinePlugin({
      'process.env': {
        NODE_ENV: '"production"'
      }
    }),
    new webpack.optimize.UglifyJsPlugin({
      sourceMap: true,
      compress: {
        warnings: false
      },
      output: {
        comments: false
      },
      ecma: 6
    }),
    new webpack.LoaderOptionsPlugin({
      minimize: true
    })
  ])
}
