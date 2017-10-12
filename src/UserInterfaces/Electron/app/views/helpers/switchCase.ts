//http://chrismontrois.net/2016/01/30/handlebars-switch/

import Handlebars = require("handlebars");

Handlebars.registerHelper("switch", function(this: any, value:any, options:any) {
	this._switch_value_ = value;
	var html = options.fn(this); // Process the body of the switch block
	delete this._switch_value_;
	return html;
});

Handlebars.registerHelper("case", function(this: any, value:any, options:any) {
	if (value == this._switch_value_) {
	return options.fn(this);
	}
});