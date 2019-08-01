module.exports = {
  pages: {
    index: {
      entry: "./src/pages/index/page.js",
      template: "public/index.html",
      title: "EnChanger",
      chunks: ["chunk-vendors", "chunk-common", "index"]
    },
    url: {
      entry: "./src/pages/url/page.js",
      template: "public/index.html",
      title: "Url",
      chunks: ["chunk-vendors", "chunk-common", "url"]
    }
  },
  publicPath: "/static"
};
