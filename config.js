const config = {
	"gatsby": {
		"pathPrefix": "/reko",
		"siteUrl": "https://uxmal.github.io/reko/",
		"gaTrackingId": null,
		"trailingSlash": false
	},
	"header": {
		"logo": "",
		"logoLink": "https://uxmal.github.io/reko/",
		"title": "Reko Decompiler",
		"githubUrl": "https://github.com/uxmal/reko",
		"helpUrl": "",
		"tweetText": "",
		"links": [
			{ "text": "", "link": ""}
		],
		"search": {
			"enabled": false,
			"indexName": "",
			"algoliaAppId": process.env.GATSBY_ALGOLIA_APP_ID,
			"algoliaSearchKey": process.env.GATSBY_ALGOLIA_SEARCH_KEY,
			"algoliaAdminKey": process.env.ALGOLIA_ADMIN_KEY
		}
	},
	"sidebar": {
		"forcedNavOrder": [
			"/introduction", // add trailing slash if enabled above
			"/installing",
			"/gallery",
    		"/codeblock"
		],
    	"collapsedNav": [
      		"/codeblock" // add trailing slash if enabled above
    	],
		"links": [
			{ "text": "GitHub repository", "link": "https://github.com/uxmal/reko/"},
		],
		"frontline": false,
		"ignoreIndex": true,
	},
	"siteMetadata": {
		"title": "@@@ | Reko Decompiler",
		"description": "Documentation built with mdx. Powering uxmal.reko.io ",
		"ogImage": null,
		"docsLocation": "https://github.com/uxmal/reko/tree/master/content",
		"favicon": ""
	},
	"pwa": {
		"enabled": false, // disabling this will also remove the existing service worker.
		"manifest": {
			"name": "Gatsby Gitbook Starter",
			"short_name": "GitbookStarter",
			"start_url": "/",
			"background_color": "#6b37bf",
			"theme_color": "#6b37bf",
			"display": "standalone",
			"crossOrigin": "use-credentials",
			icons: [
				{
					src: "src/pwa-512.png",
					sizes: `512x512`,
					type: `image/png`,
				},
			],
		},
	}
};

module.exports = config;
