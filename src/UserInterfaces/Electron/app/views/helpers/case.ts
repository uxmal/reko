//http://chrismontrois.net/2016/01/30/handlebars-switch/

module.exports = function(this: any, value:any, options:any) {
	if (value == this._switch_value_) {
	return options.fn(this);
	}
};